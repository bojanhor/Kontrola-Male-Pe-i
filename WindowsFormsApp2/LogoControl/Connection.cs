using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using Sharp7;
using Linq = System.Xml.Linq;
using System.IO;
using System.Threading;
using System.Net;

namespace WindowsFormsApp2
{
    public class Connection
    {
        public enum Status
        {
            NotInitialised = -4,
            Error = -3,
            Warning = -2,
            Connecting = -1,
            Connected = 0,
        }

        private string _LocalTSAP;
        private string _RemoteTSAP;
        private ushort _LocalTSAP_asushort;
        private ushort _RemoteTSAP_asushort;

        public int errcodeLOGO;
        public Status connectionStatusLOGO;
        public bool IsLogoConnected
        {
            get
            {
                if (connectionStatusLOGO == Status.Connected)
                {
                    return true;
                }
                return false;
            }
        }

        public Connection()
        {
            connectionStatusLOGO = Status.NotInitialised;
        }

        public IPAddress IpAddress { get; set; }

        public string LocalTSAP
        {
            get { return _LocalTSAP; }
            set
            {
                _LocalTSAP = value;
                var tmp = _LocalTSAP.Replace(".", "");
                tmp = tmp.Replace("\"", "");
                _LocalTSAP_asushort = Convert.ToUInt16(int.Parse(tmp, System.Globalization.NumberStyles.HexNumber));

            }

        }

        public string RemoteTSAP
        {
            get { return _RemoteTSAP; }
            set
            {
                _RemoteTSAP = value;
                var tmp = _RemoteTSAP.Replace(".", "");
                tmp = tmp.Replace("\"", "");
                _RemoteTSAP_asushort = Convert.ToUInt16(int.Parse(tmp, System.Globalization.NumberStyles.HexNumber));
            }


        }

        public ushort LocalTSAP_asushort { get { return _LocalTSAP_asushort; } }
        public ushort RemoteTSAP_asushort { get { return _RemoteTSAP_asushort; } }





        // Write to PLC
        public static void PLCwrite(S7Client Client, PlcVars.PlcAddress typeAndAdress, short value, out int errCode)
        {
            try
            {                
                byte[] b = new byte[6];  // temporary array for writing

                // In case if you want to write bit value
                if (typeAndAdress is PlcVars.BitAddress)
                {                    
                    bool boolval = false;
                    boolval = Convert.ToBoolean(value);
                    var bitAddres = (PlcVars.BitAddress)typeAndAdress;
                    
                    errCode = Client.DBRead(1, bitAddres.GetAddress(), S7Consts.S7WLByte, b); // read whole byte as late as possible

                    if (errCode == S7Consts.err_OK)
                    {
                        S7.SetBitAt(ref b, 0, bitAddres.GetSubAddress(), boolval);
                        errCode = Client.DBWrite(1, bitAddres.GetAddress(), 1, b); // write whole byte as soon as possible
                    }
                }

                // In case if you want to write DWORD value
                else if (typeAndAdress is PlcVars.DoubleWordAddress)
                {   
                    S7.SetDWordAt(b, 0, (uint)value);
                    errCode = Client.DBWrite(1, typeAndAdress.GetAddress(), 4, b);
                }

                // In case if you want to write Word value
                else if (typeAndAdress is PlcVars.WordAddress)
                { 
                    S7.SetWordAt(b, 0, (ushort)value);
                    errCode = Client.DBWrite(1, typeAndAdress.GetAddress(), 2, b);
                }

                // In case if you want to write Byte value
                else if (typeAndAdress is PlcVars.ByteAddress)
                {
                    S7.SetByteAt(b, 0, (byte)value);
                    errCode = Client.DBWrite(1, typeAndAdress.GetAddress(), 1, b);
                }

                else
                {
                    errCode = S7Consts.err_typeError;
                }
            }
            catch (Exception ex)
            {
                errCode = S7Consts.err_Write;
                throw new Exception(Client.ErrorText(errCode) + ". " + ex.Message);
            }
                        
        }
                

        // Read from PLC
        public static short BufferRead(S7Client Client, PlcVars.PlcAddress typeAndAdress, out int errCode)
        {            
            short value = 0;
            try
            {
                byte[] b = Client.S7PLCbuffer.GetBuffer();
                errCode = Client.S7PLCbuffer.GetError();

                // In case if you want to read bit value
                if (typeAndAdress is PlcVars.BitAddress)
                {
                    bool boolval = false;
                    var bitAdress = (PlcVars.BitAddress)typeAndAdress;


                    try
                    {                          
                        boolval = S7.GetBitAt(b, bitAdress.Address, bitAdress.GetSubAddress());
                        if (boolval) { return 1; } return 0;
                    }
                    catch (Exception)
                    {
                        errCode = S7Consts.err_Read;
                    }

                }

                // In case if you want to read DWORD value
                else if (typeAndAdress is PlcVars.DoubleWordAddress)
                {                    
                    try
                    {  
                        value = (short)Convert.ToInt32(S7.GetDWordAt(b, typeAndAdress.Address));
                    }
                    catch
                    {
                        errCode = S7Consts.err_Read;
                    }

                }


                // In case if you want to read WORD value
                else if (typeAndAdress is PlcVars.WordAddress)
                {
                    try
                    {                        
                        value = (short)(S7.GetWordAt(b, typeAndAdress.Address));
                    }
                    catch
                    {
                        errCode = S7Consts.err_Read;
                    }
                }
                
                // In case if you want to read BYTE value
                else if (typeAndAdress is PlcVars.ByteAddress)
                {
                    try
                    {                        
                        value = S7.GetByteAt(b, 0);
                    }
                    catch
                    {
                        errCode = S7Consts.err_Read;
                    }
                }

                // error type
                else
                {
                    errCode = S7Consts.err_typeError;
                }

                return value;

            }

            catch
            {
                errCode = S7Consts.err_UnspecifiedHigherLayer;
                return value;
            }
        }
    }

}

