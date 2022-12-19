using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace WindowsFormsApp2
{

    class MainTimer: System.Timers.Timer
    {        
        public bool Started { get; private set; }
        public MainTimer()
        {
            Interval = 1000;
        }

        public new virtual void Start()
        {
            Started = true;
            base.Start();
        }

        public new virtual void Stop()
        {
            Started = false;
            base.Stop(); 
        }
    }
}
