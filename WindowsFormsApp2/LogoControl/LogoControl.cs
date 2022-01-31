using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using Sharp7;
using System.Net.NetworkInformation;

namespace WindowsFormsApp2
{
    public partial class LogoControler
    {
        private readonly int[] forceRefresh = new int[Settings.Devices + 1];

        public int WatchdogRetries = 5;

        public static Misc.SmartThread[] BackgroundWorker = new Misc.SmartThread[Settings.Devices + 1];

        public Thread Watchdog_PC;

        public ushort Watchdog_PC_value = 0;

        public Prop1 Prop1 { get; set; }
        public Prop2 Prop2 { get; set; }
        public Prop3 Prop3 { get; set; }
        public Prop4 Prop4 { get; set; }
        public Prop5 Prop5 { get; set; }

        public static PropComm[] Props = new PropComm[Settings.Devices + 1];

        public static S7Client[] LOGO = new S7Client[Settings.Devices + 1];

        public static Connection[] LOGOConnection = new Connection[Settings.Devices + 1];

        void LogoControlerInit()
        {
            for (int i = 1; i < LOGO.Length; i++)
            {
                LOGO[i] = new S7Client(i);

                LOGOConnection[i] = new Connection
                {
                    IpAddress = XmlController.GetLogoIP(i),
                    LocalTSAP = XmlController.GetLogoLocalTsap(i),
                    RemoteTSAP = XmlController.GetLogoRemoteTsap(i),
                    errcodeLOGO = 0,
                    connectionStatusLOGO = Connection.Status.NotInitialised
                };

                LOGO[i].SetConnectionParams(LOGOConnection[i].IpAddress.ToString(), LOGOConnection[i].LocalTSAP_asushort, LOGOConnection[i].RemoteTSAP_asushort);
                forceRefresh[i] = 0;
            }

            Prop1 = new Prop1(LOGO[1]);
            Prop2 = new Prop2(LOGO[2]);
            Prop3 = new Prop3(LOGO[3]);
            Prop4 = new Prop4(LOGO[4]);
            Prop5 = new Prop5(LOGO[5]);

            
            Helper.LogoControllerInitialized = true;
        }

        public LogoControler()
        {
            Misc.SmartThread LogoControlerInitThread = new Misc.SmartThread(() => LogoControlerInit());
            LogoControlerInitThread.Start("LogoControlerInitThread", ApartmentState.MTA, true);
        }

        public void StartBackgroundTasks()
        {
            Watchdog_PC = new Thread(() => { Watchdog_PC_DoWork(null, null); });

            for (int i = 1; i < BackgroundWorker.Length - 1; i++)
            {


                try
                {
                    if (XmlController.GetEnabledLogo(i))
                    {
                        int tmpi = i;
                        BackgroundWorker[i] = new Misc.SmartThread(() => BackgroundWorker1_DoWork(tmpi));
                        //BackgroundWorker[i].Start("Loop - PLC " + tmpi, ApartmentState.MTA, true);
                        Thread.Sleep(50);
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("Cant load PLC loop thread correctly. Reason: " + ex.Message);
                }

            }

        }

        void BackgroundWorker1_DoWork(int device)
        {


            WL("Connecting...", device);
            LOGOConnection[device].connectionStatusLOGO = Connection.Status.Connecting;

            Thread.Sleep(1000 * device); // startup delay - not all at once

            DateTime time1;
            DateTime time2;

            int thisVal = 1;
            int prevVal = 0;
            int FailCnt = 0;
            int olderr = 0;
            int RWcyc = XmlController.GetReadWriteCycle(device);
            string progress = PropComm.NA;


           
            Val.InitializeWDTable(device);

            int errCode = S7Consts.err_OK;
            bool firstWDattemptSucc = false;
            int RWcyc_old = 0;

            int failCntr = 0;

            if (IfDisconnectProcedure(device)) { return; }

            try
            {
                if (IfDisconnectProcedure(device)) { return; }

                errCode = LOGO[device].Connect();

                if (IfDisconnectProcedure(device)) { return; }

                if ((errCode) == 0)
                {
                    WL("Connected", LOGO[device].deviceID);
                    LOGOConnection[device].connectionStatusLOGO = Connection.Status.Connected;
                }
                else
                {
                    WL(LOGO[device].ErrorText(errCode), LOGO[device].deviceID);
                    LOGOConnection[device].connectionStatusLOGO = Connection.Status.Error;
                }
            }
            catch (Exception ex)
            {
                WL("Internal fatal error on first connection attempt: " + ex.Message, device);
                LOGOConnection[device].connectionStatusLOGO = Connection.Status.Error;
            }
                        
            try
            {
                time1 = DateTime.Now;
                while (true)
                {
                   
                    if (forceRefresh[device] > 0) // force refresh values with minimum delay - int value represents how many cycles value will be forced to refresh
                    {
                        forceRefresh[device] -= 1;
                    }

                    try
                    {
                        if (IfDisconnectProcedure(device)) { return; }


                        if (LOGOConnection[device].connectionStatusLOGO == Connection.Status.Connected ||
                            LOGOConnection[device].connectionStatusLOGO == Connection.Status.Warning)
                        {
                            // 
                            LOGO[device].S7PLCbuffer.ReadPLCtoBuffer(); // READs WHOLE PLC TO BUFFER
                                                                        //

                            switch (device)
                            {
                                case 1: Prop1.SyncVars(); break;
                                case 2: Prop2.SyncVars(); break;
                                case 3: Prop3.SyncVars(); break;
                                case 4: Prop4.SyncVars(); break;
                                case 5: Prop5.SyncVars(); break;

                                default:
                                    WL("Internal Error: Switch statement does not support this device", -2); LOGOConnection[device].connectionStatusLOGO = Connection.Status.Error; break;

                            }

                        }
                        else
                        {
                            RunDiagnostics(device);
                        }

                    }
                    catch (Exception ex)
                    {
                        WL("Error: Retrieving data from PLC failed. Message source: Program looptrough inside background worker: " + ex.Message, device);
                    }

                    try
                    {
                        if (IfDisconnectProcedure(device)) { return; }

                        // Watchdog
                        errCode = Watchdog(LOGO[device], XmlController.GetWDAddress(device), ref progress, ref thisVal, ref prevVal, WatchdogRetries, ref FailCnt);

                        if (IfDisconnectProcedure(device)) { return; }

                        if (errCode != 0 && olderr != errCode)
                        {
                            WL(LOGO[device].ErrorText(errCode), device);
                            LOGOConnection[device].connectionStatusLOGO = Connection.Status.Warning;
                        }
                        else
                        {
                            if (!firstWDattemptSucc)
                            {
                                WL("Watchdog started successfully - Period (Frequency) is " + XmlController.GetReadWriteCycle(device) + "ms", LOGO[device].deviceID);
                                firstWDattemptSucc = true;
                            }
                            else if (RWcyc_old != RWcyc)
                            {
                                WL("Watchdog changed successfully - Period (Frequency) is " + XmlController.GetReadWriteCycle(device) + "ms", LOGO[device].deviceID);
                            }

                        }

                        if (errCode == (int)Connection.Status.Connected && olderr == errCode)
                        {
                            LOGOConnection[device].connectionStatusLOGO = Connection.Status.Connected;
                        }

                        RWcyc_old = RWcyc;
                        olderr = errCode;

                        if (IfDisconnectProcedure(device)) { return; }
                    }
                    catch (Exception ex)
                    {
                        WL("Error: Watchdog failed. Message source: Program looptrough inside background worker: " + ex.Message, device);
                    }

                    try
                    {
                        if (olderr != 0)
                        {
                            WL("Connection error: " + LOGO[device].ErrorText(olderr), device);                            
                            time2 = DateTime.Now.AddMilliseconds(3000);
                            LOGOConnection[device].connectionStatusLOGO = Connection.Status.Connecting;

                            // prevent to many reconnection attempts
                            while (time2 > DateTime.Now)
                            {
                                Thread.Sleep(Settings.defaultCheckTimingInterval);
                                if (IfDisconnectProcedure(device)) { return; }

                            }

                            RunDiagnostics(device);                            

                            if (IfDisconnectProcedure(device)) { return; }

                        }
                        else
                        {
                            time2 = DateTime.Now;

                            if (time2 > time1.AddMilliseconds(RWcyc))
                            {
                                if (failCntr >= 10)
                                {
                                    WL("Read-Write cycle is taking " + (int)(time2 - time1).TotalMilliseconds + "ms to complete (Cycle is set to be " + RWcyc + "ms ). Please extend Read-Write cycle", device);
                                    LOGOConnection[device].connectionStatusLOGO = Connection.Status.Warning;
                                    failCntr = 0;
                                }
                                else
                                {
                                    failCntr++;
                                }
                            }
                            else
                            {
                                while (DateTime.Now < time1.AddMilliseconds(RWcyc))
                                {
                                    if (IfDisconnectProcedure(device)) { return; }
                                    Thread.Sleep(Settings.defaultCheckTimingInterval);
                                    if (forceRefresh[device] > 0)
                                    {
                                        break;
                                    }

                                }
                                failCntr = 0;
                            }
                            time1 = DateTime.Now;
                        }
                        if (IfDisconnectProcedure(device)) { return; }

                        LOGOConnection[device].errcodeLOGO = errCode;
                       PropComm.SetWatchdogValue(progress, device);


                        if (IfDisconnectProcedure(device)) { return; }
                    }
                    catch (Exception ex)
                    {
                        WL("Error: R/W Cycle manager stopped. Message source: Program looptrough inside background worker: " + ex.Message, device);
                    }

                }
            }
            catch (Exception ex)
            {
                WL("Internal fatal error in background worker thread, Connection will be terminated: " + ex.Message, device);
                LOGOConnection[device].connectionStatusLOGO = Connection.Status.Error;
                return;
            }

        }

        private int Watchdog(S7Client Client, PlcVars.DoubleWordAddress typeAndAdress, ref string Progress, ref int thisValue, ref int previousValue, int CanFail_timesWithoutError, ref int CanFailCnt)
        {
            int err = 0;


            if (err == 0)
            {
                // watchdog function

                thisValue = Connection.BufferRead(Client, typeAndAdress, out err);
                if (err != 0 || thisValue <= 0) // if there is an error
                {
                    previousValue = thisValue;
                    Progress = "ERR";
                    CanFailCnt = 0;
                    if (err == 0)
                    {
                        err = S7Consts.err_watchdogDoesntChange;
                    }
                    return err;
                }
                else // if there is no error
                {
                    if (thisValue == previousValue) // value of watchdog does not change...
                    {
                        CanFailCnt++;
                        if (CanFailCnt >= CanFail_timesWithoutError) // ...does not change multiple times ( fail counter ++ )
                        {
                            Progress = "ERR";
                            previousValue = thisValue;
                            CanFailCnt = 0;
                            return -2;
                        }
                        else // waiting for fail counter to announce an error
                        {
                            return 0;
                        }

                    }
                    else
                    {
                        Progress = "Running: " + thisValue.ToString();
                        previousValue = thisValue;
                        CanFailCnt = 0;
                        return err;
                    }
                }

            }
            else
            {
                Progress = "ERR";
                return err;
            }



        }

        private bool IfDisconnectProcedure(int device)
        {
            if (BackgroundWorker[device] == null)
            {
                return true;
            }
            if (BackgroundWorker[device].CanelationPending)
            {
                LOGOConnection[device].connectionStatusLOGO = Connection.Status.Error;                
                return true;
            }
            else { return false; }
        }

        private void RunDiagnostics(int device)
        {
            WL("Running connection diagnostics...",device);

            Ping(device);
            
            Reconnect(LOGO[device]);
        }

        void Watchdog_PC_DoWork(object sender, EventArgs e)
        {

            bool firstpass = true;

            try
            {
                while (true)
                {


                    if (Watchdog_PC_value >= 8)
                    {
                        Watchdog_PC_value = 1;
                    }

                    Watchdog_PC_value++;
                    
                    if (firstpass)
                    {                       
                        WL("PC Watchdog is now running", 0);
                       
                        firstpass = false;
                    }





                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {

                WL("PC Watchdog has encountered an error and must be stopped: " + ex.Message, -2);
            }


        }

        void Reconnect(S7Client Client)
        {            
            int err;

            LOGOConnection[Client.deviceID].connectionStatusLOGO = Connection.Status.Connecting;
            Client.Disconnect();
            Thread.Sleep(500);
            err = Client.Connect();
            if ((err) == 0)
            {
                WL("Connected again", Client.deviceID);
                LOGOConnection[Client.deviceID].connectionStatusLOGO = Connection.Status.Connected;

            }
            else
            {
                WL("Connection failed. Trying to reconnect", Client.deviceID);
                LOGOConnection[Client.deviceID].connectionStatusLOGO = Connection.Status.Error;
            }


        }

        void Ping(int device)
        {
            var pingTries = 5; // ping retries
            var ip = LOGOConnection[device].IpAddress; // ip to ping
            Ping ping = new Ping();
            PingReply pingr = null;
            var timeout = 100; // initial timeout for ping

            int successfullPings = 0; // counter of sucessful pings

            for (int i = 0; i < pingTries; i++)
            {
                // PING     
                pingr = ping.Send(ip, timeout);

                if (pingr.Status == IPStatus.Success)
                {                    
                    successfullPings++; // if ping successfull add cntr
                }
                else
                {
                    timeout *= 2; // if ping not successfull increase timeout
                }
            }

            if (successfullPings == pingTries)
            {
                WL("  -Ping successful at: " + ip, device);
            }
            else if(successfullPings > 0)
            {
                WL("  -Ping successful but connection is poor: " + ip, device);
            }
            else
            {
                WL("  -Ping ERROR at: " + ip + " (" + pingr.Status.ToString() + ")", device);
            }

        }

        public void ConnectAsync(int device)
        {
            if (BackgroundWorker[device] != null)
            {
                if (!BackgroundWorker[device].IsBusy)
                {
                    BackgroundWorker[device].RunWorkerAsync();
                }
            }

        }

        public void DisconnectAsync(int device)
        {

            WL("Disconected by the user", device);
            BackgroundWorker[device].CancelAsync();
            LOGO[device].Disconnect();

        }

        public void ForceRefreshValuesFromPLC(int device)
        {
            if (device > 0 && device < forceRefresh.Length)
            {
                forceRefresh[device] = 5; // force refresh values with minimum delay - int value represents how many cycles value will be forced to refresh
            }
        }
            

        /// <summary>        
        /// Reports errors to console and/or gui.         
        /// </summary>
        /// <param name="message">type a message to report or log</param>
        /// <param name="device">(device 0 = info message) (device -1 = warning message) (device -2 = error message)</param>
        private void WL(string message, int device)
        {            
            var msg = "";

            if (device > 0)
            {
                msg = "Logo" + device + ": " + message;
            }

            else if (device == 0)
            {
                msg = "Info" + ": " + message;
            }

            else if (device == -1)
            {
                msg = "Warning" + ": " + message;
            }

            else if (device == -2)
            {
                msg = "Error" + ": " + message;
            }

            try
            {
                if (XmlController.IsDebugEnabled())
                {
                    System.Diagnostics.Debug.WriteLine(msg);
                }

                SettingsForm.WL(message, 0);

            }
            catch (Exception ex)
            {
                var i = ex.InnerException;
                throw new Exception("Internal error inside WL() method (used for loging and error posting).  " + ex.Message + "// Innner exception message: " + i);
            }





        }

    }
}