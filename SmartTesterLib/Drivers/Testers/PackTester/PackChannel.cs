using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace SmartTester
{
    public class PackChannel : IChannel
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public SmartTesterStep CurrentStep { get; set; }
        private List<SmartTesterStep> StepsForOneTempPoint { get; set; }
        public SmartTesterRecipe Recipe { get; set; }
        [JsonIgnore]
        public ITester Tester { get; set; }
        public IChamber Chamber { get; set; }
        public ChannelStatus Status { get; set; }
        private Token Token { get; set; }
        private double TargetTemperature { get; set; }
        private List<string> TempFileList { get; set; }
        private uint LastTimeInMS { get; set; }
        private Timer Timer { get; set; }
        private DataLogger DataLogger { get; set; }
        private Queue<StandardRow> DataQueue { get; set; }

        public void GenerateFile()
        {
            Utilities.StdFileConvert(TempFileList, Recipe.Steps, TargetTemperature);
            TempFileList.Clear();
        }

        public void Reset()
        {
            DataQueue.Clear();
            DataLogger.Close();
            Token.ShouldTimerStart = false;
            Token.IsTimerStart = false;
            LastTimeInMS = 0;
        }

        public void Stop()
        {
            Token.ShouldTimerStart = false;

            Console.WriteLine($"Stop channel {Index - 1 + 1}");
            Tester.Executor.SpecifyChannel(Index);
            Tester.Executor.Stop();
            Timer.Change(Timeout.Infinite, Timeout.Infinite);
            Token.IsTimerStart = false;
        }

        public void Start()
        {
            Token.ShouldTimerStart = true;
            Status = ChannelStatus.RUNNING;
        }

        public PackChannel(string name, int index, ITester tester, Timer timer)
        {
            Name = name;
            Index = index;
            Tester = tester;
            Timer = timer;
            TempFileList = new List<string>();
        }
        [JsonConstructor]
        public PackChannel()
        {
            ;
        }
        public PackChannel(string name, int index, ITester tester, out Token token)
        {
            Name = name;
            Index = index;
            Tester = tester;
            LastTimeInMS = 0;
            Timer = new Timer(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);
            TempFileList = new List<string>();
            token = new Token(index, $"Channel {Index} Token", TokenCallback);
            Token = token;
            Status = ChannelStatus.UNASSIGNED;
        }

        private void TokenCallback()
        {
            DataQueue = new Queue<StandardRow>();
            string fileName = $"{Tester.Name}-{Name}-{Recipe.Name}-{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";
            DataLogger = new DataLogger(GlobalSettings.OutputFolder, fileName);
            TempFileList.Add(DataLogger.FilePath);
            Tester.Executor.SpecifyChannel(Index);
            CurrentStep = StepsForOneTempPoint.First();
            Tester.Executor.SpecifyTestStep(CurrentStep);
            Tester.Executor.Start();
            Timer.Change(900, 0);
        }

        private void TimerCallback(object i)
        {
            bool ret;
            #region read data
            object row;
            PackRow packRow;
            uint channelEvents;
            do
            {
                ret = Tester.Executor.ReadRow(Index, out row, out channelEvents);
                if (!ret)
                {
                    Reset();
                    Status = ChannelStatus.ERROR;
                    Console.WriteLine("Cannot read row from tester. Please check cable connection.");
                    return;
                }
                packRow = row as PackRow;
                packRow.Mode = CurrentStep.Action.Mode;
                packRow.UpdateStatus(CurrentStep);
            }
            while (packRow.TimeInMS < (1000 + LastTimeInMS) && packRow.Status == RowStatus.RUNNING);

            LastTimeInMS = packRow.TimeInMS / 1000 * 1000;
            var strRow = packRow.ToString();
            #endregion

            #region log data
            DataLogger.AddData(strRow + "\n");
            #endregion

            #region display data
            Console.WriteLine(strRow);
            #endregion

            #region verify data
            if (channelEvents != ChannelEvents.Normal)
            {
                Reset();
                Console.WriteLine("Channel Event Error");
                return;
            }
            if (CurrentStep.Action.Mode == ActionMode.CC_DISCHARGE)
                if (packRow.Current - CurrentStep.Action.Current > StepTolerance.Current)
                {
                    Reset();
                    Status = ChannelStatus.ERROR;
                    Console.WriteLine("Current out of range.");
                    return;
                }
            #endregion

            #region change step
            if (packRow.Status != RowStatus.RUNNING)
            {
                CurrentStep = Utilities.GetNewTargetStep(CurrentStep, StepsForOneTempPoint, TargetTemperature, packRow.TimeInMS, packRow);
                if (CurrentStep == null)
                {
                    Reset();
                    Console.WriteLine($"CH{Index} Done!");
                    Status = ChannelStatus.COMPLETED;
                    //Task task = Task.Run(() => FileTransfer(DataLogger.FilePath));
                    return;
                }
                else    //新的工步
                {
                    LastTimeInMS = 0;
                    if (!Tester.Executor.SpecifyChannel(Index))
                    {
                        Reset();
                        Console.WriteLine("Cannot specify  Please check cable connection.");
                        return;
                    }
                    if (!Tester.Executor.SpecifyTestStep(CurrentStep))
                    {
                        Reset();
                        Console.WriteLine("Cannot specify test step. Please check cable connection.");
                        return;
                    }
                    if (!Tester.Executor.Start())
                    {
                        Reset();
                        Console.WriteLine("Cannot start tester. Please check cable connection.");
                        return;
                    }
                }
            }
            #endregion

            if (Token.ShouldTimerStart) //开启下一次计时
            {
                Timer.Change(950, 0);
            }
        }

        private StandardRow GetAdjustedRow(List<StandardRow> standardRows, SmartTesterStep step)
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

        public void SetStepsForOneTempPoint()
        {
            Console.WriteLine($"Chamber:{Chamber.Name}, Channel:{Name} S");
            Console.WriteLine("");
            StepsForOneTempPoint = GetCurrentSteps();
            if (StepsForOneTempPoint.Count == 0)
                Console.WriteLine("No valid temp point.");
            Console.WriteLine($"Chamber:{Chamber.Name}, Channel:{Name} E");
            Console.WriteLine("");
        }

        private List<SmartTesterStep> GetCurrentSteps()
        {
            return Recipe.Steps.Where(st => (st.TemperatureUint.Status == TemperatureStatus.REACHING && st.TemperatureUint.Target.IsCritical == false) || st.TemperatureUint.Status == TemperatureStatus.REACHED).ToList();
        }
    }
}