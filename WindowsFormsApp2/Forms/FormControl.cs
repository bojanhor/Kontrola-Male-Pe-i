using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows;
using System.IO;
using System.Xml.Linq;
using System.Threading;

namespace WindowsFormsApp2
{
    // SPLASH SCREEN FORM
    public partial class FormControl : Form
    {

        public static StreamWriter sw_Main;
        public static StreamWriter sw_UserActions;
        public static SettingsForm Form_settings;
        public static Gui_MalaPec Gui; 
        public static Identify identify;
        public static Context con;
        public static Image backgrndImage = Properties.Resources.white_background_image;
        public static bool allowbttorun = false;
        public static bool LogoffModeEnabled = false;
        Helper.Initialiser Initialiser;

        public FormControl(Context con_)
        {
            CreateSplashSceen();

            con = con_;
            identify = new Identify();

            Initialiser = new Helper.Initialiser(); // Initializes multiple modules          

            var LogFilePath = Val.BaseDirectoryPath + "\\Log\\Log.txt";
            if (File.Exists(LogFilePath))
            {
                Val.PathLogFIle = LogFilePath;
            }
            else
            {
                throw new Exception("Log file is missing in directory \"Log\". " + "Put file named \"Log.txt\" in that folder to overcome this error." );
            }

            try
            {
                if (LogFilePath == "" || LogFilePath == null || LogFilePath == "ERR")
                {
                    throw new Exception();
                }
                else
                {
                    sw_Main = new StreamWriter(LogFilePath, true, Encoding.UTF8);
                    sw_Main.WriteLine(Environment.NewLine + Environment.NewLine + "Streamwriter was initialised.");
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Problem ocurded while initialising streamwriter." + Environment.NewLine + " Error message: " + e.Message + Environment.NewLine + "Application will now close.");
                Environment.Exit(0);
            }


            // UserActionsLog CsvFile load
            var UserActionsFilePath = Val.BaseDirectoryPath + "\\Log\\UserActions.csv";
            if (File.Exists(UserActionsFilePath))
            {
                Val.PathUserActions = UserActionsFilePath;
            }
            else
            {
                throw new Exception("UserActions file is missing in directory \"Log\". " + "Put file named \"UserActions.csv\" in that folder to overcome this error.");
            }
            

            try
            {
                if (UserActionsFilePath == "" || UserActionsFilePath == null || UserActionsFilePath == "ERR")
                {
                    throw new Exception();
                }
                else
                {
                    sw_UserActions = new StreamWriter(UserActionsFilePath, true, Encoding.UTF8);
                    sw_UserActions.WriteLine(Environment.NewLine + Environment.NewLine + DateTime.Now.ToString() + ";;;Streamwriter was initialised.");
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Problem ocurded while initialising streamwriter." + Environment.NewLine + " Error message: " + e.Message + Environment.NewLine + "Application will now close.");
                Environment.Exit(0);
            }


            // Temperature Log CsvFile load

            var TemperatureLogFilePath = Val.BaseDirectoryPath + "\\Log\\Temperatures.csv";
            if (File.Exists(TemperatureLogFilePath))
            {
                Val.PathTemperatures = TemperatureLogFilePath;
            }
            else
            {
                throw new Exception("Temperatures file is missing in directory \"Log\". " + "Put file named \"Temperatures.csv\" in that folder to overcome this error.");
            }

            
            
        }

        public static void SaveXML()
        {
            int i;
            Exception last = new Exception();
            try
            {
                for (i = 0; i < 3; i++)
                {
                    try
                    {
                        if (!IsFileLocked(new FileInfo(Properties.Settings.Default.PathXML)))
                        {
                            XmlController.XmlFile.Save(Properties.Settings.Default.PathXML);
                            return;
                        }
                    }
                    catch (Exception e) { last = e; }
                }

                if (i >= 3)
                {
                    throw last;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cant Save XML File after 3 attempts: " + ex.Message);
            }

        }

        protected static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }


        public static Screen GetScreen(int requestedScreen)
        {
            var screens = Screen.AllScreens;
            var mainScreen = 0;
            if (screens.Length > 1 && mainScreen < screens.Length)
            {
                return screens[requestedScreen];
            }
            return screens[0];
        }


        public static void HideForm_Settings()
        {
            try
            {
                Form_settings.Hide();
            }
            catch { }
            
        }

        public static void ShowForm_Settings()
        {
            Form_settings.Visible = true;
        }

        
        public void Starter_DoWork()
        {

            // forms init
            try
            {
                Form_settings = new SettingsForm();

            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: Settings were not initialised: " + ex);
                MessageBox.Show("ERROR: Settings were not initialised: " + ex);
            }
                      

            try
            {
                Form_settings.SettingsAfterjobs();
                Gui = new Gui_MalaPec();

            }
            catch (Exception ex)
            {
                MessageBox.Show("GUI was not initialised properly. Application will now close: " + ex.Message);
                Environment.Exit(0);
            }



            Application.DoEvents();
            
            Application.DoEvents();
            identify.Logoff();
            while (true)
            {
                if (identify.GetPermision(1))
                {
                    break;
                }
                else
                {
                    while (true)
                    {
                        identify.ShowIDDialog();
                        if (identify.GetPermision(1))
                        {
                            break;
                        }
                        identify.ShowPermissionError();
                    }
                }

            }
           

            Invoke(new MethodInvoker(delegate { TopMost = true; }));
                        
            Application.DoEvents();


            allowbttorun = true;

            Thread clsSplsh = new Thread(closeSplash);
            clsSplsh.Start();

            Application.Run();
        }

        private void closeSplash()
        {
            Thread.Sleep(100);
            System.Windows.Forms.Application.DoEvents();
            Thread.Sleep(100);
           
            System.Windows.Forms.Application.DoEvents();
            Thread.Sleep(500);
            Invoke(new MethodInvoker(delegate { this.Hide(); }));// Hide spalshscrreen               

        }

        private void FormControl_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        public static void CloseApp_Preparation()
        {

            sw_Main.Flush();
            sw_Main.Close();

            sw_UserActions.Flush();
            sw_UserActions.Close();
                       

        }

        public static void WL(string message, int device)
        {
            string msg;

            if (device == 0)
            {
                msg = message;
            }
            else
            {
                msg = "LOGO" + device + ": " + message;
            }

            SettingsForm.WL(msg, 0);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FormControl
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "FormControl";
            this.Text = "Kontrola Male Peči (Zagon)";
            this.ResumeLayout(false);

        }

        void CreateSplashSceen()
        {
            Label l = new Label() { Text = "...Zagon Programa... ", Top = 50, Left = 100, Font = new Font("Sans Serif", 20), Width = 300, Height = 100 };
            this.Controls.Add(l);

            this.Height = 250;
            this.Width = 500;
            TopMost = true;

            var r = Screen.FromControl(this).Bounds;

            Left = (r.Width / 2) - Width / 2;
            Top = (r.Height / 2) - Height / 2;

            FormBorderStyle = FormBorderStyle.None;
        }
    }
}
