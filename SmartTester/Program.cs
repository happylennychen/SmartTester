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
            Channel channel = tester.Channels[0];

            Step chargeStep = new Step() { Index = 1, Action = new TesterAction() { Mode = ActionMode.CC_CV_CHARGE, Voltage = 4200, Current = 1500, Power = 0 } };
            JumpBehavior jpb = new JumpBehavior() { JumpType = JumpType.NEXT };
            Condition cdt = new Condition() { Parameter = Parameter.TIME, Mark = CompareMarkEnum.LargerThan, Value = 150 };
            CutOffBehavior cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            chargeStep.CutOffBehaviors.Add(cob);


            Step idleStep = new Step() { Index = 2, Action = new TesterAction() { Mode = ActionMode.REST, Voltage = 0, Current = 0, Power = 0 } };
            jpb = new JumpBehavior() { JumpType = JumpType.NEXT };
            cdt = new Condition() { Parameter = Parameter.TIME, Mark = CompareMarkEnum.LargerThan, Value = 180 };
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

            /////////////////////////////Channel Test/////////////////////////////////////////
            //channel.SetStep(step);
            //channel.Start();
            //channel.GetData();
            //channel.GetData();
            //channel.GetData();
            //channel.GetData();
            //channel.GetData();
            //channel.GetData();
            //channel.GetData();
            //channel.GetData();
            //channel.GetData();
            //channel.GetData();
            //channel.GetData();
            //channel.Stop();
            /////////////////////////////////////////////////////////////////////////////////
            //tester.SetStep(step, 1);
            //tester.Start(1);
            //tester.SetStep(step, 2);
            //tester.Start(2);
            //Task t = Task.Delay(TimeSpan.FromSeconds(3));
            //t.Wait();
            //tester.SetStep(step, 3);
            //tester.Start(3);
            //tester.SetStep(step, 4);
            //tester.Start(4);
            //t = Task.Delay(TimeSpan.FromSeconds(13));
            //t.Wait();
            //tester.Stop(3);
            //tester.Stop(4);
            //Console.ReadKey();
            //for (int i = 0; i < 8; i++)
            //{
            //tester.SetStep(idleStep, i);
            //tester.targetTemperatures[i] = 25;
            //tester.fullSteps[i] = new List<Step> { chargeStep, idleStep, dischargeStep };
            //}
            foreach (var ch in tester.Channels)
            {
                ch.TargetTemperature = 20;
                ch.FullSteps = new List<Step> { chargeStep, idleStep, cpStep, idleStep2, cpStep2, idleStep3, idleStep4 };
            }
            //var ch1 = tester.Channels.SingleOrDefault(ch => ch.Index == 1);
            //ch1.FullSteps = new List<Step> { chargeStep, idleStep, dischargeStep1 };

            //var ch2 = tester.Channels.SingleOrDefault(ch => ch.Index == 2);
            //ch2.FullSteps = new List<Step> { chargeStep, idleStep, dischargeStep2 };
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
            }
        }
    }
}
