﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Threading.Thread;

namespace SmartTester
{
    class Program
    {
        static void Main(string[] args)
        {
            string consoleOuputFile = $"Console Output {DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";
            FileStream fs = new FileStream(consoleOuputFile, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.AutoFlush = true;
            //var tempOut = Console.Out;
            //Console.SetOut(sw);

            Automator amtr = new Automator();
            if(!amtr.Init())
                return;
            //Dictionary<IChamber, Dictionary<int, List<IChannel>>> testPlanFolderTree = new Dictionary<IChamber, Dictionary<int, List<IChannel>>>();

            //Dictionary<int, List<IChannel>> value = new Dictionary<int, List<IChannel>>();
            //value.Add(1, new List<IChannel>() { amtr.Testers[0].Channels[0], amtr.Testers[0].Channels[1], amtr.Testers[0].Channels[2], amtr.Testers[0].Channels[3] });
            //testPlanFolderTree.Add(amtr.Chambers[0], value);

            //Utilities.CreateTestPlanFolders("Project4", testPlanFolderTree);
            Task task = amtr.AutoRun("Project4");
            task.Wait();

            sw.Close();
            fs.Close();
            //Console.SetOut(tempOut);
            Console.WriteLine($"Demo program completed! Please check {consoleOuputFile} for the details.");
        }

        private static bool TestPlanPreCheck()
        {
            string root = GlobalSettings.TestPlanFolderPath;
            bool ret = true;
            //Console.WriteLine("Test Plan pre-check.");
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
            //                Console.WriteLine($"Round {roundIndex} failed!");
            //                ret &= false;
            //            }
            //            else
            //                Console.WriteLine($"Round {roundIndex} pass!");
            //        }
            //        roundIndex++;
            //    }
            //    else
            //    {
            //        if (roundIndex == 1)
            //        {
            //            Console.WriteLine($"There's no test plan, please check.");
            //            ret = false;
            //            break;
            //        }
            //        else
            //        {
            //        Console.WriteLine($"All rounds test plan check finished.");
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

        private static void CreateFullSteps(out List<Step> fullSteps)
        {
            Step chargeStep = new Step() { Index = 1, Action = new TesterAction() { Mode = ActionMode.CC_CV_CHARGE, Voltage = 4200, Current = 1500, Power = 0 } };
            JumpBehavior jpb = new JumpBehavior() { JumpType = JumpType.NEXT };
            Condition cdt = new Condition() { Parameter = Parameter.CURRENT, Mark = CompareMarkEnum.SmallerThan, Value = 150 };
            CutOffBehavior cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            chargeStep.CutOffBehaviors.Add(cob);


            Step idleStep = new Step() { Index = 2, Action = new TesterAction() { Mode = ActionMode.REST, Voltage = 0, Current = 0, Power = 0 } };
            jpb = new JumpBehavior() { JumpType = JumpType.NEXT };
            cdt = new Condition() { Parameter = Parameter.TIME, Mark = CompareMarkEnum.LargerThan, Value = 1800 };
            cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            idleStep.CutOffBehaviors.Add(cob);

            Step cpStep = new Step() { Index = 3, Action = new TesterAction() { Mode = ActionMode.CP_DISCHARGE, Voltage = 0, Current = 0, Power = 6000 } };
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


            Step idleStep2 = new Step() { Index = 4, Action = new TesterAction() { Mode = ActionMode.REST, Voltage = 0, Current = 0, Power = 0 } };
            jpb = new JumpBehavior() { JumpType = JumpType.NEXT };
            cdt = new Condition() { Parameter = Parameter.TIME, Mark = CompareMarkEnum.LargerThan, Value = 30 };
            cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            idleStep2.CutOffBehaviors.Add(cob);

            Step cpStep2 = new Step() { Index = 5, Action = new TesterAction() { Mode = ActionMode.CP_DISCHARGE, Voltage = 0, Current = 0, Power = 33000 } };
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


            Step idleStep3 = new Step() { Index = 6, Action = new TesterAction() { Mode = ActionMode.REST, Voltage = 0, Current = 0, Power = 0 } };
            jpb = new JumpBehavior() { JumpType = JumpType.INDEX, Index = 3 };
            cdt = new Condition() { Parameter = Parameter.TIME, Mark = CompareMarkEnum.LargerThan, Value = 30 };
            cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            idleStep3.CutOffBehaviors.Add(cob);


            Step idleStep4 = new Step() { Index = 7, Action = new TesterAction() { Mode = ActionMode.REST, Voltage = 0, Current = 0, Power = 0 } };
            jpb = new JumpBehavior() { JumpType = JumpType.NEXT };
            cdt = new Condition() { Parameter = Parameter.TIME, Mark = CompareMarkEnum.LargerThan, Value = 1800 };
            cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            idleStep4.CutOffBehaviors.Add(cob);
            fullSteps = new List<Step> { chargeStep, idleStep, cpStep, idleStep2, cpStep2, idleStep3, idleStep4 };
        }
    }
}
