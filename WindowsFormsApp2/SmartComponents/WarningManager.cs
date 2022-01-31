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
        public static List<Warning> Warnings = new List<Warning>();
        static List<PlcVars.AlarmBit> SporocilaZaPrikaz = new List<PlcVars.AlarmBit>();

        Thread DisplayOnScreenThread;
        Thread SporocilniSistemThread;


        public WarningManager()
        {
            
            StartWarningTrackerThread(); 

            Height = 150;
            Width = 250;
            Multiline = true;            
            Setup();
                        

        }

        static Misc.SmartThread WarningTrackerThread;

        public static List<Warning> WarningsShowList;
        static readonly List<Tracker> MessageTrackerList = new List<Tracker>();

        void Setup()
        {
            bool designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
            if (!designMode)
            {
                DisplayOnScreenThread = new Thread(DisplayOnScreen);
                DisplayOnScreenThread.Start();

                SporocilniSistemThread = new Thread(SporocilniSistem_method);
                SporocilniSistemThread.Start();
            }
            ReadOnly = true;
        }

        void DisplayOnScreen()
        {
            var m = new MethodInvoker(delegate { ShowOnDisplay(); });
            while (FormControl.Gui == null)
            {
                Thread.Sleep(100);
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

            for (int i = 0; i < SporocilaZaPrikaz.Count; i++)
            {
                sporocilaZaprikazBuff += SporocilaZaPrikaz[i].Message + Environment.NewLine + Environment.NewLine;
            }
            Text = sporocilaZaprikazBuff;
        }
        public static void AddMessageForUser_Warning(string message)
        {
            if (!PreventThisMessage_IsDuplicacate(message))
            {
                Warnings.Add(new Warning(message, CreateId()));
                SysLog.Message.SetMessage("Message shown to user: " + message);
            }
        }

        void WarningTrackerThreadMethod()
        {
            bool Alarm = false;
            Warnings = new List<Warning>();

            try
            {
                foreach (var item in MessageTrackerList)
                {
                    Alarm = item.UpdateValue_TriggerAlarm();

                    if (Alarm)
                    {
                        AddMessageForUser_Warning(item.WarningMessage);
                    }
                }

                Helper.WarningManagerInitialized = true;                
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
       
        public static void RemoveMessageForUser_Warning(Warning warning)
        {
            WarningManager.Warnings.Remove(warning);
        }
        public static void RemoveMessageForUser_Warning(string warning)
        {
            if (WarningsShowList != null)
            {
                WarningsShowList.Find(item => item.GetMessage() == warning);
                foreach (var item in WarningsShowList)
                {
                    if (warning == item.GetMessage())
                    {
                        RemoveMessageForUser_Warning(item);
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

        static int CreateId()
        {
            int id = 0;
            bool again = false;

            while (true)
            {
                foreach (var item in WarningManager.Warnings)
                {
                    if (item != null)
                    {
                        if (item.GetID() == id)
                        {
                            id++;
                            again = true;
                            break;
                        }

                    }
                }

                if (!again)
                {
                    return id;
                }
                else
                {
                    again = false;
                }
            }
        }

        static bool PreventThisMessage_IsDuplicacate(string message)
        {
            if (Warnings != null)
            {
                foreach (var item in WarningManager.Warnings)
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

        void SporocilniSistem_method()
        {
            while (true)
            {

                try
                {
                    foreach (var item in PlcVars.AllAlarmMessageVars)
                    {
                        if (item != null)
                        {
                            if (item.Value == !item.InvertState)
                            {
                                if (!DoesItExist(item.Message, SporocilaZaPrikaz))
                                {
                                    SporocilaZaPrikaz.Add(item); // adds message to display if it does not exist yet   if alarm bool is == 1
                                }
                            }
                            else
                            {
                                if (DoesItExist(item.Message, SporocilaZaPrikaz))
                                {
                                    SporocilaZaPrikaz.Remove(item);// removes message to display if it exists   if alarm bool is == 0
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    SysLog.SetMessage("Napaka v sporočilnem sistemu: " + ex.Message);
                }



                Thread.Sleep(Settings.UpdateValuesPCms);
            }
        }
                
        static bool DoesItExist(string sporocilo, List<PlcVars.AlarmBit> collection)
        {
            if (collection == null)
            {
                return false;
            }
            foreach (var item in collection)
            {
                if (item.Message == sporocilo)
                {
                    return true;
                }
            }
            return false;

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

                if (typ == typeof(PlcVars.Bit))
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
                    if (
                        typ != typeof(PlcVars.Bit) ||
                        (Condition != WarningTriggerCondition.EqualTo && Condition != WarningTriggerCondition.NotEqualTo)
                        )
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


                    if (typ == typeof(PlcVars.Bit))
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
                    if (typ == typeof(PlcVars.Byte))
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
                    if (typ == typeof(PlcVars.Word))
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
                    if (typ == typeof(PlcVars.DWord))
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
            readonly int id;
            readonly string message = "";

            public Warning(string Message, int id)
            {
                this.id = id;
                message = Message;
            }

            public string GetMessage()
            {
                return message;
            }

            public int GetID()
            {
                return id;
            }
        }
    }


}