#define debug
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Threading.Thread;
namespace SmartTester
{
    public class Tester : ITester
    {
        public string Name { get; set; }
        public List<Channel> Channels { get; set; }
        private Timer mainTimer { get; set; }
        private int _counter { get; set; } = 0;
        private Stopwatch mainWatch { get; set; }
#if debug
        private DebugTesterExecutor Executor { get; set; }
#else
        private Chroma17208Executor Executor { get; set; }
#endif
        public Tester()
        { }
        public Tester(string name, int channelNumber, string ipAddress, int port, string sessionStr)
        {
            Name = name;
#if debug
            Executor = new DebugTesterExecutor();
#else
            Executor = new Chroma17208Executor();
#endif
            mainWatch = new Stopwatch();
            Channels = new List<Channel>();
#if !debug
            if (!Executor.Init(ipAddress, port, sessionStr))
            {
                Console.WriteLine("Error");
                return;
            }
#endif

            for (int i = 1; i <= channelNumber; i++)
            {
                Channel ch = new Channel($"Ch{i}", i, this, new Timer(WorkerCallback, i - 1, Timeout.Infinite, Timeout.Infinite));
                Channels.Add(ch);
            }
            mainTimer = new Timer(_ => MainCounter(), null, 100, 0);
            mainWatch.Start();
        }

        private void WorkerCallback(object i)
        {
            int counter = (int)i % Channels.Count;
            int channelIndex = counter + 1;
            Channel channel = Channels.SingleOrDefault(ch => ch.Index == channelIndex);
            //long data;
            StandardRow stdRow;
            uint channelEvents;
            if (!Executor.ReadRow(channelIndex, out stdRow, out channelEvents))
            {
                channel.Reset();
                channel.Status = ChannelStatus.ERROR;
                Console.WriteLine("Error");
                return;
            }
            var startPoint = stdRow.TimeInMS % 1000;
            do
            {
                Executor.ReadRow(channelIndex, out stdRow, out channelEvents);
                //data = stdRow.TimeInMS % 1000;
            }
            //while (data > 100 && stdRow.Status == RowStatus.RUNNING);
            while (stdRow.TimeInMS < (1000 + channel.LastTimeInMS) && stdRow.Status == RowStatus.RUNNING);

            channel.LastTimeInMS = stdRow.TimeInMS / 1000 * 1000;
            stdRow.Temperature = Executor.ReadTemperarture(channelIndex);
            channel.DataQueue.Enqueue(stdRow);
            if (stdRow.Status == RowStatus.STOP)
                stdRow = GetAdjustedRow(channel.DataQueue.ToList());
            if (channel.DataQueue.Count >= 4)
                channel.DataQueue.Dequeue();
            var strRow = stdRow.ToString();
            channel.DataLogger.AddData(strRow + "\n");
            string gap = string.Empty;
            for (int j = 0; j < counter; j++)
            {
                gap += " ";
            }
            Console.WriteLine($"{strRow,-60}Ch{gap}{channelIndex}.");
            if (channelEvents != ChannelEvents.Normal)
            {
                channel.Reset();
                Console.WriteLine("Channel Event Error");
                return;
            }
            if (stdRow.Status != RowStatus.RUNNING)
            {
                channel.Step = Utilities.GetNewTargetStep(channel.Step, channel.FullSteps, channel.TargetTemperature, stdRow.TimeInMS, stdRow);
                if (channel.Step == null)
                {
                    channel.Reset();
                    Console.WriteLine($"CH{channelIndex} Done!");
                    channel.Status = ChannelStatus.IDLE;
                    //Task task = Task.Run(() => FileTransfer(channel.DataLogger.FilePath));
                    return;
                }
                else    //新的工步
                {
                    channel.LastTimeInMS = 0;
                    if (!Executor.SpecifyChannel(channelIndex))
                    {
                        channel.Reset();
                        Console.WriteLine("Error");
                        return;
                    }
                    if (!Executor.SpecifyTestStep(channel.Step))
                    {
                        channel.Reset();
                        Console.WriteLine("Error");
                        return;
                    }
                    if (!Executor.Start())
                    {
                        channel.Reset();
                        Console.WriteLine("Error");
                        return;
                    }
                }
            }
            if (channel.ShouldTimerStart) //开启下一次计时
            {
                channel.Timer.Change(950, 0);
            }
        }

        private StandardRow GetAdjustedRow(List<StandardRow> standardRows)
        {
            if (standardRows.Count < 3)
                return standardRows.Last();
            else
            {
                var lastRow = standardRows[standardRows.Count - 1];
                var secondLastRow = standardRows[standardRows.Count - 2];
                var thirdLastRow = standardRows[standardRows.Count - 3];
                if (
                        (lastRow.Status == RowStatus.STOP && secondLastRow.Status == RowStatus.RUNNING && thirdLastRow.Status == RowStatus.RUNNING) &&
                        (
                            (lastRow.Mode == ActionMode.CC_DISCHARGE && secondLastRow.Mode == ActionMode.CC_DISCHARGE && thirdLastRow.Mode == ActionMode.CC_DISCHARGE) ||
                            (lastRow.Mode == ActionMode.CP_DISCHARGE && secondLastRow.Mode == ActionMode.CP_DISCHARGE && thirdLastRow.Mode == ActionMode.CP_DISCHARGE) ||
                            (lastRow.Mode == ActionMode.CC_CV_CHARGE && secondLastRow.Mode == ActionMode.CC_CV_CHARGE && thirdLastRow.Mode == ActionMode.CC_CV_CHARGE))
                        )
                {
                    lastRow.Voltage = GetAdjustedValue(secondLastRow.TimeInMS, secondLastRow.Voltage, thirdLastRow.TimeInMS, thirdLastRow.Voltage, lastRow.TimeInMS);
                    lastRow.Current = GetAdjustedValue(secondLastRow.TimeInMS, secondLastRow.Current, thirdLastRow.TimeInMS, thirdLastRow.Current, lastRow.TimeInMS);
                    return lastRow;
                }
                else
                    return standardRows.Last();
            }
        }

        private double GetAdjustedValue(uint x1, double y1, uint x2, double y2, uint x)
        {
            // a * x + b = y
            // a * x1 + b = y1
            // a * x2 + b = y2
            // a = (y2-y1)/(x2-x1)
            // b = y1 - x1*a
            double slope = (y2 - y1) / ((int)x2 - (int)x1);
            double offset = y1 - slope * x1;
            var output = Math.Round((slope * x + offset), 6);
            Console.WriteLine($"x1:{x1}, y1:{y1}, x2:{x2}, y2:{y2}, x:{x}, y:{output}");
            return output;
        }

        private void MainCounter()
        {
            long data;
            do
            {
                data = mainWatch.ElapsedMilliseconds % 125;
            }
            while (data > 10);
            mainTimer.Change(100, 0);

            var counter = _counter % Channels.Count;
            var channel = Channels.SingleOrDefault(ch => ch.Index == counter + 1);
            if (channel.ShouldTimerStart && !channel.IsTimerStart)     //应该开启且还没开启
            {
                channel.DataQueue = new Queue<StandardRow>();
                string filePath = $"{Name}-{channel.Name}-{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";
                channel.TempFileList.Add(filePath);
                channel.DataLogger = new DataLogger(counter + 1, filePath);
                Executor.SpecifyChannel(counter + 1);
                channel.Step = channel.FullSteps.First();
                Executor.SpecifyTestStep(channel.Step);
                Executor.Start();
                channel.Timer.Change(980, 0);
                channel.IsTimerStart = true;
            }
            _counter++;
            if (_counter == Channels.Count * 10)
            {
                _counter = 0;
            }
        }

        public void SetStep(Step step, int index)
        {
            if (!Executor.SpecifyChannel(index))
            {
                Console.WriteLine("Error");
                return;
            }
            if (!Executor.SpecifyTestStep(step))
            {
                Console.WriteLine("Error");
                return;
            }
        }

        public void Start(int index)
        {
            var channel = Channels.SingleOrDefault(ch => ch.Index == index);
            channel.ShouldTimerStart = true;
            channel.Status = ChannelStatus.RUNNING;
        }

        public void Stop(int index)
        {
            var channel = Channels.SingleOrDefault(ch => ch.Index == index);
            channel.ShouldTimerStart = false;

            Console.WriteLine($"Stop channel {index - 1 + 1}");
            Executor.SpecifyChannel(index);
            Executor.Stop();
            channel.Timer.Change(Timeout.Infinite, Timeout.Infinite);
            channel.IsTimerStart = false;
        }

        public string GetData(int index)
        {
            throw new NotImplementedException();
        }



    }
}
