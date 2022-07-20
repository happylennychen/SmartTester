using System;
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
        public Stopwatch[] Stopwatches { get; set; }
        public DebugTesterExecutor()
        {
            FilePath = $@"debug.txt";
            Stopwatches = new Stopwatch[8];
            for (int i = 0; i < Stopwatches.Length; i++)
            {
                Stopwatches[i] = new Stopwatch();
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
                if (stdRow.TimeInMS > 60000)
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

        public double ReadTemperarture(int channelIndex)
        {
            return 0;
        }

        public bool SpecifyChannel(int channelIndex)
        {
            ChannelIndex = channelIndex;
            return true;
        }

        public bool SpecifyTestStep(Step step)
        {
            return true;
        }

        public bool Start()
        {
            Stopwatches[ChannelIndex - 1].Start();
            return true;
        }

        public bool Stop()
        {
            Stopwatches[ChannelIndex - 1].Reset();
            return true;
        }
    }
}