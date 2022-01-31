using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    
    class UrnikControl : DataGridView
    {
        public Font FontForContent;
        bool WriteMode = false;
        bool datafeedWasDone = false;

        public PlcVars.Word Pon_EN_1; public PlcVars.TimeSet PonTimeOn; public PlcVars.TimeSet PonTimeOff;
        public PlcVars.Word Tor_EN_1; public PlcVars.TimeSet TorTimeOn; public PlcVars.TimeSet TorTimeOff;
        public PlcVars.Word Sre_EN_1; public PlcVars.TimeSet SreTimeOn; public PlcVars.TimeSet SreTimeOff;
        public PlcVars.Word Čet_EN_1; public PlcVars.TimeSet ČetTimeOn; public PlcVars.TimeSet ČetTimeOff;
        public PlcVars.Word Pet_EN_1; public PlcVars.TimeSet PetTimeOn; public PlcVars.TimeSet PetTimeOff;
        public PlcVars.Word Sob_EN_1; public PlcVars.TimeSet SobTimeOn; public PlcVars.TimeSet SobTimeOff;
        public PlcVars.Word Ned_EN_1; public PlcVars.TimeSet NedTimeOn; public PlcVars.TimeSet NedTimeOff;

        public UrnikControl()
        {
            Ctor();
        }

        void Ctor()
        {
            bool designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);

            PreformatDataGrid(); 

            if (!designMode)
            {                
                FillDatagridUrnik();
                FormatDataGrid();
                registerEvents();
                UpdaterThreadSetup();
                
            }            

        }

        public void FeedData(
            PlcVars.Word Pon_EN_1, PlcVars.Word Tor_EN_1, PlcVars.Word Sre_EN_1, PlcVars.Word Čet_EN_1, PlcVars.Word Pet_EN_1, PlcVars.Word Sob_EN_1, PlcVars.Word Ned_EN_1,
            PlcVars.TimeSet PonTimeOn, PlcVars.TimeSet TorTimeOn, PlcVars.TimeSet SreTimeOn, PlcVars.TimeSet ČetTimeOn, PlcVars.TimeSet PetTimeOn, PlcVars.TimeSet SobTimeOn, PlcVars.TimeSet NedTimeOn,
            PlcVars.TimeSet PonTimeOff, PlcVars.TimeSet TorTimeOff, PlcVars.TimeSet SreTimeOff, PlcVars.TimeSet ČetTimeOff, PlcVars.TimeSet PetTimeOff, PlcVars.TimeSet SobTimeOff, PlcVars.TimeSet NedTimeOff)
        {
            this.Pon_EN_1 = Pon_EN_1; this.PonTimeOn = PonTimeOn; this.PonTimeOff = PonTimeOff;
            this.Tor_EN_1 = Tor_EN_1; this.TorTimeOn = TorTimeOn; this.TorTimeOff = TorTimeOff;
            this.Sre_EN_1 = Sre_EN_1; this.SreTimeOn = SreTimeOn; this.SreTimeOff = SreTimeOff;
            this.Čet_EN_1 = Čet_EN_1; this.ČetTimeOn = ČetTimeOn; this.ČetTimeOff = ČetTimeOff;
            this.Pet_EN_1 = Pet_EN_1; this.PetTimeOn = PetTimeOn; this.PetTimeOff = PetTimeOff;
            this.Sob_EN_1 = Sob_EN_1; this.SobTimeOn = SobTimeOn; this.SobTimeOff = SobTimeOff;
            this.Ned_EN_1 = Ned_EN_1; this.NedTimeOn = NedTimeOn; this.NedTimeOff = NedTimeOff;
            datafeedWasDone = true;
        }

        void DataFeederChk()
        {
            if (!datafeedWasDone)
            {
                string msg = "Fatal error. You forgot to feed data to UrnikControl. You must run method FeedData(...) to overcome this error.";
                MessageBox.Show(msg);
                throw new Exception(msg);
            }
        }

        void UpdaterThreadSetup()
        {
            Thread UpdaterThread = new Thread(() => Updater());
            UpdaterThread.Start();
        }

        void Updater()
        {
            while (Parent == null)
            {
                Thread.Sleep(100);
            }
            Thread.Sleep(1000);

            RepairFormating();

            DataFeederChk();
            MethodInvoker m = new MethodInvoker(UpdateValues);
            while (true)
            {               
                FormControl.Gui.Invoke(m);                
                Thread.Sleep(Settings.UpdateValuesPCms);
            }

        }

        void FillDatagridUrnik()
        {
            DataGridViewTextBoxColumn dgvc1 = new DataGridViewTextBoxColumn();
            dgvc1.HeaderText = "Dan";
            dgvc1.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvc1.ReadOnly = true;

            DataGridViewComboBoxColumn cbc2 = new DataGridViewComboBoxColumn();
            cbc2.HeaderText = "On";
            cbc2.SortMode = DataGridViewColumnSortMode.NotSortable;            

            DataGridViewComboBoxColumn cbc3 = new DataGridViewComboBoxColumn();
            cbc3.HeaderText = "Off";
            cbc3.SortMode = DataGridViewColumnSortMode.NotSortable;

            DataGridViewCheckBoxColumn cbc4 = new DataGridViewCheckBoxColumn();
            cbc4.HeaderText = "En";
            cbc4.SortMode = DataGridViewColumnSortMode.NotSortable;


            Columns.Add(dgvc1);
            Columns.Add(cbc2);
            Columns.Add(cbc3);
            Columns.Add(cbc4);

            DataGridViewRow r = new DataGridViewRow();
            Rows.Add(); Rows.Add(); Rows.Add(); Rows.Add(); Rows.Add(); Rows.Add(); Rows.Add();

            var i = 0;
            Rows[0].Cells[i].Value = "Pon";
            Rows[1].Cells[i].Value = "Tor";
            Rows[2].Cells[i].Value = "Sre";
            Rows[3].Cells[i].Value = "Čet";
            Rows[4].Cells[i].Value = "Pet";
            Rows[5].Cells[i].Value = "Sob";
            Rows[6].Cells[i].Value = "Ned";

            i = 1;
            Rows[0].Cells[i] = new TimeSelector();
            Rows[1].Cells[i] = new TimeSelector();
            Rows[2].Cells[i] = new TimeSelector();
            Rows[3].Cells[i] = new TimeSelector();
            Rows[4].Cells[i] = new TimeSelector();
            Rows[5].Cells[i] = new TimeSelector();
            Rows[6].Cells[i] = new TimeSelector();

            i = 2;
            Rows[0].Cells[i] = new TimeSelector();
            Rows[1].Cells[i] = new TimeSelector();
            Rows[2].Cells[i] = new TimeSelector();
            Rows[3].Cells[i] = new TimeSelector();
            Rows[4].Cells[i] = new TimeSelector();
            Rows[5].Cells[i] = new TimeSelector();
            Rows[6].Cells[i] = new TimeSelector();

        }

        void PreformatDataGrid()
        {
            Width = 260;
            Height = 184;

            EditMode = DataGridViewEditMode.EditOnEnter;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            AllowUserToOrderColumns = false;
            AllowUserToResizeColumns = false;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;            
            RowHeadersVisible = false;
            RowTemplate.DividerHeight = 1;
            AllowUserToResizeRows = false;
            ScrollBars = ScrollBars.None;
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DefaultCellStyle.Font = FontForContent;
        }
        void FormatDataGrid()
        {
            FontForContent = new Font("Sant Serif", 12);
            Columns[0].Width = 50;
            Columns[1].Width = 80;
            Columns[2].Width = 80;
            Columns[3].Width = 45;
            Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ColumnHeadersDefaultCellStyle.Font = FontForContent;
            ColumnHeadersHeight = 30;
            Rows[0].Cells[0].Selected = false;            
                       
        }

        void registerEvents()
        {
            SelectionChanged += Dg_SelectionChanged;            
            CurrentCellDirtyStateChanged += Dg_CurrentCellDirtyStateChanged;
            CellContentClick += dg_CellContentClick;
            CellValueChanged += UrnikControl_CellValueChanged;
            CellMouseLeave += UrnikControl_CellMouseLeave;
        }

        private void UrnikControl_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
           EndEditing();
        }

        
        private void UrnikControl_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
           EndEditing();            
        }

        
        private void Dg_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            WriteMode = true;
            EndEditing();
        }


        private void Dg_SelectionChanged(object sender, EventArgs e)
        {
            ClearSelection();
        }

        private void dg_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {           
           ClearSelection();            
        }
        
        void RepairFormating()
        {
            var m = new MethodInvoker(delegate 
            {               
                var rh = Rows[0].Height * 7;
                rh += ColumnHeadersHeight;
                Height = rh + 1;

                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                
            });

            Parent.Invoke(m);

        }

        void EndEditing()
        {
            CommitEdit(DataGridViewDataErrorContexts.Commit);
            EndEdit();
            CurrentCell = null;
        }
        void UpdateValues()
        {            
            ReadFromPlc(); 
            WriteToPlc();

        }

        void ReadFromPlc()
        {
            if (WriteMode)
            {
                return;
            }

            int i = 1;
           
            Rows[0].Cells[i].Value = PonTimeOn.Value;
            Rows[1].Cells[i].Value = TorTimeOn.Value;
            Rows[2].Cells[i].Value = SreTimeOn.Value;
            Rows[3].Cells[i].Value = ČetTimeOn.Value;
            Rows[4].Cells[i].Value = PetTimeOn.Value;
            Rows[5].Cells[i].Value = SobTimeOn.Value;
            Rows[6].Cells[i].Value = NedTimeOn.Value;

            i = 2;
            Rows[0].Cells[i].Value = PonTimeOff.Value;
            Rows[1].Cells[i].Value = TorTimeOff.Value;
            Rows[2].Cells[i].Value = SreTimeOff.Value;
            Rows[3].Cells[i].Value = ČetTimeOff.Value;
            Rows[4].Cells[i].Value = PetTimeOff.Value;
            Rows[5].Cells[i].Value = SobTimeOff.Value;
            Rows[6].Cells[i].Value = NedTimeOff.Value;

            i = 3;
            (this[i, 0]).Value = Pon_EN_1.Value_short;
            (this[i, 1]).Value = Tor_EN_1.Value_short;
            (this[i, 2]).Value = Sre_EN_1.Value_short;
            (this[i, 3]).Value = Čet_EN_1.Value_short;
            (this[i, 4]).Value = Pet_EN_1.Value_short;
            (this[i, 5]).Value = Sob_EN_1.Value_short;
            (this[i, 6]).Value = Ned_EN_1.Value_short;

            
        }

        void WriteToPlc()
        {
            if (!WriteMode)
            {
                return;
            }
                     
            object buff;
            int i = 1;
           
            buff = Rows[0].Cells[i].Value; PonTimeOn.Value = buff.ToString();
            buff = Rows[1].Cells[i].Value; TorTimeOn.Value = buff.ToString();
            buff = Rows[2].Cells[i].Value; SreTimeOn.Value = buff.ToString();
            buff = Rows[3].Cells[i].Value; ČetTimeOn.Value = buff.ToString();
            buff = Rows[4].Cells[i].Value; PetTimeOn.Value = buff.ToString();
            buff = Rows[5].Cells[i].Value; SobTimeOn.Value = buff.ToString();
            buff = Rows[6].Cells[i].Value; NedTimeOn.Value = buff.ToString();

            i = 2;
            buff = Rows[0].Cells[i].Value; PonTimeOff.Value = buff.ToString();
            buff = Rows[1].Cells[i].Value; TorTimeOff.Value = buff.ToString();
            buff = Rows[2].Cells[i].Value; SreTimeOff.Value = buff.ToString();
            buff = Rows[3].Cells[i].Value; ČetTimeOff.Value = buff.ToString();
            buff = Rows[4].Cells[i].Value; PetTimeOff.Value = buff.ToString();
            buff = Rows[5].Cells[i].Value; SobTimeOff.Value = buff.ToString();
            buff = Rows[6].Cells[i].Value; NedTimeOff.Value = buff.ToString();

            i = 3;
            buff = ((DataGridViewCheckBoxCell)this[i, 0]).Value; Pon_EN_1.Value_short = toShort(buff);
            buff = ((DataGridViewCheckBoxCell)this[i, 1]).Value; Tor_EN_1.Value_short = toShort(buff);
            buff = ((DataGridViewCheckBoxCell)this[i, 2]).Value; Sre_EN_1.Value_short = toShort(buff);
            buff = ((DataGridViewCheckBoxCell)this[i, 3]).Value; Čet_EN_1.Value_short = toShort(buff);
            buff = ((DataGridViewCheckBoxCell)this[i, 4]).Value; Pet_EN_1.Value_short = toShort(buff);
            buff = ((DataGridViewCheckBoxCell)this[i, 5]).Value; Sob_EN_1.Value_short = toShort(buff);
            buff = ((DataGridViewCheckBoxCell)this[i, 6]).Value; Ned_EN_1.Value_short = toShort(buff);
            
            WriteMode = false;
           

        }

        short toShort(object val)
        {
            if (val == null)
            {
                return 0;
            }

            try
            {
                if (val.GetType() == typeof(bool))
                {
                    if ((bool) val == true)
                    {
                        return 1;
                    }
                    return 0;
                }
                if (val.GetType() == typeof(short))
                {
                    return (short)val;
                }
                throw new Exception("TypeError");
               
            }
            catch 
            { return 0; }

        }

    }
}
