using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    class TextboxShow : TextBox
    {

        Thread UpdaterThread;

        public string Prefix { get; set; }
        public string Postfix { get; set; }


        private PlcVars.Word value_;

        public PlcVars.Word Value
        {
            get
            {               
                return value_;
            }
            set
            {
                value_ = value;
            }
        }

    
        public TextboxShow()
        {
            ReadOnly = true;
            bool designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);


            if (!designMode)
            {
                UpdaterThread = new Thread(updater);
                UpdaterThread.Start();
            }
        }

        void updater()
        {
           

            var m = new MethodInvoker(delegate 
            {
                Text = Prefix + Value.Value_string + Postfix;
            });

            while (Parent == null)
            {
                Thread.Sleep(100);
            }

            Thread.Sleep(1000);

            if (value_ == null)
            {
                MessageBox.Show("You need to pass Value to TextboxShow control in order to work.");
            }

            while (true)
            {
                try
                {
                    Parent.Invoke(m);
                }
                catch 
                {

                }
                Thread.Sleep(Settings.UpdateValuesPCms);
            }
        
        }
    }

}

        

