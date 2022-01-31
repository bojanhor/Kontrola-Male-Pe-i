using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Threading;

namespace WindowsFormsApp2
{
    public class ConnectedButton : Button
    {
        private int id = -1;

        public int ID
        {
            get 
            {
                return id; 
            }
            set { id = value; }
        }

        void IDerrHandler()
        {
            if (id == -1)
            {
                var msg = "ID was not set for control ConnectedButton. You can set property ID to any number greater from 1. You must set it in Designer, or programatilcally AFTER FORM LOADS.";
                MessageBox.Show(msg);
                throw new Exception(msg);
            }
        }
        
        public int ConnectionStatus { get; set; }        
        private Bitmap disconnectedIcon;
        private Bitmap connectedIcon;
        private Bitmap connectedWarningIcon;
        private Bitmap connectingIcon;
        private int connstatcnt = 0;
        
       
        float PictureSize = 1.25F;
        
        public int RefreshOriginalVal { get; set; }

        public ConnectedButton()
        {
            bool designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);

            Text = "";

            RefreshOriginalVal = 500;
            ConnectionStatus = (int)Connection.Status.NotInitialised;

            disconnectedIcon = Properties.Resources.disconnected;
            connectedIcon = Properties.Resources.connected;
            connectedWarningIcon = Properties.Resources.connect_warning1;
            connectingIcon = Properties.Resources.connecing;
            BackgroundImage = disconnectedIcon;
            BackColor = DefaultBackColor;

            disconnectedIcon = Misc.Scale(disconnectedIcon, Height * PictureSize);
            connectedIcon = Misc.Scale(connectedIcon, Height * PictureSize);
            connectedWarningIcon = Misc.Scale(connectedWarningIcon, Height * PictureSize);
            connectingIcon = Misc.Scale(connectingIcon, Height * PictureSize);
            BackgroundImageLayout = ImageLayout.Center;
            BackColor = Color.Transparent;
            FormatBtn();
            TextChanged += ConnectedButton_TextChanged;
            UpdateConnectionStatus();

            if (designMode)
            {
                return;
            }

            // object own status retriever
            Thread updater = new Thread(new ThreadStart(Updater))
            {
                IsBackground = true,
                Name = "Updater Thread"
            };

            updater.Start();
            this.Click += Clicked;
           

        }

        private void ConnectedButton_TextChanged(object sender, EventArgs e)
        {
            if (Text != "")
            {
                Text = "";
            }
            
        }

        void FormatBtn()
        {
            Width = 60;
            Height = 40;            
        }

        
        private void Clicked(object sender, EventArgs e)
        {
            
            if (ConnectionStatus == (int)Connection.Status.Error)
            {
                Connect();
            }
            else if (ConnectionStatus == (int)Connection.Status.Connecting)
            {
                Disconnect();
            }
            else if (ConnectionStatus == (int)Connection.Status.Connected)
            {
                Disconnect();
            }
            else if (ConnectionStatus == (int)Connection.Status.Warning)
            {
                Disconnect();                
            }
            else
            {                
                Connect();
            }

        }

        public void Connect()
        {   
            if (FormControl.identify.GetPermision(4))
            {
                BackgroundImage = connectingIcon;
                FormControl.Form_settings.ConnectAsync(this.ID);
            }
            else
            {
                FormControl.identify.ShowPermissionError();
            }
        }

        public void Disconnect()
        {
            if (FormControl.identify.GetPermision(4))
            {
                BackgroundImage = connectingIcon;
                FormControl.Form_settings.DisconnectAsync(this.ID);
            }
            else
            {
                FormControl.identify.ShowPermissionError();
            }
        }

        public void UpdateConnectionStatus()
        {
            if (ConnectionStatus == (int)Connection.Status.NotInitialised){
                BackgroundImage = disconnectedIcon;
                connstatcnt = 0;
                return;}

            if (ConnectionStatus == (int)Connection.Status.Error){
                BackgroundImage = disconnectedIcon;
                connstatcnt = 0;
                return;}

            if (ConnectionStatus == (int)Connection.Status.Warning){
                BackgroundImage = connectedWarningIcon;
                connstatcnt = 0;
                return;}

            if (ConnectionStatus == (int)Connection.Status.Connecting){
                BackgroundImage = connectingIcon;
                connstatcnt = 0;
                return;}

            if (ConnectionStatus == (int)Connection.Status.Connected){       
                if (connstatcnt >= 2){
                    BackgroundImage = connectedIcon;}
                connstatcnt++;
                return;}            
        }
                

        public void RetrieveConnectionStatus()
        {
            try
            {                
                ConnectionStatus = (int)LogoControler.LOGOConnection[ID].connectionStatusLOGO;
            }
            catch (Exception e)
            {
                ConnectionStatus = (int)Connection.Status.Error;
                var a = e.Message;
            }

        }

        public void Updater()
        {            
            while (true)
            {
                try
                {
                    UpdateConnectionStatus();
                    Thread.Sleep(RefreshOriginalVal);
                    IDerrHandler();

                    RetrieveConnectionStatus();                                      
                    
                }
                catch 
                {
                    Thread.Sleep(3000);
                }                
            }
            
        }
        
    }
}
