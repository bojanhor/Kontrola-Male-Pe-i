using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Gui_MalaPec : Form
    {
        bool debugDontDisableGuiOnConnectionLost = false;
        Prop1 p = Val.logocontroler.Prop1;
        Thread DisableGuiOnConnectionLossThread;
        StopWatch stpw;
        SysTimer PopulateChart;
        Prop1 prop = Val.logocontroler.Prop1;
        Datalogger dl;

        public Gui_MalaPec()
        {
            InitializeComponent();           
            FormatTopPanel();
            FormClosed += Gui_MalaPec_FormClosed;
            Resize += Gui_MalaPec_Resize;
            SetupForm();
            Load += Gui_MalaPec_Load;
            UrnikDataFeed();
            SensorErrDataFeed();
            Temperatures_Rpm_DataFeed();
            TempSelectorDataFeed();
            connectedButton1.ID = 1;
            DisableGuiOnConnectionLossThread = new Thread(DisableGuiOnConnectionLoss);

            stpw = new StopWatch(this);
            stpw.StopwatchStarted += Stpw_StopwatchStarted;
            stpw.StopwatchStopped += Stpw_StopwatchStopped;
            stpw.StopwatchWasReset += Stpw_StopwatchWasReset;
          
            TimerSetup();
            DataloggerSetup();

            chkPauseIfLowTemp.Checked = Properties.Settings.Default.PavzirajStopwatch;
        }

        void DataloggerSetup()
        {
            try
            {
                dl = new Datalogger();
            }
            catch (Exception)
            {
                throw new Exception("Inernal error: DataloggerSetup()");
            }
            
        }

        void TimerSetup()
        {
            PopulateChart = new SysTimer(60000); // chart interval
            PopulateChart.AutoReset = true;
            PopulateChart.Elapsed += PopulateChart_Elapsed;            
        }

        private void PopulateChart_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            populateChart();           
        }

        void populateChart()
        {
            var now = DateTime.Now;

            var cdp1 = new ChartDataPoint(now, prop.TempSenZg.Value_short);
            var cdp2 = new ChartDataPoint(now, prop.TempSenSr1.Value_short);
            var cdp3 = new ChartDataPoint(now, prop.TempSenSr2.Value_short);
            var cdp4 = new ChartDataPoint(now, prop.TempSenSp.Value_short);
            var cdp5 = new ChartDataPoint(now, prop.TempSenKn.Value_short);
            var cdp6 = new ChartDataPoint(now, prop.TempSenKos.Value_short);

            try
            {
                smartChart1.AddChartData(cdp1, cdp2, cdp3, cdp4, cdp5, cdp6);      
            }
            catch (Exception) { }

            try
            {
                dl.WriteLine(cdp1, cdp2, cdp3, cdp4, cdp5, cdp6);
            }
            catch (Exception) { }
            
        }                
        void populateChartFirstTime()
        {
            populateChart(); populateChart(); // two times is workaround. Chart doesnt show line if it has just one data point
        }

        private void Stpw_StopwatchWasReset(StopWatch sender)
        {
            dl.StopCsvFileWriter();
        }

        private void Stpw_StopwatchStopped(StopWatch sender)
        {
            PopulateChart.Enabled = false;
        }

        private void Stpw_StopwatchStarted(StopWatch sender)
        {
            dl.StartCsvFileWriter();
            PopulateChart.Enabled = true;
            populateChartFirstTime();           
        }

        void enableGui(bool _enable)
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                try
                {

                    if (Controls[i].GetType() == typeof(Panel) || Controls[i].GetType().Name == typeof(ConnectedButton).Name)
                    {

                    }
                    else
                    {                        
                        Controls[i].Enabled = _enable;                        
                    }

                }
                catch
                {

                }                
                addMessageConnectionLost(_enable);
            }
        }

        void DisableGuiOnConnectionLoss()
        {
            
            var m = new MethodInvoker(delegate
            {
                if (debugDontDisableGuiOnConnectionLost)
                {
                    return;
                }
                if (LogoControler.LOGOConnection[1].connectionStatusLOGO != Connection.Status.Connected)
                {
                    enableGui(false);
                }
                else
                {
                    enableGui(true);
                }
                

            });

            while (true)
            {
                Invoke(m);
                Thread.Sleep(Settings.UpdateValuesPCms);
            }
        }

        void addMessageConnectionLost(bool _add )
        {
            if (!_add)
            {
                WarningManager.AddMessageForUser_Warning(WarningManager.NoConnWarningPLC1);
            }
            else
            {
                WarningManager.RemoveMessageForUser_Warning(WarningManager.NoConnWarningPLC1);
            }
        }

        void TempSelectorDataFeed()
        {
            try
            {
                sensorSelect1.Value = p.SelT_Reg;
                sensorSelect2.Value = p.SelT_Dif;

                sensorSelect1.Value_SelectedSensorError = p.Sel_T_Err;
                sensorSelect2.Value_SelectedSensorError = p.Dif_T_Err;
            }
            catch (Exception)
            {
                throw new Exception("Internal error: TempSelectorDataFeed()");
            }
            
        }

        void Temperatures_Rpm_DataFeed()
        {
            try
            {
                temperatureSelector_0_3001.Value = p.TempStPnt1;
                temperatureSelector_0_3002.Value = p.TempStPnt2;
                rpmSelector_30_1001.Value = p.VentStdby;
                rpmSelector_30_1003.Value = p.VntCoolOff;
                timeToGoMinutes_1_301.Value = p.VentCoolOffTime;
                autoMan01Select1.Value_Auto = p.Auto;
                autoMan01Select1.Value_Man0 = p.Off;
                autoMan01Select1.Value_Man1 = p.On;
                histHeat2_101.Value = p.HistHeat;
                actuatorStatus1.Value_PlcBit = p.Heat1;
                actuatorStatus2.Value_PlcBit = p.Heat2;
                actuatorStatus3.Value_PlcBit = p.Heat3;
                actuatorStatus4.Value_PlcBit = p.Heat4;
                textboxShow1.Value = p.TempReg;
                textboxShow2.Value = p.VentRpmCurrent;
                textboxShow3.Value = p.TempDif;


                tb1sen1.Value = p.TempSenZg; tb1sen1.Prefix = "["; tb1sen1.Postfix = "°C]";
                tb1sen2.Value = p.TempSenSr1; tb1sen2.Prefix = "["; tb1sen2.Postfix = "°C]";
                tb1sen3.Value = p.TempSenSr2; tb1sen3.Prefix = "["; tb1sen3.Postfix = "°C]";
                tb1sen4.Value = p.TempSenSp; tb1sen4.Prefix = "["; tb1sen4.Postfix = "°C]";
                tb1sen5.Value = p.TempSenKn; tb1sen5.Prefix = "["; tb1sen5.Postfix = "°C]";
                tb1sen6.Value = p.TempSenKos; tb1sen6.Prefix = "["; tb1sen6.Postfix = "°C]";

                tb2Sen1.Value = p.TempSenZg; tb2Sen1.Prefix = "["; tb2Sen1.Postfix = "°C]";
                tb2Sen2.Value = p.TempSenSr1; tb2Sen2.Prefix = "["; tb2Sen2.Postfix = "°C]";
                tb2Sen3.Value = p.TempSenSr2; tb2Sen3.Prefix = "["; tb2Sen3.Postfix = "°C]";
                tb2Sen4.Value = p.TempSenSp; tb2Sen4.Prefix = "["; tb2Sen4.Postfix = "°C]";
                tb2sen5.Value = p.TempSenKn; tb2sen5.Prefix = "["; tb2sen5.Postfix = "°C]";
                tb2sen6.Value = p.TempSenKos; tb2sen6.Prefix = "["; tb2sen6.Postfix = "°C]";

                tbtempkanal.Value = p.TempSenKn;

                tbshowNajvisjaTerr.Value = p.TempErr;
            }
            catch (Exception)
            {

                throw new Exception("Internal error: Temperatures_Rpm_DataFeed()");
            }
            

        }

        string formatTemp1(string temperature)
        {
            return ("[" + temperature + "°C]");
        }

        void SensorErrDataFeed()
        {
            try
            {
                sensorStatus1.Value_PlcBit = p.SenFail1;
                sensorStatus2.Value_PlcBit = p.SenFail2;
                sensorStatus3.Value_PlcBit = p.SenFail3;
                sensorStatus4.Value_PlcBit = p.SenFail4;
                sensorStatus5.Value_PlcBit = p.SenFail5;
                sensorStatus6.Value_PlcBit = p.SenFail6;
            }
            catch (Exception)
            {
                throw new Exception("Internal error: SensorErrDataFeed()");
            }

        }

        void UrnikDataFeed()
        {
            try
            {
                urnikControl1.FeedData(
                p.Pon_EN_1, p.Tor_EN_1, p.Sre_EN_1, p.Čet_EN_1, p.Pet_EN_1, p.Sob_EN_1, p.Ned_EN_1,
                p.PonTimeOn, p.TorTimeOn, p.SreTimeOn, p.ČetTimeOn, p.PetTimeOn, p.SobTimeOn, p.NedTimeOn,
                p.PonTimeOff, p.TorTimeOff, p.SreTimeOff, p.ČetTimeOff, p.PetTimeOff, p.SobTimeOff, p.NedTimeOff);
            }
            catch (Exception)
            {
                throw new Exception("Internal error: UrnikDataFeed()");
            }
        }

        private void Gui_MalaPec_Load(object sender, EventArgs e)
        {           
            registerEvents();
            DisableGuiOnConnectionLossThread.Start();
            Val.GuiInitialised = true;
        }

        private void Gui_MalaPec_FormClosed(object sender, FormClosedEventArgs e)
        {
            Helper.ExitApp();
        }

       
        private void FormatTopPanel()
        {            
            panelTop.Width = Width;   
            positionBtnSettings();

        }

        void positionBtnSettings()
        {
            btnSettings.Left = panelTop.Right - btnSettings.Width - 50;
        }

        private void ReFormatTopPannel()
        {
            panelTop.Width = Width;
            positionBtnSettings();
        }

        private void SetupForm()
        {
            DoubleBuffered = true;
            Shown += Gui_MalaPec_Shown;
        }

        private void Gui_MalaPec_Shown(object sender, EventArgs e)
        {
            FormControl.Form_settings.Hide();
        }

        private void Gui_MalaPec_Resize(object sender, EventArgs e)
        {
            ReFormatTopPannel();           
        }


        void registerEvents()
        {
            btnSettings.Click += BtnSettings_Click;
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            FormControl.ShowForm_Settings();
            this.Hide();

        }

        private void sensorStatus1_Click(object sender, EventArgs e)
        {

        }

        private void Gui_MalaPec_Load_1(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void warningManager1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textboxShow1_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox7_Enter(object sender, EventArgs e)
        {

        }

        private void textboxShow4_TextChanged(object sender, EventArgs e)
        {

        }

        private void temperatureSelector_0_3001_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tb1sen1_TextChanged(object sender, EventArgs e)
        {

        }

        private void rpmSelector_30_1001_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tb1sen4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textboxShow3_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblTimeSet_Click(object sender, EventArgs e)
        {

        }

        private void domainUpDown1_SelectedItemChanged(object sender, EventArgs e)
        {

        }

        private void chkPauseIfLowTemp_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkPauseIfLowTemp.Checked)
            {
                stpw.ResumeStopwatchIfSettingChanged();
            }

            Properties.Settings.Default.PavzirajStopwatch = chkPauseIfLowTemp.Checked;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void temperatureDifference_5_301_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }

    class SysTimer : System.Timers.Timer
    {
        public SysTimer() : base()
        {

        }
        public SysTimer(double interval) : base(interval)
        {

        }
    }
}
