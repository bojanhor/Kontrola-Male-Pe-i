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
    class SensorSelect : Control
    {

        Thread updaterThread;

        bool WriteMode = false;
        short previousVal = 0;

        RadioButton btn1;
        RadioButton btn2;
        RadioButton btn3;
        RadioButton btn4;
        RadioButton btn5;
        RadioButton btn6;

        Color ErrColor = Color.Red;
        Color NormalColor;

        public string SensorName1
        {
            get { return btn1.Text; }
            set { btn1.Text = value; }
        }

        public string SensorName2
        {
            get { return btn2.Text; }
            set { btn2.Text = value; }
        }

        public string SensorName3
        {
            get { return btn3.Text; }
            set { btn3.Text = value; }
        }

        public string SensorName4
        {
            get { return btn4.Text; }
            set { btn4.Text = value; }
        }

        public string SensorName5
        {
            get { return btn5.Text; }
            set { btn5.Text = value; }
        }

        public string SensorName6
        {
            get { return btn6.Text; }
            set { btn6.Text = value; }
        }


        private PlcVars.Word value_;

        public PlcVars.Word Value
        {
            get
            {
                if (value_ == null && !designMode)
                {
                    MessageBox.Show("You should provide Value to SensorSelect to operate normaly.");
                }

                return value_;
            }

            set { value_ = value; }
        }

        public PlcVars.Bit Value_SelectedSensorError { get; set; }

        bool designMode = false;


        public SensorSelect()
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

            disableUnused();

            NormalColor = btn1.BackColor; // pull normal color info
        }

        void disableUnused()
        {
            btn2.Enabled = false; btn3.Enabled = false;
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
            btn4.Checked = false;
            btn5.Checked = false;
            btn6.Checked = false;
        }

        void SelectedSensorErrorMark()
        {
            if (Value_SelectedSensorError != null)
            {
                if (Value_SelectedSensorError.Value_bool)
                {
                    btn1.BackColor = NormalColor; btn2.BackColor = NormalColor; 
                    btn3.BackColor = NormalColor; btn4.BackColor = NormalColor; 
                    btn5.BackColor = NormalColor; btn6.BackColor = NormalColor;

                    switch (Value.Value_short)
                    {
                        case 1: btn1.BackColor = ErrColor; break;
                        case 2: btn2.BackColor = ErrColor; break;
                        case 3: btn3.BackColor = ErrColor; break;
                        case 4: btn4.BackColor = ErrColor; break;
                        case 5: btn5.BackColor = ErrColor; break;
                        case 6: btn6.BackColor = ErrColor; break;
                        default:
                            break;
                    }
                }
                else
                {
                    btn1.BackColor = NormalColor;
                    btn2.BackColor = NormalColor;
                    btn3.BackColor = NormalColor;
                    btn4.BackColor = NormalColor;
                    btn5.BackColor = NormalColor;
                    btn6.BackColor = NormalColor;
                }
            }
        }
        void update()
        {
            var m = new MethodInvoker(delegate 
            {
                if (!WriteMode)
                {
                    if (Value.Value_short != previousVal)
                    {
                        switch (Value.Value_short)
                        {
                            case 1: UncheckAll(); btn1.Checked = true; break;
                            case 2: UncheckAll(); btn2.Checked = true; break;
                            case 3: UncheckAll(); btn3.Checked = true; break;
                            case 4: UncheckAll(); btn4.Checked = true; break;
                            case 5: UncheckAll(); btn5.Checked = true; break;
                            case 6: UncheckAll(); btn6.Checked = true; break;
                            default:
                                UncheckAll();
                                break;
                        }
                    }
                    previousVal = Value.Value_short;
                }
                else
                {
                    WriteMode = false;
                }

                SelectedSensorErrorMark();
                
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
            SensorName1 = "Sensor1";
            SensorName2 = "Sensor2";
            SensorName3 = "Sensor3";
            SensorName4 = "Sensor4";
            SensorName5 = "Kanal";
            SensorName6 = "Kos";
        }

        void CreateControls()
        {
            btn1 = new RadioButton();
            btn2 = new RadioButton();
            btn3 = new RadioButton();
            btn4 = new RadioButton();
            btn5 = new RadioButton();
            btn6 = new RadioButton();
        }
                
        void positionControls()
        {
            int left = 10;
            int spacing = 30;
            int top = 5;
                        
            // Container
            Height = top * 2 + 5 * spacing;
            Width = 150;

            // Buttons
            btn1.Left = left; btn1.Top = top; top += spacing;
            btn2.Left = left; btn2.Top = top; top += spacing;
            btn3.Left = left; btn3.Top = top; top += spacing;
            btn4.Left = left; btn4.Top = top; top += spacing;
            btn5.Left = left; btn5.Top = top; top += spacing;
            btn6.Left = left; btn6.Top = top; top += spacing;

        }

        void AddBtns()
        {
            Controls.Add(btn1);
            Controls.Add(btn2);
            Controls.Add(btn3);
            Controls.Add(btn4);
            Controls.Add(btn5);
            Controls.Add(btn6);

        }

        void RegisterEvents()
        {
            btn1.CheckedChanged += Btn1_CheckedChanged;
            btn2.CheckedChanged += Btn2_CheckedChanged;
            btn3.CheckedChanged += Btn3_CheckedChanged;
            btn4.CheckedChanged += Btn4_CheckedChanged;
            btn5.CheckedChanged += Btn5_CheckedChanged;
            btn6.CheckedChanged += Btn6_CheckedChanged;
        }

        private void Btn6_CheckedChanged(object sender, EventArgs e)
        {
            WriteMode = true;
            Value.Value = 6;
        }

        private void Btn5_CheckedChanged(object sender, EventArgs e)
        {
            WriteMode = true;
            Value.Value = 5;
        }


        private void Btn4_CheckedChanged(object sender, EventArgs e)
        {
            WriteMode = true;
            Value.Value = 4;
        }

        private void Btn3_CheckedChanged(object sender, EventArgs e)
        {
            WriteMode = true;
            Value.Value = 3;
        }

        private void Btn2_CheckedChanged(object sender, EventArgs e)
        {
            WriteMode = true;
            Value.Value = 2;
        }

        private void Btn1_CheckedChanged(object sender, EventArgs e)
        {
            WriteMode = true;
            Value.Value = 1;
        }
    }
}
