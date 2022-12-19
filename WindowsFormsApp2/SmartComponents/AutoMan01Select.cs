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

        RadioButton rBtnAuto;
        RadioButton rBtnOff;
        RadioButton rBtnOn;
        
        public string Text_Auto
        {
            get { return rBtnAuto.Text; }
            set { rBtnAuto.Text = value; }
        }

        public string Text_Man0
        {
            get { return rBtnOff.Text; }
            set { rBtnOff.Text = value; }
        }

        public string Text_Man1
        {
            get { return rBtnOn.Text; }
            set { rBtnOn.Text = value; }
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
            rBtnAuto.Checked = false;
            rBtnOff.Checked = false;
            rBtnOn.Checked = false;
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
                        rBtnAuto.Checked = true;
                    }
                    else if (!WordToBool(Value_Auto.Value) && WordToBool(Value_Man0.Value) && !WordToBool(Value_Man1.Value))
                    {
                        rBtnOff.Checked = true;
                    }
                    else if (!WordToBool(Value_Auto.Value) && !WordToBool(Value_Man0.Value) && WordToBool(Value_Man1.Value))
                    {
                        rBtnOn.Checked = true;
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
            rBtnAuto = new RadioButton();
            rBtnOff = new RadioButton() {Name = "ManualOffRadioBtn" };
            rBtnOn = new RadioButton();

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
            rBtnAuto.Left = left; rBtnAuto.Top = top; top += spacing;
            rBtnOff.Left = left; rBtnOff.Top = top; top += spacing;
            rBtnOn.Left = left; rBtnOn.Top = top; top += spacing;

        }

        void AddBtns()
        {
            Controls.Add(rBtnAuto);
            Controls.Add(rBtnOff);
            Controls.Add(rBtnOn);
        }

        void RegisterEvents()
        {
            rBtnAuto.CheckedChanged += Btn1_CheckedChanged;
            rBtnOff.CheckedChanged += Btn2_CheckedChanged;
            rBtnOn.CheckedChanged += Btn3_CheckedChanged;
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

    
