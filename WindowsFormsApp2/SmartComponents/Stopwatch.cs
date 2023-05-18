using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Timers;
using Timer = System.Timers.Timer;

namespace WindowsFormsApp2
{

    public class StopWatch
    {
        Timer ButtonTextUpdater;

        public bool IsInProgress  { get; private set; }
        public bool Counting { get; private set; } = false;
        Button btnStart;   
        Button btnStop;

        Label lblEstimateEnd;
        TimeSpan ts_timeSet;        
        GroupBox gbStopwatch;
        RadioButton ManualOffRadioBtn;

        System.Windows.Forms.Timer MouseDownTimer = new System.Windows.Forms.Timer();              

        Color ColorPaused = Color.Yellow;
        Color ColorRunning = Color.LightGreen;
        Color ColorStopped = Color.Red;
        Color ColorOriginal;

        Form parent;
        Prop1 p = Val.logocontroler.Prop1;

        MethodInvoker updateStopwatchTimeMethodInvoker;

        readonly string btnTxt_Stop = "STOP";
        readonly string btnTxt_Start = "START";
        readonly string btnTxt_empty = "---";
        readonly string btnTxt_Reset = "RESET";


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
            ColorOriginal = gbStopwatch.BackColor;

            ManualOffRadioBtn.Click += ManualOffRadioBtn_Click;

            updateStopwatchTimeMethodInvoker = new MethodInvoker(updateStopwatchTimeMethod);

            ts_timeSet = TimeSpan.FromSeconds(p.TimeSet.Value_short);

            ButtonTextMethodInvoker = new MethodInvoker(ButtonText);

            ButtonTextUpdater = new Timer(50);
            ButtonTextUpdater.Elapsed += ButtonTextUpdater_Elapsed;
            ButtonTextUpdater.Start();
        }

        private void ButtonTextUpdater_Elapsed(object sender, ElapsedEventArgs e)
        {
            ButtonText();
        }

        private void ManualOffRadioBtn_Click(object sender, EventArgs e)
        {
            Stop();
        }

        MethodInvoker ButtonTextMethodInvoker;
        void ButtonText()
        {
            if (FormControl.Gui.InvokeRequired)
            {
                FormControl.Gui.Invoke(ButtonTextMethodInvoker);
            }
            else
            {
                if (p.StopwatchRunning.Value_bool) // running
                {
                    btnStop.Text = btnTxt_Stop;
                    btnStart.Text = btnTxt_empty;

                    gbStopwatch.BackColor = ColorRunning;
                }
                else if (p.StopwatchStopped.Value_bool) // stopped
                {
                    btnStop.Text = btnTxt_Reset;
                    btnStart.Text = btnTxt_Start;
                }
                else if (p.StopwatchPaused.Value_bool) // paused
                {
                    if (p.StopwatchTime.Value_short >= p.TimeSet.Value_short && p.StopwatchTime.Value_short > 0) // paused - Timer Elapsed
                    {
                        btnStop.Text = btnTxt_Reset;
                        btnStart.Text = btnTxt_empty;
                        gbStopwatch.BackColor = ColorStopped;
                    }
                    else // Paused - manual
                    {
                        btnStop.Text = btnTxt_Reset;
                        btnStart.Text = btnTxt_Start;
                        gbStopwatch.BackColor = ColorOriginal;
                    }

                }
                else if (p.HeatingUp.Value_bool) // heating up
                {
                    btnStop.Text = btnTxt_Stop;
                    btnStart.Text = btnTxt_empty;

                    // blink stopwatch
                    if (p.Blink_HeatingUp.Value_bool)
                    {
                        gbStopwatch.BackColor = ColorPaused;
                    }
                    else
                    {
                        gbStopwatch.BackColor = ColorOriginal;
                    }
                }
                
            }
            
        }
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
            //if (!mainTimer.Enabled && !WaiterTimer.Enabled)
            //{
            //    if (p.TimeSet.Value_short > p.StopwatchTime.Value_short)
            //    {
            //        btnStart.Text = "Start";
            //    }
            //}            
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            if (btnStop.Text == btnTxt_Reset)
            {
                p.StopwatchReset.SendPulse();
            }

            else if (btnStop.Text == btnTxt_Stop)
            {
                p.StopwatchStop.SendPulse();
            }
           
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (p.TimeSet.Value_short == 0)
            {
                MessageBox.Show("Nastavite željeni čas!"); 
                return;
            }
            if (btnStart.Text == btnTxt_Start)
            {
                p.StopwatchStart.SendPulse();
            }

            else if (btnStart.Text == btnTxt_empty)
            {
                // ignore
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
