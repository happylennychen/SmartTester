using SmartTesterLib;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartTester
{
    public class ChamberFrame
    {
        public string ChamberInfo;
        public string TempScheduler;
        public string TestScheduler;
        public string ChannelInfo;
    }
    public class Monitor
    {
        private Timer timer { get; set; }
        private Automator amtr { get; set; }
        private List<ChamberFrame> Frames { get; set; }

        public Monitor(Automator amtr)
        {
            this.amtr = amtr;
            timer = new Timer(_ => MonitorCallback());
        }

        private void MonitorCallback()
        {
            UpdateFrame(amtr);
        }

        private void UpdateFrame(Automator amtr)
        {
            Console.Clear();
            foreach (var chamber in amtr.Chambers)
            {
                Utilities.WriteLine($"Chamber:{chamber.Name}");
                for (int i = 0; i < chamber.TempScheduler.TemperatureUintList.Count; i++)
                {
                    var tu = chamber.TempScheduler.TemperatureUintList[i];
                    Utilities.WriteLine($"\tTemp[{i}]:{tu.Target.Value} Degree,{tu.Status}");
                }
                for (int i = 0; i < chamber.TestScheduler.TestRoundList.Count; i++)
                {
                    var tr = chamber.TestScheduler.TestRoundList[i];
                    Utilities.WriteLine($"Round {i + 1}, {tr.Status}");
                    foreach (var item in tr.ChannelRecipes)
                    {
                        var ch = item.Key;
                        Utilities.WriteLine($"\tChannel:{ch.Name}, Recipe:{item.Value.Name}");
                        if (tr.Status == RoundStatus.RUNNING)
                        {
                            Utilities.WriteLine($"\t\tChannel Status:{ch.Status}");
                            foreach (var step in ch.Recipe.Steps)
                            {
                                if (step == ch.CurrentStep)
                                    Utilities.WriteLine($"\t\t\t{step.ToString()} *");
                                else
                                    Utilities.WriteLine($"\t\t\t{step.ToString()}");
                            }
                        }
                    }
                }
                Utilities.WriteLine("---------------------------------");
            }
        }

        public void Run()
        {
            timer.Change(0, 1000);
        }

        public void Stop()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}