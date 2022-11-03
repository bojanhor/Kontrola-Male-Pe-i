using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    class TimeToGoMinutes_1_30 : SelectorBaseClass
    {
        static int from = 1;
        static int to = 30;
        static int step = 1;
        static string postFix = "min";


        public TimeToGoMinutes_1_30() : base(from, to, step, postFix)
        {

        }
    }

    class HistHeat2_10 : SelectorBaseClass
    {
        static int from = 2;
        static int to = 10;
        static int step = 2;
        static string postFix = "°C";


        public HistHeat2_10() : base(from, to, step, postFix)
        {

        }
    }

    class TemperatureDifference_5_30 : SelectorBaseClass
    {
        static int from = 2;
        static int to = 15;
        static int step = 2;
        static string postFix = "°C";


        public TemperatureDifference_5_30() : base(from, to, step, postFix)
        {

        }
    }
    class TemperatureSelector_0_250 : SelectorBaseClass
    {
        static int from =  30;
        static int to = 250;
        static int step = 10;
        static string postFix = "°C";

       
        public TemperatureSelector_0_250() : base(from, to, step, postFix)
        {

        }
    }

    class TemperatureSelector_0_350 : SelectorBaseClass
    {
        static int from = 100;
        static int to = 350;
        static int step = 10;
        static string postFix = "°C";


        public TemperatureSelector_0_350() : base(from, to, step, postFix)
        {

        }
    }

    class RpmSelector_50_100 : SelectorBaseClass
    {
        static int from = 50;
        static int to = 100;
        static int step = 5;
        static string postFix = "%";


        public RpmSelector_50_100() : base(from, to, step, postFix)
        {

        }
    }

    class RpmSelector_80_100 : SelectorBaseClass
    {
        static int from = 80;
        static int to = 100;
        static int step = 5;
        static string postFix = "%";


        public RpmSelector_80_100() : base(from, to, step, postFix)
        {

        }
    }

    class SelectorBaseClass : ComboBox
    {
        public PlcVars.Word Value;
        Thread updater;
        List<string> datasource = new List<string>();
        string postFix;
        bool WriteMode = false;
        bool dropdownOpened = false;

        public SelectorBaseClass(int from, int to, int step, string postFix)
        {            
            this.postFix = postFix;

           
            DropDownStyle = ComboBoxStyle.DropDownList;

            bool designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
            if (!designMode)
            {
                populateDatasource(from, to, step, postFix);
                populateCB();

                updater = new Thread(update);
                updater.Start();
               
                this.DropDownClosed += TemperatureSelectorBaseClass_DropDownClosed;
                this.DropDown += TemperatureSelectorBaseClass_DropDown;
            }
        }

        private void TemperatureSelectorBaseClass_DropDown(object sender, EventArgs e)
        {
            WriteMode = true;
            dropdownOpened = true;
        }

        private void TemperatureSelectorBaseClass_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                WriteMode = true;
                var val = datasource[SelectedIndex];
                val = val.Replace(postFix, "");
                var value = Convert.ToInt16(val);
                Value.Value = value;
            }
            catch
            { }
            dropdownOpened = false;
        }      

        void populateDatasource(int from, int to, int step, string postFix)
        {
            string buff = ""; 
            for (int i = from; i <= to; i+= step)
            {
                buff = i.ToString() + postFix;

                if (!datasource.Contains(buff))
                {
                   datasource.Add(buff);
                }
                
            }
        }

        void populateCB()
        {
            for (int i = 0; i < datasource.Count; i++)
            {
                if (!Items.Contains(datasource[i]))
                {
                    Items.Add(datasource[i]);
                }
                
            }            
        }
       
        void update()
        {

            var m = new MethodInvoker(delegate 
            {                 
                findVal(Value.Value_string);
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
                    if (Value != null)
                    {
                        if (WriteMode)
                        {
                            if (!dropdownOpened)
                            {
                                WriteMode = false;
                            }                            
                        }
                        else
                        {
                            Parent.Invoke(m);
                        }
                        
                    }
                    else
                    {
                        MessageBox.Show("You need to provide property named Value for TemperatureSelector to operate.");
                    }
                }
                catch
                {
                    
                }
                Thread.Sleep(Settings.UpdateValuesPCms);
            }
        }

        void findVal(string val)
        {
            for (int i = 0; i < datasource.Count; i++)
            {
                if (datasource[i] == val + postFix)
                {
                    SelectedIndex = i;
                    return;
                }
                
                
            }
            SelectedIndex = -1;
        }
    }
}
