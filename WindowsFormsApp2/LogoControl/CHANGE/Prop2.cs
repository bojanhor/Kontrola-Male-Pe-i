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
        // Alarms
        public PlcVars.Bit Alarm_zavesa1;
        public PlcVars.Bit Alarm_zavesa2;
        public PlcVars.Bit Alarm_FreqX;
        public PlcVars.Bit Alarm_FreqY;
        public PlcVars.Bit Alarm_FreqT;
        public PlcVars.Bit MainSupplyErr;
        public PlcVars.Bit GobaNC;
        public PlcVars.Bit GobaNO;

        //// PositionSimulation            
        public PlcVars.Word PosX; public PlcVars.Word PosY;



        public Prop2(Sharp7.S7Client client):base(client)
        {
            // Alarms
            Alarm_zavesa1 = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(311, 0), "AKTIVIRANA JE ZAVESA 1!", false, true) { SyncEvery_X_Time = 3 };
            Alarm_zavesa2 = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(312, 0), "AKTIVIRANA JE ZAVESA 2!", false, true) { SyncEvery_X_Time = 3 };
            Alarm_FreqX = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(313, 0), "NAPAKA FREKVENČNEGA PRETVORNIKA X OSI!", false, true) { SyncEvery_X_Time = 3 };
            Alarm_FreqY = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(314, 0), "NAPAKA FREKVENČNEGA PRETVORNIKA Y OSI!", false, true) { SyncEvery_X_Time = 3 };
            Alarm_FreqT = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(322, 0), "NAPAKA FREKVENČNEGA PRETVORNIKA TRAKU!", false, true) { SyncEvery_X_Time = 3 };
            MainSupplyErr = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(323, 0), "Napaka Napajalne napetosti!", false, true) { SyncEvery_X_Time = 3 };
            GobaNC = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(324, 0), "AKTIVIRANA JE GOBASTA TIPKA (NC)!", false, true) { SyncEvery_X_Time = 3 };
            GobaNO = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(325, 0), "AKTIVIRANA JE GOBASTA TIPKA (NO)!", false, true) { SyncEvery_X_Time = 3 };
                       
        }

    }
}
