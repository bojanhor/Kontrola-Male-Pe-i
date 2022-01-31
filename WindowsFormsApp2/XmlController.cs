using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Security;
using System.Xml;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public static class XmlController
    {                        
        static string XmlNotEncriptedPath;
        static string XmlEncriptedPath;
        static string XmlEncriptedPath_tmp;
        static bool forceRefresh = false;
        public static bool encriptedMode = false;
        static bool savingFilePleaseWait = false;

        public static bool XmlControllerInitialized = false;

        // xml file
        private static XDocument _XmlFile;
        public static XDocument XmlFile
        {
            get
            {
                if (_XmlFile != null)
                {
                    return _XmlFile;
                }
                return null;
            }

            private set
            {
                _XmlFile = value;                
            }
        }

        // General section of xml file
        private static XElement _XmlGeneral;
        public static XElement XmlGeneral
        {
            get
            {
                return _XmlGeneral;
            }

            private set
            {
                _XmlGeneral = value;
            }
        }


        // LOGO connection section of xml file
        private static XElement _XmlConn;
        public static XElement XmlConn
        {
            get
            {
                return _XmlConn;
            }

            private set
            {
                _XmlConn = value;
            }
        }

        // Users section of xml file
        private static XElement _XmlUsr;
        public static XElement XmlUsr
        {
            get
            {
                return _XmlUsr;
            }

            private set
            {
                _XmlUsr = value;
            }
        }

        // Statistics section of xml file
        private static XElement _XmlStat;
        public static XElement XmlStat
        {
            get
            {
                return _XmlStat;
            }

            private set
            {
                _XmlStat = value;
            }
        }

        private static void setBaseDirPath()
        {
            
            Val.BaseDirectoryPath = Directory.GetParent(Directory.GetParent(Application.StartupPath).FullName).FullName;
        }

        private static XDocument LoadNotEncriptedXML(string XmlPath)
        {
            try
            {
                string read;

                using (StreamReader s = new StreamReader(XmlPath))
                {
                    read = s.ReadToEnd();
                }

                return XDocument.Parse(read);
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading xml from file (before encription): " + ex.Message);
            }
        }

        public static string DownloadConfigFile()
        {
            try
            {
                string read;

                using (StreamReader s = new StreamReader(XmlEncriptedPath))
                {
                    read = s.ReadToEnd();
                }

                return XmlEncription.Decrypt(read);

            }
            catch (Exception ex)
            {
                throw new Exception("Error loading xml from encripted file: " + ex.Message);
            }
        }

        public static string DownloadLogFile()
        {
            try
            {
                string read;

                using (StreamReader s = new StreamReader(SysLog.MessageManager.LogFilePath))
                {
                    read = s.ReadToEnd();
                }

                return read;

            }
            catch (Exception ex)
            {
                throw new Exception("Error loading log file: " + ex.Message);
            }
        }
        
        private static XDocument LoadAndDecriptXml()
        {
            try
            {
                string read;

                using (StreamReader s = new StreamReader(XmlEncriptedPath))
                {
                    read = s.ReadToEnd();
                }

                var decripted = XmlEncription.Decrypt(read);

                return XDocument.Parse(decripted, LoadOptions.PreserveWhitespace);
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading xml from encripted file: " + ex.Message);
            }

        }

        public static void SaveXML_User(string newContent)
        {
            SaveXML(newContent, true);
        }

        public static void SaveXML_Auto(string newContent)
        {
            SaveXML(newContent, false);
        }

        static void SaveXML(string newContent, bool ChangesFromUser)
        {
            string text;

            savingFilePleaseWait = true;

            try
            {
                text = Settings.XmlDeclaration + newContent;
                var encriptedText = XmlEncription.Encrypt(text);

                using (StreamWriter s = new StreamWriter(XmlEncriptedPath_tmp, false, Encoding.UTF8))
                {
                    s.Write(Environment.NewLine + encriptedText);
                    s.Flush();
                    s.Dispose();
                }

                if (ChangesFromUser)
                {
                    SysLog.SetMessage("ConfigFile was changed, by user.");
                }                
            }
            catch (Exception ex)
            {
                savingFilePleaseWait = false;
                var message = "Problem saving encripted config File." + ex.Message;                
                throw new Exception(message);
            }

            try
            {
                if (!encriptedMode)
                {
                    if (File.Exists(XmlNotEncriptedPath))
                    {
                        using (StreamWriter s = new StreamWriter(XmlNotEncriptedPath, false, Encoding.UTF8))
                        {
                            s.Write(text);
                            s.Flush();
                            s.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                savingFilePleaseWait = false;
                var message = "Problem saving unecripted config File." + ex.Message;                
                throw new Exception(message);
            }

            TmpToFile();
            savingFilePleaseWait = false;
        }

        static void TmpToFile()
        {
            try
            {
                if (File.Exists(XmlEncriptedPath_tmp))
                {
                    File.Copy(XmlEncriptedPath_tmp, XmlEncriptedPath,true);
                    File.Delete(XmlEncriptedPath_tmp);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Problem replacing tmp config file: " + ex.Message);
            }
        }

        static void FindFileAndEncript()
        {            
            XmlNotEncriptedPath = Val.BaseDirectoryPath +"\\"+ Settings.pathToConfigFile;
            XmlEncriptedPath = Val.BaseDirectoryPath + "\\" + Settings.pathToConfigFileEncripted;
            XmlEncriptedPath_tmp = Val.BaseDirectoryPath + "\\" + Settings.pathToConfigFileEncripted + "_tmp";

            var ii = "\"";

            // if there is Nonencripted config file (will be deleted automatically after publish)

            try
            {
                if (File.Exists(XmlNotEncriptedPath))
                {
                    if (File.Exists(XmlEncriptedPath))
                    {
                        try
                        {
                            File.Delete(XmlEncriptedPath);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Error deleting encripted config file: " + ex.Message);
                        }
                        
                    }

                    FileEncript(XmlNotEncriptedPath);
                }
                else
                {
                    if (!File.Exists(XmlEncriptedPath))
                    {
                        throw new Exception("Config file could not be found. Search was performed at locations: " + ii + XmlEncriptedPath + ii + " and " + ii + XmlNotEncriptedPath + ii + ".");
                    }                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Procedure failed: " + ex.Message);
            }
        }

        static void FileEncript(string XmlPath)
        {
            string xmlText;
            string encriptedText;

            try
            {
                xmlText = LoadNotEncriptedXML(XmlPath).ToString();
                encriptedText = XmlEncription.Encrypt(xmlText);

                using (StreamWriter s = new StreamWriter(XmlEncriptedPath))
                {
                    s.Write(encriptedText);
                    s.Flush();
                    s.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Config file encription failed: " + ex.Message);
            }

        }

        public static void XmlControllerInitialize()
        {
            try
            {
                setBaseDirPath(); // get XML path                

                FindFileAndEncript();

                XmlFile = LoadAndDecriptXml();
                SetClass();

                Misc.SmartThread refresher = new Misc.SmartThread(() => Refresher_Thread());
                refresher.Start("XmlRefresher", System.Threading.ApartmentState.MTA, true);

            }
            catch (Exception e)
            {
                var message = "Method XmlController() encountered an error with configuration file. " +
                    "Please copy proper xml file inside project folder and name it: XMLFile-Settings.xml. Error description:" + e.Message;                
                throw new Exception(message);
            }

        }

        static void Refresher_Thread()
        {
            DateTime dt1;
            XDocument newXml;


            while (true)
            {
                try
                {
                    dt1 = DateTime.Now;

                    if (!savingFilePleaseWait)
                    {
                        try
                        {
                            newXml = LoadAndDecriptXml();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Error loading XML file: " + ex.Message);
                        }


                        if (newXml.Element("root").Value != XmlFile.Element("root").Value)
                        {
                            RefreshCache(newXml); // refresh if different
                        }

                        forceRefresh = false;  // reset flag (notifies other methods that fresh copy was aquired)
                    }                    


                    while (DateTime.Now < (dt1 + TimeSpan.FromMilliseconds(Settings.XmlRefreshrate))) // wait for some time
                    {
                        System.Threading.Thread.Sleep(Settings.defaultCheckTimingInterval);

                        if (forceRefresh)  // periodically check for force refresh flag
                        {
                            break;
                        }
                    }

                    XmlControllerInitialized = true;
                    System.Threading.Thread.Sleep(100); // mandatory wait
                }
                catch (Exception ex)
                {
                    throw new Exception("Error while refreshing data from xml file. Please check XML path and data. More info: location of error - Refresher_Thread() method - Error message: " + ex.Message);
                }

            }
        }

        static void RefreshCache(XDocument FreshLoadedXML)
        {
            XmlFile = FreshLoadedXML;
            SetClass();
            Helper.XML_Was_Changed();
        }

        public static string GetXMLTextAndStopRefreshing()
        {
            return XmlFile.ToString();
        }

        public static void SaveCurrentTB(string value)
        {
            SaveXML_User(value);
        }

        public static void RefreshFile_readAgain()
        {
            forceRefresh = true;

            while (forceRefresh)
            {
                System.Threading.Thread.Sleep(Settings.defaultCheckTimingInterval);
            }
        }

        private static void SetClass()
        {
            try
            {
                XmlGeneral = XmlFile.Element("root").Element("GENERAL");                
                XmlConn = XmlFile.Element("root").Element("CONNECTION");
                XmlUsr = XmlFile.Element("root").Element("USERS");
                XmlStat = XmlFile.Element("root").Element("STATS");                
            }

            catch (Exception)
            {
                throw;
            }
        }

        // PUBLIC

        public static IPAddress GetLogoIP(int n)
        {
            if (n < 0 || n > Settings.Devices)
            {
                throw new Exception("getLogoIP() method internal error. Index out of range");
            }

            try
            {
                var IP = XmlConn.Element("LOGO" + n).Element("serverIP").Value;

                if (!string.IsNullOrEmpty(IP) && IPAddress.TryParse(IP, out IPAddress result))
                {
                    return result;
                }

                else
                {
                    throw new Exception("IP addres in config file is not valid IP. " +
                        "Correct the IP address in XMLFile-Settings.xml file at LOGO" + n + " entry");
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public static string GetLogoLocalTsap(int n)
        {
            if (n < 0 || n > Settings.Devices)
            {
                throw new Exception("getLogoLocalTsap() method internal error. Index out of range");
            }

            try
            {
                var LocalTSAP = XmlConn.Element("LOGO" + n).Element("localTSAP").Value;

                if (LocalTSAP.Length != 5 && !LocalTSAP.Contains("."))
                {
                    throw new Exception("LocalTSAP addres in config file is not valid LocalTSAP. " +
                        "Correct the LocalTSAP address in XMLFile-Settings.xml file at LOGO" + n + " entry. format must be ##.## (03.00).");
                }

                return LocalTSAP;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string GetLogoRemoteTsap(int n)
        {
            if (n < 0 || n > Settings.Devices)
            {
                throw new Exception("getLogoRemoteTsap() method internal error. Index out of range");
            }

            try
            {
                var RemoteTSAP = XmlConn.Element("LOGO" + n).Element("remoteTSAP").Value;

                if (RemoteTSAP.Length != 5 && !RemoteTSAP.Contains("."))
                {
                    throw new Exception("remoteTSAP addres in config file is not valid remoteTSAP. " +
                        "Correct the remoteTSAP address in XMLFile-Settings.xml file at LOGO" + n + " entry. format must be ##.## (02.00).");
                }

                return RemoteTSAP;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool IsDebugEnabled()
        {
            try
            {
                if (Convert.ToBoolean(XmlGeneral.Element("debugToConsole").Value))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {
                return true;
            }

        }

        public static PlcVars.DoubleWordAddress GetWDAddress(int device)
        {
            if (device < 0 || device > Settings.Devices)
            {
                throw new Exception("GetWDAddress() method internal error. Index out of range");
            }

            try
            {
                var xmlVal = XmlConn.Element("LOGO" + device).Element("watchdogAddress").Value;
                return new PlcVars.DoubleWordAddress(ushort.Parse(xmlVal));
            }
            catch (Exception)
            {
                throw new Exception(
                    "watchdogAddress value in config file is not valid watchdogAddress. " +
                    "Correct the watchdogAddress value in XMLFile-Settings.xml file at LOGO" + device + " entry. " +
                    "format must be 300.");
            }
        }

        public static bool GetEnabledLogo(int device)
        {

            try
            {
                if (device < 1 || device > Settings.Devices)
                {
                    throw new Exception();
                }
                else
                {
                    return Convert.ToBoolean(XmlConn.Element("LOGO" + device).Element("enabled").Value);
                }

            }
            catch (Exception)
            {

                throw new Exception(
                    "enabled value in config file is not valid enabled value. " +
                    "Correct the enabled value in XMLFile-Settings.xml file: at LOGO" + device + " entry. " +
                    "format must be true ore false.");
            }

        }

        public static int GetReadWriteCycle(int device)
        {
            try
            {
                if (device < 1 || device > Settings.Devices)
                {
                    throw new Exception();
                }
                else
                {
                    return Convert.ToInt16(XmlConn.Element("LOGO" + device).Element("ReadWriteCycle").Value);
                }
            }
            catch (Exception)
            {

                throw new Exception(
                    "ReadWriteCycle value in config file is not valid ReadWriteCycle value. " +
                    "Correct the ReadWriteCycle value in XMLFile-Settings.xml file at LOGO" + device + " entry. " +
                    "format must be number (example: 500).");
            }

        }
                
                     
        public static string GetDeviceName(int index)
        {
            var searchValue = "devicename";

            try
            {
                if (index < 1 || index > Settings.Devices)
                {
                    throw new Exception();
                }
                return XmlConn.Element("LOGO" + index).Element(searchValue).Value;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    searchValue + " value in config file is not valid " + searchValue + " value. " +
                    "Correct the " + searchValue + " value in XMLFile-Settings.xml file at GUI entry. " +
                    "format must be number (example: 3) - min value 1, max value  " + Settings.Devices + ". " + "Exception message: " + ex.Message);
            }
        }

        public static string GetShowName(int index)
        {
            var searchValue = "showname";

            try
            {
                if (index < 1 || index > Settings.Devices)
                {
                    throw new Exception();
                }
                return XmlConn.Element("LOGO" + index).Element(searchValue).Value;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    searchValue + " value in config file is not valid " + searchValue + " value. " +
                    "Correct the " + searchValue + " value in XMLFile-Settings.xml file at GUI entry. " +
                    "format must be number (example: 3) - min value 1, max value " + Settings.Devices + ". " + "Exception message: " + ex.Message);
            }
        }


    }
}