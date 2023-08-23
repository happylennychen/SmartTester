//using Newtonsoft.Json;
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
        public SmartTesterStep CurrentStep { get; set; }
        private List<SmartTesterStep> StepsForOneTempPoint { get; set; }
        public SmartTesterRecipe Recipe { get; set; }
        //[JsonIgnore]
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
            StdFileConvert(TempFileList, Recipe.Steps, TargetTemperature);
            TempFileList.Clear();
        }
        public void StdFileConvert(List<string> filePaths, List<SmartTesterStep> fullSteps, double targetTemperature)
        {
            uint indexOffset = 0;
            uint timeOffset = 0;
            double capacityOffset = 0;
            double totalCapacityOffset = 0;
            uint lastTimeInMS = 0;
            StandardRow lastRow = null;
            StandardRow currentRow = null;
            var newFilePath = Path.ChangeExtension(filePaths[0], "csv");
            SmartTesterStep step = fullSteps.First();
            using (FileStream stdFile = new FileStream(newFilePath, FileMode.Create))
            {
                Console.WriteLine($"{newFilePath} created.");
                using (StreamWriter stdWriter = new StreamWriter(stdFile))
                {
                    Console.WriteLine($"StreamWriter created.");
                    stdWriter.WriteLine("Index,Time(mS),Mode,Current(mA),Voltage(mV),Temperature(degC),Capacity(mAh),Total Capacity(mAh),Status");
                    foreach (var filePath in filePaths)
                    {

                        Console.WriteLine($"Trying to open file {filePath}.");
                        try
                        {
                            using (FileStream rawFile = new FileStream(filePath, FileMode.Open))
                            {
                                using (StreamReader rawReader = new StreamReader(rawFile))
                                {
                                    while (rawReader.Peek() != -1)
                                    {
                                        if (currentRow != null)
                                            lastRow = currentRow;
                                        var line = rawReader.ReadLine();
                                        currentRow = new StandardRow();
                                        currentRow.LoadFromString(line);
                                        if (lastRow == null)
                                            continue;

                                        if (lastRow.Status != RowStatus.RUNNING)
                                        {
                                            CutOffBehavior cob = GetCutOffBehavior(step, lastRow);
                                            step = GetNewTargetStep(step, fullSteps, targetTemperature, lastRow);
                                            if (step == null)
                                            {
                                                Console.WriteLine("GetNewTargetStep return null");
                                                break;
                                            }
                                            lastRow.Status = Utilities.UpdateLastRowStatus(cob);
                                        }

                                        lastRow.Index = ++indexOffset;
                                        lastRow.TimeInMS += timeOffset;
                                        lastRow.Capacity += capacityOffset;
                                        lastRow.TotalCapacity = lastRow.Capacity + totalCapacityOffset;
                                        //var offset = (int)currentRow.TimeInMS - (int)lastRow.TimeInMS - 1000;
                                        stdWriter.WriteLine(lastRow.ToString()/* + "," + offset.ToString()*/);
                                        if (lastRow.Status != RowStatus.RUNNING)
                                        {
                                            timeOffset = lastRow.TimeInMS;
                                            if (Utilities.ShouldCapacityContinue(lastRow, currentRow))
                                            {
                                                capacityOffset = lastRow.Capacity;
                                            }
                                            else
                                            {
                                                capacityOffset = 0;
                                                totalCapacityOffset = lastRow.TotalCapacity;
                                            }
                                        }
                                        lastTimeInMS = lastRow.TimeInMS;
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Cannot open file {filePath}.\n{e.Message}");
                            return;
                        }
                    }
                    //处理最后一行数据
                    currentRow.Index = ++indexOffset;
                    currentRow.TimeInMS += timeOffset;
                    currentRow.Capacity += capacityOffset;
                    currentRow.TotalCapacity = currentRow.Capacity + totalCapacityOffset;
                    currentRow.Status = Utilities.UpdateLastRowStatus(GetCutOffBehavior(step, currentRow));
                    stdWriter.WriteLine(currentRow.ToString());
                }
            }
            string newFileFullPath = Utilities.GetNewFileFullPath(newFilePath, lastTimeInMS);
            if (newFileFullPath != null)
                File.Move(newFilePath, newFileFullPath);
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

        public DebugChannel(string name, int index, ITester tester, Timer timer)
        {
            Name = name;
            Index = index;
            Tester = tester;
            Timer = timer;
            TempFileList = new List<string>();
        }
        //[JsonConstructor]
        public DebugChannel()
        {
            ;
        }
        public DebugChannel(string name, int index, ITester tester, out Token token)
        {
            Name = name;
            Index = index;
            Tester = tester;
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
            Timer.Change(980, 0);
        }

        private void TimerCallback(object i)
        {
            bool ret;
            #region read data
            object row;
            StandardRow stdRow;
            uint channelEvents;
            ret = Tester.Executor.ReadRow(Index, out row, out channelEvents);
            if (!ret)
            {
                Reset();
                Status = ChannelStatus.ERROR;
                Console.WriteLine("Cannot read row from tester. Please check cable connection.");
                return;
            }
            stdRow = row as StandardRow;
            //var startPoint = stdRow.TimeInMS % 1000;
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
                stdRow = row as StandardRow;
            }
            while (stdRow.TimeInMS < (1000 + LastTimeInMS) && stdRow.Status == RowStatus.RUNNING);

            LastTimeInMS = stdRow.TimeInMS / 1000 * 1000;
            double temperature;
            ret = Tester.Executor.ReadTemperarture(Index, out temperature);
            if (!ret)
            {
                Reset();
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
            var strRow = stdRow.ToString();
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
                Reset();
                Console.WriteLine("Channel Event Error");
                return;
            }
            if (CurrentStep.Action.Mode == ActionMode.CC_DISCHARGE)
                if (stdRow.Current - CurrentStep.Action.Current > StepTolerance.Current)
                {
                    Reset();
                    Status = ChannelStatus.ERROR;
                    Console.WriteLine("Current out of range.");
                    return;
                }
            #endregion

            #region change step
            if (stdRow.Status != RowStatus.RUNNING)
            {
                CurrentStep = GetNewTargetStep(CurrentStep, StepsForOneTempPoint, TargetTemperature, stdRow);
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
        public SmartTesterStep GetNewTargetStep(SmartTesterStep currentStep, List<SmartTesterStep> fullSteps, double temperature, IRow row)
        {
            Console.WriteLine("GetNewTargetStep");
            SmartTesterStep nextStep = null;
            CutOffBehavior cob = GetCutOffBehavior(currentStep, row);
            if (cob != null)
                nextStep = Utilities.Jump(cob, fullSteps, currentStep.Index, row);
            else
            {
                Console.WriteLine("GetCutOffBehavior return null");
                Console.WriteLine($"Index:{currentStep.Index}, Action:{currentStep.Action.Mode.ToString()}");
            }
            return nextStep;
        }
        public CutOffBehavior GetCutOffBehavior(SmartTesterStep currentStep, IRow row) //如果没有符合条件的cob，则return null
        {
            var timeSpan = row.TimeInMS;
            CutOffBehavior cob = null;
            switch (currentStep.Action.Mode)
            {
                case ActionMode.REST:// "StepFinishByCut_V":
                    cob = currentStep.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME);
                    if (cob != null)
                    {
                        var time = cob.Condition.Value;
                        Console.WriteLine($"time = {time}");
                        Console.WriteLine($"timeSpan = {timeSpan}");
                        if (Math.Abs(timeSpan / 1000 - time) < Tolerance.Time)
                        {
                            Console.WriteLine($"Meet time condition.");
                            break;
                        }
                        else
                            cob = null;
                    }
                    break;
                case ActionMode.CC_CV_CHARGE://"StepFinishByCut_I":
                    cob = currentStep.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME);
                    if (cob != null)
                    {
                        var time = cob.Condition.Value;
                        Console.WriteLine($"time = {time}");
                        Console.WriteLine($"timeSpan = {timeSpan}");
                        if (Math.Abs(timeSpan / 1000 - time) < Tolerance.Time)
                        {
                            Console.WriteLine($"Meet time condition.");
                            break;
                        }
                        else
                            cob = null;
                    }
                    cob = currentStep.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.CURRENT);
                    if (cob != null)
                    {
                        var curr = cob.Condition.Value;
                        if (curr >= row.Current)
                        {
                            Console.WriteLine($"Meet current condition.");
                            break;
                        }
                        else
                            cob = null;
                    }
                    break;
                case ActionMode.CC_DISCHARGE://"StepFinishByCut_T":
                case ActionMode.CP_DISCHARGE:
                    cob = currentStep.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME);
                    if (cob != null)
                    {
                        var time = cob.Condition.Value;
                        Console.WriteLine($"time = {time}");
                        Console.WriteLine($"timeSpan = {timeSpan}");
                        if (Math.Abs(timeSpan / 1000 - time) < Tolerance.Time)
                        {
                            Console.WriteLine($"Meet time condition.");
                            break;
                        }
                        else
                            cob = null;
                    }
                    cob = currentStep.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.VOLTAGE);
                    if (cob != null)
                    {
                        var volt = cob.Condition.Value;
                        Console.WriteLine($"volt = {volt}");
                        Console.WriteLine($"row.Voltage = {row.Voltage}");
                        if (Math.Abs(row.Voltage - volt) < Tolerance.Voltage)
                        {
                            Console.WriteLine($"Meet voltage condition.");
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"Doesn't meet voltage condition.");
                            cob = null;
                        }
                    }
                    break;
            }
            return cob;
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