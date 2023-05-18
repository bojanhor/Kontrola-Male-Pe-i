using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Timers;
using Timer = System.Timers.Timer;

namespace WindowsFormsApp2
{
    class Odsesovanje
    {
        AutoMan01Select selector;
        TextBox tbLoputaDejanska1, tbLoputaDejanska2;
        LoputaSelector_0_100 tbLoputaNastavi1, tbLoputaNastavi2;
        Man01Select hitrost;
        TemperatureSelector_0_250 avtomatskiVklopPri;

        Prop2 p2 = Val.logocontroler.Prop2;
        Form Parent;

        Timer updater = new Timer(500);
        
        public Odsesovanje(Form Parent)
        {
            this.Parent = Parent;
            selector = (AutoMan01Select)Parent.Controls.Find("autoMan01Select2", true)[0];
            tbLoputaDejanska1 = (TextBox)Parent.Controls.Find("tbLoputaDejanska1", true)[0];
            tbLoputaDejanska2 = (TextBox)Parent.Controls.Find("tbLoputaDejanska2", true)[0];
            tbLoputaNastavi1 = (LoputaSelector_0_100)Parent.Controls.Find("loputaSelector1", true)[0];
            tbLoputaNastavi2 = (LoputaSelector_0_100)Parent.Controls.Find("loputaSelector2", true)[0];
            hitrost = (Man01Select)Parent.Controls.Find("man01Select1", true)[0];
            avtomatskiVklopPri = (TemperatureSelector_0_250)Parent.Controls.Find("tsAutoVklop", true)[0];            

            selector.Value_Auto = p2.auto;
            selector.Value_Man0 = p2.man0;
            selector.Value_Man1 = p2.man1;

            hitrost.OffPulse = p2.setHitrostMala;
            hitrost.OnPulse = p2.setHitrostVelika;
            hitrost.Value = p2.readHitrost;

            tbLoputaNastavi1.Value = p2.loputa1Nastavi;
            tbLoputaNastavi2.Value = p2.loputa2Nastavi;

            avtomatskiVklopPri.Value = p2.automatskiVklopPri;

            updateGuiValuesMethodInvoker = new MethodInvoker(updateGuiValues);

            updater.Elapsed += Updater_Elapsed;
            updater.Start();

        }

        MethodInvoker updateGuiValuesMethodInvoker;

        void updateGuiValues()
        {
            if (Parent.InvokeRequired)
            {
                Parent.Invoke(updateGuiValuesMethodInvoker);
            }
            else
            {
                tbLoputaDejanska1.Text = p2.loputa1PovratnaInfo.Value_string + "%";
                tbLoputaDejanska2.Text = p2.loputa2PovratnaInfo.Value_string + "%";
            }
        }

        private void Updater_Elapsed(object sender, ElapsedEventArgs e)
        {
            updateGuiValues();
        }
    }
}