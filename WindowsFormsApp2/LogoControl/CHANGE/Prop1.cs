using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;

namespace WindowsFormsApp2
{
    public class Prop1 : PropComm
    {
        //PC Watchdog
        public PlcVars.Word PCWD;

        public PlcVars.Bit  PecOnPulse, PecOffPulse, PecOnStatus;

        public PlcVars.Word HistHeat;

        public PlcVars.AlarmBit SenFail1, SenFail2, SenFail3, SenFail4, SenFail5, SenFail6;

        public PlcVars.Word SelT_Reg, SelT_Dif;

        public PlcVars.Word VentStdby;

        public PlcVars.Word VntDiffMax, VntDiffMin;

        public PlcVars.Word SpdupDiff, VntCoolOff;

        public PlcVars.Word VentCoolOffTime;

        public PlcVars.Word TempStPnt1, TempStPnt2;

        public PlcVars.Bit Sel_T_Err, Dif_T_Err;        

        public PlcVars.Bit Heat1, Heat2, Heat3, Heat4;

        public PlcVars.Word TempReg, TempDif, TempErr;

        public PlcVars.Word VentRpmCurrent;

        public PlcVars.Word TempSenZg, TempSenSr1, TempSenSr2, TempSenSp, TempSenKn, TempSenKos;

        public PlcVars.Bit BuzzFromPC, BuzzFromPcEndCycle, BuzzFromPcERROR;

        public PlcVars.Bit StopwatchStart, StopwatchStop, StopwatchReset, StopwatchRunning, TimeReached, StopwatchPaused, StopwatchStopped, HeatingUp;
        public PlcVars.Word StopwatchTime, TimeSet, PauseIfTlow;

        public PlcVars.Bit Blink_HeatingUp;

        //Alarm Bits
        public PlcVars.AlarmBit DiffVntErr, MaxTempReached, VentDemand, MussLauf, VrataOdprta, CoolOff, VentActive, TempDosezena;
        public PlcVars.AlarmBit PCWDFail;
        public PlcVars.AlarmBit VarnostniTermostatOk, FreqOk, EMG_OK, EMG_Aux_OK, Sw102PositionOK; // inverted


        public Prop1(Sharp7.S7Client client) : base(client)
        {
            //PC Watchdog
            PCWD = new PlcVars.Word(this, new PlcVars.WordAddress(GetPCWD_Address()), true);

            HistHeat = new PlcVars.Word(this, new PlcVars.WordAddress(100), true);

            SenFail1 = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(10, 0), "Napaka temperaturnega tipala Zg", false, true);
            SenFail2 = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(11, 0), "Napaka temperaturnega tipala Sr1", false, true);
            SenFail3 = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(12, 0), "Napaka temperaturnega tipala Sr2", false, true);
            SenFail4 = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(13, 0), "Napaka temperaturnega tipala Sp", false, true);
            SenFail5 = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(14, 0), "Napaka temperaturnega tipala Kanal", false, true);
            SenFail6 = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(16, 0), "Napaka temperaturnega tipala Kosa", false, true);

            SelT_Reg = new PlcVars.Word(this, new PlcVars.WordAddress(20), true);
            SelT_Dif = new PlcVars.Word(this, new PlcVars.WordAddress(22), true);

            VentStdby = new PlcVars.Word(this, new PlcVars.WordAddress(24), true);

            VntDiffMax = new PlcVars.Word(this, new PlcVars.WordAddress(26), true);
            VntDiffMin = new PlcVars.Word(this, new PlcVars.WordAddress(28), true);

            SpdupDiff = new PlcVars.Word(this, new PlcVars.WordAddress(30), true);
            VntCoolOff = new PlcVars.Word(this, new PlcVars.WordAddress(32), true);

            VentCoolOffTime = new PlcVars.Word(this, new PlcVars.WordAddress(38), true);

            PecOnPulse = new PlcVars.Bit(this, new PlcVars.BitAddress(87, 0), true);
            PecOffPulse = new PlcVars.Bit(this, new PlcVars.BitAddress(88, 0), true);
            PecOnStatus = new PlcVars.Bit(this, new PlcVars.BitAddress(89, 0), false);

            TempStPnt1 = new PlcVars.Word(this, new PlcVars.WordAddress(90),true);
            TempStPnt2 = new PlcVars.Word(this, new PlcVars.WordAddress(92), true);

            Sel_T_Err = new PlcVars.Bit(this, new PlcVars.BitAddress(110,0), false);
            Dif_T_Err = new PlcVars.Bit(this, new PlcVars.BitAddress(111, 0), false);
            
            Heat1 = new PlcVars.Bit(this, new PlcVars.BitAddress(130,0), false);
            Heat2 = new PlcVars.Bit(this, new PlcVars.BitAddress(132,0), false);
            Heat3 = new PlcVars.Bit(this, new PlcVars.BitAddress(134, 0), false);
            Heat4 = new PlcVars.Bit(this, new PlcVars.BitAddress(136, 0), false);

            TempReg = new PlcVars.Word(this, new PlcVars.WordAddress(140), false);
            TempDif = new PlcVars.Word(this, new PlcVars.WordAddress(142), false);

            TempErr = new PlcVars.Word(this, new PlcVars.WordAddress(144), true);

            VentRpmCurrent = new PlcVars.Word(this, new PlcVars.WordAddress(150), true);

            DiffVntErr = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(120, 0), "Napaka Nastavitev diference ventilacije - Max mora biiti večji od Min.", false, true);
            MaxTempReached = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(121, 0), "Presežena je najvišja dovoljena temperatura.", false, true);

            VarnostniTermostatOk = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(160, 0), "[Zagon je preprečen] NAPAKA - Varnostni termostat", false, true);
            FreqOk = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(161, 0), "[Zagon je preprečen] NAPAKA - Frekvenčnik", false, true);
            EMG_OK = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(162, 0), "[Zagon je preprečen] NAPAKA - Varnostni mehanizem 1 - goba", false, true);
            EMG_Aux_OK = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(163, 0), "[Zagon je preprečen] - Varnostni mehanizem 2 - goba", false, true);
            Sw102PositionOK = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(164, 0), "[Zagon je preprečen] - Stikalo za prisilno delovanje ventilacije ni v ustrezni poziciji", true, true);

            VentDemand = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(167, 0), "[ROČNI NAČIN - PRISILNO DELOVANJE]", false, true);
            MaxTempReached = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(167, 0), "[ROČNI NAČIN - PRISILNO DELOVANJE]", false, true);

            VrataOdprta = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(170, 0), "Vrata so odprta", false, true);
            CoolOff = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(171, 0), "Ohlajanje v teku", false, true);
            VentActive = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(172, 0), "Ventilacija aktivna", false, true);

            TempDosezena = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(180, 0), "Tempreatura je dosežena", false, true);
            PCWDFail = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(188, 0), "Krmilnik ne zazna povezave s programom PC", false, true);

            BuzzFromPC = new PlcVars.Bit(this, new PlcVars.BitAddress(190, 0), true);
            BuzzFromPcEndCycle = new PlcVars.Bit(this, new PlcVars.BitAddress(192, 0), true);
            BuzzFromPcERROR = new PlcVars.Bit(this, new PlcVars.BitAddress(196, 0), true);

            TempSenZg = new PlcVars.Word(this, new PlcVars.WordAddress(210), false);
            TempSenSr1 = new PlcVars.Word(this, new PlcVars.WordAddress(212), false);
            TempSenSr2 = new PlcVars.Word(this, new PlcVars.WordAddress(214), false);
            TempSenSp= new PlcVars.Word(this, new PlcVars.WordAddress(216), false);
            TempSenKn = new PlcVars.Word(this, new PlcVars.WordAddress(218), false);
            TempSenKos = new PlcVars.Word(this, new PlcVars.WordAddress(220), false);

            StopwatchStart = new PlcVars.Bit(this,new PlcVars.BitAddress(240,0),true);
            StopwatchStop = new PlcVars.Bit(this, new PlcVars.BitAddress(242, 0), true);
            StopwatchReset = new PlcVars.Bit(this, new PlcVars.BitAddress(244, 0), true);
            StopwatchTime = new PlcVars.Word(this, new PlcVars.WordAddress(246), false);
            StopwatchRunning = new PlcVars.Bit(this, new PlcVars.BitAddress(248, 0), false);
            TimeReached = new PlcVars.Bit(this, new PlcVars.BitAddress(249, 0), false);
            PauseIfTlow = new PlcVars.Word(this, new PlcVars.WordAddress(252), true);
            TimeSet = new PlcVars.Word(this, new PlcVars.WordAddress(250), true);
            StopwatchPaused = new PlcVars.Bit(this, new PlcVars.BitAddress(254, 0), false);
            StopwatchStopped = new PlcVars.Bit(this, new PlcVars.BitAddress(256, 0), false);
            HeatingUp = new PlcVars.Bit(this, new PlcVars.BitAddress(258, 0), false);

            Blink_HeatingUp = new PlcVars.Bit(this, new PlcVars.BitAddress(260, 0), false);
        }

    }
}
