using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;


namespace WindowsFormsApp2
{

    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 



        public static Context con;

        [STAThread]
        static void Main()
        {
            // HighDpi help
            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();

            Misc.CheckAndKillAnotherInstance();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            con = new Context();
            FormControl Form_splash = new FormControl(con); Form_splash.Show();
            Thread Starter = new Thread(Form_splash.Starter_DoWork);
            Starter.Name = "Starter Thread";
            Starter.SetApartmentState(ApartmentState.STA);
            Starter.IsBackground = true;
            Starter.Start();
            try
            {
                WinApi.TimeBeginPeriod(1); // increase resolution for thread sleep
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Application could not set timer resolutions to 1ms. " +
                    "Default value will be used. This could result in poor network performance(data retrievial). " +
                    "If you expirience disconnecting clients or similar issues, please decreese pool time.");
            }

            Application.Run(con);
            if (con.Restart) // for restarting application
            {
                Application.Restart();
                Process.GetCurrentProcess().Kill();
            }
            
        }

        public static Context GetContext()
        {
            return con;
        }

        // HighDpi help
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
    

    public class Context : ApplicationContext
    {
        public bool Restart = false;
    }

    

}
