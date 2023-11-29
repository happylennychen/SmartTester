using System.Diagnostics;

namespace SmartTesterLib
{
    public class PackTesterExecutor : ITesterExecutor
    {
        private const byte TemperatureIndex = 0x08;
        private const byte VoltageIndex = 0x8D;
        private const byte CurrentIndex = 0x0A;
        public Stopwatch Stopwatch { get; set; }   //用秒表来控制通道的状态
        private ActionMode CurrentStepActionMode { get; set; }

        private Charger Charger { get; set; }
        private Load Load { get; set; }
        private OZ93510 OZ93510 { get; set; }

        public PackTesterExecutor()
        {
            Charger = new Charger();
            Load = new Load();
            OZ93510 = new OZ93510();
            Stopwatch = new Stopwatch();
        }
        public bool Init(string ipAddress, int port, string sessionStr)
        {
            bool ret = false;
            ret = OZ93510.Init();
            if (ret == false)
                return false;

            ret = Charger.Init();
            if (ret == false)
                return false;

            ret = Load.Init();
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
                Charger.ReadData(out current,out voltage);
            }
            else
            {
                Load.ReadData(out current,out voltage);
                
            }
            row = null;
            channelEvents = 0;
            var packRow = new PackRow();
            ushort wData;
            UInt32 ret = LibErrorCode.IDS_ERR_SUCCESSFUL;
            ret = OZ93510.OnWordRead(TemperatureIndex, out wData);
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
            ret = OZ93510.OnWordRead(TemperatureIndex, out wData);
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
                    Load.PowerOff();
                    Charger.SetChargeParameters(step);
                    
                }
                else
                {
                    Charger.PowerOff();
                    Load.SetDischargeParameters(step);
                    
                }

            }
            catch (Exception e)
            {

                Utilities.WriteLine(e.Message);
                throw;
            }

            Utilities.WriteLine("Pack SpecifyTestStep");
            return true;
        }

        public bool Start()
        {
            try
            {
                if (CurrentStepActionMode == SmartTesterLib.ActionMode.CC_CV_CHARGE)
                {
                    Charger.PowerOn();
                }
                else
                {
                    Load.PowerOn();
                }

            }
            catch (Exception e)
            {
                Stop();
                Utilities.WriteLine(e.Message);
            }

            Utilities.WriteLine("Pack Start");
            Stopwatch.Restart();
            return true;
        }

        public bool Stop()
        {
            try
            {
                Charger.PowerOff();
                //charger.SetLocalMode();
                Load.PowerOff();

            }
            catch (Exception)
            {
                throw;
            }

            Utilities.WriteLine("Pack Stop");
            Stopwatch.Reset();
            return true;
        }
                    //if (ElementDefine.m_bPEC)





    }
}