using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        Timer MouseDownTimer = new Timer();
        MainTimer mainTimer = new MainTimer();

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
            
            btnTimesetUp.MouseDown += BtnTimesetUp_MouseDown;
            btnTimesetUp.MouseUp += BtnTimesetUp_MouseUp;

            btnTimesetDn.MouseDown += BtnTimesetDn_MouseDown;
            btnTimesetDn.MouseUp += BtnTimesetDn_MouseUp;

            mainTimer.Tick += MainTimer_Tick;

            loadPreviousTime();

            m = new MethodInvoker(updateForm_invoker);

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

            if (ts_time >= ts_timeSet)
            {
                mainTimer.Stop();
                MessageBox.Show("Čas je potekel.");
            }
        }

        private void BtnTimesetDn_MouseUp(object sender, MouseEventArgs e)
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
            
            MouseDownTimer = new Timer();
            MouseDownTimer.Interval = 200;
            holddur = 0;
            MouseDownTimer.Start();
            MouseDownTimer.Tick += MouseDownTimer_dn_Tick;
        }

        private void BtnTimesetUp_MouseUp(object sender, MouseEventArgs e)
        {
            MouseDownTimer.Stop();
            MouseDownTimer.Dispose();
            updateForm();
        }

        int holddur = 0;
        private void BtnTimesetUp_MouseDown(object sender, MouseEventArgs e)
        {
            ts_timeSet = ts_timeSet.Add(new TimeSpan(0, 1, 0));
            updateForm();
            MouseDownTimer = new Timer();
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
            if (mainTimer.Started)
            {
                mainTimer.Stop();
            }
            else
            {
                ts_time = TimeSpan.FromSeconds(0);
            }
            updateForm();
        }
        
        private void BtnStart_Click(object sender, EventArgs e)
        {
            mainTimer.Start();
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
