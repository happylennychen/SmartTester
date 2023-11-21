﻿//#define debug
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartTesterLib
{
    public static class Utilities
    {
        /*
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
        */

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

        //internal static void CreateOutputFolder(IChamber chamber)
        //{
        //    string outputFolder = Path.Combine(GlobalSettings.OutputFolder, chamber.Name, "R" + GlobalSettings.ChamberRoundIndex[chamber].ToString());
        //    Directory.CreateDirectory(outputFolder);
        //}


        //private static RowStatus UpdateRowStatus(RowBase row, CutOffBehavior cob, double targetTemperature)
        public static RowStatus UpdateLastRowStatus(CutOffBehavior cob)    //根据cob来推出RowStatus，条件是找到了正确的cob
        {
            RowStatus rs = RowStatus.UNKNOWN;
            if (cob != null)
                switch (cob.Condition.Parameter)
                {
                    case Parameter.CURRENT:
                        rs = RowStatus.CUT_OFF_BY_CURRENT;
                        break;
                    case Parameter.POWER:
                        rs = RowStatus.CUT_OFF_BY_POWER;
                        break;
                    case Parameter.TEMPERATURE:
                        rs = RowStatus.CUT_OFF_BY_TEMPERATURE;
                        break;
                    case Parameter.TIME:
                        rs = RowStatus.CUT_OFF_BY_TIME;
                        break;
                    case Parameter.VOLTAGE:
                        rs = RowStatus.CUT_OFF_BY_VOLTAGE;
                        break;
                    default:
                        rs = RowStatus.UNKNOWN;
                        break;
                }
            return rs;
        }

        public static bool ShouldCapacityContinue(StandardRow lastRow, StandardRow stdRow)
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
        public static SmartTesterStep Jump(CutOffBehavior cob, List<SmartTesterStep> fullSteps, int currentStepIndex, IRow row)
        {
            SmartTesterStep nextStep = null;
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


        public static bool LoadTestFromFolder(string folderPath, List<IChamber> chambers, List<ITester> testers, out List<SmartTesterRecipe> output)
        {
            output = new List<SmartTesterRecipe>();
            try
            {
                var files = Directory.GetFiles(folderPath, "*.testplan");
                IChannel channel = GetChannelFromFolderPath(folderPath, testers);
                IChamber chamber = GetChamberFromFolderPath(folderPath, chambers);
                foreach (var file in files)
                {
                    string json = File.ReadAllText(file);
                    //var test = JsonConvert.DeserializeObject<SmartTesterRecipe>(json);
                    //test.Chamber = chamber;
                    //test.Channel = channel;
                    //output.Add(test);
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

        public static string GetTestPlanOneRoundFolderPath(string projectName, IChamber chamber, int index)
        {
            string folderPath = Path.Combine(GlobalSettings.TestPlanFolderPath, projectName, chamber.Name, "R" + index.ToString());
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
        public static bool LoadTestsForOneRound(string projectName, List<IChamber> chambers, List<ITester> testers, IChamber chamber, int index, out List<SmartTesterRecipe> tests)
        {
            bool ret = false;
            tests = new List<SmartTesterRecipe>();
            string oneRoundFolderPath = GetTestPlanOneRoundFolderPath(projectName, chamber, index);
            if (!Directory.Exists(oneRoundFolderPath))
                return false;
            //foreach (var testerFolderPath in Directory.EnumerateDirectories(oneRoundFolderPath))
            var folders = Directory.EnumerateDirectories(oneRoundFolderPath);
            if (folders.Count() == 0)
                return false;
            foreach (var testerFolderPath in folders)
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
                    SmartTesterRecipe test;
                    //if (!LoadTestFromFolder(channelFolderPath, chamber, tester, channel, out test))
                    //return false;
                    //tests.Add(test);
                    var ret1 = LoadTestFromFolder(channelFolderPath, chamber, tester, channel, out test);
                    if (ret1)
                        tests.Add(test);
                    ret |= ret1;
                }
            }
            return ret;
            //if (Directory.Exists(folderPath))
            //{
            //    if (!LoadTestFromFolder(folderPath, chambers, testers, out tests))
            //        return false;
            //    return true;
            //}
            //else
            //    return false;
        }

        private static bool LoadTestFromFolder(string channelFolderPath, IChamber chamber, ITester tester, IChannel channel, out SmartTesterRecipe test)
        {
            test = null;
            try
            {
                var files = Directory.GetFiles(channelFolderPath, "*.testplan");
                if (files.Count() != 1)
                    return false;
                test = LoadRecipeFromFile(files[0]);
                //test.Chamber = chamber;
                //test.Channel = channel;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error! {e.Message}");
                return false;
            }
            return true;
        }

        public static SmartTesterRecipe LoadRecipeFromFile(string filePath)
        {
            string json = File.ReadAllText(filePath);
            string recipeName = GetRecipeNameFromFileName(Path.GetFileNameWithoutExtension(filePath));
            int dischargeTemp = GetDischargeTemperatureFromFileName(Path.GetFileNameWithoutExtension(filePath));
            var rec = JsonConvert.DeserializeObject<SmartTesterRecipe>(json);
            UpdateRecipe(ref rec, dischargeTemp, recipeName);
            return rec;
        }

        private static string GetRecipeNameFromFileName(string fileName)
        {
            return fileName.Replace(fileName.Split('-')[0], "").Trim('-');
        }

        private static int GetDischargeTemperatureFromFileName(string fileName)
        {
            return Convert.ToInt32(fileName.Split('-')[0].Replace("Deg", ""));
        }

        private static void UpdateRecipe(ref SmartTesterRecipe rec, int dischargeTemp, string recipeName)
        {
            rec.Name = recipeName;
            foreach (var step in rec.Steps)
            {
                var t = new TemperatureTarget();
                if (step.Action.Mode == ActionMode.CC_CV_CHARGE)
                {
                    t.IsCritical = true;
                    t.Value = 25;
                }
                else
                {
                    t.IsCritical = false;
                    t.Value = dischargeTemp;
                }
                step.Target = t;
            }
        }

        private static IChannel GetChannelFromFolderPath(string channelFolderPath, ITester tester)
        {
            var channelIndex = Convert.ToInt32(channelFolderPath.Replace(GlobalSettings.TestPlanFolderPath, string.Empty).Split('\\')[4].Replace("CH", ""));
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

        public static bool LoadConfiguration(out Configuration conf)
        {
            conf = new Configuration();
            //try
            //{
            //    string jsonString = File.ReadAllText(GlobalSettings.ConfigurationFilePath);
            //    conf = JsonConvert.DeserializeObject<Configuration>(jsonString);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine($"Error! Load Configuration Failed! {e.Message}");
            //    return false;
            //}
            try
            {
                string jsonString = File.ReadAllText(GlobalSettings.ConfigurationFilePath);
                using (JsonDocument document = JsonDocument.Parse(jsonString))
                {
                    JsonElement root = document.RootElement;
                    JsonElement testersElement = root.GetProperty("Testers");
                    foreach (JsonElement testerElement in testersElement.EnumerateArray())
                    {
                        ITester tester = null;
                        var className = testerElement.GetProperty("Class").GetString();
                        var id = testerElement.GetProperty("Id").GetInt32();
                        var name = testerElement.GetProperty("Name").GetString();
                        var channelNumber = testerElement.GetProperty("ChannelNumber").GetInt32();
                        var ipAddress = testerElement.GetProperty("IpAddress").GetString();
                        var port = testerElement.GetProperty("Port").GetInt32();
                        var sessionStr = testerElement.GetProperty("SessionStr").GetString();
                        switch (className)
                        {
                            case "Chroma17208":
                                tester = new Chroma17208(id, name!, channelNumber, ipAddress!, port, sessionStr!);
                                break;
                            case "DebugTester":
                                tester = new DebugTester(id, name!, channelNumber, ipAddress!, port, sessionStr!);
                                break;
                            case "PackTester":
                                tester = new PackTester(id, name!, channelNumber, ipAddress!, port, sessionStr!);
                                break;
                            default:
                                break;
                        }
                        if (tester != null)
                            conf.Testers.Add(tester);
                    }
                    JsonElement chambersElement = root.GetProperty("Chambers");
                    foreach (JsonElement chamberElement in chambersElement.EnumerateArray())
                    {
                        IChamber chamber = null;
                        var className = chamberElement.GetProperty("Class").GetString();
                        var id = chamberElement.GetProperty("Id").GetInt32();
                        var manufacturer = chamberElement.GetProperty("Manufacturer").GetString();
                        var name = chamberElement.GetProperty("Name").GetString();
                        var lowestTemperature = chamberElement.GetProperty("LowestTemperature").GetDouble();
                        var highestTemperature = chamberElement.GetProperty("HighestTemperature").GetDouble();
                        var ipAddress = chamberElement.GetProperty("IpAddress").GetString();
                        var port = chamberElement.GetProperty("Port").GetInt32();
                        switch (className)
                        {
                            case "Chamber":
                                chamber = new Chamber(id, manufacturer!, name!, highestTemperature, lowestTemperature, ipAddress!, port);
                                break;
                            case "DebugChamber":
                                chamber = new DebugChamber(id, manufacturer!, name!, highestTemperature, lowestTemperature);
                                break;
                            default:
                                break;
                        }
                        if (chamber != null)
                        {
                            conf.Chambers.Add(chamber);
                        }
                    }
                }
            }
            catch(Exception e) 
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        public static bool CreateOutputFolderRoot()
        {
            try
            {
                GlobalSettings.OutputFolder = Path.Combine("Output", DateTime.Now.ToString("yyyyMMddHHmmss"));
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
    public static class Tolerance
    {
        public const int Voltage = 15;
        public const int Current = 2;
        public const int Power = 1;
        public const int Temperature = 1;
        public const int Time = 1;
    }
}
