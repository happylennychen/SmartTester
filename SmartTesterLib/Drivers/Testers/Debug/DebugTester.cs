using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace SmartTester
{
    public class DebugTester : ITester
    {
        public int Id { get; set; }
        [JsonIgnore]
        public List<IChannel> Channels { get; set; }
        public string Name { get; set; }
        public int ChannelNumber { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string SessionStr { get; set; }
        [JsonIgnore]
        public ITesterExecutor Executor { get; set; }
        private Timer mainTimer { get; set; }
        private int _counter { get; set; } = 0;
        private Stopwatch mainWatch { get; set; }

        [JsonConstructor]
        public DebugTester(int id, string name, int channelNumber, string ipAddress, int port, string sessionStr)
        {
            Id = id;
            Name = name;
            ChannelNumber = channelNumber;
            IpAddress = ipAddress;
            Port = port;
            SessionStr = sessionStr;
            Executor = new DebugTesterExecutor(Name);
            mainWatch = new Stopwatch();
            Channels = new List<IChannel>();

            for (int i = 1; i <= channelNumber; i++)
            {
                DebugChannel ch = new DebugChannel($"Ch{i}", i, this, new Timer(WorkerCallback, i - 1, Timeout.Infinite, Timeout.Infinite));
                Channels.Add(ch);
            }
            if (!Executor.Init(ipAddress, port, sessionStr))
            {
                Console.WriteLine("Error");
                return;
            }
            mainTimer = new Timer(_ => MainCounter(), null, 100, 0);
            mainWatch.Start();
        }
        private void WorkerCallback(object i)
        {
            bool ret;
            int counter = (int)i % Channels.Count;
            int channelIndex = counter + 1;
            IChannel channel = Channels.SingleOrDefault(ch => ch.Index == channelIndex);
            //Console.WriteLine($"CH{channel.Index},{DateTime.Now.ToString("ss.fff")},{Math.Round(mainWatch.Elapsed.TotalMilliseconds, 0)}");
            //return;
            //long data;
            #region read data
            StandardRow stdRow;
            uint channelEvents;
            ret = Executor.ReadRow(channelIndex, out stdRow, out channelEvents);
            if (!ret)
            {
                channel.Reset();
                channel.Status = ChannelStatus.ERROR;
                Console.WriteLine("Cannot read row from tester. Please check cable connection.");
                return;
            }
            var startPoint = stdRow.TimeInMS % 1000;
            do
            {
                ret = Executor.ReadRow(channelIndex, out stdRow, out channelEvents);
                if (!ret)
                {
                    channel.Reset();
                    channel.Status = ChannelStatus.ERROR;
                    Console.WriteLine("Cannot read row from tester. Please check cable connection.");
                    return;
                }
                //data = stdRow.TimeInMS % 1000;
            }
            //while (data > 100 && stdRow.Status == RowStatus.RUNNING);
            while (stdRow.TimeInMS < (1000 + channel.LastTimeInMS) && stdRow.Status == RowStatus.RUNNING);

            channel.LastTimeInMS = stdRow.TimeInMS / 1000 * 1000;
            double temperature;
            ret = Executor.ReadTemperarture(channelIndex, out temperature);
            if (!ret)
            {
                channel.Reset();
                channel.Status = ChannelStatus.ERROR;
                Console.WriteLine("Cannot read temperature from tester. Please check cable connection.");
                return;
            }
            stdRow.Temperature = temperature;
            channel.DataQueue.Enqueue(stdRow);
            if (stdRow.Status == RowStatus.STOP)
                stdRow = GetAdjustedRow(channel.DataQueue.ToList(), channel.CurrentStep);
            if (channel.DataQueue.Count >= 4)
                channel.DataQueue.Dequeue();
            var strRow = stdRow.ToString();
            #endregion

            #region log data
            channel.DataLogger.AddData(strRow + "\n");
            #endregion

            #region display data
            string gap = string.Empty;
            for (int j = 0; j < counter; j++)
            {
                gap += " ";
            }
            Console.WriteLine($"{strRow,-60}Ch{gap}{channelIndex}.");
            #endregion

            #region verify data
            if (channelEvents != ChannelEvents.Normal)
            {
                channel.Reset();
                Console.WriteLine("Channel Event Error");
                return;
            }
            if (channel.CurrentStep.Action.Mode == ActionMode.CC_DISCHARGE)
                if (stdRow.Current - channel.CurrentStep.Action.Current > StepTolerance.Current)
                {
                    channel.Reset();
                    channel.Status = ChannelStatus.ERROR;
                    Console.WriteLine("Current out of range.");
                    return;
                }
            #endregion

            #region change step
            if (stdRow.Status != RowStatus.RUNNING)
            {
                channel.CurrentStep = Utilities.GetNewTargetStep(channel.CurrentStep, channel.FullStepsForOneTempPoint, channel.TargetTemperature, stdRow.TimeInMS, stdRow);
                if (channel.CurrentStep == null)
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
                        Console.WriteLine("Cannot specify channel. Please check cable connection.");
                        return;
                    }
                    if (!Executor.SpecifyTestStep(channel.CurrentStep))
                    {
                        channel.Reset();
                        Console.WriteLine("Cannot specify test step. Please check cable connection.");
                        return;
                    }
                    if (!Executor.Start())
                    {
                        channel.Reset();
                        Console.WriteLine("Cannot start tester. Please check cable connection.");
                        return;
                    }
                }
            }
            #endregion

            if (channel.ShouldTimerStart) //开启下一次计时
            {
                channel.Timer.Change(950, 0);
            }
        }

        private StandardRow GetAdjustedRow(List<StandardRow> standardRows, Step step)
        {
            List<StandardRow> rows = GetLastStepRows(standardRows);
            if (rows.Count < 3) //如果没有足够多的行，则根据设定值来修正最后一行
            {
                var lastRow = rows.Last();
                switch (step.Action.Mode)
                {
                    case ActionMode.CC_DISCHARGE:
                        lastRow.Voltage = step.CutOffBehaviors.SingleOrDefault(cob => cob.Condition.Parameter == Parameter.VOLTAGE).Condition.Value;
                        lastRow.Current = step.Action.Current;
                        break;
                    case ActionMode.CP_DISCHARGE:
                        lastRow.Voltage = step.CutOffBehaviors.SingleOrDefault(cob => cob.Condition.Parameter == Parameter.VOLTAGE).Condition.Value;
                        lastRow.Current = step.Action.Power / lastRow.Voltage * 1000;
                        break;
                }
                return lastRow;
            }
            else    //如果有足够多行，则用插值的方式来修正最后一行
            {
                var lastRow = rows[rows.Count - 1];
                var secondLastRow = rows[rows.Count - 2];
                var thirdLastRow = rows[rows.Count - 3];
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
                    return rows.Last();
            }
        }

        private List<StandardRow> GetLastStepRows(List<StandardRow> standardRows)   //虽然只有短短几行，但是也可能有多个工步，我们只需要最后一个工步的数据
        {
            List<StandardRow> output = new List<StandardRow>();
            Stack<StandardRow> rowStack = new Stack<StandardRow>();
            standardRows.Reverse();
            foreach (var row in standardRows)
            {
                if (row.Status == RowStatus.RUNNING || standardRows.First() == row)
                    rowStack.Push(row);
                else
                    break;
            }
            while (rowStack.Count() != 0)
                output.Add(rowStack.Pop());
            return output;
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
                string fileName = $"{Name}-{channel.Name}-{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";
                channel.DataLogger = new DataLogger(channel.Chamber, counter + 1, fileName);
                channel.TempFileList.Add(channel.DataLogger.FilePath);
                Executor.SpecifyChannel(counter + 1);
                channel.CurrentStep = channel.FullStepsForOneTempPoint.First();
                Executor.SpecifyTestStep(channel.CurrentStep);
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
            throw new System.NotImplementedException();
        }
    }
}