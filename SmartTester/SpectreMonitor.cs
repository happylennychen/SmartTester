using SmartTesterLib;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartTester
{
    public class SpectreMonitor
    {
        private Timer timer { get; set; }
        private List<IChamber> Chambers { get; set; }
        private string[] SpinnerList = new string[] { "|", "/", "-", "\\" };
        private string Spinner { get { return SpinnerList[SpinnerCounter % 4]; } }
        private uint SpinnerCounter { get; set; }

        public SpectreMonitor(IEnumerable<IChamber> chambers)
        {
            this.Chambers = chambers.ToList();
            timer = new Timer(_ => MonitorCallback());
        }

        private void MonitorCallback()
        {
            SpinnerCounter++;
            AnsiConsole.Clear();
            Render(Chambers);
        }

        private void Render(List<IChamber> chambers)
        {
            var root = new Tree("Chambers");
            foreach (var chamber in chambers)
            {
                var cmbNode = root.AddNode(chamber.Name);
                var channelNode = cmbNode.AddNode("Channels");
                if (chamber.Channels != null)
                    foreach (var ch in chamber.Channels)
                    {
                        var chNode = channelNode.AddNode($"{ch}:{ch.Recipe}");
                        if (ch.Status == ChannelStatus.RUNNING)
                        {
                            foreach (var step in ch.Recipe.Steps)
                            {
                                TreeNode stepNode;
                                if (step == ch.CurrentStep)
                                    stepNode = chNode.AddNode($"{step} {Spinner}");
                                else
                                    stepNode = chNode.AddNode($"{step}");
                            }
                        }
                    }
                var tempNode = cmbNode.AddNode("Temperature Schedule");
                for (int i = 0; i < chamber.TempScheduler.TemperatureUintList.Count; i++)
                {
                    var tu = chamber.TempScheduler.TemperatureUintList[i];
                    if (tu == chamber.TempScheduler.GetCurrentTemp())
                        tempNode.AddNode($"Temp {i}:{tu.Target.Value} Degree,{tu.Status} {Spinner}");
                    else
                        tempNode.AddNode($"Temp {i}:{tu.Target.Value} Degree,{tu.Status}");
                }
                var testNode = cmbNode.AddNode("Test Schedule");
                var trl = chamber.TestScheduler.TestRoundList;
                foreach (var tr in trl)
                {
                    var index = trl.IndexOf(tr);
                    if(tr.Status == RoundStatus.RUNNING)
                        testNode.AddNode($"Round {index}, {tr.Status} {Spinner}");
                    else
                        testNode.AddNode($"Round {index}, {tr.Status}");
                }
            }
            AnsiConsole.Write(root);
            var rule = new Rule();
            AnsiConsole.Write(rule);
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