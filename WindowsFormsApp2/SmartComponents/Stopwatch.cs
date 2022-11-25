using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace WindowsFormsApp2
{
    class StopWatch
    {
        bool debugTemperaruraJeDosezena = false;
        public bool IsInProgress { get; private set; }
        Button btnStart;
        public delegate void Started(StopWatch sender); public event Started StopwatchStarted;        
        Button btnStop;
        public delegate void Stopped(StopWatch sender); public event Stopped StopwatchStopped;
        public delegate void WasReset(StopWatch sender); public event WasReset StopwatchWasReset;
        Label time;
        Label timeSet;        
        TimeSpan ts_timeSet;
        TimeSpan ts_time;
        Button btnTimesetUp;        
        Button btnTimesetDn;
        Form form;
        MethodInvoker m;
        System.Windows.Forms.Timer MouseDownTimer = new System.Windows.Forms.Timer();
        MainTimer mainTimer = new MainTimer();
        MainTimer WaiterTimer = new MainTimer();        

        Color ColorPaused = Color.LightYellow;
        Color ColorRunning = Color.Red;
        Color ColorStopped = Color.LightGreen;
        Color ColorOriginal;
       

        public StopWatch(Form form, Button btnStart, Button btnStop, Label time, Label timeSet, Button btnTimesetUp, Button btnTimesetDn)
        {
            this.form = form;
            this.btnStart = btnStart;
            this.btnStop = btnStop;
            this.time = time;
            this.timeSet = timeSet;        
             
            this.btnTimesetUp = btnTimesetUp;
            this.btnTimesetDn = btnTimesetDn;

            btnStart.Click += BtnStart_Click;
            btnStop.Click += BtnStop_Click;

            mainTimer.Tick += MainTimer_Tick;
            WaiterTimer.Tick += WaiterTimer_Tick;    

            stopwatchEventReg();
            loadPreviousTime();

            m = new MethodInvoker(updateForm_invoker);
            ColorOriginal = time.BackColor;

            btnStop.Text = "---";

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
                prop.ForceEnableHeat.Value = 0;
                prop.ForceDisableHeat.Value = 1;                

                // stop stopwatch
                mainTimer.Stop();
                StopwatchStopped.Invoke(this);
                IsInProgress = false;
                time.BackColor = ColorStopped;

                // buzz
                prop.BuzzFromPC.SendPulse();

                //
                IsInProgress = false;
                ts_time = ts_timeSet;
                btnStop.Text = "Reset";
                
            }
            else
            {
                time.BackColor = ColorRunning;
            }
            updateForm();
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
        }

        private void BtnTimesetDn_MouseDown(object sender, MouseEventArgs e)
        {
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

        int holddur = 0;
        private void BtnTimesetUp_MouseDown(object sender, MouseEventArgs e)
        {         
            ts_timeSet = ts_timeSet.Add(new TimeSpan(0, 1, 0));            
            MouseDownTimer = new System.Windows.Forms.Timer();
            MouseDownTimer.Interval = 200;
            holddur = 0;
            MouseDownTimer.Start();
            MouseDownTimer.Tick += MouseDownTimer_up_Tick;
            updateForm();
        }

        private void MouseDownTimer_up_Tick(object sender, EventArgs e)
        {
            if (holddur >= 17)
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
            if (holddur >= 17)
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
            if (mainTimer.Started)
            {
                time.BackColor = ColorStopped;
                mainTimer.Stop();
                btnStop.Text = "Reset";
            }
            else if(IsInProgress) // main timer is stopped AND IsInProgress
            {
                ts_time = TimeSpan.FromSeconds(0);
                btnStop.Text = "---";
                time.BackColor = ColorOriginal;
                StopwatchStopped.Invoke(this);
                IsInProgress = false;
                StopwatchWasReset.Invoke(this);
            }
            else
            {
                ts_time = TimeSpan.FromSeconds(0);
                btnStop.Text = "---";
                time.BackColor = ColorOriginal;
                StopwatchWasReset.Invoke(this);
            }

            // if heating enabled disable it
            var prop = Val.logocontroler.Prop1;            
            prop.ForceEnableHeat.Value = 0;
            prop.ForceDisableHeat.Value = 1;
                        
            //
            updateForm();
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            WaiterTimer.Start();
            btnStop.Text = "Pause";
            var prop = Val.logocontroler.Prop1;
            prop.ForceEnableHeat.Value = 1;
            prop.ForceDisableHeat.Value = 0;
            IsInProgress = true;
            if (!mainTimer.Started)
            {
                StopwatchStarted.Invoke(this);
            }
        }

        private void WaiterTimer_Tick(object sender, EventArgs e)
        {
            // if heating disabled enable it
            WaiterTimer.Interval = 1000;

            // if temperature reached
            var val = Val.logocontroler.Prop1.TempDosezena;
            if (val.Value_bool || debugTemperaruraJeDosezena)
            {
                mainTimer.Start();                

                //
                time.BackColor = ColorRunning;
                WaiterTimer.Stop();
            }
            else
            {
                if (time.BackColor == ColorPaused)
                {
                    time.BackColor = ColorOriginal;
                }
                else
                {
                    time.BackColor = ColorPaused;
                }
                
            }
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
            m.Invoke();
        }
    }
}
