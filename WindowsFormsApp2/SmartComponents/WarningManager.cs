using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;

namespace WindowsFormsApp2
{

    public class WarningManager : RichTextBox
    {
        // Local warnings
        public static WarningNoConnection NoConnWarningPLC1 = new WarningNoConnection();

        // --

        public static List<Warning> Warnings = new List<Warning>();        

        Thread DisplayOnScreenThread;

        public WarningManager()
        {       
            StartWarningTrackerThread(); 
            Height = 150;
            Width = 250;
            Multiline = true;            
            Setup();
                        

        }

        static Misc.SmartThread WarningTrackerThread;        
        static readonly List<Tracker> MessageTrackerList = new List<Tracker>();

        void Setup()
        {
            bool designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
            if (!designMode)
            {
                DisplayOnScreenThread = new Thread(DisplayOnScreen);
                DisplayOnScreenThread.Start();

            }
            ReadOnly = true;
        }

        void DisplayOnScreen()
        {
            var m = new MethodInvoker(delegate { ShowOnDisplay(); });
            while (FormControl.Gui == null)
            {
                Thread.Sleep(100); 
                Application.DoEvents();
            }
            Thread.Sleep(1000);

            while (true)
            {
                try
                {
                    FormControl.Gui.Invoke(m);
                }
                catch
                {

                }
                Thread.Sleep(Settings.UpdateValuesPCms);
            }
        }

        string sporocilaZaprikazBuff = "";
        void ShowOnDisplay()
        {
            sporocilaZaprikazBuff = "";            

            for (int i = 0; i < Warnings.Count; i++)
            {
                sporocilaZaprikazBuff += Warnings[i].GetMessage() + Environment.NewLine + Environment.NewLine;
            }
            Text = sporocilaZaprikazBuff;
        }
        public static void AddMessageForUser_Warning(string message)
        {
            if (!PreventThisMessage_IsDuplicacate(message))
            {
                Warnings.Add(new Warning(message));
                SysLog.Message.SetMessage("Message shown to user: " + message);
            }
        }

        void WarningTrackerThreadMethod()
        {
            bool Alarm = false;
            Warnings = new List<Warning>();            
            
            try
            {
                while (true)
                {
                    Thread.Sleep(Settings.UpdateValuesPCms);

                    foreach (var item in MessageTrackerList)
                    {
                        Alarm = item.UpdateValue_TriggerAlarm();

                        if (Alarm)
                        {
                            AddMessageForUser_Warning(item.WarningMessage);
                        }
                        else
                        {                            
                            RemoveMessageForUser_Warning(item.WarningMessage);
                        }
                    }
                }                
                                               
            }
            catch (Exception ex)
            {
                throw new Exception("WarningTrackerThread encountered an error and was terminated: " + ex.Message);
            }
        }

        public static void AddWarningTrackerFromPLCVar(PlcVars.PlcType PlcVar, object valueToTrigerWarning, WarningTriggerCondition Condition, string WarningMessage)
        {
            Tracker t = new Tracker(PlcVar, valueToTrigerWarning, Condition, WarningMessage);
            MessageTrackerList.Add(t);
                        
        }

        public static void AddMessageForUser_Warning(Warning warning)
        {
            if (!PreventThisMessage_IsDuplicacate(warning.GetMessage()))
            { 
                Warnings.Add(warning);
                SysLog.Message.SetMessage("Message shown to user: " + warning);
            }
            
        }

        public static void RemoveMessageForUser_Warning(Warning warning)
        {
            if (Warnings.Contains(warning))
            {
                Warnings.Remove(warning);
            }            
        }
        public static void RemoveMessageForUser_Warning(string warning)
        {
            if (Warnings != null)
            {         
                Warning buff;                
                for (int i = 0; i < Warnings.Count; i++)
                {
                    buff = Warnings[i];
                    if (buff.GetMessage() == warning)
                    {
                        Warnings.RemoveAt(i);
                        return;
                    }
                }
            }
            
        }

        public enum WarningTriggerCondition
        {
            EqualTo,
            NotEqualTo,
            GreaterThan,
            LessThan,
            GreaterThanOrEqualTo,
            LessThanOrEqualTo
        }

        
        static bool PreventThisMessage_IsDuplicacate(string message)
        {
            if (Warnings != null)
            {
                foreach (var item in Warnings)
                {
                    if (message == item.GetMessage())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        void StartWarningTrackerThread()
        {
            WarningTrackerThread = new Misc.SmartThread(() => WarningTrackerThreadMethod());
            WarningTrackerThread.Start("WarningTrackerThread", ApartmentState.MTA, true);
        }

    

        class Tracker
        {
            public PlcVars.PlcType PlcVar;
            public object valueToTrigerWarning;
            public WarningTriggerCondition Condition;
            public string WarningMessage;

            public Tracker(PlcVars.PlcType PlcVar, object valueToTrigerWarning, WarningTriggerCondition Condition, string WarningMessage)
            {
                this.WarningMessage = WarningMessage;

                // Type checks only
                var typ = PlcVar.GetType();
                PlcVars.PlcType buff;

                var typ1 = valueToTrigerWarning.GetType();
                object buff1;

                if (typ == typeof(PlcVars.AlarmBit))
                {
                    buff = (PlcVars.AlarmBit)PlcVar;
                }

                else if (typ == typeof(PlcVars.Bit))
                {
                    buff = (PlcVars.Bit)PlcVar;
                }

                else if (typ == typeof(PlcVars.Byte))
                {
                    buff = (PlcVars.Byte)PlcVar;
                }

                else if (typ == typeof(PlcVars.Word))
                {
                    buff = (PlcVars.Word)PlcVar;
                }

                else if (typ == typeof(PlcVars.DWord))
                {
                    buff = (PlcVars.DWord)PlcVar;
                }

                else
                {
                    throw new Exception("Error: Unsupported type was passed (PlcVars.PlcType PlcVar). Supported types are: PlcVars.Bit, PlcVars.Byte, PlcVars.Word, PlcVars.DWord. Parent Class of this exception is WarningManager");
                }

                this.PlcVar = buff;

                //


                string messageErrorTypeConflict = "Error: Type conflict. If PlcVars.Bit was passed, valueToTrigerWarning should be of type bool, and vice versa.";

               
                if (typ1 == typeof(bool))
                {
                    if (Condition != WarningTriggerCondition.EqualTo && Condition != WarningTriggerCondition.NotEqualTo)
                    { }
                    else if (typ == typeof(PlcVars.AlarmBit))
                    { }
                    else if (typ != typeof(PlcVars.Bit))
                    { }
                    else
                    {
                        throw new Exception(messageErrorTypeConflict);
                    }

                    buff1 = (bool)valueToTrigerWarning;
                }

                else if (typ1 == typeof(short))
                {
                    if (typ == typeof(PlcVars.Bit))
                    {
                        throw new Exception(messageErrorTypeConflict);
                    }
                    buff1 = (short)valueToTrigerWarning;
                }

                else if (typ1 == typeof(int))
                {
                    if (typ == typeof(PlcVars.Bit))
                    {
                        throw new Exception(messageErrorTypeConflict);
                    }
                    buff1 = (int)valueToTrigerWarning;
                }

                else if (typ1 == typeof(bool?))
                {
                    if (typ != typeof(PlcVars.Bit))
                    {
                        throw new Exception(messageErrorTypeConflict);
                    }
                    buff1 = (bool?)valueToTrigerWarning;
                }

                else if (typ1 == typeof(short?))
                {
                    if (typ == typeof(PlcVars.Bit))
                    {
                        throw new Exception(messageErrorTypeConflict);
                    }
                    buff1 = (short?)valueToTrigerWarning;
                }

                else if (typ1 == typeof(int?))
                {
                    if (typ == typeof(PlcVars.Bit))
                    {
                        throw new Exception(messageErrorTypeConflict);
                    }
                    buff1 = (int?)valueToTrigerWarning;
                }

                else
                {
                    throw new Exception("Error: Unsupported type was passed (object valueToTrigerWarning). Supported types are: bool, short, int, bool?, short?, int? . Parent Class of this exception is WarningManager");
                }

                this.valueToTrigerWarning = buff1;


                this.Condition = Condition;
            }

            public bool UpdateValue_TriggerAlarm()
            {
                try
                {
                    // Type checks
                    var typ = PlcVar.GetType();

                    if (typ == typeof(PlcVars.AlarmBit))
                    {
                        var buff = (PlcVars.AlarmBit)PlcVar;
                        var val = buff.Value;

                        if (val != null)
                        {
                            return CompareBool_Alarm((bool)val);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (typ == typeof(PlcVars.Bit))
                    {
                        var buff = (PlcVars.Bit)PlcVar;
                        var val = buff.Value;

                        if (val != null)
                        {
                            return CompareBool_Alarm((bool)val);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (typ == typeof(PlcVars.Byte))
                    {
                        var buff = (PlcVars.Byte)PlcVar;
                        var val = buff.Value;

                        if (val != null)
                        {
                            return CompareOthers_Alarm((short)val);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (typ == typeof(PlcVars.Word))
                    {
                        var buff = (PlcVars.Word)PlcVar;
                        var val = buff.Value;

                        if (val != null)
                        {
                            return CompareOthers_Alarm((short)val);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (typ == typeof(PlcVars.DWord))
                    {
                        var buff = (PlcVars.DWord)PlcVar;
                        var val = buff.Value;

                        if (val != null)
                        {
                            return CompareOthers_Alarm((short)val);
                        }
                        else
                        {
                            return false;
                        }
                    }

                    throw new Exception("Wrong type was stored in collection.");
                }
                catch (Exception ex)
                {

                    throw new Exception("UpdateValue() reportred an Error (parent class: WarningManager): " + ex.Message);
                }

            }

            bool CompareBool_Alarm(bool val)
            {

                if (Condition == WarningTriggerCondition.EqualTo)
                {
                    if (val == (bool)valueToTrigerWarning)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                if (Condition == WarningTriggerCondition.NotEqualTo)
                {
                    if (val == (bool)valueToTrigerWarning)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                throw new Exception("Condition parameter was invalid.");
            }

            bool CompareOthers_Alarm(short val)
            {
                if (Condition == WarningTriggerCondition.EqualTo)
                {
                    if (val == (int)valueToTrigerWarning)
                    {
                        return true;
                    }
                    else { return false; }
                }

                if (Condition == WarningTriggerCondition.GreaterThan)
                {
                    if (val > (int)valueToTrigerWarning)
                    {
                        return true;
                    }
                    else { return false; }
                }

                if (Condition == WarningTriggerCondition.GreaterThanOrEqualTo)
                {
                    if (val >= (int)valueToTrigerWarning)
                    {
                        return true;
                    }
                    else { return false; }
                }

                if (Condition == WarningTriggerCondition.LessThan)
                {
                    if (val < (int)valueToTrigerWarning)
                    {
                        return true;
                    }
                    else { return false; }
                }

                if (Condition == WarningTriggerCondition.LessThanOrEqualTo)
                {
                    if (val <= (int)valueToTrigerWarning)
                    {
                        return true;
                    }
                    else { return false; }
                }

                if (Condition == WarningTriggerCondition.NotEqualTo)
                {
                    if (val != (int)valueToTrigerWarning)
                    {
                        return true;
                    }
                    else { return false; }
                }

                throw new Exception("Condition parameter was invalid.");
            }
        }

        public class Warning
        {       
            readonly string message = "";

            public Warning(string Message)
            {               
                message = Message;
            }

            public string GetMessage()
            {
                return message;
            }
            
        }

        public class WarningNoConnection : Warning
        {            
            static readonly string message = "NI POVEZAVE S KRMILNIKOM";

            public WarningNoConnection() : base(message)
            {
               
            }
                       
        }
    }


}