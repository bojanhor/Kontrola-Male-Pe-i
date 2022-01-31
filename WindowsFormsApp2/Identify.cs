using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WindowsFormsApp2
{
    public class Identify
    {       
        public Int64 LoggedInID { get; private set; }
        public string LogedInUserName { get; private set; }
        private bool FormLoginIsShown = false;

        Form prompt = new Form();
        Label textLabel = new Label() { Left = 50, Top = 20, Width = 400 };
        MaskedTextBox inputBox = new MaskedTextBox() { Left = 50, Top = 50, Width = 400 };
        Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 80 };
        Button ExitForm = new Button() { Text = "Exit application", Left = 240, Width = 100, Top = 80 };
        Button LogoffOnForm = new Button() { Text = "LogOff", Left = 130, Width = 100, Top = 80 };
        private Int64[] identifiers = new Int64[31];
        string text = "Enter your ID or Scan your user Barcode - (1234 to stay logged off)";
        string caption = "Identification";
        int success = 0;

        public Identify()
        {
            prompt.ControlBox = true;
            prompt.MinimizeBox = false;
            prompt.MaximizeBox = false;
            prompt.Width = 500;
            prompt.Height = 200;
            prompt.Text = caption;
            textLabel.Text = text;
            inputBox.TabIndex = 0;
            inputBox.TextChanged += new EventHandler(delegate { TextChanged(null, null); });
            inputBox.KeyPress += new KeyPressEventHandler(Ok);
            inputBox.PasswordChar = '*';

            confirmation.Enabled = false;
            confirmation.TabIndex = 1;
            confirmation.Click += new EventHandler(delegate { Confirm(); });
            ExitForm.Click += new EventHandler(delegate { Exit(); });
            LogoffOnForm.Click += new EventHandler(delegate { Logoff(); });
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox);
            prompt.Controls.Add(ExitForm);
            if (FormControl.LogoffModeEnabled)
            {
                prompt.Controls.Add(LogoffOnForm);
            }


        }

        public void ShowIDDialog()
        {           
            Int64 buff;
            inputBox.Text = "";
            success = 0;

            try
            {
                if (!Convert.ToBoolean(XmlController.XmlGeneral.Element("LogInRequired").Value))
                {
                    LoggedInID = 1234;
                    return;
                }
            }
            catch
            { }

            while (success <= 0)
            {
                try
                {
                    FormLoginIsShown = true;
                    if (success == -1)
                    {
                        prompt.ShowDialog();
                    }

                    if (inputBox.Text == "")
                    {
                        prompt.ShowDialog();

                    }

                    if (inputBox.Text != "")
                    {
                        buff = Convert.ToInt64(inputBox.Text);
                        if (buff >= 0)
                        {
                            identifiers = IDs(XmlController.XmlFile);
                            if (identifiers != null)
                            {
                                for (int i = 1; i < identifiers.Length; i++)
                                {
                                    if (identifiers[i] == buff)
                                    {                                        
                                        LoggedInID = buff;
                                        SetNameOfLogedInUserPrivate();
                                        FormLoginIsShown = false;                                        
                                        return;
                                    }
                                }

                            }
                            else { throw new Exception("INTERNAL ERROR! Please check xml file settings."); }

                        }
                        throw new Exception("Wrong password or ID!");
                    }
                    FormLoginIsShown = false;
                    if (success == 2)
                    {
                        return;
                    }

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    success = -1;
                    FormLoginIsShown = false;
                }
            }
            FormLoginIsShown = false;
            MessageBox.Show("Internal error with login form! Application will now exit. Please check xml file settings.");
            Environment.Exit(0);

        }

        public static string GetUserFromID(Int64 ID)
        {
            try
            {
                for (int i = 1; i < 31; i++)
                {
                    if (ID == Convert.ToInt64(XmlController.XmlFile.Element("root").Element("USERS").Element("User" + i).Element("ID").Value))
                    {
                        return XmlController.XmlFile.Element("root").Element("USERS").Element("User" + i).Element("Name").Value;
                    }
                }

                return "ERROR! No such UserID.";
            }
            catch
            {
                throw new Exception("Internal error Retrievinng IDs of Users From XML File.");
            }

        }

        public Int64[] IDs(XDocument settingsXML)
        {
            try
            {
                try
                {
                    for (int i = 1; i < identifiers.Length; i++)
                    {
                        identifiers[i] = Convert.ToInt64(settingsXML.Element("root").Element("USERS").Element("User" + i).Element("ID").Value);
                    }
                    return identifiers;
                }
                catch
                {

                    throw new Exception();
                }



            }
            catch (Exception e)
            {
                MessageBox.Show("Error while loading user IDs and passwords from database. You will not be able to login: " + e.Message);
                return null;
            }


        }

        public void Logoff()
        {
            Int64 logofID = 1234;
            Int64 buff;
            success = 0;

            if (!FormControl.LogoffModeEnabled)
            {
                ShowIDDialog();
                return;
            }

                        

            while (success <= 0)
            {
                try
                {
                    buff = logofID;
                    if (buff >= 0)
                    {
                        identifiers = IDs(XmlController.XmlFile);
                        if (identifiers != null)
                        {
                            for (int i = 1; i < identifiers.Length; i++)
                            {
                                if (identifiers[i] == buff)
                                {
                                    LoggedInID = buff;
                                    SetNameOfLogedInUserPrivate();
                                    if (FormLoginIsShown)
                                    {
                                        success = 2;
                                        prompt.Hide();
                                        FormLoginIsShown = false;
                                    }
                                    break;
                                }
                            }


                        }
                        else { throw new Exception("INTERNAL ERROR! Please check xml file settings."); }
                        return;
                    }
                    else { throw new Exception("Wrong password or ID!"); }



                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    success = -1;
                }
            }
            MessageBox.Show("Internal error with login form! Application will now exit. Please check xml file settings.");
            Environment.Exit(0);

        }


        public string GetName(int index)
        {
            try
            {
                string a = XmlController.XmlFile.Element("root").Element("USERS").Element("User" + index).Element("Name").Value.Replace("\"", "");
                return a;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while loading user Names from database. You will not be able to login: " + e.Message);
                return null;
            }

        }

        public bool GetPermision(int permissionNum)
        {

            // 1  - Lahko se prijavi na startu programa
            // 2  - Lahko vstopa v meni nastavitev komunikacije
            // 3  - Lahko dostopa do nastavitev posamezne kadi (globalno)
            // 4  - Lahko poveže ali prekine povezavo s krmilniki z glavnega zaslona
            // 5  - Lahko zažene ali ustavi povezavo s sistemom glavnega zaslona
            // 6  - Lahko zažene ali ustavi posamezne kadi z glavnega zaslona
            // 7  - Lahko nastavlja prisilne zagone grelnikov
            // 8  - 
            // 9  - 
            // 10 - 
            var Collection = IDs(XmlController.XmlFile);
            for (int i = 1; i < Collection.Length; i++)
            {
                if (Collection[i] == LoggedInID)
                {
                    try
                    {
                        string a = XmlController.XmlFile.Element("root").Element("USERS").Element("User" + i).Element("permission" + permissionNum).Value.Replace("\"", "");
                        return Convert.ToBoolean(a);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Error while loading permissions from database. You will not be able acces this menu form or function: " + e.Message);
                        return false;
                    }
                }
            }
            return false;

        }

        public void ShowPermissionError()
        {
            MessageBox.Show("You do not have permission to proceed! Please log with different ID in and try again");            
        }

        public string GetNameOfLogedInUser()
        {
            return LogedInUserName;
        }

        public Int64 GetIDLogedInUser()
        {
            return LoggedInID;
        }

        private void SetNameOfLogedInUserPrivate()
        {
            Int64 tmp;
            try
            {
                for (int i = 1; i < identifiers.Length; i++)
                {
                    tmp = Convert.ToInt64(XmlController.XmlFile.Element("root").Element("USERS").Element("User" + i).Element("ID").Value.Replace("\"", ""));
                    if (tmp == LoggedInID)
                    {
                        LogedInUserName = GetName(i);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while loading user Names from database. You will not be able to login: " + e.Message);
            }
        }


        private void TextChanged(object sender, KeyPressEventArgs e)
        {

            if (inputBox.TextLength > 0)
            {
                confirmation.Enabled = true;

            }
            else
            {
                confirmation.Enabled = false;

            }

            if (true)
            {
                Int64 buff;
                try
                {
                    if (inputBox.Text != "")
                    {
                        buff = Convert.ToInt64(inputBox.Text);
                        if (buff >= 0)
                        {
                            identifiers = IDs(XmlController.XmlFile);
                            if (identifiers != null)
                            {
                                for (int i = 1; i < identifiers.Length; i++)
                                {
                                    if (identifiers[i] == buff)
                                    {
                                        if (confirmation.Enabled == true)
                                        {
                                            prompt.Close();
                                        }
                                        break;
                                    }
                                }
                            }
                            else { throw new Exception("INTERNAL ERROR! Please check xml file settings."); }
                            return;
                        }
                    }
                }
                catch { }

            }
        }

        private void Ok(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) // if enter
            {
                if (confirmation.Enabled == true)
                {
                    prompt.Close();
                    return;
                }
            }

            if (e.KeyChar == (char)Keys.D0) { return; }
            if (e.KeyChar == (char)Keys.D1) { return; }
            if (e.KeyChar == (char)Keys.D2) { return; }
            if (e.KeyChar == (char)Keys.D3) { return; }
            if (e.KeyChar == (char)Keys.D4) { return; }
            if (e.KeyChar == (char)Keys.D5) { return; }
            if (e.KeyChar == (char)Keys.D6) { return; }
            if (e.KeyChar == (char)Keys.D7) { return; }
            if (e.KeyChar == (char)Keys.D8) { return; }
            if (e.KeyChar == (char)Keys.D9) { return; }
            if (e.KeyChar == (char)Keys.Back) { return; }

            else { e.Handled = true; }

        }

        private void Confirm()
        {
            prompt.Close();
        }

        private void Exit()
        {
            if (MessageBox.Show("Are you sure you want to quit", "Quit?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

                Environment.Exit(0);
            }

        }


    }
}
