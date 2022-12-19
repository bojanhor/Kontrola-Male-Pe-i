using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp2
{

    class StopWatch
    {
        Thread selfControlThread;
        bool debugTemperaruraJeDosezena = false;
        public bool IsInProgress { get; private set; }
        Button btnStart;
        public delegate void Started(StopWatch sender); public event Started StopwatchStarted;        
        Button btnStop;
        public delegate void Stopped(StopWatch sender); public event Stopped StopwatchStopped;
        public delegate void WasReset(StopWatch sender); public event WasReset StopwatchWasReset;
        Label time;
        Label timeSet;
        Label lblEstimateEnd;
        TimeSpan ts_timeSet;
        TimeSpan ts_time;
        Button btnTimesetUp;        
        Button btnTimesetDn;
        GroupBox gbStopwatch;
        CheckBox chkPauseIfLowTemp;
        RadioButton ManualOffRadioBtn;


        MethodInvoker m;
        System.Windows.Forms.Timer MouseDownTimer = new System.Windows.Forms.Timer();
        MainTimer mainTimer = new MainTimer();
        MainTimer WaiterTimer = new MainTimer();        

        Color ColorPaused = Color.LightYellow;
        Color ColorRunning = Color.LightGreen;
        Color ColorStopped = Color.Pink;
        Color ColorOriginal;

        Form parent;
       

        public StopWatch(Form Parent)
        {
            parent = Parent;
            sartBtnTextDeciderInvoker = new MethodInvoker(sartBtnTextDeciderMethod);
            // Finding controls by name from form
            btnStart = (Button)Parent.Controls.Find("btnStart", true)[0];
            btnStop = (Button)Parent.Controls.Find("btnStop", true)[0];
            time = (Label)Parent.Controls.Find("lblTime", true)[0];
            timeSet = (Label)Parent.Controls.Find("lblTimeSet", true)[0];        
             
            btnTimesetUp = (Button)Parent.Controls.Find("btnUp", true)[0];
            btnTimesetDn = (Button)Parent.Controls.Find("btnDown", true)[0];

            gbStopwatch = (GroupBox)Parent.Controls.Find("gbStoparica", true)[0];
            chkPauseIfLowTemp = (CheckBox)Parent.Controls.Find("chkPauseIfLowTemp", true)[0];
            ManualOffRadioBtn = (RadioButton)Parent.Controls.Find("ManualOffRadioBtn", true)[0];
            lblEstimateEnd = (Label)Parent.Controls.Find("lblEstimateEnd", true)[0];
            ////

            btnStart.Click += BtnStart_Click;
            btnStop.Click += BtnStop_Click;

            mainTimer.Elapsed += MainTimer_Tick;
            WaiterTimer.Elapsed += WaiterTimer_Tick;    

            stopwatchEventReg();
            loadPreviousTime();

            m = new MethodInvoker(updateForm_invoker);
            estimateTimeLeftInvoker = new MethodInvoker(estimateTimeLeftMethod);
            ColorOriginal = gbStopwatch.BackColor;

            btnStop.Text = "---";

            StopwatchStarted += StopWatch_StopwatchStarted;
            StopwatchStopped += StopWatch_StopwatchStopped;
            StopwatchWasReset += StopWatch_StopwatchWasReset;

            ManualOffRadioBtn.Click += ManualOffRadioBtn_Click;           
        }

        private void ManualOffRadioBtn_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void StopWatch_StopwatchWasReset(StopWatch sender)
        {
            
        }

        private void StopWatch_StopwatchStopped(StopWatch sender)
        {
            if (selfControlThread != null)
            {
                selfControlThread.Abort();
            }            
        }

        private void StopWatch_StopwatchStarted(StopWatch sender)
        {
            selfControlThread = new Thread(selfControlMethod);
            selfControlThread.Start();
        }

        bool wasStoppedBySelfControl = false;
        Prop1 p = Val.logocontroler.Prop1;
        void selfControlMethod()
        {
            while (true)
            {
                if (mainTimer.Enabled)
                {
                    if (!p.TempDosezena.Value_bool)
                    {
                        if (chkPauseIfLowTemp.Checked)
                        {
                            Pause();
                            wasStoppedBySelfControl = true;
                        }                        
                    }
                }

                //
                if (!WaiterTimer.Enabled)
                {
                    if (!mainTimer.Enabled)
                    {
                        if (p.TempDosezena.Value_bool)
                        {
                            if (wasStoppedBySelfControl)
                            {
                                Resume();
                                wasStoppedBySelfControl = false;
                            }                            
                        }
                    }
                }
               
                Thread.Sleep(500);
            }

        }

        public void ResumeStopwatchIfSettingChanged()
        {
            if (!WaiterTimer.Enabled)
            {
                if (!mainTimer.Enabled)
                {

                    if (wasStoppedBySelfControl)
                    {
                        Resume();
                        wasStoppedBySelfControl = false;
                    }

                }
            }
        }

        void stopwatchEventReg()
        {
            btnTimesetUp.MouseDown += BtnTimesetUp_MouseDown;
            btnTimesetUp.MouseUp += BtnTimesetUp_MouseUp;
            btnTimesetUp.LostFocus += BtnTimesetUp_LostFocus;

            btnTimesetDn.MouseDown += BtnTimesetDn_MouseDown;
            btnTimesetDn.MouseUp += BtnTimesetDn_MouseUp;
            btnTimesetDn.LostFocus += BtnTimesetDn_LostFocus;


        }

        private void BtnTimesetDn_LostFocus(object sender, EventArgs e)
        {
            if (MouseDownTimer.Enabled)
            {
                unnholdBtn();
            }
        }

        private void BtnTimesetUp_LostFocus(object sender, EventArgs e)
        {
            if (MouseDownTimer.Enabled)
            {
                unnholdBtn();
            }
        }

        void updateForm_invoker()
        {
            timeSet.Text = TimeToString(ts_timeSet);            
            time.Text = TimeToString(ts_time);
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {

            ts_time = ts_time.Add(TimeSpan.FromSeconds(1));     
                        
            // time has elapsed
            if (ts_time >= ts_timeSet)
            {
                // if heating enabled disable it
                var prop = Val.logocontroler.Prop1;               
                prop.On.Value = 0;
                prop.Off.Value = 1;                

                // stop stopwatch
                mainTimer.Stop();
                StopwatchStopped.Invoke(this);
                IsInProgress = false;
                gbStopwatch.BackColor = ColorStopped;

                // buzz
                prop.BuzzFromPcEndCycle.SendPulse();

                //
                IsInProgress = false;
                ts_time = ts_timeSet;
                parent.Invoke(new MethodInvoker(delegate { btnStop.Text = "Reset"; })); 
                
            }
            else
            {
                gbStopwatch.BackColor = ColorRunning;
                parent.Invoke(new MethodInvoker(delegate { btnStop.Text = "Pause"; }));
            }
            parent.Invoke(estimateTimeLeftInvoker);
            updateForm();
        }

        MethodInvoker estimateTimeLeftInvoker;
        void estimateTimeLeftMethod()
        {
            var ts_txt = DateTime.Now + (ts_timeSet - ts_time);
            var txt = ts_txt.ToString("HH:mm");
            lblEstimateEnd.Text = txt;
        }

        private void BtnTimesetDn_MouseUp(object sender, MouseEventArgs e)
        {
            unnholdBtn();
        }

        private void BtnTimesetUp_MouseUp(object sender, MouseEventArgs e)
        {
            unnholdBtn();
        }

        void unnholdBtn()
        {
            MouseDownTimer.Stop();
            MouseDownTimer.Dispose();
            updateForm();
            saveTime();
            mouseDown = false;
        }

        int holddur = 0;
        bool mouseDown = false;
        private void BtnTimesetDn_MouseDown(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                return;
            }
            mouseDown = true;
            ts_timeSet = ts_timeSet.Add(new TimeSpan(0, -1, 0));
            if (ts_timeSet <= new TimeSpan(0))
            {
                ts_timeSet = new TimeSpan(0);
                return;
            }

            MouseDownTimer = new System.Windows.Forms.Timer();
            MouseDownTimer.Interval = 200;
            holddur = 0;

            MouseDownTimer.Start();
            MouseDownTimer.Tick += MouseDownTimer_dn_Tick;
            updateForm();
        }        
                
        private void BtnTimesetUp_MouseDown(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                return;
            }
            mouseDown = true;
            ts_timeSet = ts_timeSet.Add(new TimeSpan(0, 1, 0));
            MouseDownTimer = new System.Windows.Forms.Timer();
            MouseDownTimer.Interval = 200;
            holddur = 0;

            MouseDownTimer.Start();
            MouseDownTimer.Tick += MouseDownTimer_up_Tick;
            parent.Invoke(sartBtnTextDeciderInvoker);
            updateForm();            
        }

        MethodInvoker sartBtnTextDeciderInvoker;
        void sartBtnTextDeciderMethod()
        {
            if (!mainTimer.Enabled && !WaiterTimer.Enabled)
            {
                if (ts_timeSet > ts_time)
                {
                    btnStart.Text = "Start";
                }
            }            
        }

        private void MouseDownTimer_up_Tick(object sender, EventArgs e)
        {
            if (holddur >= 40)
            {
                ts_timeSet = ts_timeSet.Add(new TimeSpan(0, 8, 0));
            }
            else if (holddur >= 30)
            {
                ts_timeSet = ts_timeSet.Add(new TimeSpan(0, 5, 0));
            }
            else if (holddur >= 17)
            {
                ts_timeSet = ts_timeSet.Add(new TimeSpan(0, 3, 0));
            }
            else if (holddur >= 10)
            {
                ts_timeSet = ts_timeSet.Add(new TimeSpan(0, 2, 0));
            }
            else if (holddur >= 5)
            {
                ts_timeSet = ts_timeSet.Add(new TimeSpan(0, 1, 0));
                MouseDownTimer.Interval = 100;
            }
            else 
            {
                ts_timeSet = ts_timeSet.Add(new TimeSpan(0, 1, 0));
            }
            
            holddur++;
            updateForm();
        }

        private void MouseDownTimer_dn_Tick(object sender, EventArgs e)
        {
            if (ts_timeSet <= new TimeSpan(0))
            {
                ts_timeSet = new TimeSpan(0);
                updateForm();
                return;
            }
            if (holddur >= 40)
            {
                ts_timeSet = ts_timeSet.Add(new TimeSpan(0, -8, 0));
            }
            else if (holddur >= 30)
            {
                ts_timeSet = ts_timeSet.Add(new TimeSpan(0, -5, 0));
            }
            else if (holddur >= 17)
            {
                ts_timeSet = ts_timeSet.Add(new TimeSpan(0, -3, 0));
            }
            else if (holddur >= 10)
            {
                ts_timeSet = ts_timeSet.Add(new TimeSpan(0, -2, 0));
            }
            else if (holddur >= 5)
            {
                ts_timeSet = ts_timeSet.Add(new TimeSpan(0, -1, 0));
                MouseDownTimer.Interval = 100;
            }
            else
            {
                ts_timeSet = ts_timeSet.Add(new TimeSpan(0, -1, 0));
            }

            if (ts_timeSet <= new TimeSpan(0))
            {
                ts_timeSet = new TimeSpan(0);                             
            }

            holddur++;
            updateForm();
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            
            WaiterTimer.Stop();
            parent.Invoke(new MethodInvoker(delegate { 
                btnStart.Text = "Start"; 
            }));

            //           
            
            if (mainTimer.Started)
            {
                gbStopwatch.BackColor = ColorStopped;
                mainTimer.Stop();
                parent.Invoke(new MethodInvoker(delegate {
                    btnStop.Text = "Reset";
                }));
            }
            else if(IsInProgress) // main timer is stopped AND IsInProgress
            {
                if (MessageBox.Show("Sarža še ni zaključena, ali vseeno želite ponastaviti štoparico? ","POZOR!",MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ts_time = TimeSpan.FromSeconds(0);
                    parent.Invoke(new MethodInvoker(delegate {
                        btnStop.Text = "---";
                    }));
                    gbStopwatch.BackColor = ColorOriginal;
                    StopwatchStopped.Invoke(this);
                    IsInProgress = false;
                    StopwatchWasReset.Invoke(this);
                }               
            }
            else
            {
                ts_time = TimeSpan.FromSeconds(0);
                        parent.Invoke(new MethodInvoker(delegate {
                            btnStop.Text = "---";
                        }));
                gbStopwatch.BackColor = ColorOriginal;
                StopwatchWasReset.Invoke(this);
            }

            // if heating enabled disable it
            var prop = Val.logocontroler.Prop1;            
            prop.On.Value = 0;
            prop.Off.Value = 1;
                        
            //
            updateForm();
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (WaiterTimer.Started || mainTimer.Started)
            {
                return;
            }
            WaiterTimer.Start();
            btnStop.Text = "Stop";
            btnStart.Text = "---";           
            var prop = Val.logocontroler.Prop1;
            prop.On.Value = 1;
            prop.Off.Value = 0;
            IsInProgress = true;
            if (!mainTimer.Started)
            {
                StopwatchStarted.Invoke(null);
            }

        }

        
        private void WaiterTimer_Tick(object sender, EventArgs e)
        {
            parent.Invoke(new MethodInvoker(delegate { btnStop.Text = "Stop"; }));
            // if heating disabled enable it
            WaiterTimer.Interval = 1000;

            // if temperature reached
            var val = Val.logocontroler.Prop1.TempDosezena;
            if (val.Value_bool || debugTemperaruraJeDosezena)
            {
                mainTimer.Start();
                p.BuzzFromPcTemperatureReached.SendPulse();

                //
                gbStopwatch.BackColor = ColorRunning;
                WaiterTimer.Stop();
            }
            else
            {
                if (gbStopwatch.BackColor == ColorPaused)
                {
                    gbStopwatch.BackColor = ColorOriginal;
                }
                else
                {
                    gbStopwatch.BackColor = ColorPaused;
                }
                
            }
        }

        public void Pause()
        {
            if (mainTimer != null)
            {
                mainTimer.Stop();
            }
        }

        public void Resume()
        {
            if (mainTimer != null)
            {
                mainTimer.Start();
            }            
        }

        public void Stop()
        {
            BtnStop_Click(null,null);
        }


        void loadPreviousTime()
        {
            try
            {
                ts_timeSet = stringToTime(Properties.Settings.Default.StopWatchTime);
            }
            catch (Exception)
            {
                ts_timeSet = new TimeSpan(0, 0, 0);
            }

            timeSet.Text = TimeToString(ts_timeSet);         

        }

        void saveTime()
        {
            Properties.Settings.Default.StopWatchTime = TimeToString(ts_timeSet);
            Properties.Settings.Default.Save();
        }

        TimeSpan stringToTime(string val)
        {
            try
            {
                var h = (val.Split(':')[0]);
                var m = val.Split(':')[1];
                var s = val.Split(':')[2];

                int h_ = Convert.ToInt32(h);
                int m_ = Convert.ToInt32(m);
                int s_ = Convert.ToInt32(s);

                return new TimeSpan(h_, m_, s_);
            }
            catch (Exception)
            {
                return new TimeSpan(0,0,0);
            }
        }

        string TimeToString(TimeSpan val)
        {
            return val.ToString("c");
        }

        void updateForm()
        {
            parent.Invoke(m);
        }
    }
}
