using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public class TimeSelector : DataGridViewComboBoxCell
    {
        List<string> datasource = new List<string>();

        string[] mins = { "00", "15", "30", "45" };
        string[] hrs = { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23" };

        public TimeSelector()
        {
            datasource.Add(PropComm.NA);
            foreach (var item in hrs)
            {
                foreach (var it in mins)
                {
                    datasource.Add(item + ":" + it);
                }
            }
            Value = datasource[0];
            DataSource = datasource;
        }
    }
}
