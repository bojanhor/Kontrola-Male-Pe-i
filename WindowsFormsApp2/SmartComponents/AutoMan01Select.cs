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
    class AutoMan01Select : Control
    {

        Thread updaterThread;

        bool WriteMode = false;
        
        RadioButton btn1;
        RadioButton btn2;
        RadioButton btn3;      
                
        public string Text_Auto
        {
            get { return btn1.Text; }
            set { btn1.Text = value; }
        }

        public string Text_Man0
        {
            get { return btn2.Text; }
            set { btn2.Text = value; }
        }

        public string Text_Man1
        {
            get { return btn3.Text; }
            set { btn3.Text = value; }
        }

        PlcVars.Word value_Auto;
        public PlcVars.Word Value_Auto
        {
            get
            {
                if (value_Auto == null && !designMode)
                {
                    MessageBox.Show("You should provide Values to AutoMan01Select to operate normaly.");
                }

                return value_Auto;
            }

            set { value_Auto = value; }
        }

        PlcVars.Word value_Man0;
        public PlcVars.Word Value_Man0
        {
            get
            {
                if (value_Man0 == null && !designMode)
                {
                    MessageBox.Show("You should provide Values to AutoMan01Select to operate normaly.");
                }

                return value_Man0;
            }

            set { value_Man0 = value; }
        }

        PlcVars.Word value_Man1;
        public PlcVars.Word Value_Man1
        {
            get
            {
                if (value_Man1 == null && !designMode)
                {
                    MessageBox.Show("You should provide Values to AutoMan01Select to operate normaly.");
                }

                return value_Man1;
            }

            set { value_Man1 = value; }
        }

        bool designMode = false;

        public AutoMan01Select()
        {
            CreateControls();
            names();
            positionControls();
            AddBtns();
            designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
            if (!designMode)
            {
                RegisterEvents();
                ReadFromPlc();
            }           
        }

        void ReadFromPlc()
        {
            updaterThread = new Thread(update);
            updaterThread.Start();
        }

        void UncheckAll()
        {
            btn1.Checked = false;
            btn2.Checked = false;
            btn3.Checked = false;            
        }

        bool WordToBool(short? val)
        {
            if (val == null)
            {
                return false;
            }
            else if (val == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        void update()
        {
            var m = new MethodInvoker(delegate
            {
                if (!WriteMode)
                {
                    if (WordToBool(Value_Auto.Value) && !WordToBool(Value_Man0.Value) && !WordToBool(Value_Man1.Value))
                    {
                        btn1.Checked = true;
                    }
                    else if (!WordToBool(Value_Auto.Value) && WordToBool(Value_Man0.Value) && !WordToBool(Value_Man1.Value))
                    {
                        btn2.Checked = true;
                    }
                    else if (!WordToBool(Value_Auto.Value) && !WordToBool(Value_Man0.Value) && WordToBool(Value_Man1.Value))
                    {
                        btn3.Checked = true;
                    }
                    else
                    {
                        UncheckAll();
                    }
                }
                else
                {
                    WriteMode = false;
                }                

            });

            while (Parent == null)
            {
                Thread.Sleep(100);
            }

            Thread.Sleep(1000);

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

        void names()
        {
            Text_Auto = "AUTO";
            Text_Man0 = "OFF";
            Text_Man1 = "ON";
        }

        void CreateControls()
        {
            btn1 = new RadioButton();
            btn2 = new RadioButton();
            btn3 = new RadioButton();

        }

        void positionControls()
        {
            int left = 10;
            int spacing = 23;
            int top = 5;

            // Container
            Height = top * 2 + 3 * spacing;
            Width = 100;

            // Buttons
            btn1.Left = left; btn1.Top = top; top += spacing;
            btn2.Left = left; btn2.Top = top; top += spacing;
            btn3.Left = left; btn3.Top = top; top += spacing;

        }

        void AddBtns()
        {
            Controls.Add(btn1);
            Controls.Add(btn2);
            Controls.Add(btn3);
        }

        void RegisterEvents()
        {
            btn1.CheckedChanged += Btn1_CheckedChanged;
            btn2.CheckedChanged += Btn2_CheckedChanged;
            btn3.CheckedChanged += Btn3_CheckedChanged;
        }               
       
        private void Btn3_CheckedChanged(object sender, EventArgs e)
        {
            WriteMode = true;
            Value_Auto.Value = 0; Value_Man0.Value = 0; Value_Man1.Value = 1;
        }

        private void Btn2_CheckedChanged(object sender, EventArgs e)
        {
            WriteMode = true;
            Value_Auto.Value = 0; Value_Man0.Value = 1; Value_Man1.Value = 0;
        }

        private void Btn1_CheckedChanged(object sender, EventArgs e)
        {
            WriteMode = true;
            Value_Auto.Value = 1; Value_Man0.Value = 0; Value_Man1.Value = 0;
        }
    }
}
