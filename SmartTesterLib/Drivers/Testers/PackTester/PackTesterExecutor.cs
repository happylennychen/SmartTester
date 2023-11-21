using System.Diagnostics;
using Cobra.Communication;
using Cobra.Common;

namespace SmartTesterLib
{
    public class PackTesterExecutor : ITesterExecutor
    {
        private const uint RetryCount = 15;
        private const byte BusAddress = 0x16;
        private const byte TemperatureIndex = 0x08;
        private const byte VoltageIndex = 0x8D;
        private const byte CurrentIndex = 0x0A;
        public Stopwatch Stopwatch { get; set; }   //用秒表来控制通道的状态
        private CCommunicateManager m_Interface { get; set; }
        private ActionMode CurrentStepActionMode { get; set; }

        Charger charger = new Charger();
        Load load = new Load();
        OZ93510 oz93510 = new OZ93510();

        public PackTesterExecutor()
        {
            m_Interface = new CCommunicateManager();

            Stopwatch = new Stopwatch();
        }
        public bool Init(string ipAddress, int port, string sessionStr)
        {
            bool ret = false;
            ret = oz93510.Init();
            if (ret == false)
                return false;

            ret = charger.Init();
            if (ret == false)
                return false;

            ret = load.Init();
            if (ret == false)
                return false;
            return true;
        }
        public bool ReadRow(int channelIndex, out object row, out uint channelEvents)
        {
            UInt32 current;
            UInt32 voltage;
            if (CurrentStepActionMode == ActionMode.CC_CV_CHARGE)
            {
                charger.ReadData(out current,out voltage);
            }
            else
            {
                load.ReadData(out current,out voltage);
                
            }
            row = null;
            channelEvents = 0;
            var packRow = new PackRow();
            ushort wData;
            UInt32 ret = LibErrorCode.IDS_ERR_SUCCESSFUL;
            ret = OnWordRead(TemperatureIndex, out wData);
            if (ret != LibErrorCode.IDS_ERR_SUCCESSFUL)
            { return false; }
            packRow.Temperature = wData;
            packRow.Current = current;
            packRow.Voltage = voltage;
            //ret = OnWordRead(VoltageIndex, out wData);
            //if (ret != LibErrorCode.IDS_ERR_SUCCESSFUL)
            //{ return false; }
            //ret = OnWordRead(CurrentIndex, out wData);
            //if (ret != LibErrorCode.IDS_ERR_SUCCESSFUL)
            //{ return false; }
            packRow.TimeInMS = (uint)Stopwatch.ElapsedMilliseconds;
            row = packRow;
            channelEvents = 0;
            return true;
        }

        public bool ReadTemperarture(int channelIndex, out double temperature)
        {
            UInt32 ret = LibErrorCode.IDS_ERR_SUCCESSFUL;
            ushort wData;
            temperature = 0;
            ret = OnWordRead(TemperatureIndex, out wData);
            if (ret != LibErrorCode.IDS_ERR_SUCCESSFUL)
            { return false; }
            temperature = (double)wData / 10;
            return true;
        }

        public bool SpecifyChannel(int channelIndex)
        {
            return true;
        }

        public bool SpecifyTestStep(SmartTesterStep step)
        {
            try
            {
                if (step.Action.Mode == ActionMode.CC_CV_CHARGE)
                {
                    load.PowerOff();
                    charger.SetChargeParameters(step);
                    
                }
                else
                {
                    charger.PowerOff();
                    load.SetDischargeParameters(step);
                    
                }

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
                throw;
            }

            Console.WriteLine("Pack SpecifyTestStep");
            return true;
        }

        public bool Start()
        {
            try
            {
                if (CurrentStepActionMode == SmartTesterLib.ActionMode.CC_CV_CHARGE)
                {
                    charger.PowerOn();
                }
                else
                {
                    load.PowerOn();
                }

            }
            catch (Exception e)
            {
                Stop();
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Pack Start");
            Stopwatch.Restart();
            return true;
        }

        public bool Stop()
        {
            try
            {
                charger.PowerOff();
                //charger.SetLocalMode();
                load.PowerOff();

            }
            catch (Exception)
            {
                throw;
            }

            Console.WriteLine("Pack Stop");
            Stopwatch.Reset();
            return true;
        }
        private UInt32 OnBlockRead(byte cmd, ref TSMBbuffer pval)
        {
            int crcLen = 0;
            byte[] databuf = null;
            UInt16 DataOutLen = 0;
            UInt16 DataInLen = (UInt16)(pval.length + 1);
            byte[] sendbuf = new byte[2];
            UInt32 ret = LibErrorCode.IDS_ERR_SUCCESSFUL;
            databuf = new byte[DataInLen];
            sendbuf[0] = BusAddress;
            sendbuf[1] = cmd;
            for (int i = 0; i < RetryCount; i++)
            {
                if (m_Interface.ReadDevice(sendbuf, ref databuf, ref DataOutLen, DataInLen))
                {
                    Array.Copy(databuf, 0, pval.bdata, 0, pval.length);
                    //if (ElementDefine.m_bPEC)
                    if (true)
                    {
                        if (pval.length == 32)
                        {
                            crcLen = databuf[0] + 1;
                            if (databuf[crcLen] != calc_crc_read2(sendbuf[0], sendbuf[1], pval.bdata, crcLen))
                            {
                                ret = LibErrorCode.IDS_ERR_BUS_DATA_PEC_ERROR;
                                continue;
                            }
                        }
                        else if (databuf[pval.length] != calc_crc_read2(sendbuf[0], sendbuf[1], pval.bdata, pval.length))
                        {
                            ret = LibErrorCode.IDS_ERR_BUS_DATA_PEC_ERROR;
                            continue;
                        }
                    }
                    ret = LibErrorCode.IDS_ERR_SUCCESSFUL;
                    break;
                }
                ret = LibErrorCode.IDS_ERR_DEM_FUN_TIMEOUT;
                Thread.Sleep(10);
            }

            return ret;
        }
        private UInt32 OnWordRead(byte cmd, out ushort wData)
        {
            UInt32 ret = LibErrorCode.IDS_ERR_SUCCESSFUL;
            TSMBbuffer buffer = new TSMBbuffer();
            ret = OnBlockRead(cmd, ref buffer);
            if (ret != LibErrorCode.IDS_ERR_SUCCESSFUL)
            {
                wData = 0;
                return ret;
            }
            wData = SharedFormula.MAKEWORD(buffer.bdata[0], buffer.bdata[1]);
            return ret;
        }
        private byte calc_crc_read2(byte slave_addr, byte reg_addr, byte[] data, int dataLen = 3)
        {
            byte[] pdata = new byte[3 + dataLen];//new byte[5];

            pdata[0] = slave_addr;
            pdata[1] = reg_addr;
            pdata[2] = (byte)(slave_addr | 0x01);
            Array.Copy(data, 0, pdata, 3, dataLen);

            return crc8_calc(ref pdata, (UInt16)(3 + dataLen));
        }
        private byte crc8_calc(ref byte[] pdata, UInt16 n)
        {
            byte crc = 0;
            byte crcdata;
            UInt16 i, j;

            for (i = 0; i < n; i++)
            {
                crcdata = pdata[i];
                for (j = 0x80; j != 0; j >>= 1)
                {
                    if ((crc & 0x80) != 0)
                    {
                        crc <<= 1;
                        crc ^= 0x07;
                    }
                    else
                        crc <<= 1;

                    if ((crcdata & j) != 0)
                        crc ^= 0x07;
                }
            }
            return crc;
        }
    }
}