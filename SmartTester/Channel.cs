using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartTester
{
    public class Channel
    {
        public Channel(string name, int index, Tester tester, Timer timer)
        {
            Name = name;
            Index = index;
            Tester = tester;
            Timer = timer;
            TempFileList = new List<string>();
        }

        public int Index { get; set; }
        public Tester Tester { get; set; }
        public string Name { get; set; }
        public Timer Timer { get; set; }
        //public Stopwatch Stopwatch { get; internal set; }
        public DataLogger DataLogger { get; internal set; }
        public Queue<StandardRow> DataQueue { get; set; }
        public Step Step { get; internal set; } //当前Step
        public List<Step> FullSteps { get; internal set; }  //同一温度下的工步集合
        public bool IsTimerStart { get; internal set; }
        public bool ShouldTimerStart { get; internal set; }
        public double TargetTemperature { get; internal set; }
        public ChannelStatus Status { get; internal set; }
        public List<string> TempFileList { get; internal set; }

        public void GenerateFile(List<Step> fullSteps)
        {
            Utilities.FileConvert(TempFileList, fullSteps, TargetTemperature);
            TempFileList.Clear();
        }
    }
}
