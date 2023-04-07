using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace WindowsFormsApp2
{
    class Zracenje
    {
        SysTimer DelayStopZracenja;
        Thread Zracenje_Thread;
        StopWatch stpw;
        public int Ontime_s 
        { get; set; } = 0;
        public int Offtime_s 
        { get; set; } = 0;

        Prop2 p2;

        public Zracenje()
        {
            stpw = Val.StopWatch;
            stpw.StopwatchStarted += Stpw_StopwatchStarted;
            stpw.StopwatchStopped += Stpw_StopwatchStopped;        

            Ontime_s = 0;
            Offtime_s = 0;

            p2 = Val.logocontroler.Prop2;
           
            

        }

        private void Stpw_StopwatchStopped(StopWatch sender)
        {
            DelayStopZracenja = new SysTimer(new TimeSpan(0,10 ,0).TotalMilliseconds);
            DelayStopZracenja.AutoReset = false;
            DelayStopZracenja.Start();
            DelayStopZracenja.Elapsed += DelayStopZracenja_Elapsed;
        }

        private void DelayStopZracenja_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Zraci(false);
        }

        private void Stpw_StopwatchStarted(StopWatch sender)
        {
            Zraci(true);

        }


        public void Zraci(bool set)
        {
            if (set)
            {
                p2.MinZracenje.Value_short = 1;
            }
            else
            {
                p2.MinZracenje.Value_short = 0;
            }
        }
    }
}
