using System;
using NationalInstruments.Visa;
using System.Net.Sockets;
using System.Linq;
using System.Text;

namespace SmartTesterLib
{
    public class Chroma17208Executor : ITesterExecutor
    {
        private static MessageBasedSession? mbSession;
        //private static TcpClient tcpClient;
        private static NetworkStream? stream;
        private object HiokiLock = new object();
        private object ChromaLock = new object();

        public bool Init(string ipAddress, int port, string sessionStr)
        {
            if (!OpenTcp(ipAddress, 1000, port))
                return false;

            string line = $":STARt\n";
            stream!.Write(Encoding.ASCII.GetBytes(line), 0, line.Length);

            return Open17208Session(sessionStr);
        }

        private bool OpenTcp(string ipAddress, int connectTimeout, int port)
        {
            var tcpClient = new TcpClient();
            var result = tcpClient.BeginConnect(ipAddress, port, null, null);
            var success = result.AsyncWaitHandle.WaitOne(connectTimeout);
            if (!success)
            {
                tcpClient.Close();
                return false;
            }
            tcpClient.EndConnect(result);

            stream = tcpClient.GetStream();
            stream.ReadTimeout = connectTimeout;
            return true;
        }

        //public double ReadCapacity(int channel)
        //{
        //    string cmd = $"MEAS:AH? {channel.ToString()}";
        //    var result = SCPIQuary(cmd);
        //    return double.Parse(result);
        //}

        //public double ReadCurrent(int channel)
        //{
        //    string cmd = $"MEAS:CURR? {channel.ToString()}";
        //    var result = SCPIQuary(cmd);
        //    return double.Parse(result);
        //}

        //public uint ReadEvents(int channel)
        //{
        //    string cmd = $"MEAS:PROT? {channel.ToString()}";
        //    var result = SCPIQuary(cmd);
        //    uint prot = uint.Parse(result);
        //    if (prot != 0x00000000)
        //        return ChannelEvents.Error;
        //    else
        //        return ChannelEvents.Normal;
        //}

        //public double ReadPower(int channel)
        //{
        //    string cmd = $"MEAS:POW? {channel.ToString()}";
        //    var result = SCPIQuary(cmd);
        //    return double.Parse(result);
        //}

        public bool ReadRow(int channel, out object row, out uint channelEvents)
        {
            lock (ChromaLock)
            {
                string cmd = $"MEAS:ALL? {channel.ToString()}";
                string result;
                bool ret = SCPIQuary(cmd, out result);
                if (ret == false)
                {
                    row = null;
                    channelEvents = 0;
                    return false;
                }
                var standardRow = new StandardRow();
                var strs = result.Split(',');
                standardRow.Capacity = double.Parse(strs[8]) * 1000;
                standardRow.Current = double.Parse(strs[6]) * 1000;
                //standardRow.Index = ???
                standardRow.Mode = StringToActionMode(strs[0]);
                standardRow.Status = GetStatusFromString(strs[3]);
                //standardRow.Temperature = ReadTemperarture(channel);
                standardRow.TimeInMS = uint.Parse(strs[1]);
                standardRow.Voltage = double.Parse(strs[5]) * 1000;
                //standardRow.TotalCapacity = ???
                channelEvents = GetEventsFromString(strs[2]);
                row = standardRow;
                return true;
            }
        }

        private uint GetEventsFromString(string v)
        {
            var evt = uint.Parse(v);
            if (evt == 0)
                return ChannelEvents.Normal;
            else
                return ChannelEvents.Error;
        }

        private RowStatus GetStatusFromString(string v)
        {
            RowStatus output;
            switch (v)
            {
                case "0":
                    output = RowStatus.RUNNING;
                    break;
                case "18":
                    output = RowStatus.STOP;
                    break;
                default:
                    output = RowStatus.UNKNOWN;
                    break;
            }
            return output;
        }

        private ActionMode StringToActionMode(string v)
        {
            ActionMode output;
            switch (v)
            {
                case "0":
                    output = ActionMode.REST;
                    break;
                case "2":
                    output = ActionMode.CC_CV_CHARGE;
                    break;
                case "3":
                    output = ActionMode.CC_DISCHARGE;
                    break;
                case "5":
                    output = ActionMode.CP_DISCHARGE;
                    break;
                default:
                    output = ActionMode.NA;
                    break;
            }
            return output;
        }

        //public RowStatus ReadStatus(int channel)
        //{
        //    string cmd = $"MEAS:STATU? {channel.ToString()}";
        //    var result = SCPIQuary(cmd);
        //    return GetStatusFromString(result);
        //}

        public bool ReadTemperarture(int channel, out double temperature)
        {
            //return 25.0;
            lock (HiokiLock)
            {
                string line = $":MEMORY:AREAL? UNIT1,CH{channel.ToString()}\n";
                try
                {
                    stream.Write(Encoding.ASCII.GetBytes(line), 0, line.Length);
                    string response = GetResponse(stream);
                    temperature = double.Parse(response) / 100.0;
                    return true;
                }
                catch (Exception e)
                {
                    Utilities.WriteLine($"{e.Message}\n{line} cannot be executed.");
                    temperature = 0;
                    return false;
                }
            }
        }
        private static string GetResponse(NetworkStream stream)
        {
            string str = string.Empty;
            while (true)
            {
                var bt = stream.ReadByte();
                if (bt == '\n')
                    break;
                else
                    str += (char)bt;
            }
            return str;
        }

        //public double ReadVoltage(int channel)
        //{
        //    string cmd = $"MEAS:VOLT? {channel.ToString()}";
        //    var result = SCPIQuary(cmd);
        //    return double.Parse(result);
        //}

        public bool SpecifyChannel(int channel)
        {
            lock (ChromaLock)
            {
                return SCPIWrite($"CHAN {channel.ToString()}");
            }
        }

        public bool SpecifyTestStep(SmartTesterStep step)
        {
            lock (ChromaLock)
            {
                bool ret;
                ret = SCPIWrite($"SOUR:CURR:RANGE:AUTO ON");
                if (!ret)
                    return ret;
                var current = step.Action.Mode == ActionMode.CP_DISCHARGE ? 30 : step.Action.Current / 1000.0;
                return SCPIWrite($"SOUR:ALL {ActionModeToString(step.Action.Mode)}," +
                    $"{GetCutOffTime(step)}," +
                    $"{(step.Action.Voltage / 1000.0).ToString()}," +
                    $"{(current).ToString()}," +
                    $"{(step.Action.Power / 1000.0).ToString()}," +
                    $"{GetCutOffVoltage(step)}," +
                    $"{GetCutOffCurrent(step)}");
            }
        }

        private string GetCutOffCurrent(SmartTesterStep step)
        {
            var cob = step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.CURRENT);
            if (cob != null)
                return (cob.Condition.Value / 1000.0).ToString();
            else
                return "0";
        }

        private string GetCutOffVoltage(SmartTesterStep step)
        {
            var cob = step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.VOLTAGE);
            if (cob != null)
                return (cob.Condition.Value / 1000.0).ToString();
            else
                return "0";
        }

        private string GetCutOffTime(SmartTesterStep step)
        {
            var cob = step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME);
            if (cob != null)
                return cob.Condition.Value.ToString();
            else
                return "0";
        }

        private string ActionModeToString(ActionMode mode)
        {
            string output;
            switch (mode)
            {
                case ActionMode.CC_CV_CHARGE:
                    output = "CCCVC";
                    break;
                case ActionMode.CC_DISCHARGE:
                    output = "CCD";
                    break;
                case ActionMode.CP_DISCHARGE:
                    output = "CPD";
                    break;
                case ActionMode.REST:
                    output = "REST";
                    break;
                default:
                    output = "REST";
                    break;
            }
            return output;
        }

        public bool Start()
        {
            lock (ChromaLock)
            {
                return SCPIWrite("OUTP:STAT ON");
            }
        }

        public bool Stop()
        {
            lock (ChromaLock)
            {
                return SCPIWrite("OUTP:STAT OFF");
            }
        }
        private static bool SCPIQuary(string quaryCmd, out string output)
        {
            int retry = 5;
            for (int i = 0; i < retry; i++)
            {
                try
                {
                    //Utilities.WriteLine(quaryCmd);
                    string textToWrite = ReplaceCommonEscapeSequences(quaryCmd);
                    mbSession.RawIO.Write(textToWrite);
                    output = mbSession.RawIO.ReadString().TrimEnd('\n');
                    return true;
                }
                catch (Exception exp)
                {
                    Utilities.WriteLine($"{exp.Message}\n{quaryCmd} cannot be executed.");
                }
            }
            output = string.Empty;
            return false;
        }

        private static bool SCPIWrite(string writeCmd)
        {
            try
            {
                Utilities.WriteLine(writeCmd);
                string textToWrite = ReplaceCommonEscapeSequences(writeCmd);
                mbSession.RawIO.Write(textToWrite);
                return true;
            }
            catch (Exception exp)
            {
                Utilities.WriteLine($"{exp.Message}\n{writeCmd} cannot be executed.");
                return false;
            }
        }
        private static string ReplaceCommonEscapeSequences(string s)
        {
            if (!s.Contains(@"\n"))
                s += @"\n";
            return s.Replace("\\n", "\n").Replace("\\r", "\r");
        }

        //private static string InsertCommonEscapeSequences(string s)
        //{
        //    return s.Replace("\n", "\\n").Replace("\r", "\\r");
        //}

        //private static void CloseSession()
        //{
        //    mbSession.Dispose();
        //}

        private static bool Open17208Session(string str)
        {
            using (var rmSession = new ResourceManager())
            {
                try
                {
                    mbSession = (MessageBasedSession)rmSession.Open(str);
                    mbSession.TerminationCharacter = 0x0a;
                    mbSession.TerminationCharacterEnabled = true;
                }
                catch (InvalidCastException)
                {
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }
    }
}