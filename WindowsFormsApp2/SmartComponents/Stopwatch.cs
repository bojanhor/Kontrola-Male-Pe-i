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
        Button btnStart;
        Button btnStop;
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
        int WaiterTimerInterval;

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
            WaiterTimerInterval = 1000;

            stopwatchEventReg();
            loadPreviousTime();

            m = new MethodInvoker(updateForm_invoker);
            ColorOriginal = time.BackColor;            

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
            updateForm();

            

            

            // time has elapsed
            if (ts_time >= ts_timeSet)
            {
                // if heating enabled disable it
                var prop = Val.logocontroler.Prop1;               
                prop.ForceEnableHeat.Value = 0;
                prop.ForceDisableHeat.Value = 1;                

                // stop stopwatch
                mainTimer.Stop();
                time.BackColor = ColorStopped;

                // buzz
                prop.BuzzFromPC.SendPulse();
            }
            else
            {
                time.BackColor = ColorRunning;
            }

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
        }

        private void BtnTimesetDn_MouseDown(object sender, MouseEventArgs e)
        {
            if (ts_timeSet <= new TimeSpan(0))
            {
                ts_timeSet = new TimeSpan(0);
                updateForm();
                return;
            }
            ts_timeSet = ts_timeSet.Add(new TimeSpan(0, -1, 0));
            
            MouseDownTimer = new System.Windows.Forms.Timer();
            MouseDownTimer.Interval = 200;
            holddur = 0;
            MouseDownTimer.Start();
            MouseDownTimer.Tick += MouseDownTimer_dn_Tick;
        }        

        int holddur = 0;
        private void BtnTimesetUp_MouseDown(object sender, MouseEventArgs e)
        {
            ts_timeSet = ts_timeSet.Add(new TimeSpan(0, 1, 0));
            updateForm();
            MouseDownTimer = new System.Windows.Forms.Timer();
            MouseDownTimer.Interval = 200;
            holddur = 0;
            MouseDownTimer.Start();
            MouseDownTimer.Tick += MouseDownTimer_up_Tick;
        }

        private void MouseDownTimer_up_Tick(object sender, EventArgs e)
        {
            if (holddur >= 15)
            {
                ts_timeSet = ts_timeSet.Add(new TimeSpan(0, 5, 0));
            }
            else if (holddur >= 10)
            {
                ts_timeSet = ts_timeSet.Add(new TimeSpan(0, 3, 0));
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
            if (holddur >= 15)
            {
                ts_timeSet = ts_timeSet.Add(new TimeSpan(0, -5, 0));
            }
            else if (holddur >= 10)
            {
                ts_timeSet = ts_timeSet.Add(new TimeSpan(0, -3, 0));
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

            holddur++;
            updateForm();
        }

        private void BtnTimesetDn_Click(object sender, EventArgs e)
        {
            if (ts_timeSet <= new TimeSpan(0))
            {
                ts_timeSet = new TimeSpan(0);
            }
            ts_timeSet = ts_timeSet.Add(new TimeSpan(0, -1, 0));
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
            else
            {
                ts_time = TimeSpan.FromSeconds(0);
                btnStop.Text = "Stop";
                time.BackColor = ColorOriginal;
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
            var prop = Val.logocontroler.Prop1;
            prop.ForceEnableHeat.Value = 1;
            prop.ForceDisableHeat.Value = 0;

        }

        private void WaiterTimer_Tick(object sender, EventArgs e)
        {
            // if heating disabled enable it
            WaiterTimer.Interval = WaiterTimerInterval = 1000;

            // if temperature reached
            var val = Val.logocontroler.Prop1.TempDosezena;
            if (val.Value_bool)
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
