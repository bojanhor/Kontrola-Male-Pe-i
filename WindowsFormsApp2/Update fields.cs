using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sharp7;
using Linq = System.Xml.Linq;

namespace WindowsFormsApp2
{
    public partial class SettingsForm : Form
    {



        #region UPDATE_FIELDS


        public void UpdateFieldsXML()
        {
            try
            {
                settingsXML = Linq.XDocument.Load(Properties.Settings.Default.PathXML);
                var conn = XmlController.XmlConn;

                UpdateFieldsLOGO1(conn.Element("LOGO1"));
                UpdateFieldsLOGO2(conn.Element("LOGO2"));
                UpdateFieldsLOGO3(conn.Element("LOGO3"));
                UpdateFieldsLOGO4(conn.Element("LOGO4"));
                UpdateFieldsLOGO5(conn.Element("LOGO5"));
                UpdateFieldsLOGO6(conn.Element("LOGO6"));
                UpdateFieldsLOGO7(conn.Element("LOGO7"));
                UpdateFieldsLOGO8(conn.Element("LOGO8"));
                

                FormControl.WL("Configuration was reloaded from XML file", 0);

                #region find duplicates
                List<string> IPs = new List<string>();

                if (CheckBoxLOGO_EN1.Checked) { if (textBoxDeviceIPLOGO1.Text != "") { IPs.Add(textBoxDeviceIPLOGO1.Text); } }
                if (CheckBoxLOGO_EN2.Checked) { if (textBoxDeviceIPLOGO2.Text != "") { IPs.Add(textBoxDeviceIPLOGO2.Text); } }
                if (CheckBoxLOGO_EN3.Checked) { if (textBoxDeviceIPLOGO3.Text != "") { IPs.Add(textBoxDeviceIPLOGO3.Text); } }
                if (CheckBoxLOGO_EN4.Checked) { if (textBoxDeviceIPLOGO4.Text != "") { IPs.Add(textBoxDeviceIPLOGO4.Text); } }
                if (CheckBoxLOGO_EN5.Checked) { if (textBoxDeviceIPLOGO5.Text != "") { IPs.Add(textBoxDeviceIPLOGO5.Text); } }
                if (CheckBoxLOGO_EN6.Checked) { if (textBoxDeviceIPLOGO6.Text != "") { IPs.Add(textBoxDeviceIPLOGO6.Text); } }
                if (CheckBoxLOGO_EN7.Checked) { if (textBoxDeviceIPLOGO7.Text != "") { IPs.Add(textBoxDeviceIPLOGO7.Text); } }
                if (CheckBoxLOGO_EN8.Checked) { if (textBoxDeviceIPLOGO8.Text != "") { IPs.Add(textBoxDeviceIPLOGO8.Text); } }
               

                if (HasDuplicates(IPs))
                { FormControl.WL("Duplicated IP adresses found in configuration file. this can lead to unexpected behaviour. Please change IP adresses so each device will have unique IP adress and then restart the application.", -1); }

                
                #endregion

                checkBoxAutoconnect.Checked = Convert.ToBoolean(XmlController.XmlGeneral.Element("AutoConnect").Value.Replace("\"", ""));
                checkBox_Generate_PC.Checked = Convert.ToBoolean(XmlController.XmlGeneral.Element("GeneratePC_WD").Value.Replace("\"", ""));
                settingsXML.Save(textBoxPathXML.Text);

            }
            catch (Exception e)
            {

                FormControl.WL("Error updating fields on form: " + e.Message ,-1);
            }

            try
            {
                settingsXML = Linq.XDocument.Load(textBoxPathXML.Text);
                var root = settingsXML.Element("root");

                if (InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate 
                    {
                        textBoxPathLOG.Text = Val.PathLogFIle;
                        textBoxPathUALOG.Text = Val.PathUserActions;

                    }));
                }
                else
                {
                    textBoxPathLOG.Text = Val.PathLogFIle;
                    textBoxPathUALOG.Text = Val.PathUserActions;
                }
                settingsXML.Save(textBoxPathXML.Text);
            }
            catch (Exception ex)
            {
                FormControl.WL("Error updating fields on form (paths to log files): " + ex.Message, -1);
            }


        }

        private void UpdateFieldsLOGO1(Linq.XElement config)
        {
            CheckBoxLOGO_EN1.Checked = Convert.ToBoolean(config.Element("enabled").Value.Replace("\"", ""));

            textBoxDeviceIPLOGO1.Text = config.Element("serverIP").Value.Replace("\"", "");            

            textBoxLocalTSAPLOGO1.Text = config.Element("localTSAP").Value.Replace("\"", "");            

            textBoxRemoteTSAPLOGO1.Text = config.Element("remoteTSAP").Value.Replace("\"", "");            

            CheckBoxWatchdogENLOGO1.Checked = Convert.ToBoolean(config.Element("watchdogEN").Value.Replace("\"", ""));

            TextBoxWatchdogAddressLOGO1.Text = config.Element("watchdogAddress").Value.Replace("\"", "");
            TextBoxRWcycLOGO1.Text = config.Element("ReadWriteCycle").Value.Replace("\"", "");

            if (!Convert.ToBoolean(config.Element("show").Value.Replace("\"", ""))) { config.Element("enabled").SetValue("false"); config.Save(textBoxPathXML.Text); }

        }

        private void UpdateFieldsLOGO2(Linq.XElement config)
        {
            CheckBoxLOGO_EN2.Checked = Convert.ToBoolean(config.Element("enabled").Value.Replace("\"", ""));

            textBoxDeviceIPLOGO2.Text = config.Element("serverIP").Value.Replace("\"", "");
            
            textBoxLocalTSAPLOGO2.Text = config.Element("localTSAP").Value.Replace("\"", "");
           
            textBoxRemoteTSAPLOGO2.Text = config.Element("remoteTSAP").Value.Replace("\"", "");
            
            CheckBoxWatchdogENLOGO2.Checked = Convert.ToBoolean(config.Element("watchdogEN").Value.Replace("\"", ""));

            TextBoxWatchdogAddressLOGO2.Text = config.Element("watchdogAddress").Value.Replace("\"", "");
            TextBoxRWcycLOGO2.Text = config.Element("ReadWriteCycle").Value.Replace("\"", "");

            if (!Convert.ToBoolean(config.Element("show").Value.Replace("\"", ""))) { config.Element("enabled").SetValue("false"); config.Save(textBoxPathXML.Text); }

        }

        private void UpdateFieldsLOGO3(Linq.XElement config)
        {
            CheckBoxLOGO_EN3.Checked = Convert.ToBoolean(config.Element("enabled").Value.Replace("\"", ""));

            textBoxDeviceIPLOGO3.Text = config.Element("serverIP").Value.Replace("\"", "");
           
            textBoxLocalTSAPLOGO3.Text = config.Element("localTSAP").Value.Replace("\"", "");
            
            textBoxRemoteTSAPLOGO3.Text = config.Element("remoteTSAP").Value.Replace("\"", "");
            
            CheckBoxWatchdogENLOGO3.Checked = Convert.ToBoolean(config.Element("watchdogEN").Value.Replace("\"", ""));

            TextBoxWatchdogAddressLOGO3.Text = config.Element("watchdogAddress").Value.Replace("\"", "");
            TextBoxRWcycLOGO3.Text = config.Element("ReadWriteCycle").Value.Replace("\"", "");

            if (!Convert.ToBoolean(config.Element("show").Value.Replace("\"", ""))) { config.Element("enabled").SetValue("false"); config.Save(textBoxPathXML.Text); }

        }

        private void UpdateFieldsLOGO4(Linq.XElement config)
        {
            CheckBoxLOGO_EN4.Checked = Convert.ToBoolean(config.Element("enabled").Value.Replace("\"", ""));

            textBoxDeviceIPLOGO4.Text = config.Element("serverIP").Value.Replace("\"", "");
            
            textBoxLocalTSAPLOGO4.Text = config.Element("localTSAP").Value.Replace("\"", "");
            
            textBoxRemoteTSAPLOGO4.Text = config.Element("remoteTSAP").Value.Replace("\"", "");
            
            CheckBoxWatchdogENLOGO4.Checked = Convert.ToBoolean(config.Element("watchdogEN").Value.Replace("\"", ""));

            TextBoxWatchdogAddressLOGO4.Text = config.Element("watchdogAddress").Value.Replace("\"", "");
            TextBoxRWcycLOGO4.Text = config.Element("ReadWriteCycle").Value.Replace("\"", "");

            if (!Convert.ToBoolean(config.Element("show").Value.Replace("\"", ""))) { config.Element("enabled").SetValue("false"); config.Save(textBoxPathXML.Text); }

        }

        private void UpdateFieldsLOGO5(Linq.XElement config)
        {
            CheckBoxLOGO_EN5.Checked = Convert.ToBoolean(config.Element("enabled").Value.Replace("\"", ""));

            textBoxDeviceIPLOGO5.Text = config.Element("serverIP").Value.Replace("\"", "");
            
            textBoxLocalTSAPLOGO5.Text = config.Element("localTSAP").Value.Replace("\"", "");
            
            textBoxRemoteTSAPLOGO5.Text = config.Element("remoteTSAP").Value.Replace("\"", "");
            
            CheckBoxWatchdogENLOGO5.Checked = Convert.ToBoolean(config.Element("watchdogEN").Value.Replace("\"", ""));

            TextBoxWatchdogAddressLOGO5.Text = config.Element("watchdogAddress").Value.Replace("\"", "");
            TextBoxRWcycLOGO5.Text = config.Element("ReadWriteCycle").Value.Replace("\"", "");

            if (!Convert.ToBoolean(config.Element("show").Value.Replace("\"", ""))) { config.Element("enabled").SetValue("false"); config.Save(textBoxPathXML.Text); }
        }

        private void UpdateFieldsLOGO6(Linq.XElement config)
        {
            CheckBoxLOGO_EN6.Checked = Convert.ToBoolean(config.Element("enabled").Value.Replace("\"", ""));

            CheckBoxLOGO_EN6.Checked = Convert.ToBoolean(config.Element("enabled").Value.Replace("\"", ""));

            textBoxDeviceIPLOGO6.Text = config.Element("serverIP").Value.Replace("\"", "");
            
            textBoxLocalTSAPLOGO6.Text = config.Element("localTSAP").Value.Replace("\"", "");
            
            textBoxRemoteTSAPLOGO6.Text = config.Element("remoteTSAP").Value.Replace("\"", "");
            
            CheckBoxWatchdogENLOGO6.Checked = Convert.ToBoolean(config.Element("watchdogEN").Value.Replace("\"", ""));

            TextBoxWatchdogAddressLOGO6.Text = config.Element("watchdogAddress").Value.Replace("\"", "");
            TextBoxRWcycLOGO6.Text = config.Element("ReadWriteCycle").Value.Replace("\"", "");

            if (!Convert.ToBoolean(config.Element("show").Value.Replace("\"", ""))) { config.Element("enabled").SetValue("false"); config.Save(textBoxPathXML.Text); }
        }

        private void UpdateFieldsLOGO7(Linq.XElement config)
        {
            CheckBoxLOGO_EN7.Checked = Convert.ToBoolean(config.Element("enabled").Value.Replace("\"", ""));

            CheckBoxLOGO_EN7.Checked = Convert.ToBoolean(config.Element("enabled").Value.Replace("\"", ""));

            textBoxDeviceIPLOGO7.Text = config.Element("serverIP").Value.Replace("\"", "");
           
            textBoxLocalTSAPLOGO7.Text = config.Element("localTSAP").Value.Replace("\"", "");
           
            textBoxRemoteTSAPLOGO7.Text = config.Element("remoteTSAP").Value.Replace("\"", "");
           
            CheckBoxWatchdogENLOGO7.Checked = Convert.ToBoolean(config.Element("watchdogEN").Value.Replace("\"", ""));

            TextBoxWatchdogAddressLOGO7.Text = config.Element("watchdogAddress").Value.Replace("\"", "");
            TextBoxRWcycLOGO7.Text = config.Element("ReadWriteCycle").Value.Replace("\"", "");

            if (!Convert.ToBoolean(config.Element("show").Value.Replace("\"", ""))) { config.Element("enabled").SetValue("false"); config.Save(textBoxPathXML.Text); }

        }

        private void UpdateFieldsLOGO8(Linq.XElement config)
        {
            CheckBoxLOGO_EN8.Checked = Convert.ToBoolean(config.Element("enabled").Value.Replace("\"", ""));

            textBoxDeviceIPLOGO8.Text = config.Element("serverIP").Value.Replace("\"", "");
           
            textBoxLocalTSAPLOGO8.Text = config.Element("localTSAP").Value.Replace("\"", "");
           
            textBoxRemoteTSAPLOGO8.Text = config.Element("remoteTSAP").Value.Replace("\"", "");
           
            CheckBoxWatchdogENLOGO8.Checked = Convert.ToBoolean(config.Element("watchdogEN").Value.Replace("\"", ""));

            TextBoxWatchdogAddressLOGO8.Text = config.Element("watchdogAddress").Value.Replace("\"", "");
            TextBoxRWcycLOGO8.Text = config.Element("ReadWriteCycle").Value.Replace("\"", "");

            if (!Convert.ToBoolean(config.Element("show").Value.Replace("\"", ""))) { config.Element("enabled").SetValue("false"); config.Save(textBoxPathXML.Text); }

        }


        #endregion UPDATE_FIELDS

        

    }
}
