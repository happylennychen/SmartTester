using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTester
{
    public static class Utilities
    {
        public static void FileConvert(string filePath)
        {
            uint indexOffset = 0;
            uint timeOffset = 0;
            double lastCapacity = 0;
            double capacityOffset = 0;
            double totalCapacityOffset = 0;
            bool isNewStep = false;
            ActionMode lastMode = ActionMode.REST;
            using (FileStream rawFile = new FileStream(filePath, FileMode.Open))
            {
                using (StreamReader rawReader = new StreamReader(rawFile))
                {
                    using (FileStream stdFile = new FileStream(Path.ChangeExtension(filePath, "csv"), FileMode.Create))
                    {
                        using (StreamWriter stdWriter = new StreamWriter(stdFile))
                        {
                            stdWriter.WriteLine("Index,Time(mS),Mode,Current(mA),Voltage(mV),Temperature(degC),Capacity(mAh),Total Capacity(mAh),Status");
                            while (rawReader.Peek() != -1)
                            {
                                var line = rawReader.ReadLine();
                                StandardRow stdRow = new StandardRow(line);
                                if (isNewStep)
                                {
                                    if (lastMode == stdRow.Mode)
                                        capacityOffset = lastCapacity;
                                }
                                stdRow.Index = ++indexOffset;
                                stdRow.TimeInMS += timeOffset;
                                stdRow.Capacity += capacityOffset;
                                stdRow.TotalCapacity = stdRow.Capacity + totalCapacityOffset;
                                stdWriter.WriteLine(stdRow.ToString());
                                if (stdRow.Status == RowStatus.STOP)
                                {
                                    timeOffset = stdRow.TimeInMS;
                                    lastMode = stdRow.Mode;
                                    lastCapacity = stdRow.Capacity;
                                    totalCapacityOffset = stdRow.TotalCapacity;
                                    isNewStep = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static void CreateOutputFolder()
        {
            string outputFolder = Path.Combine(GlobalSettings.OutputFolder, GlobalSettings.RoundIndex.ToString());
            Directory.CreateDirectory(outputFolder);
        }

        public static void FileConvert(List<string> filePaths, List<Step> fullSteps, double targetTemperature)
        {
            uint indexOffset = 0;
            uint timeOffset = 0;
            double capacityOffset = 0;
            double totalCapacityOffset = 0;
            uint lastTimeInMS = 0;
            StandardRow lastRow = null;
            StandardRow currentRow = null;
            var newFilePath = Path.ChangeExtension(filePaths[0], "csv");
            Step step = fullSteps.First();
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
                                        currentRow = new StandardRow(line);
                                        if (lastRow == null)
                                            continue;

                                        if (lastRow.Status != RowStatus.RUNNING)
                                        {
                                            CutOffBehavior cob = GetCutOffBehavior(step, lastRow.TimeInMS, lastRow);
                                            step = GetNewTargetStep(step, fullSteps, targetTemperature, lastRow.TimeInMS, lastRow);
                                            UpdateRowStatus(ref lastRow, cob, targetTemperature);
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
                                            if (capacityShouldContinue(lastRow, currentRow))
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
                        }
                    }
                    //处理最后一行数据
                    currentRow.Index = ++indexOffset;
                    currentRow.TimeInMS += timeOffset;
                    currentRow.Capacity += capacityOffset;
                    currentRow.TotalCapacity = currentRow.Capacity + totalCapacityOffset;
                    UpdateRowStatus(ref currentRow, GetCutOffBehavior(step, currentRow.TimeInMS, currentRow), targetTemperature);
                    stdWriter.WriteLine(currentRow.ToString());
                }
            }
            string newFileFullPath = GetNewFileFullPath(newFilePath, lastTimeInMS);
            if (newFileFullPath != null)
                File.Move(newFilePath, newFileFullPath);
        }

        private static void UpdateRowStatus(ref StandardRow row, CutOffBehavior cob, double targetTemperature)
        {
            if (cob != null)
                switch (cob.Condition.Parameter)
                {
                    case Parameter.CURRENT:
                        row.Status = RowStatus.CUT_OFF_BY_CURRENT;
                        break;
                    case Parameter.POWER:
                        row.Status = RowStatus.CUT_OFF_BY_POWER;
                        break;
                    case Parameter.TEMPERATURE:
                        row.Status = RowStatus.CUT_OFF_BY_TEMPERATURE;
                        break;
                    case Parameter.TIME:
                        row.Status = RowStatus.CUT_OFF_BY_TIME;
                        break;
                    case Parameter.VOLTAGE:
                        row.Status = RowStatus.CUT_OFF_BY_VOLTAGE;
                        break;
                    default:
                        row.Status = RowStatus.UNKNOWN;
                        break;
                }
        }

        private static bool capacityShouldContinue(StandardRow lastRow, StandardRow stdRow)
        {
            return lastRow.Mode == stdRow.Mode;
        }

        public static string GetNewFileFullPath(string newFilePath, uint lastTimeInMS)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(newFilePath);
            var directory = Path.GetDirectoryName(newFilePath);
            var startTimeInString = fileNameWithoutExtension.Split('-').Last();
            DateTime startTime;
            if (DateTime.TryParseExact(startTimeInString, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out startTime))
            {
                var duration = TimeSpan.FromMilliseconds(lastTimeInMS);
                var endTimeInString = (startTime + duration).ToString("yyyyMMddHHmmss");
                return directory + "\\" + fileNameWithoutExtension + "-" + endTimeInString + ".csv";
                //File.Move(newFilePath, fileNameWithoutExtension + "-" + endTimeInString + ".csv");
            }
            else
                return null;
        }

        public static Step GetNewTargetStep(Step currentStep, List<Step> fullSteps, double temperature, uint timeSpan, StandardRow row)
        {
            Console.WriteLine("GetNewTargetStep");
            Step nextStep = null;
            CutOffBehavior cob = GetCutOffBehavior(currentStep, timeSpan, row);
            if (cob != null)
                nextStep = Jump(cob, fullSteps, currentStep.Index, row);
            return nextStep;
        }
        private static CutOffBehavior GetCutOffBehavior(Step currentStep, uint timeSpan, StandardRow row)
        {
            CutOffBehavior cob = null;
            switch (currentStep.Action.Mode)
            {
                case ActionMode.REST:// "StepFinishByCut_V":
                    cob = currentStep.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME);
                    break;
                case ActionMode.CC_CV_CHARGE://"StepFinishByCut_I":
                    cob = currentStep.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME);
                    if (cob != null)
                    {
                        var time = cob.Condition.Value;
                        Console.WriteLine($"time = {time}");
                        Console.WriteLine($"timeSpan = {timeSpan}");
                        if (Math.Abs(timeSpan / 1000 - time) < 1)
                        {
                            Console.WriteLine($"Meet time condition.");
                            break;
                        }
                        else
                            cob = null;
                    }
                    cob = currentStep.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.CURRENT);
                    break;
                case ActionMode.CC_DISCHARGE://"StepFinishByCut_T":
                case ActionMode.CP_DISCHARGE:
                    cob = currentStep.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME);
                    if (cob != null)
                    {
                        var time = cob.Condition.Value;
                        Console.WriteLine($"time = {time}");
                        Console.WriteLine($"timeSpan = {timeSpan}");
                        if (Math.Abs(timeSpan / 1000 - time) < 1)
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
                        if (Math.Abs(row.Voltage - volt) < 15)
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
        private static Step Jump(CutOffBehavior cob, List<Step> fullSteps, int currentStepIndex, StandardRow row)
        {
            Step nextStep = null;
            if (cob.JumpBehaviors.Count == 1)
            {
                var jpb = cob.JumpBehaviors.First();
                switch (jpb.JumpType)
                {
                    case JumpType.INDEX:
                        nextStep = fullSteps.SingleOrDefault(o => o.Index == jpb.Index);
                        break;
                    case JumpType.END:
                        break;
                    case JumpType.NEXT:
                        nextStep = fullSteps.SingleOrDefault(o => o.Index == currentStepIndex + 1);
                        break;
                    case JumpType.LOOP:
                        throw new NotImplementedException();
                }
            }
            else if (cob.JumpBehaviors.Count > 1)
            {
                JumpBehavior validJPB = null;
                foreach (var jpb in cob.JumpBehaviors)
                {
                    bool isConditionMet = false;
                    double leftvalue = 0;
                    double rightvalue = jpb.Condition.Value;
                    switch (jpb.Condition.Parameter)
                    {
                        case Parameter.CURRENT: leftvalue = row.Current; break;
                        case Parameter.POWER: leftvalue = row.Current * row.Voltage; break;
                        case Parameter.TEMPERATURE: leftvalue = row.Temperature; break;
                        case Parameter.TIME: leftvalue = row.TimeInMS / 1000; break;
                        case Parameter.VOLTAGE: leftvalue = row.Voltage; break;
                    }
                    switch (jpb.Condition.Mark)
                    {
                        case CompareMarkEnum.EqualTo:
                            if (leftvalue == rightvalue)
                                isConditionMet = true;
                            break;
                        case CompareMarkEnum.LargerThan:
                            if (leftvalue > rightvalue)
                                isConditionMet = true;
                            break;
                        case CompareMarkEnum.SmallerThan:
                            if (leftvalue < rightvalue)
                                isConditionMet = true;
                            break;
                    }
                    if (isConditionMet)
                    {
                        validJPB = jpb;
                        break;
                    }
                }
                if (validJPB != null)
                {
                    switch (validJPB.JumpType)
                    {
                        case JumpType.INDEX:
                            nextStep = fullSteps.SingleOrDefault(o => o.Index == validJPB.Index);
                            break;
                        case JumpType.END:
                            break;
                        case JumpType.NEXT:
                            nextStep = fullSteps.SingleOrDefault(o => o.Index == currentStepIndex + 1);
                            break;
                        case JumpType.LOOP:
                            throw new NotImplementedException();
                    }
                }
            }
            return nextStep;
        }


        public static List<Test> LoadTestFromFile(string folderPath)
        {
            List<Test> output = new List<Test>();
            var files = Directory.GetFiles(folderPath, "*.testplan");
            foreach (var file in files)
            {
                string json = File.ReadAllText(file);
                var test = JsonConvert.DeserializeObject<Test>(json);
                output.Add(test);
            }
            return output;
        }
    }
}
