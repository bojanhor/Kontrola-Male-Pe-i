using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WindowsFormsApp2
{

    public class PropComm
    {
        public const string NA = "_N/A_";

        public Sharp7.S7Client Client;
        public AutoSync AutoSync = new AutoSync();
        bool firstSync = true;
        int poolCnt = 1;
        private static string[] WatchdogValues = new string[Settings.Devices + 1];

        public PlcVars.LogoClock LogoClock;

        public PropComm(Sharp7.S7Client client)
        {
            Client = client;
            LogoClock = new PlcVars.LogoClock(this);
        }

        public void SyncVars()
        {
            FirstSync();

            SyncEveryLoopVars(AutoSync.VarsListPool[1]);

            // devides autossync variables which were scheduled to be synced every second loop to two groups. group 1 is synced 1st loop, group 2 is synced 2nd loop
            SmartSyncDistributor(AutoSync.VarsListPool[2], 2, poolCnt);
            SmartSyncDistributor(AutoSync.VarsListPool[3], 3, poolCnt);
            SmartSyncDistributor(AutoSync.VarsListPool[4], 4, poolCnt);
            SmartSyncDistributor(AutoSync.VarsListPool[5], 5, poolCnt);

            PoolCnt();
        }

        void SyncEveryLoopVars(List<PlcVars.PlcType> collection)
        {
            PlcVars.PlcType item;
            for (int i = 0; i < collection.Count; i++)
            {
                item = collection[i];
                item.SyncWithPLC();
            }
            
        }
        void SmartSyncDistributor(List<PlcVars.PlcType> collection, int syncEveryXvariable, int CurrentPoolNum)
        {
            PlcVars.PlcType item;

            if (syncEveryXvariable<2 || syncEveryXvariable >5)
            {
                throw new Exception("Internal fatal error. SyncEveryXvariable out of range.");               
            }

            for (int i = 0; i < collection.Count; i++)
            {
                item = collection[i];
                if (CurrentPoolNum % syncEveryXvariable == 0)
                {
                    if (i % syncEveryXvariable == 0)
                    {
                        item.SyncWithPLC(); // syncs if i == 0,3,6,9,12,15...
                    }
                }

                if (CurrentPoolNum % syncEveryXvariable == 1)
                {
                    if (i % syncEveryXvariable == 1)
                    {
                        item.SyncWithPLC(); // syncs if i == 1,4,7,10,13,16...
                    }
                }

                if (CurrentPoolNum % syncEveryXvariable == 2)
                {
                    if (i % syncEveryXvariable == 2)
                    {
                        item.SyncWithPLC(); // syncs if i == 2,5,8,11,14,17...
                    }
                }

                if (CurrentPoolNum % syncEveryXvariable == 3)
                {
                    if (i % syncEveryXvariable == 3)
                    {
                        item.SyncWithPLC(); // syncs if i == 2,5,8,11,14,17...
                    }
                }

                if (CurrentPoolNum % syncEveryXvariable == 4)
                {
                    if (i % syncEveryXvariable == 4)
                    {
                        item.SyncWithPLC(); // syncs if i == 2,5,8,11,14,17...
                    }
                }  
            }
        }

        void FirstSync()
        {
            if (!firstSync)
            {
                return;
            }

            // code only on first sync
            foreach (var item in AutoSync.VarsList)
            {
                AutoSync.VarsListPool[item.SyncEvery_X_Time].Add(item); // adds variables to different pools of syncing. if variable must be synced every 2nd loop then for is added to list 2; 
            }

            firstSync = false;
        }

        void PoolCnt()
        {
            if (poolCnt >= 60) // common denominator of 1,2,3,4,5 is 60
            {
                poolCnt = 1;
            }
            else
            {
                poolCnt++;
            }
        }

       
        public static void SetWatchdogValue(string Value, int deviceNum)
        {
            WatchdogValues[deviceNum] = Value;
        }
        public static string GetWatchdogValue(int deviceNum)
        {
            try
            {
                return WatchdogValues[deviceNum];
            }
            catch
            {
                return PropComm.NA;
            }
            
        }

        public static int GetWatchdogNumDevices()
        {
            try
            {
                return WatchdogValues.Length;
            }
            catch
            {
                return 0;
            }

        }

        public static int GetPCWD_Address()
        {
            try
            {
                return Convert.ToInt32(XmlController.XmlGeneral.Element("AddressPC_WD").Value);
            }
            catch
            {
                throw new Exception("AddressPC_WD is not valid xml entry. Please provide valid entry for that, for example 100.");
            }
        }

    }

    public class AutoSync
    {
        public List<PlcVars.PlcType> VarsList { get; private set; }
        public List<PlcVars.PlcType>[] VarsListPool = new List<PlcVars.PlcType>[6]; // different pools for syncing every other loop


        public AutoSync()
        {
            VarsList = new List<PlcVars.PlcType>();

            for (int i = 1; i <= 5; i++)
            {
                VarsListPool[i] = new List<PlcVars.PlcType>();
            }

        }
        public void Add(PlcVars.PlcType var)
        {
            VarsList.Add(var);
        }

       
    }

    

}


