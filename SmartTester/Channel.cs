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
        public int Index { get; set; }
        public string Name { get; set; }
        public Timer Timer { get; set; }
        public Stopwatch Stopwatch { get; internal set; }
        public DataLogger DataLogger { get; internal set; }
        public Step Step { get; internal set; }
        public List<Step> FullSteps { get; internal set; }
        public bool IsTimerStart { get; internal set; }
        public bool ShouldTimerStart { get; internal set; }
        public double TargetTemperature { get; internal set; }
    }
}
