using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Sharp7;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using System.Threading;

namespace WindowsFormsApp2
{


    public partial class SettingsForm : Form
    {

        public XDocument settingsXML;
        public int WatchdogRetries = 1;
        Thread switchToGuiThread;

        public bool init = false;


        public SettingsForm()
        {
            switchToGuiThread = new Thread(SwitchToGui);
            FormClosing += SettingsForm_FormClosing;
            this.Shown += SettingsForm_Shown;
            FormControl.Form_settings = this;


            try
            {
                InitializeComponent();
                Show();               

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
                MessageBox.Show(e.Message);
            }


            //AUTOCONNECT

            Val.logocontroler.StartBackgroundTasks();

            try
            {
                if (Convert.ToBoolean(XmlController.XmlGeneral.Element("AutoConnect").Value))
                {
                    WL("Autoconnect at startup", 0);

                    object o = (string)"Skip UpdateFieldsXML";
                    ButtonConnectALL_Click(o, null);

                }
            }
            catch (Exception e)
            {
                var message = "AutoConnect failed. Try connecting manualy. Details: " + e.Message;
                MessageBox.Show(message);
                WL(message, 0);
            }


        }

        private void SettingsForm_Shown(object sender, EventArgs e)
        {
            switchToGuiThread.Start();
        }

        Action ShowGuiAfterDly = new Action(delegate { Thread.Sleep(5000);  });

        private void SwitchToGui()
        {
            var m = new MethodInvoker(delegate 
            {
                while (FormControl.Gui == null)
                {
                    Thread.Sleep(100);
                }
                FormControl.Gui.Show();
                FormControl.HideForm_Settings();
            });

            Invoke(m);
                     
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Helper.ExitApp();
        }


        // Reload button XML
        private void ButtonReloadXML_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        #region CheckBoxLOGO_EN_CheckedChanged

        private void CheckBoxLOGO_EN1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (CheckBoxLOGO_EN1.Checked)
                {
                    XmlController.XmlConn.Element("LOGO1").Element("enabled").SetValue("true");
                }
                else
                {
                    XmlController.XmlConn.Element("LOGO1").Element("enabled").SetValue("false");
                }
                FormControl.SaveXML();

            }
            catch (Exception ex)
            {
                WL("Error while changing settings - XML file entry is corupted (entry: LOGO enabled): " + ex.Message, 1);
            }

        }

        private void CheckBoxLOGO_EN2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (CheckBoxLOGO_EN2.Checked)
                {
                    XmlController.XmlConn.Element("LOGO2").Element("enabled").SetValue("true");
                }
                else
                {
                    XmlController.XmlConn.Element("LOGO2").Element("enabled").SetValue("false");
                }
                FormControl.SaveXML();
            }
            catch (Exception ex)
            {
                WL("Error while changing settings - XML file entry is corupted (entry: LOGO enabled): " + ex.Message, 2);
            }
        }

        private void CheckBoxLOGO_EN3_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (CheckBoxLOGO_EN3.Checked)
                {
                    XmlController.XmlConn.Element("LOGO3").Element("enabled").SetValue("true");
                }
                else
                {
                    XmlController.XmlConn.Element("LOGO3").Element("enabled").SetValue("false");
                }
                FormControl.SaveXML();
            }
            catch (Exception ex)
            {
                WL("Error while changing settings - XML file entry is corupted (entry: LOGO enabled): " + ex.Message, 3);
            }
        }

        private void CheckBoxLOGO_EN4_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (CheckBoxLOGO_EN4.Checked)
                {
                    XmlController.XmlConn.Element("LOGO4").Element("enabled").SetValue("true");
                }
                else
                {
                    XmlController.XmlConn.Element("LOGO4").Element("enabled").SetValue("false");
                }
                FormControl.SaveXML();
            }
            catch (Exception ex)
            {
                WL("Error while changing settings - XML file entry is corupted (entry: LOGO enabled): " + ex.Message, 4);
            }
        }

        private void CheckBoxLOGO_EN5_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (CheckBoxLOGO_EN5.Checked)
                {
                    XmlController.XmlConn.Element("LOGO5").Element("enabled").SetValue("true");
                }
                else
                {
                    XmlController.XmlConn.Element("LOGO5").Element("enabled").SetValue("false");
                }
                FormControl.SaveXML();
            }
            catch (Exception ex)
            {
                WL("Error while changing settings - XML file entry is corupted (entry: LOGO enabled): " + ex.Message, 5);
            }
        }

        private void CheckBoxLOGO_EN6_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (CheckBoxLOGO_EN6.Checked)
                {
                    XmlController.XmlConn.Element("LOGO6").Element("enabled").SetValue("true");
                }
                else
                {
                    XmlController.XmlConn.Element("LOGO6").Element("enabled").SetValue("false");
                }
                FormControl.SaveXML();
            }
            catch (Exception ex)
            {
                WL("Error while changing settings - XML file entry is corupted (entry: LOGO enabled): " + ex.Message, 6);
            }
        }

        private void CheckBoxLOGO_EN7_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (CheckBoxLOGO_EN7.Checked)
                {
                    XmlController.XmlConn.Element("LOGO7").Element("enabled").SetValue("true");
                }
                else
                {
                    XmlController.XmlConn.Element("LOGO7").Element("enabled").SetValue("false");
                }
                FormControl.SaveXML();
            }
            catch (Exception ex)
            {
                WL("Error while changing settings - XML file entry is corupted (entry: LOGO enabled): " + ex.Message, 7);
            }
        }

        private void CheckBoxLOGO_EN8_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (CheckBoxLOGO_EN8.Checked)
                {
                    XmlController.XmlConn.Element("LOGO8").Element("enabled").SetValue("true");
                }
                else
                {
                    XmlController.XmlConn.Element("LOGO8").Element("enabled").SetValue("false");
                }
                FormControl.SaveXML();
            }
            catch (Exception ex)
            {
                WL("Error while changing settings - XML file entry is corupted (entry: LOGO enabled): " + ex.Message, 8);
            }
        }




        #endregion CheckBoxLOGO_EN_CheckedChanged

        #region btnConnect

        private void BtnConnectLOGO1_Click(object sender, EventArgs e)
        {
            ConnectAsync(1);
        }


        private void BtnConnectLOGO2_Click(object sender, EventArgs e)
        {
            ConnectAsync(2);
        }

        private void BtnConnectLOGO3_Click(object sender, EventArgs e)
        {
            ConnectAsync(3);
        }

        private void BtnConnectLOGO4_Click(object sender, EventArgs e)
        {
            ConnectAsync(4);
        }

        private void BtnConnectLOGO5_Click(object sender, EventArgs e)
        {
            ConnectAsync(5);
        }

        private void BtnConnectLOGO6_Click(object sender, EventArgs e)
        {
            ConnectAsync(6);
        }

        private void BtnConnectLOGO7_Click(object sender, EventArgs e)
        {
            ConnectAsync(7);
        }

        private void BtnConnectLOGO8_Click(object sender, EventArgs e)
        {
            ConnectAsync(8);
        }


        #endregion btnConnect

        #region btnDisconnect

        private void BtnDisconnectLOGO1_Click(object sender, EventArgs e)
        {
            DisconnectAsync(1);
        }
        private void BtnDisconnectLOGO2_Click(object sender, EventArgs e)
        {
            DisconnectAsync(2);
        }
        private void BtnDisconnectLOGO3_Click(object sender, EventArgs e)
        {
            DisconnectAsync(3);
        }
        private void BtnDisconnectLOGO4_Click(object sender, EventArgs e)
        {
            DisconnectAsync(4);
        }
        private void BtnDisconnectLOGO5_Click(object sender, EventArgs e)
        {
            DisconnectAsync(5);
        }
        private void BtnDisconnectLOGO6_Click(object sender, EventArgs e)
        {
            DisconnectAsync(6);
        }
        private void BtnDisconnectLOGO7_Click(object sender, EventArgs e)
        {
            DisconnectAsync(7);
        }
        private void BtnDisconnectLOGO8_Click(object sender, EventArgs e)
        {
            DisconnectAsync(8);
        }
        private void BtnDisconnectLOGO9_Click(object sender, EventArgs e)
        {
            DisconnectAsync(9);
        }
        private void BtnDisconnectLOGO10_Click(object sender, EventArgs e)
        {
            DisconnectAsync(10);
        }
        private void BtnDisconnectLOGO11_Click(object sender, EventArgs e)
        {
            DisconnectAsync(11);
        }
        private void BtnDisconnectLOGO12_Click(object sender, EventArgs e)
        {
            DisconnectAsync(12);
        }
        private void BtnDisconnectLOGO13_Click(object sender, EventArgs e)
        {
            DisconnectAsync(13);
        }
        private void BtnDisconnectLOGO14_Click(object sender, EventArgs e)
        {
            DisconnectAsync(14);
        }
        private void BtnDisconnectLOGO15_Click(object sender, EventArgs e)
        {
            DisconnectAsync(15);
        }
        private void BtnDisconnectLOGO16_Click(object sender, EventArgs e)
        {
            DisconnectAsync(16);
        }
        private void BtnDisconnectLOGO17_Click(object sender, EventArgs e)
        {
            DisconnectAsync(17);
        }
        private void BtnDisconnectLOGO18_Click(object sender, EventArgs e)
        {
            DisconnectAsync(18);
        }
        private void BtnDisconnectLOGO19_Click(object sender, EventArgs e)
        {
            DisconnectAsync(19);
        }
        private void BtnDisconnectLOGO20_Click(object sender, EventArgs e)
        {
            DisconnectAsync(20);
        }

        #endregion btnDisconnect

        #region ENWatchdog

        private void CheckBoxWatchdogENLOGO1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                XmlController.XmlConn.Element("LOGO1").Element("watchdogEN").SetValue(CheckBoxWatchdogENLOGO1.Checked);
                settingsXML.Save(textBoxPathXML.Text);
            }
            catch (Exception ex)
            {

                WL("Error while changing settings - XML file entry is corupted (entry: LOGO watchdogEN): " + ex.Message, 1);
            }

        }

        private void CheckBoxWatchdogENLOGO2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                XmlController.XmlConn.Element("LOGO2").Element("watchdogEN").SetValue(CheckBoxWatchdogENLOGO2.Checked);
                settingsXML.Save(textBoxPathXML.Text);
            }
            catch (Exception ex)
            {

                WL("Error while changing settings - XML file entry is corupted (entry: LOGO watchdogEN): " + ex.Message, 2);
            }
        }

        private void CheckBoxWatchdogENLOGO3_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                XmlController.XmlConn.Element("LOGO3").Element("watchdogEN").SetValue(CheckBoxWatchdogENLOGO3.Checked);
                settingsXML.Save(textBoxPathXML.Text);
            }
            catch (Exception ex)
            {

                WL("Error while changing settings - XML file entry is corupted (entry: LOGO watchdogEN): " + ex.Message, 3);
            }
        }

        private void CheckBoxWatchdogENLOGO4_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                XmlController.XmlConn.Element("LOGO4").Element("watchdogEN").SetValue(CheckBoxWatchdogENLOGO4.Checked);
                settingsXML.Save(textBoxPathXML.Text);
            }
            catch (Exception ex)
            {

                WL("Error while changing settings - XML file entry is corupted (entry: LOGO watchdogEN): " + ex.Message, 4);
            }
        }

        private void CheckBoxWatchdogENLOGO5_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                XmlController.XmlConn.Element("LOGO5").Element("watchdogEN").SetValue(CheckBoxWatchdogENLOGO5.Checked);
                settingsXML.Save(textBoxPathXML.Text);
            }
            catch (Exception ex)
            {

                WL("Error while changing settings - XML file entry is corupted (entry: LOGO watchdogEN): " + ex.Message, 5);
            }
        }

        private void CheckBoxWatchdogENLOGO6_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                XmlController.XmlConn.Element("LOGO6").Element("watchdogEN").SetValue(CheckBoxWatchdogENLOGO6.Checked);
                settingsXML.Save(textBoxPathXML.Text);
            }
            catch (Exception ex)
            {

                WL("Error while changing settings - XML file entry is corupted (entry: LOGO watchdogEN): " + ex.Message, 6);
            }
        }

        private void CheckBoxWatchdogENLOGO7_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                XmlController.XmlConn.Element("LOGO7").Element("watchdogEN").SetValue(CheckBoxWatchdogENLOGO7.Checked);
                settingsXML.Save(textBoxPathXML.Text);
            }
            catch (Exception ex)
            {

                WL("Error while changing settings - XML file entry is corupted (entry: LOGO watchdogEN): " + ex.Message, 7);
            }
        }

        private void CheckBoxWatchdogENLOGO8_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                XmlController.XmlConn.Element("LOGO8").Element("watchdogEN").SetValue(CheckBoxWatchdogENLOGO8.Checked);
                settingsXML.Save(textBoxPathXML.Text);
            }
            catch (Exception ex)
            {

                WL("Error while changing settings - XML file entry is corupted (entry: LOGO watchdogEN): " + ex.Message, 8);
            }
        }




        #endregion ENWatchdog

        #region AddressWatchdog


        private void TextBoxWatchdogAddressLOGO1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                XmlController.XmlConn.Element("LOGO1").Element("watchdogAddress").SetValue(TextBoxWatchdogAddressLOGO1.Text);
                settingsXML.Save(textBoxPathXML.Text);
            }
            catch (Exception ex)
            {

                WL("Error while changing settings - XML file entry is corupted (entry: LOGO watchdogEN): " + ex.Message, 1);
            }

        }

        private void TextBoxWatchdogAddressLOGO2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                XmlController.XmlConn.Element("LOGO2").Element("watchdogAddress").SetValue(TextBoxWatchdogAddressLOGO2.Text);
                settingsXML.Save(textBoxPathXML.Text);
            }
            catch (Exception ex)
            {

                WL("Error while changing settings - XML file entry is corupted (entry: LOGO watchdogEN): " + ex.Message, 2);
            }
        }

        private void TextBoxWatchdogAddressLOGO3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                XmlController.XmlConn.Element("LOGO3").Element("watchdogAddress").SetValue(TextBoxWatchdogAddressLOGO3.Text);
                settingsXML.Save(textBoxPathXML.Text);
            }
            catch (Exception ex)
            {

                WL("Error while changing settings - XML file entry is corupted (entry: LOGO watchdogEN): " + ex.Message, 3);
            }
        }

        private void TextBoxWatchdogAddressLOGO4_TextChanged(object sender, EventArgs e)
        {
            try
            {
                XmlController.XmlConn.Element("LOGO4").Element("watchdogAddress").SetValue(TextBoxWatchdogAddressLOGO4.Text);
                settingsXML.Save(textBoxPathXML.Text);
            }
            catch (Exception ex)
            {

                WL("Error while changing settings - XML file entry is corupted (entry: LOGO watchdogEN): " + ex.Message, 4);
            }
        }

        private void TextBoxWatchdogAddressLOGO5_TextChanged(object sender, EventArgs e)
        {
            try
            {
                XmlController.XmlConn.Element("LOGO5").Element("watchdogAddress").SetValue(TextBoxWatchdogAddressLOGO5.Text);
                settingsXML.Save(textBoxPathXML.Text);
            }
            catch (Exception ex)
            {

                WL("Error while changing settings - XML file entry is corupted (entry: LOGO watchdogEN): " + ex.Message, 5);
            }
        }

        private void TextBoxWatchdogAddressLOGO6_TextChanged(object sender, EventArgs e)
        {
            try
            {
                XmlController.XmlConn.Element("LOGO6").Element("watchdogAddress").SetValue(TextBoxWatchdogAddressLOGO6.Text);
                settingsXML.Save(textBoxPathXML.Text);
            }
            catch (Exception ex)
            {

                WL("Error while changing settings - XML file entry is corupted (entry: LOGO watchdogEN): " + ex.Message, 6);
            }
        }

        private void TextBoxWatchdogAddressLOGO7_TextChanged(object sender, EventArgs e)
        {
            try
            {
                XmlController.XmlConn.Element("LOGO7").Element("watchdogAddress").SetValue(TextBoxWatchdogAddressLOGO7.Text);
                settingsXML.Save(textBoxPathXML.Text);
            }
            catch (Exception ex)
            {

                WL("Error while changing settings - XML file entry is corupted (entry: LOGO watchdogEN): " + ex.Message, 7);
            }
        }

        private void TextBoxWatchdogAddressLOGO8_TextChanged(object sender, EventArgs e)
        {
            try
            {
                XmlController.XmlConn.Element("LOGO8").Element("watchdogAddress").SetValue(TextBoxWatchdogAddressLOGO8.Text);
                settingsXML.Save(textBoxPathXML.Text);
            }
            catch (Exception ex)
            {

                WL("Error while changing settings - XML file entry is corupted (entry: LOGO watchdogEN): " + ex.Message, 8);
            }
        }



        #endregion

        #region RWcycle        

        private void TextBoxRWcycLOGO1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                XmlController.XmlConn.Element("LOGO1").Element("ReadWriteCycle").SetValue(TextBoxRWcycLOGO1.Text);
                settingsXML.Save(textBoxPathXML.Text);


            }
            catch (Exception ex) { WL("Error while changing settings - XML file entry is corupted (entry: LOGO enabled): " + ex.Message, 1); }
        }


        private void TextBoxRWcycLOGO2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                XmlController.XmlConn.Element("LOGO2").Element("ReadWriteCycle").SetValue(TextBoxRWcycLOGO2.Text);
                settingsXML.Save(textBoxPathXML.Text);

                FormControl.Form_settings.TextBoxRWcycLOGO2.Text = TextBoxRWcycLOGO2.Text;
            }
            catch (Exception ex) { WL("Error while changing settings - XML file entry is corupted (entry: LOGO enabled): " + ex.Message, 2); }


        }

        private void TextBoxRWcycLOGO3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                XmlController.XmlConn.Element("LOGO3").Element("ReadWriteCycle").SetValue(TextBoxRWcycLOGO3.Text);
                settingsXML.Save(textBoxPathXML.Text);


            }
            catch (Exception ex) { WL("Error while changing settings - XML file entry is corupted (entry: LOGO enabled): " + ex.Message, 3); }
        }

        private void TextBoxRWcycLOGO4_TextChanged(object sender, EventArgs e)
        {
            try
            {
                XmlController.XmlConn.Element("LOGO4").Element("ReadWriteCycle").SetValue(TextBoxRWcycLOGO4.Text);
                settingsXML.Save(textBoxPathXML.Text);


            }
            catch (Exception ex) { WL("Error while changing settings - XML file entry is corupted (entry: LOGO enabled): " + ex.Message, 4); }
        }

        private void TextBoxRWcycLOGO5_TextChanged(object sender, EventArgs e)
        {
            try
            {
                XmlController.XmlConn.Element("LOGO5").Element("ReadWriteCycle").SetValue(TextBoxRWcycLOGO5.Text);
                settingsXML.Save(textBoxPathXML.Text);


            }
            catch (Exception ex) { WL("Error while changing settings - XML file entry is corupted (entry: LOGO enabled): " + ex.Message, 5); }
        }

        private void TextBoxRWcycLOGO6_TextChanged(object sender, EventArgs e)
        {
            try
            {
                XmlController.XmlConn.Element("LOGO6").Element("ReadWriteCycle").SetValue(TextBoxRWcycLOGO6.Text);
                settingsXML.Save(textBoxPathXML.Text);


            }
            catch (Exception ex) { WL("Error while changing settings - XML file entry is corupted (entry: LOGO enabled): " + ex.Message, 6); }
        }

        private void TextBoxRWcycLOGO7_TextChanged(object sender, EventArgs e)
        {
            try
            {
                XmlController.XmlConn.Element("LOGO7").Element("ReadWriteCycle").SetValue(TextBoxRWcycLOGO7.Text);
                settingsXML.Save(textBoxPathXML.Text);


            }
            catch (Exception ex) { WL("Error while changing settings - XML file entry is corupted (entry: LOGO enabled): " + ex.Message, 7); }
        }

        private void TextBoxRWcycLOGO8_TextChanged(object sender, EventArgs e)
        {
            try
            {
                XmlController.XmlConn.Element("LOGO8").Element("ReadWriteCycle").SetValue(TextBoxRWcycLOGO8.Text);
                settingsXML.Save(textBoxPathXML.Text);


            }
            catch (Exception ex) { WL("Error while changing settings - XML file entry is corupted (entry: LOGO enabled): " + ex.Message, 8); }
        }






        #endregion

        // button open XML
        private void ButtonOpenXML_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(textBoxPathXML.Text);
                WL("XML file was opend", 0);
            }
            catch (Exception ex)
            {
                WL("Can not open XML File - check that path is valid or that file exists: " + ex.Message, -1);
            }
        }

        // Browse button LOG
       

        // button open LOG
        private void ButtonOpenLOG_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(textBoxPathLOG.Text);
                WL("Log file was opend", 0);
            }
            catch (Exception ex)
            {
                WL("Can not open Log File - check that path is valid or that file exists: " + ex.Message, -1);
            }
        }

              
    
        // button connect all
        private void ButtonConnectALL_Click(object sender, EventArgs e)
        {
            bool skip = false;
            if (sender != null)
            {
                if (sender.GetType().Equals(typeof(string)))
                {
                    string sender_ = (string)sender;
                    if (sender_ == "Skip UpdateFieldsXML")
                    {
                        skip = true;
                    }
                }
            }

            if (!skip)
            {
                UpdateFieldsXML(); // do not update if bool sender is true
            }
            object device = new object[1];

            if (IsEnabled(1)) { BtnConnectLOGO1_Click(device = (int)1, null); }
            if (IsEnabled(2)) { BtnConnectLOGO2_Click(device = (int)2, null); }
            if (IsEnabled(3)) { BtnConnectLOGO3_Click(device = (int)3, null); }
            if (IsEnabled(4)) { BtnConnectLOGO4_Click(device = (int)4, null); }
            if (IsEnabled(5)) { BtnConnectLOGO5_Click(device = (int)5, null); }
            if (IsEnabled(6)) { BtnConnectLOGO6_Click(device = (int)6, null); }
            if (IsEnabled(7)) { BtnConnectLOGO7_Click(device = (int)7, null); }
            if (IsEnabled(8)) { BtnConnectLOGO8_Click(device = (int)8, null); }

        }

        // button disconnect all
        public void ButtonDisconnectALL_Click(object sender, EventArgs e)
        {
            if (IsEnabled(1)) { BtnDisconnectLOGO1_Click(null, null); }
            if (IsEnabled(2)) { BtnDisconnectLOGO2_Click(null, null); }
            if (IsEnabled(3)) { BtnDisconnectLOGO3_Click(null, null); }
            if (IsEnabled(4)) { BtnDisconnectLOGO4_Click(null, null); }
            if (IsEnabled(5)) { BtnDisconnectLOGO5_Click(null, null); }
            if (IsEnabled(6)) { BtnDisconnectLOGO6_Click(null, null); }
            if (IsEnabled(7)) { BtnDisconnectLOGO7_Click(null, null); }
            if (IsEnabled(8)) { BtnDisconnectLOGO8_Click(null, null); }

        }

        private bool IsEnabled(int device)
        {
            try
            {
                return Convert.ToBoolean(XmlController.XmlConn.Element("LOGO" + device).Element("enabled").Value);
            }
            catch (Exception)
            {
                return false;
            }

        }

        private void CheckBoxAutoconnect_CheckedChanged(object sender, EventArgs e)
        {
            settingsXML.Element("root").Element("GENERAL").Element("AutoConnect").SetValue(checkBoxAutoconnect.Checked.ToString());
            settingsXML.Save(textBoxPathXML.Text);
        }


        // after form shows up
        // after form shows up
        // after form shows up
        public void SettingsAfterjobs()
        {

            FormControl.Form_settings.TextBoxWatchdogAddressLOGO1 = TextBoxWatchdogAddressLOGO1;
            FormControl.Form_settings.TextBoxWatchdogAddressLOGO2 = TextBoxWatchdogAddressLOGO2;
            FormControl.Form_settings.TextBoxWatchdogAddressLOGO3 = TextBoxWatchdogAddressLOGO3;
            FormControl.Form_settings.TextBoxWatchdogAddressLOGO4 = TextBoxWatchdogAddressLOGO4;
            FormControl.Form_settings.TextBoxWatchdogAddressLOGO5 = TextBoxWatchdogAddressLOGO5;
            FormControl.Form_settings.TextBoxWatchdogAddressLOGO6 = TextBoxWatchdogAddressLOGO6;
            FormControl.Form_settings.TextBoxWatchdogAddressLOGO7 = TextBoxWatchdogAddressLOGO7;
            FormControl.Form_settings.TextBoxWatchdogAddressLOGO8 = TextBoxWatchdogAddressLOGO8;


            textBoxPathXML.Text = Properties.Settings.Default.PathXML;
            init = true;


            // Checking if path exists XML
            if (File.Exists(Settings.pathToConfigFile))  // TODO fix
            {
                UpdateFieldsXML();
            }
                       

            try
            {
                settingsXML = XDocument.Load(textBoxPathXML.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Please provide valid XML file (or path). Application will now close. ");
                Environment.Exit(0);
            }


            // log file
            try
            {
                textBoxPathLOG.Text = Val.PathLogFIle;
            }

            catch
            {
                MessageBox.Show("Error reading path to Log file.");
                Environment.Exit(0);
            }

            // user actions log file
            try
            {
                textBoxPathUALOG.Text = Val.PathUserActions;
            }

            catch
            {
                MessageBox.Show("Error reading path to User Actions Log file.");
                Environment.Exit(0);
            }

            // temperature log file
            try
            {
                textBoxPathTemperatureLog.Text = Val.PathTemperatures;
            }

            catch
            {
                MessageBox.Show("Error reading path to Temperature log file.");
                Environment.Exit(0);
            }

            // show devices
            try
            {
                if (!Convert.ToBoolean(XmlController.XmlConn.Element("LOGO1").Element("show").Value)) { tabControl1.TabPages.Remove(tabPageLOGO1); CheckBoxLOGO_EN1.Hide(); }
                if (!Convert.ToBoolean(XmlController.XmlConn.Element("LOGO2").Element("show").Value)) { tabControl1.TabPages.Remove(tabPageLOGO2); CheckBoxLOGO_EN2.Hide(); }
                if (!Convert.ToBoolean(XmlController.XmlConn.Element("LOGO3").Element("show").Value)) { tabControl1.TabPages.Remove(tabPageLOGO3); CheckBoxLOGO_EN3.Hide(); }
                if (!Convert.ToBoolean(XmlController.XmlConn.Element("LOGO4").Element("show").Value)) { tabControl1.TabPages.Remove(tabPageLOGO4); CheckBoxLOGO_EN4.Hide(); }
                if (!Convert.ToBoolean(XmlController.XmlConn.Element("LOGO5").Element("show").Value)) { tabControl1.TabPages.Remove(tabPageLOGO5); CheckBoxLOGO_EN5.Hide(); }
                if (!Convert.ToBoolean(XmlController.XmlConn.Element("LOGO6").Element("show").Value)) { tabControl1.TabPages.Remove(tabPageLOGO6); CheckBoxLOGO_EN6.Hide(); }
                if (!Convert.ToBoolean(XmlController.XmlConn.Element("LOGO7").Element("show").Value)) { tabControl1.TabPages.Remove(tabPageLOGO7); CheckBoxLOGO_EN7.Hide(); }
                if (!Convert.ToBoolean(XmlController.XmlConn.Element("LOGO8").Element("show").Value)) { tabControl1.TabPages.Remove(tabPageLOGO8); CheckBoxLOGO_EN8.Hide(); }


                WL("GUI was loaded successfully", 0);
            }
            catch (Exception ex)
            {
                WL("Error while loading GUI - XML file entry is corupted (show): " + ex.Message, -1);
            }

            try
            {
                WatchdogRetries = int.Parse(settingsXML.Element("root").Element("GENERAL").Element("WatchdogRetries").Value);
            }
            catch (Exception ex)
            {
                WL("Error while loading configuration - XML file entry is corupted (WatchdogRetries): " + ex.Message, -1);
            }


            // Load Values to FormControl.bt1        

            #region Load Values to gui



            textBoxDeviceIPLOGO1.Text = XmlController.XmlConn.Element("LOGO1").Element("serverIP").Value.Replace("\"", "");
            textBoxDeviceIPLOGO2.Text = XmlController.XmlConn.Element("LOGO2").Element("serverIP").Value.Replace("\"", "");
            textBoxDeviceIPLOGO3.Text = XmlController.XmlConn.Element("LOGO3").Element("serverIP").Value.Replace("\"", "");
            textBoxDeviceIPLOGO4.Text = XmlController.XmlConn.Element("LOGO4").Element("serverIP").Value.Replace("\"", "");
            textBoxDeviceIPLOGO5.Text = XmlController.XmlConn.Element("LOGO5").Element("serverIP").Value.Replace("\"", "");
            textBoxDeviceIPLOGO6.Text = XmlController.XmlConn.Element("LOGO6").Element("serverIP").Value.Replace("\"", "");
            textBoxDeviceIPLOGO7.Text = XmlController.XmlConn.Element("LOGO7").Element("serverIP").Value.Replace("\"", "");
            textBoxDeviceIPLOGO8.Text = XmlController.XmlConn.Element("LOGO8").Element("serverIP").Value.Replace("\"", "");



            #endregion




            // Worker for populating values
            Thread Populator = new Thread(new ThreadStart(Populate));
            Populator.Name = "Populator";
            Populator.IsBackground = true;
            Populator.Start();

        }


        // open form GUI
        public void BtnOpenGUI_click(object sender, EventArgs e)
        {

            FormControl.HideForm_Settings();
            FormControl.Gui.Show();

        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormControl.HideForm_Settings();
            e.Cancel = true;
        }


        public void ShowDialog1()
        {
            OpenFileDialog d1 = new OpenFileDialog();
            d1.Title = "Select XML configuration file";
            d1.Filter = "XML files (*.xml)|*.xml";

            try
            {
                if (d1.ShowDialog() == DialogResult.OK)
                {
                    this.Invoke(new MethodInvoker(delegate { textBoxPathXML.Text = d1.FileName; ; })); ;
                    this.Invoke(new MethodInvoker(delegate { UpdateFieldsXML(); })); ;
                    WL("Browse for XML file finished", 0);
                }
                else
                {
                    WL("No changes were made while browsing for XML file", 0);
                }

            }

            catch (Exception ex)
            {

                WL("Error opening Browse dialog: " + ex.Message, -1);
            }

        }

        public static bool HasDuplicates(List<string> text)
        {

            for (int i = 0; i < text.Count; i++)
            {
                for (int ii = 0; ii < text.Count; ii++)
                {
                    if (i != ii)
                    {
                        if (text[i] == text[ii])
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        public static void WL(string text, int errSeverity)
        {

            var message = DateTime.Now.ToString();


            if (errSeverity > 0)
            {
                message = message + " - LOGO!" + errSeverity + ": " + text;
            }
            else if (errSeverity == S7Consts.err_OK)
            {
                message = message + " - Info" + ": " + text;
            }
            else if (errSeverity == S7Consts.err_typeError)
            {
                message = message + " - ERRROR" + ": " + text;

                if (FormControl.Form_settings.InvokeRequired)
                {
                    FormControl.Form_settings.Invoke(new MethodInvoker(delegate { FormControl.Form_settings.debug.BackColor = Color.Red; }));
                }
                else
                {
                    FormControl.Form_settings.debug.BackColor = Color.Red;
                }

            }
            else if (errSeverity == S7Consts.err_watchdogDoesntChange)
            {
                message = Environment.NewLine + message + " - !!! CRITTICAL ERRROR !!!" + ": " + text + Environment.NewLine;
                if (FormControl.Form_settings.InvokeRequired)
                {
                    FormControl.Form_settings.Invoke(new MethodInvoker(delegate { FormControl.Form_settings.debug.BackColor = Color.Red; }));
                }
                else
                {
                    FormControl.Form_settings.debug.BackColor = Color.Red;
                }


            }

            if (FormControl.Form_settings.InvokeRequired)
            {
                FormControl.Form_settings.Invoke(new MethodInvoker(delegate { FormControl.Form_settings.debug.AppendText(message + Environment.NewLine); }));
            }
            else
            {
                FormControl.Form_settings.debug.AppendText(message + Environment.NewLine);
            }

            Console.WriteLine(message);

            try
            {
                FormControl.sw_Main.WriteLine(message);
                FormControl.sw_Main.Flush();
            }

            catch (Exception ex)
            {
                if (FormControl.Form_settings.InvokeRequired)
                {
                    FormControl.Form_settings.Invoke(new MethodInvoker(delegate { FormControl.Form_settings.debug.AppendText(DateTime.Now.ToString() + " - Error" + ": " + "Error writing to log file: " + ex.Message + Environment.NewLine); }));
                }
                else
                {
                    FormControl.Form_settings.debug.AppendText(DateTime.Now.ToString() + " - Error" + ": " + "Error writing to log file: " + ex.Message + Environment.NewLine);
                }
                Console.WriteLine(DateTime.Now.ToString() + " - Error" + ": " + "Error writing to log file: " + ex.Message);
            }

        }

        public void ConnectAsync(int device)
        {
            if (LogoControler.BackgroundWorker[device] != null)
            {
                if (!LogoControler.BackgroundWorker[device].IsBusy)
                {
                    LogoControler.BackgroundWorker[device].RunWorkerAsync();
                }
            }

        }

        public void DisconnectAsync(int device)
        {
            FormControl.WL("Disconected by the user", device);
            LogoControler.BackgroundWorker[device].CancelAsync();
            LogoControler.LOGO[device].Disconnect();
        }

        public void Populate()
        {

            DateTime time1;
            DateTime time2;
            double refreshOriginalVal = 500;
            double refreshCalculated = 500;

            MethodInvoker mi = (new MethodInvoker(delegate
            {
                labelWatchdogRuning1.Text = PropComm.GetWatchdogValue(1);
                labelWatchdogRuning2.Text = PropComm.GetWatchdogValue(2);
                labelWatchdogRuning3.Text = PropComm.GetWatchdogValue(3);
                labelWatchdogRuning4.Text = PropComm.GetWatchdogValue(4);
                labelWatchdogRuning5.Text = PropComm.GetWatchdogValue(5);
                labelWatchdogRuning6.Text = PropComm.GetWatchdogValue(6);
                labelWatchdogRuning7.Text = PropComm.GetWatchdogValue(7);
                labelWatchdogRuning8.Text = PropComm.GetWatchdogValue(8);


            }));

            try
            {
                refreshOriginalVal = int.Parse(settingsXML.Element("root").Element("GENERAL").Element("SettingsMenuRefreshrate").Value);
                if (refreshOriginalVal > 1000 || refreshOriginalVal < 100)
                {
                    refreshOriginalVal = 1000;
                    FormControl.WL("GUI Refresh rate is not a suitable value (must not be >1000 or <100). Default value of 1000ms is set.(message source: Form Settings)", -1);
                }
            }
            catch (Exception)
            {
                FormControl.WL("GUI Refresh rate entry can not be found in XML file provided. Default value of 500ms is set.(message source: Form Settings)", -1);
            }

            while (true)
            {
                try
                {
                    time1 = DateTime.Now;

                    this.Invoke(mi);
                    //Application.DoEvents();

                    time2 = DateTime.Now;
                    refreshCalculated = refreshOriginalVal - (time2 - time1).TotalMilliseconds;
                    if (refreshCalculated < 1) { refreshCalculated = 1; }
                    if (refreshCalculated > 3000) { refreshCalculated = 3000; }
                    Thread.Sleep(Convert.ToInt32(refreshCalculated));

                }
                catch
                {
                    Thread.Sleep(1000);
                }
            }
        }

        public string GetRWcycle(int device)
        {
            if (device == 1) { return TextBoxRWcycLOGO1.Text; }
            if (device == 2) { return TextBoxRWcycLOGO2.Text; }
            if (device == 3) { return TextBoxRWcycLOGO3.Text; }
            if (device == 4) { return TextBoxRWcycLOGO4.Text; }
            if (device == 5) { return TextBoxRWcycLOGO5.Text; }
            if (device == 6) { return TextBoxRWcycLOGO6.Text; }
            if (device == 7) { return TextBoxRWcycLOGO7.Text; }
            if (device == 8) { return TextBoxRWcycLOGO8.Text; }


            else
            {
                return "ERR";
            }

        }

        private void generatePC_WD_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkBox_Generate_PC.Checked)
                {
                    settingsXML.Element("root").Element("GENERAL").Element("GeneratePC_WD").SetValue("true");
                }
                else
                {
                    settingsXML.Element("root").Element("GENERAL").Element("GeneratePC_WD").SetValue("false");
                }
                settingsXML.Save(textBoxPathXML.Text);

            }
            catch (Exception ex)
            {
                WL("Error while changing settings - XML file entry is corupted (entry: GeneratePC_WD): " + ex.Message, 0);
            }
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();

            if (cd.ShowDialog() == DialogResult.OK)
            {
                textboxCode.Text = cd.Color.ToArgb().ToString();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button4.Text = "Restarting..";
            button4.Enabled = false;
            ButtonDisconnectALL_Click(null, null);

            Application.DoEvents();
            Application.DoEvents();
            Application.DoEvents();

            FormControl.CloseApp_Preparation();
            Program.GetContext().Restart = true;
            Program.GetContext().ExitThread();
        }

    }
}
