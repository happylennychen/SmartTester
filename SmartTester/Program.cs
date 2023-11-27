﻿//#define UseFileInsteadOfConsole
//#define debug
#define shell
using SmartTesterLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using static System.Threading.Thread;

namespace SmartTester
{
    class Program//调用automator提供的API，提供基本的操作界面。后期可以用图形界面替代。
    {
        [STAThread]
        static void Main(string[] args)
        {
#if UseFileInsteadOfConsole
            Utilities.CreateConsoleFolder();
            string consoleOuputFile = Path.Combine(GlobalSettings.ConsoleFolderPath, $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt");
            FileStream fs = new FileStream(consoleOuputFile, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.AutoFlush = true;
            var tempOut = Console.Out;
            Console.SetOut(sw);
#endif
#if shell
            string testPlanFolder = @"D:\O2Micro\Source Codes\BC Lab\ST\SmartTester\SmartTester\bin\Debug\net6.0\Test Plan\";
            List<string> recipeFiles = Directory.EnumerateFiles(testPlanFolder, "*.testplan").ToList();
            Utilities.CreateOutputFolderRoot();
            Automator amtr = new Automator();
            Monitor monitor = new Monitor(amtr);
            //TestPlanScheduler scheduler = new TestPlanScheduler();
            if (!amtr.InitHW())
                return;
            bool bQuit = false;
            while (!bQuit)
            {
                Console.WriteLine("1. Setup Chambers. 2.Configure Test Rounds 3. Run 4. Stop 5. Start Monitor 6. Stop Monitor 7.Quit");
                var key = Console.ReadLine();
                switch (key.Trim())
                {
                    case "1":
                        SetupChambers(amtr);
                        break;
                    case "2":
                        ConfigureTestRounds(amtr.Chambers, recipeFiles);
                        break;
                    case "3":
                        Task task = amtr.AsyncStartChambers();
                        //task.Wait();
                        break;
                    case "4": break;
                    case "5":
                        monitor.Run();
                        break;
                    case "6":
                        monitor.Stop();
                        break;
                    case "7":
                        bQuit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid input!");
                        break;
                }
            }
#else
#if debug
            amtr.PutChannelsInChamber(amtr.Testers[0].Channels.Where(ch => ch.Index <= 4), amtr.Chambers[0]);
            amtr.PutChannelsInChamber(amtr.Testers[0].Channels.Where(ch => ch.Index > 4), amtr.Chambers[1]);
            //amtr.PutChannelsInChamber(amtr.Testers[0].Channels.Where(ch => ch.Index == 1), amtr.Chambers[0]);
            #region Chamber1
            var chm1_r1_ch3 = Utilities.LoadRecipeFromFile(@"D:\O2Micro\Source Codes\BC Lab\ST\SmartTester\SmartTester\bin\Debug\net6.0\Test Plan\2Chambers1Tester_2\PUL-80\R1\17208Auto\CH3\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm1_r2_ch1 = Utilities.LoadRecipeFromFile(@"D:\O2Micro\Source Codes\BC Lab\ST\SmartTester\SmartTester\bin\Debug\net6.0\Test Plan\2Chambers1Tester_2\PUL-80\R2\17208Auto\CH1\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm1_r2_ch2 = Utilities.LoadRecipeFromFile(@"D:\O2Micro\Source Codes\BC Lab\ST\SmartTester\SmartTester\bin\Debug\net6.0\Test Plan\2Chambers1Tester_2\PUL-80\R2\17208Auto\CH2\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm1_r2_ch3 = Utilities.LoadRecipeFromFile(@"D:\O2Micro\Source Codes\BC Lab\ST\SmartTester\SmartTester\bin\Debug\net6.0\Test Plan\2Chambers1Tester_2\PUL-80\R2\17208Auto\CH3\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm1_r2_ch4 = Utilities.LoadRecipeFromFile(@"D:\O2Micro\Source Codes\BC Lab\ST\SmartTester\SmartTester\bin\Debug\net6.0\Test Plan\2Chambers1Tester_2\PUL-80\R2\17208Auto\CH4\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");

            Dictionary<IChannel, SmartTesterRecipe> channelRecipesForR1 = new Dictionary<IChannel, SmartTesterRecipe>();
            channelRecipesForR1.Add(amtr.Testers[0].Channels[0], chm1_r1_ch3);

            TestRound round1 = new TestRound(channelRecipesForR1);
            amtr.Chambers[0].TestScheduler.AppendTestRound(round1);

            Dictionary<IChannel, SmartTesterRecipe> channelRecipesForR2 = new Dictionary<IChannel, SmartTesterRecipe>();
            channelRecipesForR2.Add(amtr.Testers[0].Channels[0], chm1_r2_ch1);

            TestRound round2 = new TestRound(channelRecipesForR2);
            amtr.Chambers[0].TestScheduler.AppendTestRound(round2);
            #endregion

            #region Chamber2
            var chm2_r1_ch5 = Utilities.LoadRecipeFromFile(@"D:\O2Micro\Source Codes\BC Lab\ST\SmartTester\SmartTester\bin\Debug\net6.0\Test Plan\2Chambers1Tester_2\PUL-82\R1\17208Auto\CH5\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm2_r1_ch6 = Utilities.LoadRecipeFromFile(@"D:\O2Micro\Source Codes\BC Lab\ST\SmartTester\SmartTester\bin\Debug\net6.0\Test Plan\2Chambers1Tester_2\PUL-82\R1\17208Auto\CH5\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm2_r1_ch7 = Utilities.LoadRecipeFromFile(@"D:\O2Micro\Source Codes\BC Lab\ST\SmartTester\SmartTester\bin\Debug\net6.0\Test Plan\2Chambers1Tester_2\PUL-82\R1\17208Auto\CH5\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm2_r1_ch8 = Utilities.LoadRecipeFromFile(@"D:\O2Micro\Source Codes\BC Lab\ST\SmartTester\SmartTester\bin\Debug\net6.0\Test Plan\2Chambers1Tester_2\PUL-82\R1\17208Auto\CH5\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");

            var chm2_r2_ch5 = Utilities.LoadRecipeFromFile(@"D:\O2Micro\Source Codes\BC Lab\ST\SmartTester\SmartTester\bin\Debug\net6.0\Test Plan\2Chambers1Tester_2\PUL-82\R2\17208Auto\CH5\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm2_r2_ch6 = Utilities.LoadRecipeFromFile(@"D:\O2Micro\Source Codes\BC Lab\ST\SmartTester\SmartTester\bin\Debug\net6.0\Test Plan\2Chambers1Tester_2\PUL-82\R2\17208Auto\CH5\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm2_r2_ch7 = Utilities.LoadRecipeFromFile(@"D:\O2Micro\Source Codes\BC Lab\ST\SmartTester\SmartTester\bin\Debug\net6.0\Test Plan\2Chambers1Tester_2\PUL-82\R2\17208Auto\CH5\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm2_r2_ch8 = Utilities.LoadRecipeFromFile(@"D:\O2Micro\Source Codes\BC Lab\ST\SmartTester\SmartTester\bin\Debug\net6.0\Test Plan\2Chambers1Tester_2\PUL-82\R2\17208Auto\CH5\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");

            var chm2_r3_ch5 = Utilities.LoadRecipeFromFile(@"D:\O2Micro\Source Codes\BC Lab\ST\SmartTester\SmartTester\bin\Debug\net6.0\Test Plan\2Chambers1Tester_2\PUL-82\R3\17208Auto\CH5\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm2_r3_ch6 = Utilities.LoadRecipeFromFile(@"D:\O2Micro\Source Codes\BC Lab\ST\SmartTester\SmartTester\bin\Debug\net6.0\Test Plan\2Chambers1Tester_2\PUL-82\R3\17208Auto\CH5\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm2_r3_ch7 = Utilities.LoadRecipeFromFile(@"D:\O2Micro\Source Codes\BC Lab\ST\SmartTester\SmartTester\bin\Debug\net6.0\Test Plan\2Chambers1Tester_2\PUL-82\R3\17208Auto\CH5\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");
            var chm2_r3_ch8 = Utilities.LoadRecipeFromFile(@"D:\O2Micro\Source Codes\BC Lab\ST\SmartTester\SmartTester\bin\Debug\net6.0\Test Plan\2Chambers1Tester_2\PUL-82\R3\17208Auto\CH5\0Deg-NOZZLE-INSTALL-STANDARD-IDLE-60S.testplan");

            var r1 = new Dictionary<IChannel, SmartTesterRecipe>();
            r1.Add(amtr.Testers[0].Channels[4], chm2_r1_ch5);
            r1.Add(amtr.Testers[0].Channels[5], chm2_r1_ch6);
            r1.Add(amtr.Testers[0].Channels[6], chm2_r1_ch7);
            r1.Add(amtr.Testers[0].Channels[7], chm2_r1_ch8);
            var R1 = new TestRound(r1);
            amtr.Chambers[1].TestScheduler.AppendTestRound(R1);

            var r2 = new Dictionary<IChannel, SmartTesterRecipe>();
            r2.Add(amtr.Testers[0].Channels[4], chm2_r2_ch5);
            r2.Add(amtr.Testers[0].Channels[5], chm2_r2_ch6);
            r2.Add(amtr.Testers[0].Channels[6], chm2_r2_ch7);
            r2.Add(amtr.Testers[0].Channels[7], chm2_r2_ch8);
            var R2 = new TestRound(r2);
            amtr.Chambers[1].TestScheduler.AppendTestRound(R2);

            var r3 = new Dictionary<IChannel, SmartTesterRecipe>();
            r3.Add(amtr.Testers[0].Channels[4], chm2_r3_ch5);
            r3.Add(amtr.Testers[0].Channels[5], chm2_r3_ch6);
            r3.Add(amtr.Testers[0].Channels[6], chm2_r3_ch7);
            r3.Add(amtr.Testers[0].Channels[7], chm2_r3_ch8);
            var R3 = new TestRound(r3);
            amtr.Chambers[1].TestScheduler.AppendTestRound(R3);//*/
            #endregion
#else
            amtr.PutChannelsInChamber(amtr.Testers[0].Channels.Where(ch => ch.Index == 1), amtr.Chambers[0]);
            var chm1_r1_ch3 = Utilities.LoadRecipeFromFile(@"E:\Smart Tester Test\test\25Deg-Charge Test.testplan");
            Dictionary<IChannel, SmartTesterRecipe> channelRecipesForR1 = new Dictionary<IChannel, SmartTesterRecipe>();
            channelRecipesForR1.Add(amtr.Testers[0].Channels[0], chm1_r1_ch3);

            TestRound round1 = new TestRound(channelRecipesForR1);
            amtr.Chambers[0].TestScheduler.AppendTestRound(round1);
#endif

            Task task = amtr.AsyncStartChambers();
            Monitor monitor = new Monitor(amtr);
            monitor.Run();
            //Task task = amtr.AsyncStartChamber(amtr.Chambers[0]);
            //Task task = amtr.AsyncStartOneRound(amtr.Chambers[1], R3);
            task.Wait();
#endif
#if UseFileInsteadOfConsole
            sw.Close();
            fs.Close();
            Console.SetOut(tempOut);
            Utilities.WriteLine($"Demo program completed! Please check {consoleOuputFile} for the details.");
#else
            Utilities.WriteLine($"Demo program completed!");
            Console.ReadLine();
#endif
        }


        private static SmartTesterRecipe SpecifyRecipe(List<string> recipeFiles)
        {
            Console.WriteLine($"There're {recipeFiles.Count} files. Which one do you want to choose?");
            foreach (var recipeFile in recipeFiles)
                Console.WriteLine($"{recipeFiles.IndexOf(recipeFile)},{recipeFile}");
            Console.WriteLine("Please enter the file Index.");
            var fileIndexStr = Console.ReadLine();
            var fileIndex = Convert.ToInt32(fileIndexStr);
            var selectedFile = recipeFiles[fileIndex];
            return Utilities.LoadRecipeFromFile(selectedFile);
        }
        private static List<IChannel> SpecifyChannels(List<ITester> testers)
        {
            List<IChannel> output = new List<IChannel>();
            bool bQuit = false;
            while (!bQuit)
            {
                Console.WriteLine("1. Add channels 2.Quit");
                var key = Console.ReadLine();
                switch (key.Trim())
                {
                    case "1":
                        var tester = SpecifyTester(testers);
                        var channel = SpecifyChannel(tester.Channels);
                        if (!output.Contains(channel))
                            output.Add(channel);
                        break;
                    case "2":
                        bQuit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid input!");
                        break;
                }
            }
            return output;
        }

        private static IChannel SpecifyChannel(List<IChannel> channels)
        {
            Console.WriteLine($"There're {channels.Count} channels. Which one do you want to choose?");
            foreach (var channel in channels)
                Console.WriteLine($"Index:{channel.Index} Name:{channel.Name}");
            Console.WriteLine("Please enter the channel Index.");
            var channelIDStr = Console.ReadLine();
            var channelID = Convert.ToInt32(channelIDStr);
            return channels.SingleOrDefault(t => t.Index == channelID);
        }

        private static ITester SpecifyTester(List<ITester> testers)
        {
            Console.WriteLine($"There're {testers.Count} testers. Which one do you want to choose?");
            foreach (var tester in testers)
                Console.WriteLine($"ID:{tester.Id} Name:{tester.Name}");
            Console.WriteLine("Please enter the tester ID.");
            var testerIDStr = Console.ReadLine();
            var testerID = Convert.ToInt32(testerIDStr);
            return testers.SingleOrDefault(t => t.Id == testerID);
        }

        private static IChamber SpecifyChamber(List<IChamber> chambers)
        {
            Console.WriteLine($"There're {chambers.Count} chambers. Which one do you want to choose?");
            foreach (var chamber in chambers)
                Console.WriteLine($"ID:{chamber.Id} Name:{chamber.Name}");
            Console.WriteLine("Please enter the chamber ID.");
            var chamberIDStr = Console.ReadLine();
            var chamberID = Convert.ToInt32(chamberIDStr);
            return chambers.SingleOrDefault(c => c.Id == chamberID);
        }
        private static void SetupChambers(Automator amtr)
        {
            var selectedChamber = SpecifyChamber(amtr.Chambers);
            var selectedChannels = SpecifyChannels(amtr.Testers);
            amtr.PutChannelsInChamber(selectedChannels, selectedChamber);
            Console.WriteLine($"{selectedChannels.Count} channels has been put into {selectedChamber.ToString()}");
        }

        private static void ConfigureTestRounds(List<IChamber> chambers, List<string> recipeFiles)
        {
            var selectedChamber = SpecifyChamber(chambers);
            //var trl = selectedChamber.TestScheduler.TestRoundList;
            Dictionary<IChannel, SmartTesterRecipe> channelRecipes = new Dictionary<IChannel, SmartTesterRecipe>();

            bool bQuit = false;
            while (!bQuit)
            {
                Console.WriteLine("1. Add channel-recipe 2.Quit");
                var key = Console.ReadLine();
                switch (key.Trim())
                {
                    case "1":
                        var selectedChannel = SpecifyChannel(selectedChamber.Channels);
                        var recipe = SpecifyRecipe(recipeFiles);
                        if (!channelRecipes.ContainsKey(selectedChannel))
                            channelRecipes.Add(selectedChannel, recipe);
                        break;
                    case "2":
                        bQuit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid input!");
                        break;
                }
            }
            TestRound tr = new TestRound(channelRecipes);
            selectedChamber.TestScheduler.AppendTestRound(tr);
        }

        private static bool TestPlanPreCheck()
        {
            string root = GlobalSettings.TestPlanFolderPath;
            bool ret = true;
            //Utilities.WriteLine("Test Plan pre-check.");
            //int roundIndex = 1;
            //while (true)
            //{
            //    var dirs = Directory.GetDirectories(root);
            //    if (dirs.Contains($@"{root}{roundIndex}"))
            //    {
            //        var folderPath = $@"{root}{roundIndex}";

            //        List<Test> tests = Utilities.LoadTestFromFile(folderPath);
            //        foreach (var test in tests)
            //        {
            //            if (test.Chamber.Name == MyChamber.Name)
            //                test.Chamber = MyChamber;
            //            if (test.Channel.Tester.Name == "17208Auto")
            //                test.Channel = MyTester.Channels.SingleOrDefault(ch => ch.Index == GetChannelIndex(test.Channel.Name));
            //        }
            //        var testsGroupedbyChamber = tests.GroupBy(t => t.Chamber);
            //        foreach (var tst in testsGroupedbyChamber)
            //        {
            //            if (!Automator.ChamberGroupTestCheck(tst.ToList()))
            //            {
            //                Utilities.WriteLine($"Round {roundIndex} failed!");
            //                ret &= false;
            //            }
            //            else
            //                Utilities.WriteLine($"Round {roundIndex} pass!");
            //        }
            //        roundIndex++;
            //    }
            //    else
            //    {
            //        if (roundIndex == 1)
            //        {
            //            Utilities.WriteLine($"There's no test plan, please check.");
            //            ret = false;
            //            break;
            //        }
            //        else
            //        {
            //        Utilities.WriteLine($"All rounds test plan check finished.");
            //        break;
            //        }
            //    }
            //}
            return ret;
        }

        private static int GetChannelIndex(string name)
        {
            return int.Parse(System.Text.RegularExpressions.Regex.Replace(name, @"[^0-9]+", ""));
        }

        private static void CreateFullSteps(out List<SmartTesterStep> fullSteps)
        {
            SmartTesterStep chargeStep = new SmartTesterStep() { Index = 1, Action = new TesterAction() { Mode = ActionMode.CC_CV_CHARGE, Voltage = 4200, Current = 1500, Power = 0 } };
            JumpBehavior jpb = new JumpBehavior() { JumpType = JumpType.NEXT };
            Condition cdt = new Condition() { Parameter = Parameter.CURRENT, Mark = CompareMarkEnum.SmallerThan, Value = 150 };
            CutOffBehavior cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            chargeStep.CutOffBehaviors.Add(cob);


            SmartTesterStep idleStep = new SmartTesterStep() { Index = 2, Action = new TesterAction() { Mode = ActionMode.REST, Voltage = 0, Current = 0, Power = 0 } };
            jpb = new JumpBehavior() { JumpType = JumpType.NEXT };
            cdt = new Condition() { Parameter = Parameter.TIME, Mark = CompareMarkEnum.LargerThan, Value = 1800 };
            cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            idleStep.CutOffBehaviors.Add(cob);

            SmartTesterStep cpStep = new SmartTesterStep() { Index = 3, Action = new TesterAction() { Mode = ActionMode.CP_DISCHARGE, Voltage = 0, Current = 0, Power = 6000 } };
            jpb = new JumpBehavior() { JumpType = JumpType.INDEX, Index = 7 };
            cdt = new Condition() { Parameter = Parameter.VOLTAGE, Mark = CompareMarkEnum.SmallerThan, Value = 2500 };
            cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            cpStep.CutOffBehaviors.Add(cob);
            JumpBehavior jpb2 = new JumpBehavior() { JumpType = JumpType.NEXT };
            Condition cdt2 = new Condition() { Parameter = Parameter.TIME, Mark = CompareMarkEnum.LargerThan, Value = 120 };
            CutOffBehavior cob2 = new CutOffBehavior() { Condition = cdt2 };
            cob2.JumpBehaviors.Add(jpb2);
            cpStep.CutOffBehaviors.Add(cob2);


            SmartTesterStep idleStep2 = new SmartTesterStep() { Index = 4, Action = new TesterAction() { Mode = ActionMode.REST, Voltage = 0, Current = 0, Power = 0 } };
            jpb = new JumpBehavior() { JumpType = JumpType.NEXT };
            cdt = new Condition() { Parameter = Parameter.TIME, Mark = CompareMarkEnum.LargerThan, Value = 30 };
            cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            idleStep2.CutOffBehaviors.Add(cob);

            SmartTesterStep cpStep2 = new SmartTesterStep() { Index = 5, Action = new TesterAction() { Mode = ActionMode.CP_DISCHARGE, Voltage = 0, Current = 0, Power = 33000 } };
            jpb = new JumpBehavior() { JumpType = JumpType.INDEX, Index = 7 };
            cdt = new Condition() { Parameter = Parameter.VOLTAGE, Mark = CompareMarkEnum.SmallerThan, Value = 2500 };
            cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            cpStep2.CutOffBehaviors.Add(cob);
            jpb2 = new JumpBehavior() { JumpType = JumpType.NEXT };
            cdt2 = new Condition() { Parameter = Parameter.TIME, Mark = CompareMarkEnum.LargerThan, Value = 120 };
            cob2 = new CutOffBehavior() { Condition = cdt2 };
            cob2.JumpBehaviors.Add(jpb2);
            cpStep2.CutOffBehaviors.Add(cob2);


            SmartTesterStep idleStep3 = new SmartTesterStep() { Index = 6, Action = new TesterAction() { Mode = ActionMode.REST, Voltage = 0, Current = 0, Power = 0 } };
            jpb = new JumpBehavior() { JumpType = JumpType.INDEX, Index = 3 };
            cdt = new Condition() { Parameter = Parameter.TIME, Mark = CompareMarkEnum.LargerThan, Value = 30 };
            cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            idleStep3.CutOffBehaviors.Add(cob);


            SmartTesterStep idleStep4 = new SmartTesterStep() { Index = 7, Action = new TesterAction() { Mode = ActionMode.REST, Voltage = 0, Current = 0, Power = 0 } };
            jpb = new JumpBehavior() { JumpType = JumpType.NEXT };
            cdt = new Condition() { Parameter = Parameter.TIME, Mark = CompareMarkEnum.LargerThan, Value = 1800 };
            cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            idleStep4.CutOffBehaviors.Add(cob);
            fullSteps = new List<SmartTesterStep> { chargeStep, idleStep, cpStep, idleStep2, cpStep2, idleStep3, idleStep4 };
        }
    }
}
