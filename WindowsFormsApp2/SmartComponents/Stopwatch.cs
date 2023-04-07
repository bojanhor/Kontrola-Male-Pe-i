using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp2
{

    public class StopWatch
    {
        Thread selfControlThread;
        bool debugTemperaruraJeDosezena = false;
        public bool IsInProgress  { get; private set; }
        public bool Counting { get; private set; } = false;
        Button btnStart;
        public delegate void Started(StopWatch sender); public event Started StopwatchStarted;        
        Button btnStop;
        public delegate void Stopped(StopWatch sender); public event Stopped StopwatchStopped;
        public delegate void WasReset(StopWatch sender); public event WasReset StopwatchWasReset;
        public delegate void WasPaused(StopWatch sender); public event WasReset StopwatchWasPaused;

        Label lblEstimateEnd;
        TimeSpan ts_timeSet;        
        GroupBox gbStopwatch;
        RadioButton ManualOffRadioBtn;

        System.Windows.Forms.Timer MouseDownTimer = new System.Windows.Forms.Timer();
        MainTimer mainTimer = new MainTimer();
        MainTimer WaiterTimer = new MainTimer();        

        Color ColorPaused = Color.LightYellow;
        Color ColorRunning = Color.LightGreen;
        Color ColorStopped = Color.Pink;
        Color ColorOriginal;

        Form parent;
        Prop1 p = Val.logocontroler.Prop1;

        MethodInvoker updateStopwatchTimeMethodInvoker;


        public StopWatch(Form Parent)
        {
            parent = Parent;
            sartBtnTextDeciderInvoker = new MethodInvoker(sartBtnTextDeciderMethod);
            // Finding controls by name from form
            btnStart = (Button)Parent.Controls.Find("btnStart", true)[0];
            btnStop = (Button)Parent.Controls.Find("btnStop", true)[0];                

            gbStopwatch = (GroupBox)Parent.Controls.Find("gbStoparica", true)[0];            
            ManualOffRadioBtn = (RadioButton)Parent.Controls.Find("ManualOffRadioBtn", true)[0];
            lblEstimateEnd = (Label)Parent.Controls.Find("lblEstimateEnd", true)[0];
            ////

            btnStart.Click += BtnStart_Click;
            btnStop.Click += BtnStop_Click;

            mainTimer.Elapsed += MainTimer_Tick;
            WaiterTimer.Elapsed += WaiterTimer_Tick;    

            estimateTimeLeftInvoker = new MethodInvoker(estimateTimeLeftMethod);
            ColorOriginal = gbStopwatch.BackColor;

            btnStop.Text = "---";

            StopwatchStarted += StopWatch_StopwatchStarted;
            StopwatchStopped += StopWatch_StopwatchStopped;
            StopwatchWasReset += StopWatch_StopwatchWasReset;
            StopwatchWasPaused += StopWatch_StopwatchWasPaused;

            ManualOffRadioBtn.Click += ManualOffRadioBtn_Click;

            updateStopwatchTimeMethodInvoker = new MethodInvoker(updateStopwatchTimeMethod);

            ts_timeSet = TimeSpan.FromSeconds(p.TimeSet.Value_short);
        }

        private void StopWatch_StopwatchWasPaused(StopWatch sender)
        {
            Val.logocontroler.Prop1.StopwatchStop.SendPulse();
        }

        private void ManualOffRadioBtn_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void StopWatch_StopwatchWasReset(StopWatch sender)
        {
            p.StopwatchReset.SendPulse();            
        }

        private void StopWatch_StopwatchStopped(StopWatch sender)
        {
            p.StopwatchStop.SendPulse();        

            if (selfControlThread != null)
            {
                selfControlThread.Abort();
            }    
            

        }

        private void StopWatch_StopwatchStarted(StopWatch sender)
        {
            p.StopwatchStart.SendPulse();            

            selfControlThread = new Thread(selfControlMethod);
            selfControlThread.Start();
        }

        bool wasStoppedBySelfControl = false;
        
        void selfControlMethod()
        {
            while (true)
            {
                updateStopwatchTime();

                if (mainTimer.Enabled)
                {
                    if (!p.TempDosezena.Value_bool)
                    {
                        if (FormControl.Gui.cbPauseIfTlow.Checked)
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

        private void MainTimer_Tick(object sender, EventArgs e)
        {        
            // time has elapsed
            if (p.StopwatchTime.Value_short >= p.TimeSet.Value_short)
            {
                // if heating enabled disable it
                var prop = Val.logocontroler.Prop1;               
                prop.On.Value = 0;
                prop.Off.Value = 1;                

                // stop stopwatch
                mainTimer.Stop();
                Counting = false;
                StopwatchStopped.Invoke(this);
                IsInProgress = false;
                gbStopwatch.BackColor = ColorStopped;

                // buzz
                prop.BuzzFromPcEndCycle.SendPulse();

                //
                IsInProgress = false;               
                parent.Invoke(new MethodInvoker(delegate { btnStop.Text = "Reset"; })); 
                
            }
            else
            {
                gbStopwatch.BackColor = ColorRunning;
                parent.Invoke(new MethodInvoker(delegate { btnStop.Text = "Pause"; }));
            }
            parent.Invoke(estimateTimeLeftInvoker);
            
        }

        MethodInvoker estimateTimeLeftInvoker;
        void estimateTimeLeftMethod()
        {
            var ts_txt = DateTime.Now + (TimeSpan.FromSeconds(p.TimeSet.Value_short) - TimeSpan.FromSeconds(p.StopwatchTime.Value_short));
            var txt = ts_txt.ToString("HH:mm");
            lblEstimateEnd.Text = txt;
        }
        
        void updateStopwatchTimeMethod()
        {
            var timeInSeconds = p.StopwatchTime.Value_short;
            var time = TimeSpan.FromSeconds(timeInSeconds);
            var txt = TimeToString(time);
            FormControl.Gui.lblStopwatchTime.Text = txt;
        }

        void updateStopwatchTime()
        {
            FormControl.Gui.Invoke(updateStopwatchTimeMethodInvoker);
        }

   
        MethodInvoker sartBtnTextDeciderInvoker;
        void sartBtnTextDeciderMethod()
        {
            if (!mainTimer.Enabled && !WaiterTimer.Enabled)
            {
                if (p.TimeSet.Value_short > p.StopwatchTime.Value_short)
                {
                    btnStart.Text = "Start";
                }
            }            
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
                StopwatchWasPaused.Invoke(this);
                gbStopwatch.BackColor = ColorStopped;
                mainTimer.Stop();
                Counting = false;
                parent.Invoke(new MethodInvoker(delegate {
                    btnStop.Text = "Reset";
                }));
            }
            else if(IsInProgress) // main timer is stopped AND IsInProgress
            {
                if (MessageBox.Show("Sarža še ni zaključena, ali vseeno želite ponastaviti štoparico? ","POZOR!",MessageBoxButtons.YesNo) == DialogResult.Yes)
                {   
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
                Counting = true;
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

            parent.Invoke(estimateTimeLeftInvoker);
            
        }

        public void Pause()
        {
            if (mainTimer != null)
            {
                mainTimer.Stop();
                Counting = false;
            }
        }

        public void Resume()
        {
            if (mainTimer != null)
            {
                mainTimer.Start();
                Counting = true;
            }            
        }

        public void Stop()
        {
            BtnStop_Click(null,null);
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

    }
}
