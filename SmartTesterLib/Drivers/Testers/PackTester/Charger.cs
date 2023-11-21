using System.IO.Ports;
using System.Runtime.InteropServices;

namespace SmartTesterLib
{
    public enum Command
    {
        SET_OPERATION_MODE = 0x20,
        SET_OUTPUT_MODE = 0x21,
        SET_MAX_VOLTAGE = 0x22,
        SET_VOLTAGE = 0x23,
        SET_CURRENT = 0x24,
        SET_ADDRESS = 0x25,
        READ_DATA = 0x26,
    }
    public enum OperationMode
    {
        LOCAL,
        REMOTE
    }
    public enum OutputMode
    {
        OFF,
        ON
    }
    public class Charger
    {
        private const byte HEAD = 0xAA;
        private const byte ADDRESS = 0x00;
        public SerialPort PowerPort = new SerialPort();
        public byte CalcCommandChecksum(byte[] cmds)
        {
            byte sum = 0;
            foreach (byte cmd in cmds)
            {
                sum += cmd;
            }
            return sum;
        }
        public bool Init()
        {
            PowerPort.PortName = "COM18";
            PowerPort.Parity = Parity.None;
            PowerPort.BaudRate = 4800;
            PowerPort.DataBits = 8;
            PowerPort.StopBits = StopBits.One;
            PowerPort.Parity = Parity.None;
            if (PowerPort.IsOpen != true)
            {
                try
                {
                    PowerPort.Open();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
            else
            {
                try
                {
                    PowerPort.Close();
                    PowerPort.Open();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
            SetOperationMode(OperationMode.REMOTE);
            return true;
        }

        private bool CheckSettingResult()
        {
            byte[] readCmd = new byte[26];

            while (PowerPort.ReadByte() == 0xAA)
            {
                while (PowerPort.ReadByte() == 0x00)
                {
                    readCmd[0] = 0xAA;
                    readCmd[1] = 0x00;
                    for (int i = 2; i < 26; i++)
                    {
                        readCmd[i] = (byte)PowerPort.ReadByte();
                    }
                    break;
                }
                break;
            }
            if (readCmd[3] == 0x80)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetOperationMode(OperationMode mode)
        {
            byte[] cmd = new byte[26];
            cmd[0] = HEAD;
            cmd[1] = ADDRESS;
            cmd[2] = (byte)Command.SET_OPERATION_MODE;
            cmd[3] = (byte)mode;
            cmd[25] = CalcCommandChecksum(cmd);
            PowerPort.Write(cmd, 0, 26);
        }
        public bool ReadData(out UInt32 current, out UInt32 voltage)
        {
            byte[] cmds = new byte[26];
            byte[] readCmds = new byte[26];
            cmds[0] = HEAD;
            cmds[1] = ADDRESS;
            cmds[2] = (byte)Command.READ_DATA;
            cmds[25] = CalcCommandChecksum(cmds);
            try
            {
                PowerPort.Write(cmds, 0, 26);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            while (PowerPort.ReadByte() == HEAD)
            {
                while (PowerPort.ReadByte() == ADDRESS)
                {
                    readCmds[0] = HEAD;
                    readCmds[1] = ADDRESS;
                    for (int i = 2; i < 26; i++)
                    {
                        readCmds[i] = (byte)PowerPort.ReadByte();
                    }
                    break;
                }
                break;
            }
            var checkSum = readCmds[25];
            readCmds[25] = 0;
            current = MakeWord(readCmds[3], readCmds[4]);
            voltage = MakeDWord(readCmds[5], readCmds[6], readCmds[7], readCmds[8]);
            if (CalcCommandChecksum(readCmds) != checkSum)
                return false;
            return true;
        }

        private UInt32 MakeDWord(byte a, byte b, byte c, byte d)
        {
            UInt32 result = d;
            result <<= 8;
            result |= c;
            result <<= 8;
            result |= b;
            result <<= 8;
            result |= a;
            return result;
        }

        private UInt16 MakeWord(byte lowByte, byte highByte)
        {
            UInt16 output = highByte;
            output <<= 8;
            output |= lowByte;
            return output;
        }
        public void PowerOn()
        {
            byte[] cmd = new byte[26];
            cmd[0] = HEAD;
            cmd[1] = ADDRESS;
            cmd[2] = (byte)Command.SET_OUTPUT_MODE;
            cmd[3] = (byte)OutputMode.ON;
            cmd[25] = CalcCommandChecksum(cmd);
            PowerPort.Write(cmd, 0, 26);
        }
        public void PowerOff()
        {
            byte[] cmds = new byte[26];
            cmds[0] = HEAD;
            cmds[1] = ADDRESS;
            cmds[2] = (byte)Command.SET_OUTPUT_MODE;
            cmds[3] = (byte)OutputMode.OFF;
            cmds[25] = CalcCommandChecksum(cmds);
            PowerPort.Write(cmds, 0, 26);

            SetOperationMode(OperationMode.LOCAL);
        }
        private void SetCurrent(SmartTesterStep step)
        {
            byte[] cmds = new byte[26];


            cmds[0] = HEAD;
            cmds[1] = ADDRESS;
            cmds[2] = (byte)Command.SET_CURRENT;
            cmds[3] = (byte)(step.Action.Current);
            cmds[4] = (byte)(step.Action.Current >> 8);
            cmds[25] = CalcCommandChecksum(cmds);
            PowerPort.Write(cmds, 0, 26);
        }

        private void SetVoltage(SmartTesterStep step)
        {
            byte[] cmds = new byte[26];
            cmds[0] = HEAD;
            cmds[1] = ADDRESS;
            cmds[2] = (byte)Command.SET_VOLTAGE;
            cmds[3] = (byte)(step.Action.Voltage);
            cmds[4] = (byte)(step.Action.Voltage >> 8);
            cmds[5] = (byte)(step.Action.Voltage >> 16);
            cmds[6] = (byte)(step.Action.Voltage >> 24);
            cmds[25] = CalcCommandChecksum(cmds);
            PowerPort.Write(cmds, 0, 26);
        }

        public void SetChargeParameters(SmartTesterStep step)
        {
            SetCurrent(step);
            SetVoltage(step);

        }
    }
}
