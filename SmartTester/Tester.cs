﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartTester
{
    public class Tester : ITester
    {
        public int Id { get; set; }
        public string Manufacturer { get; set; }
        public string Name { get; set; }
        public List<Channel> Channels { get; set; }
        private Timer mainTimer { get; set; }
        private Timer[] Timers { get; set; }
        //private bool isMainTimerRunning;
        //private Timer[] timers;
        private uint[] startTimes { get; set; }
        private uint[] timeSpans { get; set; }
        public double[] targetTemperatures { get; set; }
        private Step[] steps { get; set; }
        public List<Step>[] fullSteps { get; set; }
        private int _counter { get; set; } = 0;
        private bool[] _shouldTimerStart { get; set; }
        private bool[] _isTimerStart { get; set; }
        private DataLogger[] dataLoggers { get; set; }
        private Stopwatch[] stopwatchs { get; set; }
        private Stopwatch mainWatch { get; set; }
        private Chroma17208Executor Executor { get; set; }
        public Tester(string name, int channelNumber)
        {
            Name = name;
            Channels = new List<Channel>();
            startTimes = new uint[channelNumber];
            timeSpans = new uint[channelNumber];
            stopwatchs = new Stopwatch[channelNumber];
            Timers = new Timer[channelNumber];
            mainWatch = new Stopwatch();
            Executor = new Chroma17208Executor();
            dataLoggers = new DataLogger[channelNumber];
            targetTemperatures = new double[channelNumber];
            steps = new Step[channelNumber];
            fullSteps = new List<Step>[channelNumber];

            if (!Executor.Init())
            {
                Console.WriteLine("Error");
                return;
            }

            for (int i = 1; i <= channelNumber; i++)
            {
                Channel ch = new Channel();
                ch.Name = $"Ch{i}";
                ch.Index = i;
                Channels.Add(ch);
                ch.Tester = this;
                //ch.Timer = new Timer(_ => TimerOperation(i), null, Timeout.Infinite, 0);
                Timers[i - 1] = new Timer(WorkerCallback, i - 1, Timeout.Infinite, Timeout.Infinite);
                stopwatchs[i - 1] = new Stopwatch();
                //dataLoggers[i - 1] = new DataLogger(i, $"{Name}-{ch.Name}-{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt");
            }
            mainTimer = new Timer(_ => MainCounter(), null, 100, 0);
            _shouldTimerStart = new bool[channelNumber];
            _isTimerStart = new bool[channelNumber];
            mainWatch.Start();
        }

        private void WorkerCallback(object i)
        {
            int index = (int)i % Channels.Count;
            int channelIndex = index + 1;
            long data;
            //Channel channel = Channels.SingleOrDefault(ch => ch.Index == index + 1);
            StandardRow stdRow;
            uint channelEvents;
            if (!Executor.ReadRow(channelIndex, out stdRow, out channelEvents))
            {
                Console.WriteLine("Error");
                return;
            }
            var startPoint = stdRow.TimeInMS % 1000;
            do
            {
                //data = stopwatchs[index].ElapsedMilliseconds % 1000;
                //Console.WriteLine($"Read data from channel {index + 1}. Start point is {startPoint}, data is {data}");
                //stdRow = channel.GetData();
                Executor.ReadRow(channelIndex, out stdRow, out channelEvents);
                data = stdRow.TimeInMS % 1000;
            }
            while (data > 100 && stdRow.Status == RowStatus.RUNNING);
            stdRow.Temperature = Executor.ReadTemperarture(channelIndex);
            var strRow = stdRow.ToString();
            dataLoggers[index].AddData(strRow + "\n");
            if (channelEvents != ChannelEvents.Normal)
            {
                Console.WriteLine("Channel Event Error");
                return;
            }
            if (stdRow.Status != RowStatus.RUNNING)
            {
                timeSpans[index] = stdRow.TimeInMS - startTimes[index];
                steps[index] = GetNewTargetStep(steps[index], fullSteps[index], targetTemperatures[index], timeSpans[index], stdRow);
                if (steps[index] == null)
                {
                    dataLoggers[index].Close();
                    _shouldTimerStart[index] = false;
                    _isTimerStart[index] = false;
                    Console.WriteLine($"CH{channelIndex} Done!");
                    return;
                }
                else
                {
                    if (!Executor.SpecifyChannel(channelIndex))
                    {
                        Console.WriteLine("Error");
                        return;
                    }
                    if (!Executor.SpecifyTestStep(steps[index]))
                    {
                        Console.WriteLine("Error");
                        return;
                    }
                    if (!Executor.Start())
                    {
                        Console.WriteLine("Error");
                        return;
                    }
                }
            }
            var timer = Timers[index];
            var enable = _shouldTimerStart[index];
            if (enable) //开启下一次计时
            {
                timer.Change(950, 0);
            }
            string gap = string.Empty;
            for (int j = 0; j < index; j++)
            {
                gap += " ";
            }
            Console.WriteLine($"{strRow}...SP:{startPoint}...Data:{data}...Ch{gap}{channelIndex}");
        }

        private void MainCounter()
        {
            var startPoint = mainWatch.ElapsedMilliseconds % 125;
            long data;
            do
            {
                data = mainWatch.ElapsedMilliseconds % 125;
            }
            while (data > 10);
            mainTimer.Change(100, 0);
            //Console.WriteLine($"Main Counter Start point is {startPoint}, data is {data}, next delay is {100}");

            var counter = _counter % Channels.Count;
            var timer = Timers[counter];
            if (_shouldTimerStart[counter] && !_isTimerStart[counter])     //应该开启且还没开启
            {
                //Console.WriteLine($"Start channel {counter + 1}");
                //stopwatchs[counter].Start();
                //Channels.SingleOrDefault(ch => ch.Index == counter + 1).Start();
                dataLoggers[counter] = new DataLogger(counter + 1, $"{Name}-{Channels.SingleOrDefault(ch => ch.Index == counter + 1).Name}-{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt");
                Executor.SpecifyChannel(counter + 1);
                steps[counter] = fullSteps[counter].First();
                Executor.SpecifyTestStep(steps[counter]);
                Executor.Start();
                startTimes[counter] = 0;
                //dataLogger = new DataLogger(1, GetFileName());
                timer.Change(980, 0);
                _isTimerStart[counter] = true;
            }
            if (_counter >= Channels.Count * 9)
            {
                if (_isTimerStart[_counter % Channels.Count])
                    dataLoggers[_counter % Channels.Count].Flush();
            }
            _counter++;
            if (_counter == Channels.Count * 10)
            {
                _counter = 0;
            }
        }

        public void SetStep(Step step, int index)
        {
            //var channel = Channels.SingleOrDefault(ch => ch.Index == index);
            //channel.SetStep(step);
            if (!Executor.SpecifyChannel(index))
            {
                Console.WriteLine("Error");
                return;
            }
            if (!Executor.SpecifyTestStep(step))
            {
                Console.WriteLine("Error");
                return;
            }
        }

        public void Start(int index)
        {
            //var channel = Channels.SingleOrDefault(ch => ch.Index == index);
            //channel.Start();
            //channel.IsStarted = true;
            _shouldTimerStart[index - 1] = true;
        }

        public void Stop(int index)
        {
            //var channel = Channels.SingleOrDefault(ch => ch.Index == index);
            //channel.Stop();
            _shouldTimerStart[index - 1] = false;

            Console.WriteLine($"Stop channel {index - 1 + 1}");
            //stopwatchs[index - 1].Reset();
            Executor.SpecifyChannel(index);
            Executor.Stop();
            Timers[index - 1].Change(Timeout.Infinite, Timeout.Infinite);
            _isTimerStart[index - 1] = false;
        }

        public string GetData(int index)
        {
            throw new NotImplementedException();
        }

        private Step GetNewTargetStep(Step currentStep, List<Step> fullSteps, double temperature, uint timeSpan, StandardRow row)
        {
            Step nextStep = null;
            CutOffBehavior cob = null;
            switch (currentStep.Action.Mode)
            {
                case ActionMode.REST:// "StepFinishByCut_V":
                    cob = currentStep.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME);
                    break;
                case ActionMode.CC_CV_CHARGE://"StepFinishByCut_I":
                    cob = currentStep.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.CURRENT);
                    break;
                case ActionMode.CC_DISCHARGE://"StepFinishByCut_T":
                case ActionMode.CP_DISCHARGE:
                    cob = currentStep.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME);
                    if (cob != null)
                    {
                        var time = cob.Condition.Value;
                        if (Math.Abs(timeSpan / 1000 - time) < 1)
                            break;
                        else
                            cob = null;
                    }
                    cob = currentStep.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.VOLTAGE);
                    if (cob != null)
                    {
                        var volt = cob.Condition.Value;
                        if (Math.Abs(row.Voltage * 1000 - volt) < 5)
                            break;
                        else
                            cob = null;
                    }
                    break;
            }
            if (cob != null)
                nextStep = Jump(cob, fullSteps, currentStep.Index, row);
            return nextStep;
        }

        private static Step Jump(CutOffBehavior cob, List<Step> fullSteps, int currentStepIndex, StandardRow row)
        {
            Step nextStep = null;
            if (cob.JumpBehaviors.Count == 1)
            {
                var jpb = cob.JumpBehaviors.First();
                switch (jpb.JumpType)
                {
                    case JumpType.INDEX:
                        nextStep = fullSteps.SingleOrDefault(o => o.Index == jpb.Index);
                        break;
                    case JumpType.END:
                        break;
                    case JumpType.NEXT:
                        nextStep = fullSteps.SingleOrDefault(o => o.Index == currentStepIndex + 1);
                        break;
                    case JumpType.LOOP:
                        throw new NotImplementedException();
                }
            }
            else if (cob.JumpBehaviors.Count > 1)
            {
                JumpBehavior validJPB = null;
                foreach (var jpb in cob.JumpBehaviors)
                {
                    bool isConditionMet = false;
                    double leftvalue = 0;
                    double rightvalue = jpb.Condition.Value;
                    switch (jpb.Condition.Parameter)
                    {
                        case Parameter.CURRENT: leftvalue = row.Current; break;
                        case Parameter.POWER: leftvalue = row.Current * row.Voltage; break;
                        case Parameter.TEMPERATURE: leftvalue = row.Temperature; break;
                        case Parameter.TIME: leftvalue = row.TimeInMS / 1000; break;
                        case Parameter.VOLTAGE: leftvalue = row.Voltage; break;
                    }
                    switch (jpb.Condition.Mark)
                    {
                        case CompareMarkEnum.EqualTo:
                            if (leftvalue == rightvalue)
                                isConditionMet = true;
                            break;
                        case CompareMarkEnum.LargerThan:
                            if (leftvalue > rightvalue)
                                isConditionMet = true;
                            break;
                        case CompareMarkEnum.SmallerThan:
                            if (leftvalue < rightvalue)
                                isConditionMet = true;
                            break;
                    }
                    if (isConditionMet)
                    {
                        validJPB = jpb;
                        break;
                    }
                }
                if (validJPB != null)
                {
                    switch (validJPB.JumpType)
                    {
                        case JumpType.INDEX:
                            nextStep = fullSteps.SingleOrDefault(o => o.Index == validJPB.Index);
                            break;
                        case JumpType.END:
                            break;
                        case JumpType.NEXT:
                            nextStep = fullSteps.SingleOrDefault(o => o.Index == currentStepIndex + 1);
                            break;
                        case JumpType.LOOP:
                            throw new NotImplementedException();
                    }
                }
            }
            return nextStep;
        }
    }
}
