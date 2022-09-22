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

        public string FilePath { get; set; }
        public int ChannelIndex { get; set; }
        private int ChannelNunber { get; set; }
        private Stopwatch[] Stopwatches { get; set; }   //用秒表来控制每个通道的状态
        private Queue<StandardRow>[] DataQueues { get; set; }    //用来记录历史数据，以便推演新数据
        private uint DataLength { get; set; }       //DataQueue的长度
        private PseudoHardware[] PseudoHardwares { get; set; }
        public DebugTesterExecutor(string name)
        {
            FilePath = $@"{name} debug {DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";
            ChannelNunber = 8;
            PseudoHardwares = new PseudoHardware[ChannelNunber];
            for (int i = 0; i < ChannelNunber; i++)
            {
                PseudoHardwares[i] = new PseudoHardware();
            }
        }
        public bool ReadRow(int channelIndex, out StandardRow stdRow, out uint channelEvents)
        {
            lock (HWLock)
            {
                //var time = (uint)Stopwatches[channelIndex - 1].ElapsedMilliseconds;
                //if (channelIndex == 3 && time > 3000 && time < 4000)
                //{
                //    Console.WriteLine("Channel 3 spend a long time reading.");
                //    Thread.Sleep(3000);
                //}
                Console.WriteLine($"Channel {channelIndex} read row");
                stdRow = new StandardRow();
                stdRow.TimeInMS = (uint)Stopwatches[channelIndex - 1].ElapsedMilliseconds;
                if (stdRow.TimeInMS > 3000)
                    stdRow.Status = RowStatus.STOP;
                AccessFile($"Channel {channelIndex}, Time: {stdRow.TimeInMS}, Thread:{CurrentThread.ManagedThreadId}");
                channelEvents = 0;
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
            temperature = 0;
            return true;
        }

        public bool SpecifyChannel(int channelIndex)
        {
            ChannelIndex = channelIndex;
            return true;
        }

        public bool SpecifyTestStep(Step step)
        {
            PseudoHardwares[ChannelIndex - 1].Step = step;
            return true;
        }

        public bool Start()
        {
            PseudoHardwares[ChannelIndex - 1].Stopwatch.Start();
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