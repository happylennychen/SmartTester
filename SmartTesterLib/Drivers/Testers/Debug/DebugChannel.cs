using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace SmartTester
{
    public class DebugChannel : IChannel
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public Timer StandardTimer { get; set; }
        public Timer WaveformTimer { get; set; }
        public DataLogger DataLogger { get; set; }
        public Queue<StandardRow> DataQueue { get; set; }
        public Step CurrentStep { get; set; }
        public List<Step> FullStepsForOneTempPoint { get; set; }
        public List<double> Waveform { get; set; }
        public int SampleRate { get; set; }
        public bool IsStandardTimerStart { get; set; }
        public bool ShouldStandardTimerStart { get; set; }
        public bool IsWaveformTimerStart { get; set; }
        public bool ShouldWaveformTimerStart { get; set; }
        public double TargetTemperature { get; set; }
        public ChannelStatus Status { get; set; }
        public List<string> TempFileList { get; set; }
        [JsonIgnore]
        public ITester Tester { get; set; }
        public IChamber Chamber { get; set; }
        public uint LastTimeInMS { get; set; }
        public uint Offset { get; set; }  //记录每个工步的初始时间偏差
        public WorkMode WorkMode { get; set; }
        //public Stopwatch Anchor { get; set; }
        private int WaveformIndex = 0;

        public void GenerateFile(List<Step> fullSteps)
        {
            Utilities.FileConvert(TempFileList, fullSteps, TargetTemperature);
            TempFileList.Clear();
        }

        public void StandardReset()
        {
            Offset = 0;
            StandardTimer.Change(Timeout.Infinite, Timeout.Infinite);
            DataQueue.Clear();
            DataLogger.Close();
            ShouldStandardTimerStart = false;
            IsStandardTimerStart = false;
            LastTimeInMS = 0;
        }

        public void WaveformReset()
        {
            Offset = 0;
            Waveform.Clear();
            WaveformIndex = 0;
            WaveformTimer.Change(Timeout.Infinite, Timeout.Infinite);
            DataQueue.Clear();
            DataLogger.Close();
            ShouldWaveformTimerStart = false;
            IsWaveformTimerStart = false;
            LastTimeInMS = 0;
        }
        public DebugChannel(string name, int index, ITester tester, Timer timer)
        {
            Name = name;
            Index = index;
            Tester = tester;
            StandardTimer = timer;
            TempFileList = new List<string>();
        }
        public DebugChannel(string name, int index, ITester tester)
        {
            Name = name;
            Index = index;
            Tester = tester;
            StandardTimer = new Timer(StandardWorkerCallback, null, Timeout.Infinite, Timeout.Infinite);
            WaveformTimer = new Timer(WaveformWorkerCallback, null, Timeout.Infinite, Timeout.Infinite);
            TempFileList = new List<string>();
        }

        [JsonConstructor]
        public DebugChannel()
        {
            ;
        }
        private void StandardWorkerCallback(object i)
        {
            bool ret;
            //int counter = (int)i % Channels.Count;
            //int Index = counter + 1;
            //IChannel channel = Channels.SingleOrDefault(ch => ch.Index == Index);
            //Console.WriteLine($"CH{Index},{DateTime.Now.ToString("ss.fff")},{Math.Round(mainWatch.Elapsed.TotalMilliseconds, 0)}");
            //return;
            //long data;
            #region read data
            StandardRow stdRow;
            uint channelEvents;
            if (Offset == 0)        //工步的第一次
            {
                ret = Tester.Executor.ReadRow(Index, out stdRow, out channelEvents);
                if (!ret)
                {
                    StandardReset();
                    Status = ChannelStatus.ERROR;
                    Console.WriteLine("Cannot read row from tester. Please check cable connection.");
                    return;
                }
                var startPoint = stdRow.TimeInMS % 1000;
                Offset = startPoint + 15;
                Console.WriteLine($"Set offset to {Offset}.");
            }
            do
            {
                ret = Tester.Executor.ReadRow(Index, out stdRow, out channelEvents);
                if (!ret)
                {
                    StandardReset();
                    Status = ChannelStatus.ERROR;
                    Console.WriteLine("Cannot read row from tester. Please check cable connection.");
                    return;
                }
                //data = stdRow.TimeInMS % 1000;
            }
            //while (data > 100 && stdRow.Status == RowStatus.RUNNING);
            while (stdRow.TimeInMS < (1000 + LastTimeInMS + Offset) && stdRow.Status == RowStatus.RUNNING);

            LastTimeInMS = stdRow.TimeInMS / 1000 * 1000;
            double temperature;
            ret = Tester.Executor.ReadTemperarture(Index, out temperature);
            if (!ret)
            {
                StandardReset();
                Status = ChannelStatus.ERROR;
                Console.WriteLine("Cannot read temperature from tester. Please check cable connection.");
                return;
            }
            stdRow.Temperature = temperature;
            DataQueue.Enqueue(stdRow);
            if (stdRow.Status == RowStatus.STOP)
                stdRow = GetAdjustedRow(DataQueue.ToList(), CurrentStep);
            if (DataQueue.Count >= 4)
                DataQueue.Dequeue();
            //var strRow = stdRow.ToString();
            var strRow = GetRowString(stdRow, Offset);
            #endregion

            #region log data
            DataLogger.AddData(strRow + "\n");
            #endregion

            #region display data
            string gap = string.Empty;
            for (int j = 0; j < Index - 1; j++)
            {
                gap += " ";
            }
            Console.WriteLine($"{strRow,-60}Ch{gap}{Index}.");
            #endregion

            #region verify data
            if (channelEvents != ChannelEvents.Normal)
            {
                StandardReset();
                Console.WriteLine("Channel Event Error");
                return;
            }
            if (CurrentStep.Action.Mode == ActionMode.CC_DISCHARGE)
                if (stdRow.Current - CurrentStep.Action.Current > StepTolerance.Current)
                {
                    StandardReset();
                    Status = ChannelStatus.ERROR;
                    Console.WriteLine("Current out of range.");
                    return;
                }
            #endregion

            #region change step
            if (stdRow.Status != RowStatus.RUNNING)
            {
                CurrentStep = Utilities.GetNewTargetStep(CurrentStep, FullStepsForOneTempPoint, TargetTemperature, stdRow.TimeInMS, stdRow);
                if (CurrentStep == null)
                {
                    StandardReset();
                    Console.WriteLine($"CH{Index} Done!");
                    Status = ChannelStatus.IDLE;
                    //Task task = Task.Run(() => FileTransfer(DataLogger.FilePath));
                    return;
                }
                else    //新的工步
                {
                    LastTimeInMS = 0;
                    if (!Tester.Executor.SpecifyChannel(Index))
                    {
                        StandardReset();
                        Console.WriteLine("Cannot specify  Please check cable connection.");
                        return;
                    }
                    if (!Tester.Executor.SpecifyTestStep(CurrentStep))
                    {
                        StandardReset();
                        Console.WriteLine("Cannot specify test step. Please check cable connection.");
                        return;
                    }
                    if (!Tester.Executor.Start())
                    {
                        StandardReset();
                        Console.WriteLine("Cannot start tester. Please check cable connection.");
                        return;
                    }
                }
            }
            #endregion

            if (ShouldStandardTimerStart) //开启下一次计时
            {
                StandardTimer.Change(950, 0);
            }
        }
        private string GetRowString(StandardRow stdRow, uint offset)
        {
            var newRow = stdRow.Clone();
            newRow.TimeInMS -= offset;
            return newRow.ToString();
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


        private void WaveformWorkerCallback(object i)
        {
            bool ret;
            #region read data
            StandardRow stdRow;
            uint channelEvents;
            if (Offset == 0)        //工步的第一次
            {
                ret = Tester.Executor.ReadRow(Index, out stdRow, out channelEvents);
                if (!ret)
                {
                    WaveformReset();
                    Status = ChannelStatus.ERROR;
                    Console.WriteLine("Cannot read row from tester. Please check cable connection.");
                    return;
                }
                var startPoint = stdRow.TimeInMS % 1000;
            }
            do
            {
                ret = Tester.Executor.ReadRow(Index, out stdRow, out channelEvents);
                if (!ret)
                {
                    WaveformReset();
                    Status = ChannelStatus.ERROR;
                    Console.WriteLine("Cannot read row from tester. Please check cable connection.");
                    return;
                }
            }
            while (stdRow.TimeInMS < (1000 + LastTimeInMS + Offset) && stdRow.Status == RowStatus.RUNNING);

            LastTimeInMS = stdRow.TimeInMS / 1000 * 1000;
            double temperature;
            ret = Tester.Executor.ReadTemperarture(Index, out temperature);
            if (!ret)
            {
                WaveformReset();
                Status = ChannelStatus.ERROR;
                Console.WriteLine("Cannot read temperature from tester. Please check cable connection.");
                return;
            }
            stdRow.Temperature = temperature;
            DataQueue.Enqueue(stdRow);
            if (stdRow.Status == RowStatus.STOP)
                stdRow = GetAdjustedRow(DataQueue.ToList(), CurrentStep);
            if (DataQueue.Count >= 4)
                DataQueue.Dequeue();
            //var strRow = stdRow.ToString();
            var strRow = GetRowString(stdRow, Offset);
            #endregion

            #region log data
            DataLogger.AddData(strRow + "\n");
            #endregion

            #region display data
            string gap = string.Empty;
            for (int j = 0; j < Index - 1; j++)
            {
                gap += " ";
            }
            Console.WriteLine($"{strRow,-60}Ch{gap}{Index}.");
            #endregion

            #region verify data
            if (channelEvents != ChannelEvents.Normal)
            {
                WaveformReset();
                Console.WriteLine("Channel Event Error");
                return;
            }
            if (CurrentStep.Action.Mode == ActionMode.CC_DISCHARGE)
                if (stdRow.Current - CurrentStep.Action.Current > StepTolerance.Current)
                {
                    WaveformReset();
                    Status = ChannelStatus.ERROR;
                    Console.WriteLine("Current out of range.");
                    return;
                }
            #endregion

            #region change step
            {
                //CurrentStep = Utilities.GetNewTargetStep(CurrentStep, FullStepsForOneTempPoint, TargetTemperature, stdRow.TimeInMS, stdRow);
                //if (CurrentStep == null)
                //{
                //    Reset();
                //    Console.WriteLine($"CH{Index} Done!");
                //    Status = ChannelStatus.IDLE;
                //    //Task task = Task.Run(() => FileTransfer(DataLogger.FilePath));
                //    return;
                //}
                //else    //新的工步
                //{
                //    LastTimeInMS = 0;
                //    if (!Tester.Executor.SpecifyChannel(Index))
                //    {
                //        Reset();
                //        Console.WriteLine("Cannot specify  Please check cable connection.");
                //        return;
                //    }
                //    if (!Tester.Executor.SpecifyTestStep(CurrentStep))
                //    {
                //        Reset();
                //        Console.WriteLine("Cannot specify test step. Please check cable connection.");
                //        return;
                //    }
                //    if (!Tester.Executor.Start())
                //    {
                //        Reset();
                //        Console.WriteLine("Cannot start tester. Please check cable connection.");
                //        return;
                //    }
                //}
                if (stdRow.Status != RowStatus.RUNNING || (Waveform.Count <= WaveformIndex && Waveform.Count != 0))
                {
                    WaveformReset();
                    Console.WriteLine($"CH{Index} Done!");
                    Status = ChannelStatus.IDLE;
                    return;
                }
                else
                {
                    var current = Waveform[WaveformIndex++];
                    Tester.Executor.ChangeCurrent(current / 1000);
                }
            }
            #endregion


            if (ShouldWaveformTimerStart) //开启下一次计时
            {
                WaveformTimer.Change(950, 0);
            }
        }
        public void Start(WorkMode workMode)
        {
            if (workMode == WorkMode.Standard)
            {
                ShouldStandardTimerStart = true;
            }
            else if (workMode == WorkMode.Waveform)
            {
                ShouldWaveformTimerStart = true;
            }
            Status = ChannelStatus.RUNNING;
            WorkMode = workMode;
        }

        public void Stop()
        {
            Console.WriteLine($"Stop channel {Index}");

            Tester.Executor.SpecifyChannel(Index);
            Tester.Executor.Stop();

            ShouldStandardTimerStart = false;
            StandardTimer.Change(Timeout.Infinite, Timeout.Infinite);
            IsStandardTimerStart = false;

            ShouldWaveformTimerStart = false;
            WaveformTimer.Change(Timeout.Infinite, Timeout.Infinite);
            IsWaveformTimerStart = false;
        }
    }
}