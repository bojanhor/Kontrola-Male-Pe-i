using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;

namespace WindowsFormsApp2
{
    public class Prop2 : PropComm
    {

        public PlcVars.Word auto, man0, man1;
        public PlcVars.Word loputa1PovratnaInfo, loputa2PovratnaInfo;

        public PlcVars.Word loputa1Nastavi, loputa2Nastavi;

        public PlcVars.Word automatskiVklopPri;

        public PlcVars.Bit setHitrostMala, setHitrostVelika, readHitrost;

        public Prop2(Sharp7.S7Client client):base(client)
        {
            auto = new PlcVars.Word(this, new PlcVars.WordAddress(10), true);
            man0 = new PlcVars.Word(this, new PlcVars.WordAddress(12), true);
            man1 = new PlcVars.Word(this, new PlcVars.WordAddress(14), true);

            setHitrostMala = new PlcVars.Bit(this, new PlcVars.BitAddress(20,0), true);
            setHitrostVelika = new PlcVars.Bit(this, new PlcVars.BitAddress(21, 0), true);
            readHitrost = new PlcVars.Bit(this, new PlcVars.BitAddress(22, 0), false);

            automatskiVklopPri = new PlcVars.Word(this, new PlcVars.WordAddress(40), true);

            loputa1Nastavi = new PlcVars.Word(this, new PlcVars.WordAddress(110), true);
            loputa2Nastavi = new PlcVars.Word(this, new PlcVars.WordAddress(112), true);

            loputa1PovratnaInfo = new PlcVars.Word(this, new PlcVars.WordAddress(120), false);
            loputa2PovratnaInfo = new PlcVars.Word(this, new PlcVars.WordAddress(130), false);


        }

    }
}
