using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Tester tester = new Tester("Chroma17208M", 8);
            //Tester tester2 = new Tester("Chroma17216", 8);
            List<Step> fullSteps;
            CreateFullSteps(out fullSteps);
            /*foreach (var ch in tester.Channels)
            {
                ch.TargetTemperature = 20;
                ch.FullSteps = fullSteps;
            }
            while (true)
            {
                Console.WriteLine("Enter \"n\" to start channel n, \"a to h\" to stop channel n. Q to quit.");
                var line = Console.ReadLine();
                int result;
                if (line.ToUpper() == "Q")
                    break;
                else if (int.TryParse(line, out result))
                {
                    if (result > 0 && result < 9)
                    {
                        tester.Start(result);
                    }
                }
                else if (line.Length == 1)
                {
                    char x = line[0];
                    result = x - 'a' + 1;

                    tester.Stop(result);
                }
                else
                {
                    Console.WriteLine("Wrong command.");
                }
            }*/
            Chamber cmb1 = new Chamber() { Id = 1, Manufacturer = "Hongzhan", Name = "PUL80", HighestTemperature = 150, LowestTemperature = -40 };
            //Chamber cmb2 = new Chamber() { Id = 2, Manufacturer = "Hongzhan", Name = "PUL90", HighestTemperature = 150, LowestTemperature = -40 };
            Automator automator = new Automator();
            List<Test> tests = new List<Test>();
            tests.Add(new Test() { Channel = tester.Channels.SingleOrDefault(ch => ch.Index == 1), Chamber = cmb1, Steps = fullSteps, DischargeTemperature = 0 });
            tests.Add(new Test() { Channel = tester.Channels.SingleOrDefault(ch => ch.Index == 2), Chamber = cmb1, Steps = fullSteps, DischargeTemperature = 0 });
            tests.Add(new Test() { Channel = tester.Channels.SingleOrDefault(ch => ch.Index == 3), Chamber = cmb1, Steps = fullSteps, DischargeTemperature = 0 });
            //tests.Add(new Test() { Channel = tester.Channels.SingleOrDefault(ch => ch.Index == 4), Chamber = cmb2, Steps = fullSteps, DischargeTemperature = 20 });
            //tests.Add(new Test() { Channel = tester.Channels.SingleOrDefault(ch => ch.Index == 5), Chamber = cmb2, Steps = fullSteps, DischargeTemperature = 20 });
            Task t = automator.Start(tests);
            t.Wait();
            Console.WriteLine("Demo program completed!");
            Console.ReadLine();
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

            //Step dischargeStep1 = new Step() { Index = 3, Action = new TesterAction() { Mode = ActionMode.CC_DISCHARGE, Voltage = 0, Current = 15000, Power = 0 } };
            //jpb = new JumpBehavior() { JumpType = JumpType.NEXT };
            //cdt = new Condition() { Parameter = Parameter.VOLTAGE, Mark = CompareMarkEnum.SmallerThan, Value = 2500 };
            //cob = new CutOffBehavior() { Condition = cdt };
            //cob.JumpBehaviors.Add(jpb);
            //dischargeStep1.CutOffBehaviors.Add(cob);

            //Step dischargeStep2 = new Step() { Index = 3, Action = new TesterAction() { Mode = ActionMode.CC_DISCHARGE, Voltage = 0, Current = 24000, Power = 0 } };
            //jpb = new JumpBehavior() { JumpType = JumpType.NEXT };
            //cdt = new Condition() { Parameter = Parameter.VOLTAGE, Mark = CompareMarkEnum.SmallerThan, Value = 2500 };
            //cob = new CutOffBehavior() { Condition = cdt };
            //cob.JumpBehaviors.Add(jpb);
            //dischargeStep2.CutOffBehaviors.Add(cob);

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
