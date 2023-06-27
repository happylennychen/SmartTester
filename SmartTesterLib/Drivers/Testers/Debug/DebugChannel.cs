﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace SmartTester
{
    public class DebugChannel : IChannel
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public Timer Timer { get; set; }
        public DataLogger DataLogger { get; set; }
        public Queue<StandardRow> DataQueue { get; set; }
        public Step CurrentStep { get; set; }
        public List<Step> FullStepsForOneTempPoint { get; set; }
        public bool IsTimerStart { get; set; }
        public bool ShouldTimerStart { get; set; }
        public double TargetTemperature { get; set; }
        public ChannelStatus Status { get; set; }
        public List<string> TempFileList { get; set; }
        [JsonIgnore]
        public ITester Tester { get; set; }
        public IChamber Chamber { get; set; }
        public uint LastTimeInMS { get; set; }
        //public uint Offset { get; set; }  //记录每个工步的初始时间偏差
        //public Stopwatch Anchor { get; set; }

        public void GenerateFile(List<Step> fullSteps)
        {
            Utilities.FileConvert(TempFileList, fullSteps, TargetTemperature);
            TempFileList.Clear();
        }

        public void Reset()
        {
            DataQueue.Clear();
            DataLogger.Close();
            ShouldTimerStart = false;
            IsTimerStart = false;
            LastTimeInMS = 0;
        }

        public void Stop()
        {
            ShouldTimerStart = false;

            Console.WriteLine($"Stop channel {Index - 1 + 1}");
            Tester.Executor.SpecifyChannel(Index);
            Tester.Executor.Stop();
            Timer.Change(Timeout.Infinite, Timeout.Infinite);
            IsTimerStart = false;
        }

        public void Start()
        {
            ShouldTimerStart = true;
            Status = ChannelStatus.RUNNING;
        }

        public DebugChannel(string name, int index, ITester tester, Timer timer)
        {
            Name = name;
            Index = index;
            Tester = tester;
            Timer = timer;
            TempFileList = new List<string>();
        }
        [JsonConstructor]
        public DebugChannel()
        {
            ;
        }
    }
}