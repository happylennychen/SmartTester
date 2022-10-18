﻿using Newtonsoft.Json;
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

        public static bool CreateTestPlanFolders(string projectName, Dictionary<IChamber, Dictionary<int, List<IChannel>>> testPlanFolderTree)
        {
            string projectPath = Path.Combine(GlobalSettings.TestPlanFolderPath, projectName);
            if (Directory.Exists(projectName))
            {
                Console.WriteLine($"Error! {projectName} already existed.");
                return false;
            }
            try
            {
                Directory.CreateDirectory(projectPath);
                foreach (var chamber in testPlanFolderTree.Keys)
                {
                    string chamberPath = Path.Combine(projectPath, chamber.Name);
                    Directory.CreateDirectory(chamberPath);
                    foreach (var roundIndex in testPlanFolderTree[chamber].Keys)
                    {
                        string roundPath = Path.Combine(chamberPath, roundIndex.ToString());
                        Directory.CreateDirectory(roundPath);
                        var testers = testPlanFolderTree[chamber][roundIndex].Select(ch => ch.Tester).Distinct().ToList();
                        foreach (var tester in testers)
                        {
                            string testerPath = Path.Combine(roundPath, tester.Name);
                            Directory.CreateDirectory(testerPath);
                            var channels = testPlanFolderTree[chamber][roundIndex].Where(ch => ch.Tester == tester).ToList();
                            foreach (var channel in channels)
                            {
                                string channelPath = Path.Combine(testerPath, channel.Index.ToString());
                                Directory.CreateDirectory(channelPath);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error! {e.Message}");
                return false;
            }
            return true;
        }

        internal static void CreateOutputFolder(IChamber chamber)
        {
            string outputFolder = Path.Combine(GlobalSettings.OutputFolder, GlobalSettings.ChamberRoundIndex[chamber].ToString());
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
                                            if (ShouldCapacityContinue(lastRow, currentRow))
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

        private static bool ShouldCapacityContinue(StandardRow lastRow, StandardRow stdRow)
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


        public static bool LoadTestFromFolder(string folderPath, List<IChamber> chambers, List<ITester> testers, out List<Test> output)
        {
            output = new List<Test>();
            try
            {
                var files = Directory.GetFiles(folderPath, "*.testplan");
                IChannel channel = GetChannelFromFolderPath(folderPath, testers);
                IChamber chamber = GetChamberFromFolderPath(folderPath, chambers);
                foreach (var file in files)
                {
                    string json = File.ReadAllText(file);
                    var test = JsonConvert.DeserializeObject<Test>(json);
                    test.Chamber = chamber;
                    test.Channel = channel;
                    output.Add(test);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error! {e.Message}");
                return false;
            }
            return true;
        }

        private static IChamber GetChamberFromFolderPath(string folderPath, List<IChamber> chambers)
        {
            var path = folderPath.Replace(GlobalSettings.TestPlanFolderPath, string.Empty);
            var chamberName = path.Split('\\')[1];
            var chamber = chambers.SingleOrDefault(cmb => cmb.Name == chamberName);
            return chamber;

        }

        private static IChannel GetChannelFromFolderPath(string folderPath, List<ITester> testers)
        {
            var path = folderPath.Replace(GlobalSettings.TestPlanFolderPath, string.Empty);
            var testerName = path.Split('\\')[3];
            var channelIndex = Convert.ToInt32(path.Split('\\')[4]);
            var tester = testers.SingleOrDefault(tst => tst.Name == testerName);
            var channel = tester.Channels.SingleOrDefault(ch => ch.Index == channelIndex);
            return channel;
        }

        public static bool ChamberGroupTestCheck(List<Test> tests)
        {
            if (tests.GroupBy(t => t.DischargeTemperature).Count() != 1)
            {
                Console.WriteLine("Error. No unified discharge temperature.");
                return false;
            }
            if (tests.GroupBy(t => t.Channel).Where(g => g.Count() > 1).Count() > 0)
            {
                Console.WriteLine("Error. Multiple tests used same channel(s).");
                return false;
            }
            return true;
        }

        public static string GetTestPlanOneRoundFolderPath(string projectName, IChamber chamber, int index)
        {
            string folderPath = Path.Combine(GlobalSettings.TestPlanFolderPath, projectName, chamber.Name, index.ToString());
            return folderPath;
        }

        public static string GetTestPlanProjectFolderPath(string projectName)
        {
            string folderPath = Path.Combine(GlobalSettings.TestPlanFolderPath, projectName);
            return folderPath;
        }

        public static string GetTestPlanChamberFolderPath(string projectName, IChamber chamber)
        {
            string folderPath = Path.Combine(GlobalSettings.TestPlanFolderPath, projectName, chamber.Name);
            return folderPath;
        }
        public static bool LoadTestsForOneRound(string projectName, List<IChamber> chambers, List<ITester> testers, IChamber chamber, int index, out List<Test> tests)
        {
            tests = new List<Test>();
            string oneRoundFolderPath = GetTestPlanOneRoundFolderPath(projectName, chamber, index);
            foreach (var testerFolderPath in Directory.EnumerateDirectories(oneRoundFolderPath))
            {
                ITester tester = GetTesterFromFolderPath(testerFolderPath, testers);
                if (tester == null)
                {
                    Console.WriteLine($"There's no available tester in {testerFolderPath}");
                    return false;
                }
                foreach (var channelFolderPath in Directory.EnumerateDirectories(testerFolderPath))
                {
                    IChannel channel = GetChannelFromFolderPath(channelFolderPath, tester);
                    if (channel == null)
                    {
                        Console.WriteLine($"There's no available channel in {channelFolderPath}");
                        return false;
                    }
                    Test test;
                    if (!LoadTestFromFolder(channelFolderPath, chamber, tester, channel, out test))
                        return false;
                    tests.Add(test);
                }
            }
            return true;
            //if (Directory.Exists(folderPath))
            //{
            //    if (!LoadTestFromFolder(folderPath, chambers, testers, out tests))
            //        return false;
            //    return true;
            //}
            //else
            //    return false;
        }

        private static bool LoadTestFromFolder(string channelFolderPath, IChamber chamber, ITester tester, IChannel channel, out Test test)
        {
            test = null;
            try
            {
                var files = Directory.GetFiles(channelFolderPath, "*.testplan");
                if (files.Count() != 1)
                    return false;
                string json = File.ReadAllText(files[0]);
                test = JsonConvert.DeserializeObject<Test>(json);
                test.Chamber = chamber;
                test.Channel = channel;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error! {e.Message}");
                return false;
            }
            return true;
        }

        private static IChannel GetChannelFromFolderPath(string channelFolderPath, ITester tester)
        {
            var channelIndex = Convert.ToInt32(channelFolderPath.Replace(GlobalSettings.TestPlanFolderPath, string.Empty).Split('\\')[4]);
            return tester.Channels.SingleOrDefault(ch => ch.Index == channelIndex);
        }

        private static ITester GetTesterFromFolderPath(string testerFolderPath, List<ITester> testers)
        {
            var testerName = testerFolderPath.Replace(GlobalSettings.TestPlanFolderPath, string.Empty).Split('\\')[3];
            return testers.SingleOrDefault(tst => tst.Name == testerName);
        }

        public static bool TestPlanFullCheck(string projectName, List<IChamber> chambers, List<ITester> testers)
        {
            string root = GlobalSettings.TestPlanFolderPath;
            bool ret = true;
            //Console.WriteLine("Test Plan pre-check.");
            //foreach (var chamber in chambers)
            //{
            //    var roundIndex = GlobalSettings.ChamberRoundIndex[chamber];
            //    while (true)
            //    {
            //        string folderPath = GetTestPlanOneRoundFolderPath(projectName, chamber, roundIndex);
            //        if (Directory.Exists(folderPath))
            //        {
            //            List<Test> tests;
            //            if (!LoadTestFromFolder(folderPath, chambers, testers, out tests))
            //                return false;
            //            if (!Utilities.ChamberGroupTestCheck(tests))
            //            {
            //                Console.WriteLine($"Round {roundIndex} failed!");
            //                ret &= false;
            //            }
            //            else
            //                Console.WriteLine($"Round {roundIndex} pass!");
            //            roundIndex++;
            //        }
            //        else
            //        {
            //            if (roundIndex == 1)
            //            {
            //                Console.WriteLine($"There's no test plan, please check.");
            //                ret = false;
            //                break;
            //            }
            //            else
            //            {
            //                Console.WriteLine($"All rounds test plan check finished.");
            //                break;
            //            }
            //        }
            //    }
            //}
            return ret;
        }
        public static bool SaveConfiguration(List<DebugChamber> chambers, List<DebugTester> testers)
        {
            try
            {
                Configuration conf = new Configuration(chambers, testers);
                //string jsonString = System.Text.Json.JsonSerializer.Serialize(conf);
                string jsonString = JsonConvert.SerializeObject(conf, Formatting.Indented);
                File.WriteAllText(GlobalSettings.ConfigurationFilePath, jsonString);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error! Create Configuration Failed!");
                return false;
            }
            return true;
        }

        public static bool LoadConfiguration(out Configuration conf)
        {
            conf = null;
            try
            {
                string jsonString = File.ReadAllText(GlobalSettings.ConfigurationFilePath);
                conf = JsonConvert.DeserializeObject<Configuration>(jsonString);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error! Load Configuration Failed! {e.Message}");
                return false;
            }
            return true;
        }

        public static bool CreateOutputFolderRoot()
        {
            try
            {
                GlobalSettings.OutputFolder = Path.Combine("Output",DateTime.Now.ToString("yyyyMMddHHmmss"));
                Directory.CreateDirectory(GlobalSettings.OutputFolder);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error! " + e.Message);
                return false;
            }
            return true;
        }

        public static bool CreateConsoleFolder()
        {
            try
            {
                Directory.CreateDirectory(GlobalSettings.ConsoleFolderPath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error! " + e.Message);
                return false;
            }
            return true;
        }
    }
}