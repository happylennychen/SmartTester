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
            Step chargeStep = new Step() { Index = 1, Action = new TesterAction() { Mode = ActionMode.CC_CV_CHARGE, Voltage = 3780, Current = 1500, Power = 0 } };
            JumpBehavior jpb = new JumpBehavior() { JumpType = JumpType.NEXT };
            Condition cdt = new Condition() { Parameter = Parameter.CURRENT, Mark = CompareMarkEnum.SmallerThan, Value = 1250 };
            CutOffBehavior cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            chargeStep.CutOffBehaviors.Add(cob);


            Step idleStep = new Step() { Index = 2, Action = new TesterAction() { Mode = ActionMode.REST, Voltage = 0, Current = 0, Power = 0 } };
            jpb = new JumpBehavior() { JumpType = JumpType.NEXT };
            cdt = new Condition() { Parameter = Parameter.TIME, Mark = CompareMarkEnum.LargerThan, Value = 10 };
            cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            idleStep.CutOffBehaviors.Add(cob);

            Step dischargeStep = new Step() { Index = 3, Action = new TesterAction() { Mode = ActionMode.CC_DISCHARGE, Voltage = 0, Current = 1000, Power = 0 } };
            jpb = new JumpBehavior() { JumpType = JumpType.NEXT };
            cdt = new Condition() { Parameter = Parameter.VOLTAGE, Mark = CompareMarkEnum.SmallerThan, Value = 3730 };
            cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            dischargeStep.CutOffBehaviors.Add(cob);
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
                ch.TargetTemperature = 25;
                ch.FullSteps = new List<Step> { chargeStep, idleStep, dischargeStep };
            }
            while (true)
            {
                Console.WriteLine("Enter \"n\" to start channel n, \"a to h\" to stop channel n. Q to quit.");
                var line = Console.ReadLine();
                int result;
                if (line == "Q")
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
