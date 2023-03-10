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

        public PlcVars.Word Musslauf_Zracenje, MinZracenje, MaxZracenje;
        public PlcVars.Word StopnjaZracenja;
        public PlcVars.Word PozicijaLoputeNastavljena;
        public PlcVars.Word PozicijaLoputeDejanska;
        public PlcVars.Word ZracenjeOnTime;
        public PlcVars.Word ZracenjeOffTime;

        public Prop2(Sharp7.S7Client client):base(client)
        {
            MinZracenje = new PlcVars.Word(this, new PlcVars.WordAddress(8), true);
            MaxZracenje = new PlcVars.Word(this, new PlcVars.WordAddress(10), true);
            Musslauf_Zracenje = new PlcVars.Word(this, new PlcVars.WordAddress(12), true);
            StopnjaZracenja = new PlcVars.Word(this, new PlcVars.WordAddress(100), false);
            PozicijaLoputeNastavljena = new PlcVars.Word(this, new PlcVars.WordAddress(110), false);
            PozicijaLoputeDejanska = new PlcVars.Word(this, new PlcVars.WordAddress(120), false);
            ZracenjeOnTime = new PlcVars.Word(this, new PlcVars.WordAddress(14), true);
            ZracenjeOffTime = new PlcVars.Word(this, new PlcVars.WordAddress(16), true);
        }

    }
}
