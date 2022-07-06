﻿using System;
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
        public Stopwatch Stopwatch { get; internal set; }
        public DataLogger DataLogger { get; internal set; }
        public Queue<StandardRow> DataQueue { get; set; }
        public Step Step { get; internal set; }
        public List<Step> FullSteps { get; internal set; }
        public bool IsTimerStart { get; internal set; }
        public bool ShouldTimerStart { get; internal set; }
        public double TargetTemperature { get; internal set; }
        public ChannelStatus Status { get; internal set; }
        public List<string> TempFileList { get; internal set; }

        public void GenerateFile()
        {
            Utilities.FileConvert(TempFileList, FullSteps, TargetTemperature);
            TempFileList.Clear();
        }
    }
}
