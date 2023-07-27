using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static System.Threading.Thread;

namespace SmartTester
{
    public class DebugTesterExecutor : ITesterExecutor
    {
        private object HWLock = new object();

        private string FilePath { get; set; }
        private int ChannelIndex { get; set; }
        private int ChannelNunber { get; set; }
        //private Stopwatch[] Stopwatches { get; set; }   //用秒表来控制每个通道的状态
        //private Queue<StandardRow>[] DataQueues { get; set; }    //用来记录历史数据，以便推演新数据
        //private uint DataLength { get; set; }       //DataQueue的长度
        private PseudoHardware[] PseudoHardwares { get; set; }
        public DebugTesterExecutor(string name)
        {
            FilePath = $@"{name} debug {DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";
            ChannelNunber = 8;
            PseudoHardwares = new PseudoHardware[ChannelNunber];
            for (int i = 0; i < ChannelNunber; i++)
            {
                //PseudoHardwares[i] = new PseudoHardware(500, 100 - i, 25 + 0.3 * i, 25 + 0.5 * i, 5 + 0.2 * i, 5 + 0.1 * i);
                PseudoHardwares[i] = new PseudoHardware(500, 150 - i, 50, 35 + 0.3 * i, 35 + 0.5 * i, 15 + 0.2 * i, 15 + 0.1 * i);
            }
            //Stopwatches = new Stopwatch[ChannelNunber];
            //for (int i = 0; i < ChannelNunber; i++)
            //{
            //    Stopwatches[i] = new Stopwatch();
            //}
        }
        public bool ReadRow(int channelIndex, out object row, out uint channelEvents)
        {
            lock (HWLock)
            {
                //var time = (uint)Stopwatches[channelIndex - 1].ElapsedMilliseconds;
                //if (channelIndex == 3 && time > 3000 && time < 4000)
                //{
                //    Console.WriteLine("Channel 3 spend a long time reading.");
                //    Thread.Sleep(3000);
                //}
                //Console.WriteLine($"Channel {channelIndex} read row");
                //stdRow = new StandardRow();
                //stdRow.TimeInMS = (uint)PseudoHardwares[channelIndex - 1].Stopwatch.ElapsedMilliseconds;
                //if (stdRow.TimeInMS > 3000)
                //    stdRow.Status = RowStatus.STOP;
                var stdRow = PseudoHardwares[channelIndex - 1].GetStandardRow();
                AccessFile($"Channel {channelIndex}, Time: {stdRow.TimeInMS}, Thread:{CurrentThread.ManagedThreadId}");
                channelEvents = 0;
                row = stdRow;
                return true;
            }
        }

        private void AccessFile(string v)
        {
            FileStream fileStream = new FileStream(FilePath, FileMode.Append);
            StreamWriter sw = new StreamWriter(fileStream);
            sw.WriteLine(v);
            sw.Flush();
            sw.Close();
            fileStream.Close();
        }

        public bool ReadTemperarture(int channelIndex, out double temperature)
        {
            temperature = PseudoHardwares[channelIndex - 1].Battery.Temperature;
            return true;
        }

        public bool SpecifyChannel(int channelIndex)
        {
            ChannelIndex = channelIndex;
            return true;
        }

        public bool SpecifyTestStep(SmartTesterStep step)
        {
            PseudoHardwares[ChannelIndex - 1].Step = step;
            return true;
        }

        public bool Start()
        {
            PseudoHardwares[ChannelIndex - 1].Start();
            return true;
        }

        public bool Stop()
        {
            PseudoHardwares[ChannelIndex - 1].Stopwatch.Reset();
            return true;
        }

        public bool Init(string ipAddress, int port, string sessionStr)
        {
            return true;
        }
    }
}