using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Windows.Forms;

namespace WindowsFormsApp2
{

    public static class Val // used to hold values that are same for all instances / users
    {       
        public static LogoControler logocontroler;       
        public static PC_WD PCWD;
        public static WarningManager WarningManager;       

        public static string PathLogFIle = "";
        public static string PathUserActions = "";
        public static string PathTemperatures = "";
        public static string BaseDirectoryPath = "";

        public static bool GuiInitialised = false;
        
        
        public static void InitialiseClass()
        {
            // TODO solve this
            //SysLog.Message = new SysLog.MessageManager();

            XmlController.XmlControllerInitialize();            
            WarningManager = new WarningManager();              // in new thread
            logocontroler = new LogoControler();                // in new thread
            PCWD = new PC_WD();                                 // in new thread
  
        }

       
        public static void InitializeWDTable(int device)
        {
          
                if (XmlController.GetEnabledLogo(device))
                {
                PropComm.SetWatchdogValue("Not running", device);
                }
                else
                {
                    PropComm.SetWatchdogValue("Disabled", device);
                }
            
        }
                               
      
        
                
    }
}