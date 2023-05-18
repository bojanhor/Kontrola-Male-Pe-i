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
using System.Timers;

namespace WindowsFormsApp2
{
    public partial class Gui_MalaPec : Form
    {        
        bool debugDontDisableGuiOnConnectionLost = false;

        Prop1 p1 = Val.logocontroler.Prop1; 
        Prop2 p2 = Val.logocontroler.Prop2;

        Thread DisableGuiOnConnectionLossThread;
        System.Timers.Timer RefreshGui;
        public StopWatch stpw;
        SysTimer PopulateChart;
        SysTimer UpdateLoputa;
        Prop1 prop = Val.logocontroler.Prop1;
        Datalogger dl;
        Odsesovanje Zracenje;
        uint StevecSarz = 0;
        MethodInvoker updateStopwatchTimeMethodInvoker;

        public Gui_MalaPec()
        {
            InitializeComponent();           
            FormatTopPanel();
            FormClosed += Gui_MalaPec_FormClosed;
            Resize += Gui_MalaPec_Resize;
            SetupForm();
            Load += Gui_MalaPec_Load;
            SensorErrDataFeed();
            Temperatures_Rpm_DataFeed();
            TempSelectorDataFeed();
            connectedButton1.ID = 1;
            DisableGuiOnConnectionLossThread = new Thread(DisableGuiOnConnectionLoss);

            stpw = new StopWatch(this);
            Val.StopWatch = stpw;

            Zracenje = new Odsesovanje(this);
          
            TimerSetup();

            RefreshGui = new System.Timers.Timer(100);
            RefreshGui.AutoReset = true;
            RefreshGui.Elapsed += RefreshGui_Elapsed;
            RefreshGui.Start();

            updateStopwatchTimeMethodInvoker = new MethodInvoker(updateStopwatchTimeMethod);

        }

        private void RefreshGui_Elapsed(object sender, ElapsedEventArgs e)
        {
            updateStopwatchTime();
        }

        int count = 0;
        void updateStopwatchTimeMethod()
        {
            var timeInSeconds = prop.StopwatchTime.Value_short;
            var time = TimeSpan.FromSeconds(timeInSeconds);
            var txt = TimeToString(time);
            FormControl.Gui.lblStopwatchTime.Text = txt;

            timeInSeconds = prop.TimeSet.Value_short;
            time = TimeSpan.FromSeconds(timeInSeconds);
            txt = TimeToString(time);
            FormControl.Gui.lblTimeSet1.Text = txt;

            if (count > 10)
            {
                count = 0;
                if (p1.PauseIfTlow.Value_short == 1)
                {
                    cbPauseIfTlow.Checked = true;
                }
                else
                {
                    cbPauseIfTlow.Checked = false;
                }
            }

            count++;
        }
                
        void updateStopwatchTime()
        {
            try
            {
                FormControl.Gui.Invoke(updateStopwatchTimeMethodInvoker);
            }
            catch 
            {

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
            var cdp7 = new ChartDataPoint(now, prop.TempStPnt1.Value_short);
            var cdp8 = new ChartDataPoint(now, prop.TempStPnt2.Value_short);
            var nfo9 = FormControl.Gui.stpw.Counting.ToString();

            try
            {
                smartChart1.AddChartData(cdp1, cdp2, cdp3, cdp4, cdp5, cdp6);      
            }
            catch (Exception) { }

            try
            {
                dl.WriteLine(cdp1, cdp2, cdp3, cdp4, cdp5, cdp6, cdp7, cdp8, nfo9);
            }
            catch (Exception) 
            { }
            
        }                
        void populateChartFirstTime()
        {
            populateChart();         
            populateChart(); // two times is workaround. Chart doesnt show line if it has just one data point
        }

        private void Stpw_StopwatchWasReset(StopWatch sender)
        {
            if (dl != null)
            {
                dl.StopCsvFileWriter();
                dl = null;
            }            
        }

        private void Stpw_StopwatchStopped(StopWatch sender)
        {
            populateChart();
        }

        private void Stpw_StopwatchStarted(StopWatch sender)
        {
            if (dl == null)
            {
                // occours when datalogger and chart need to be reset
                dl = new Datalogger();
                smartChart1.ResetChart();
                populateChartFirstTime();
                PopulateChart.Enabled = true;
                IncrementStevecSarz();

            }
            dl.StartCsvFileWriter();

        }

        void IncrementStevecSarz()
        {
            StevecSarz++;
            lblStevecSarz.Text = StevecSarz.ToString();
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
                sensorSelect1.Value = p1.SelT_Reg;
                sensorSelect2.Value = p1.SelT_Dif;

                sensorSelect1.Value_SelectedSensorError = p1.Sel_T_Err;
                sensorSelect2.Value_SelectedSensorError = p1.Dif_T_Err;
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
                temperatureSelector_0_3001.Value = p1.TempStPnt1;
                temperatureSelector_0_3002.Value = p1.TempStPnt2;
                rpmSelector_30_1001.Value = p1.VentStdby;
                rpmSelector_30_1003.Value = p1.VntCoolOff;
                timeToGoMinutes_1_301.Value = p1.VentCoolOffTime;
                autoMan01Select1.Value = p1.PecOnStatus;
                autoMan01Select1.OnPulse = p1.PecOnPulse;
                autoMan01Select1.OffPulse = p1.PecOffPulse;
                histHeat2_101.Value = p1.HistHeat;
                actuatorStatus1.Value_PlcBit = p1.Heat1;
                actuatorStatus2.Value_PlcBit = p1.Heat2;
                actuatorStatus3.Value_PlcBit = p1.Heat3;
                actuatorStatus4.Value_PlcBit = p1.Heat4;
                textboxShow1.Value = p1.TempReg;
                textboxShow2.Value = p1.VentRpmCurrent;
                textboxShow3.Value = p1.TempDif;


                tb1sen1.Value = p1.TempSenZg; tb1sen1.Prefix = "["; tb1sen1.Postfix = "°C]";
                tb1sen2.Value = p1.TempSenSr1; tb1sen2.Prefix = "["; tb1sen2.Postfix = "°C]";
                tb1sen3.Value = p1.TempSenSr2; tb1sen3.Prefix = "["; tb1sen3.Postfix = "°C]";
                tb1sen4.Value = p1.TempSenSp; tb1sen4.Prefix = "["; tb1sen4.Postfix = "°C]";
                tb1sen5.Value = p1.TempSenKn; tb1sen5.Prefix = "["; tb1sen5.Postfix = "°C]";
                tb1sen6.Value = p1.TempSenKos; tb1sen6.Prefix = "["; tb1sen6.Postfix = "°C]";

                tb2Sen1.Value = p1.TempSenZg; tb2Sen1.Prefix = "["; tb2Sen1.Postfix = "°C]";
                tb2Sen2.Value = p1.TempSenSr1; tb2Sen2.Prefix = "["; tb2Sen2.Postfix = "°C]";
                tb2Sen3.Value = p1.TempSenSr2; tb2Sen3.Prefix = "["; tb2Sen3.Postfix = "°C]";
                tb2Sen4.Value = p1.TempSenSp; tb2Sen4.Prefix = "["; tb2Sen4.Postfix = "°C]";
                tb2sen5.Value = p1.TempSenKn; tb2sen5.Prefix = "["; tb2sen5.Postfix = "°C]";
                tb2sen6.Value = p1.TempSenKos; tb2sen6.Prefix = "["; tb2sen6.Postfix = "°C]";

                tbtempkanal.Value = p1.TempSenKn;

                tbshowNajvisjaTerr.Value = p1.TempErr;
            }
            catch (Exception)
            {

                throw new Exception("Internal error: Temperatures_Rpm_DataFeed()");
            }
            

        }

        void SensorErrDataFeed()
        {
            try
            {
                sensorStatus1.Value_PlcBit = p1.SenFail1;
                sensorStatus2.Value_PlcBit = p1.SenFail2;
                sensorStatus3.Value_PlcBit = p1.SenFail3;
                sensorStatus4.Value_PlcBit = p1.SenFail4;
                sensorStatus5.Value_PlcBit = p1.SenFail5;
                sensorStatus6.Value_PlcBit = p1.SenFail6;
            }
            catch (Exception)
            {
                throw new Exception("Internal error: SensorErrDataFeed()");
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
       
        private void btnStart_Click(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void temperatureDifference_5_301_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox7_Enter_1(object sender, EventArgs e)
        {

        }

        private void tbOdpirajLoputeDo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        string TimeToString(TimeSpan val)
        {
            return val.ToString("c");
        }

        private void btn_Plus1_Click(object sender, EventArgs e)
        {
            if (p1.TimeSet.Value_short >= 18000)
            {
                p1.TimeSet.Value_short = 18000;
                return;
            }
            p1.TimeSet.Value_short += 60;
        }

        private void btn_Plus10_Click(object sender, EventArgs e)
        {
            if (p1.TimeSet.Value_short >= 18000)
            {
                p1.TimeSet.Value_short = 18000;
                return;
            }
            p1.TimeSet.Value_short += 600;
        }

        private void btn_Plus30_Click(object sender, EventArgs e)
        {
            if (p1.TimeSet.Value_short >= 18000)
            {
                p1.TimeSet.Value_short = 18000;
                return;
            }
            p1.TimeSet.Value_short += 1800;
        }

        private void btn_Minus1_Click(object sender, EventArgs e)
        {
            if (p1.TimeSet.Value_short < 60)
            {
                p1.TimeSet.Value_short = 0;
                return;
            }
            p1.TimeSet.Value_short -= 60;
        }

        private void btn_Minus10_Click(object sender, EventArgs e)
        {
            if (p1.TimeSet.Value_short < 600)
            {
                p1.TimeSet.Value_short = 0;
                return;
            }
            p1.TimeSet.Value_short -= 600;
        }

        private void btn_Minus30_Click(object sender, EventArgs e)
        {
            if (p1.TimeSet.Value_short < 1800)
            {
                p1.TimeSet.Value_short = 0;
                return;
            }
            p1.TimeSet.Value_short -= 1800;
        }

        private void cbPauseIfTlow_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPauseIfTlow.Checked)             
            {
                p1.PauseIfTlow.Value_short = 1;
            }
            else
            {
                p1.PauseIfTlow.Value_short = 0;
            }
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
