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
    class SensorStatus : Control
    {

        Color OkColor = Color.Green;
        Color ErrColor = Color.Red;
        Color ExceptionColor = Color.Orange;

        Label l = new Label();
        TextBox t = new TextBox();
        Thread UpdateThread;

        public PlcVars.Bit Value_PlcBit { get; set; }

        public override string Text
        {
            get { return l.Text; }
            set { l.Text = value; }
        }


        public SensorStatus()
        {
            Width = 150;
            Height = 25;

            l.Width = 120;
            l.Top = 4;
            Controls.Add(l);

            t.Width = 20;
            t.Height = 20;
            t.Left = l.Width + 10;
            t.BackColor = ExceptionColor;
            Controls.Add(t);

            bool designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);

            if (!designMode)
            {
                UpdateThread = new Thread(update);
                UpdateThread.Start();
            }


        }

        void update()
        {
            var ex = new MethodInvoker(delegate { t.BackColor = ExceptionColor; ; });
            var gn = new MethodInvoker(delegate { t.BackColor = OkColor; ; });
            var rd = new MethodInvoker(delegate { t.BackColor = ErrColor; ; });

            while (Parent == null || !Parent.IsHandleCreated)
            {
                Thread.Sleep(100);
            }
            Thread.Sleep(1000);
            while (true)
            {
                
                try
                {
                    if (Value_PlcBit == null || Value_PlcBit.Value == null)
                    {
                        Parent.Invoke(ex);
                    }
                    else if ((bool)Value_PlcBit.Value)
                    {                       
                        Parent.Invoke(rd);
                    }
                    else
                    {
                        Parent.Invoke(gn);
                    }
                }
                catch
                {
                    Parent.Invoke(ex);
                }

                Thread.Sleep(Settings.UpdateValuesPCms);
            }

        }


    }
}
