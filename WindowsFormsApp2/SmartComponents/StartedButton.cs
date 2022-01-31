using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace WindowsFormsApp2
{
    public class StartedButton : Button
    {
        
        public int ID { get;}
        public string Showname { get; set; }
        public int StartedStatus { get; set; }
        int refreshOriginalVal = 500;
        private Bitmap startedIcon;
        private Bitmap stoppedIcon; 
        private Bitmap disabledIcon;
        private int connstatcnt = 0;

        public StartedButton(int ID, int Height, int Width, int Top)
        {            
            this.ID = ID;
            this.Height = Height;
            this.Width = Width;
            this.Top = Top;
            
            StartedStatus = (int)StatedStatus.Disabled;
            Enabled = false;

            stoppedIcon = Properties.Resources.Stop;
            startedIcon = Properties.Resources.Start;
            disabledIcon = Properties.Resources.disconnected;
            
            stoppedIcon = Misc.Scale(stoppedIcon, this.Height*0.75F);
            startedIcon = Misc.Scale(startedIcon, this.Height * 0.75F);
            disabledIcon = Misc.Scale(disabledIcon, this.Height * 0.75F);
            this.BackgroundImageLayout = ImageLayout.Center;
            BackColor = Color.Transparent;
            BackgroundImage = disabledIcon;

            // object own status retriever
            BackgroundWorker updater = new BackgroundWorker();
            updater.DoWork += new DoWorkEventHandler(Updater);
            updater.RunWorkerAsync();

            // on click
            this.Click += Clicked;
            
        }

        public enum StatedStatus : int
        {
            Started = 0,
            Stopped = -1,
            Disabled = -2
        }
        

        private void Clicked(object sender, EventArgs e)
        {
            
            if (StartedStatus == (int)StatedStatus.Started)
            {
                Stop();                
            }
            else if (StartedStatus == (int)StatedStatus.Stopped)
            {
                Start();                
            }
            else 
            {
                StartedStatus = (int)StatedStatus.Disabled;
            }
            
        }

        public void Start()
        {  

        }

        public void Stop()
        {
           
        }

        public void UpdateConnectionStatus()
        {
            bool en = Enabled;

            if (StartedStatus == (int)StatedStatus.Started)
            {
                if (connstatcnt >= 3)
                {
                    BackgroundImage = startedIcon; en = true;
                }
                connstatcnt++;                
            }

            else if (StartedStatus == (int)StatedStatus.Stopped)
            {
                if (connstatcnt >= 3)
                {
                    BackgroundImage = stoppedIcon; en = true;                    
                }
                connstatcnt++;               
            }

            else if (StartedStatus == (int)StatedStatus.Disabled)
            {
                BackgroundImage = disabledIcon; en = false;
                connstatcnt = 0;
            }

            if (en != Enabled)
            {
                Invoke(new MethodInvoker(delegate { Enabled = en; }));
            }
        }
               

        public void RetrieveConnectionStatus()
        {
            
        }

        public void Updater(object sender, DoWorkEventArgs e)
        {            
            DateTime t1;           
            while (true)
            {
                try
                {
                    t1 = DateTime.Now.AddMilliseconds(refreshOriginalVal/2);
                    RetrieveConnectionStatus();
                    UpdateConnectionStatus();
                    while (DateTime.Now < t1)
                    {                        
                        //Application.DoEvents();
                        System.Threading.Thread.Sleep(100);
                    }
                }
                catch 
                {
                    
                }
                
                
            }
            
        }

       
        
    }
}
