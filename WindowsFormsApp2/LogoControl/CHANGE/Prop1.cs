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

        public PlcVars.Word Pon_EN_1; public PlcVars.TimeSet PonTimeOn; public PlcVars.TimeSet PonTimeOff;
        public PlcVars.Word Tor_EN_1; public PlcVars.TimeSet TorTimeOn; public PlcVars.TimeSet TorTimeOff;
        public PlcVars.Word Sre_EN_1; public PlcVars.TimeSet SreTimeOn; public PlcVars.TimeSet SreTimeOff;
        public PlcVars.Word Čet_EN_1; public PlcVars.TimeSet ČetTimeOn; public PlcVars.TimeSet ČetTimeOff;
        public PlcVars.Word Pet_EN_1; public PlcVars.TimeSet PetTimeOn; public PlcVars.TimeSet PetTimeOff;
        public PlcVars.Word Sob_EN_1; public PlcVars.TimeSet SobTimeOn; public PlcVars.TimeSet SobTimeOff;
        public PlcVars.Word Ned_EN_1; public PlcVars.TimeSet NedTimeOn; public PlcVars.TimeSet NedTimeOff;

        public PlcVars.Word AutoHeat, ForceDisableHeat, ForceEnableHeat;

        public PlcVars.Word HistHeat;

        public PlcVars.AlarmBit SenFail1, SenFail2, SenFail3, SenFail4, SenFail5;

        public PlcVars.Word SelT_Reg, SelT_Dif;

        public PlcVars.Word VentStdby;

        public PlcVars.Word VntDiffMax, VntDiffMin;

        public PlcVars.Word SpdupDiff, VntCoolOff;

        public PlcVars.Word VentCoolOffTime;

        public PlcVars.Word TempStPnt1, TempStPnt2;

        public PlcVars.Bit Sel_T_Err, Dif_T_Err;        

        public PlcVars.Bit Heat1, Heat2;

        public PlcVars.Word Temp;

        public PlcVars.Word VentRpmCurrent;

        //Alarm Bits
        public PlcVars.AlarmBit DiffVntErr, MaxTempReached, VentDemand, MussLauf, VrataOdprta, CoolOff;
        public PlcVars.AlarmBit VarnostniTermostatOk, FreqOk, EMG_OK, EMG_Aux_OK, Sw102PositionOK; // inverted


        public Prop1(Sharp7.S7Client client) : base(client)
        {
            //PC Watchdog
            PCWD = new PlcVars.Word(this, new PlcVars.WordAddress(GetPCWD_Address()), true);
            
            Pon_EN_1 = new PlcVars.Word(this, new PlcVars.WordAddress(70), true); PonTimeOn = new PlcVars.TimeSet(this, new PlcVars.WordAddress(40), true); PonTimeOff = new PlcVars.TimeSet(this, new PlcVars.WordAddress(42), true);
            Tor_EN_1 = new PlcVars.Word(this, new PlcVars.WordAddress(72), true); TorTimeOn = new PlcVars.TimeSet(this, new PlcVars.WordAddress(44), true); TorTimeOff = new PlcVars.TimeSet(this, new PlcVars.WordAddress(46), true);
            Sre_EN_1 = new PlcVars.Word(this, new PlcVars.WordAddress(74), true); SreTimeOn = new PlcVars.TimeSet(this, new PlcVars.WordAddress(48), true); SreTimeOff = new PlcVars.TimeSet(this, new PlcVars.WordAddress(50), true);
            Čet_EN_1 = new PlcVars.Word(this, new PlcVars.WordAddress(76), true); ČetTimeOn = new PlcVars.TimeSet(this, new PlcVars.WordAddress(52), true); ČetTimeOff = new PlcVars.TimeSet(this, new PlcVars.WordAddress(54), true);
            Pet_EN_1 = new PlcVars.Word(this, new PlcVars.WordAddress(78), true); PetTimeOn = new PlcVars.TimeSet(this, new PlcVars.WordAddress(56), true); PetTimeOff = new PlcVars.TimeSet(this, new PlcVars.WordAddress(58), true);
            Sob_EN_1 = new PlcVars.Word(this, new PlcVars.WordAddress(80), true); SobTimeOn = new PlcVars.TimeSet(this, new PlcVars.WordAddress(60), true); SobTimeOff = new PlcVars.TimeSet(this, new PlcVars.WordAddress(62), true);
            Ned_EN_1 = new PlcVars.Word(this, new PlcVars.WordAddress(82), true); NedTimeOn = new PlcVars.TimeSet(this, new PlcVars.WordAddress(64), true); NedTimeOff = new PlcVars.TimeSet(this, new PlcVars.WordAddress(66), true);

            AutoHeat = new PlcVars.Word(this, new PlcVars.WordAddress(84), true);
            ForceDisableHeat = new PlcVars.Word(this, new PlcVars.WordAddress(86), true);
            ForceEnableHeat = new PlcVars.Word(this, new PlcVars.WordAddress(88), true);

            HistHeat = new PlcVars.Word(this, new PlcVars.WordAddress(100), true);

            SenFail1 = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(10, 0), "Napaka temperaturnega tipala Zg", false, true);
            SenFail2 = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(11, 0), "Napaka temperaturnega tipala Sr1", false, true);
            SenFail3 = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(12, 0), "Napaka temperaturnega tipala Sr2", false, true);
            SenFail4 = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(13, 0), "Napaka temperaturnega tipala Sp", false, true);
            SenFail5 = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(14, 0), "Napaka temperaturnega tipala Kanal", false, true);

            SelT_Reg = new PlcVars.Word(this, new PlcVars.WordAddress(20), true);
            SelT_Dif = new PlcVars.Word(this, new PlcVars.WordAddress(22), true);

            VentStdby = new PlcVars.Word(this, new PlcVars.WordAddress(24), true);

            VntDiffMax = new PlcVars.Word(this, new PlcVars.WordAddress(26), true);
            VntDiffMin = new PlcVars.Word(this, new PlcVars.WordAddress(28), true);

            SpdupDiff = new PlcVars.Word(this, new PlcVars.WordAddress(30), true);
            VntCoolOff = new PlcVars.Word(this, new PlcVars.WordAddress(32), true);

            VentCoolOffTime = new PlcVars.Word(this, new PlcVars.WordAddress(38), true);

            TempStPnt1 = new PlcVars.Word(this, new PlcVars.WordAddress(90),true);
            TempStPnt2 = new PlcVars.Word(this, new PlcVars.WordAddress(92), true);

            Sel_T_Err = new PlcVars.Bit(this, new PlcVars.BitAddress(110,0), false);
            Dif_T_Err = new PlcVars.Bit(this, new PlcVars.BitAddress(111, 0), false);
            
            Heat1 = new PlcVars.Bit(this, new PlcVars.BitAddress(130,0), false);
            Heat2 = new PlcVars.Bit(this, new PlcVars.BitAddress(132,0), false);

            Temp = new PlcVars.Word(this, new PlcVars.WordAddress(140), true);

            VentRpmCurrent = new PlcVars.Word(this, new PlcVars.WordAddress(150), true);

            DiffVntErr = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(120, 0), "Napaka Nastavitev diference ventilacije - Max mora biiti večji od Min.", false, true);
            MaxTempReached = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(121, 0), "Presežena je najvišja dovoljena temperatura.", false, true);

            VarnostniTermostatOk = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(160, 0), "[Zagon je preprečen] NAPAKA - Varnostni termostat", true, true);
            FreqOk = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(161, 0), "[Zagon je preprečen] NAPAKA - Frekvenčnik", true, true);
            EMG_OK = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(162, 0), "[Zagon je preprečen] NAPAKA - Varnostni mehanizem 1", true, true);
            EMG_Aux_OK = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(163, 0), "[Zagon je preprečen] - Varnostni mehanizem 2", true, true);
            Sw102PositionOK = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(164, 0), "[Zagon je preprečen] - Stikalo za prisilno delovanje ventilacije ni v ustrezni poziciji", true, true);

            VentDemand = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(167, 0), "[ROČNI NAČIN - PRISILNO DELOVANJE]", false, true);
            MaxTempReached = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(167, 0), "[ROČNI NAČIN - PRISILNO DELOVANJE]", false, true);

            VrataOdprta = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(170, 0), "Vrata so odprta", false, true);
            CoolOff = new PlcVars.AlarmBit(this, new PlcVars.BitAddress(171, 0), "Ohlajanje v teku", false, true);

        }

    }
}
