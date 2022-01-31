using System;
using System.Collections.Generic;
using System.Linq;

namespace WindowsFormsApp2
{
    public static class Settings
    {

        // importatnt settings (will reflect significant changes)

        

        public static readonly int UpdateValuesPCms = 500;          // Frekvenca osveževanja vrednosti

        public static readonly int Devices = 5;                      // how many devices supported (do not change)        

        public static readonly int XmlRefreshrate = 5000;                                // scans for changes in xml file (should be very high number - 60000)

        public static readonly bool EnableHighPerformanceSync = true;           // Enables tcp comunication without delay (disable if CPU usage is to high)
        
        public static readonly string XmlDeclaration = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"; // used to recreate xml config file        
        
        public static readonly string pathToConfigFileEncripted = "config.encripted";

        public static readonly string pathToConfigFile = "XMLFile-Settings.xml";

        public static readonly uint logFileMaxSizeKB = 4000;
        
        // less important (not very significant)

        public static readonly int defaultCheckTimingInterval = 40;      // used for loops checking - lower value means higher precision timing

        public static readonly string defaultDateFormat = "dd.MM";
        public static readonly string defaultDateFormatY = "dd.MM.yyyy";
        public static readonly string defaultTimeFormat = "HH:mm:ss";
        public static readonly string defaultDateTimeFormat = defaultDateFormat + "  " + defaultTimeFormat + " ";
        public static readonly string defaultDateTimeFormatY = defaultDateFormatY + "  " + defaultTimeFormat + " ";        

        
    }



    

}