using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace SmartTester
{
    public class DebugTester : ITester
    {
        public int Id { get; set; }
        [JsonIgnore]
        public List<IChannel> Channels { get; set; }
        public string Name { get; set; }
        public int ChannelNumber { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string SessionStr { get; set; }
        [JsonIgnore]
        public ITesterExecutor Executor { get; set; }
        private Timer mainTimer { get; set; }
        private int _counter { get; set; } = 0;
        private Stopwatch mainWatch { get; set; }

        [JsonConstructor]
        public DebugTester(int id, string name, int channelNumber, string ipAddress, int port, string sessionStr)
        {
            Id = id;
            Name = name;
            ChannelNumber = channelNumber;
            IpAddress = ipAddress;
            Port = port;
            SessionStr = sessionStr;
            Executor = new DebugTesterExecutor(Name);
            mainWatch = new Stopwatch();
            Channels = new List<IChannel>();

            for (int i = 1; i <= channelNumber; i++)
            {
                //DebugChannel ch = new DebugChannel($"Ch{i}", i, this, new Timer(WorkerCallback, i - 1, Timeout.Infinite, Timeout.Infinite));
                DebugChannel ch = new DebugChannel($"Ch{i}", i, this);
                Channels.Add(ch);
            }
            if (!Executor.Init(ipAddress, port, sessionStr))
            {
                Console.WriteLine("Error");
                return;
            }
            mainTimer = new Timer(_ => MainCounter(), null, 100, 0);
            mainWatch.Start();
        }

        private void MainCounter()
        {
            long data;
            do
            {
                data = mainWatch.ElapsedMilliseconds % 125;
            }
            while (data > 10);
            mainTimer.Change(100, 0);

            var counter = _counter % Channels.Count;
            var channel = Channels.SingleOrDefault(ch => ch.Index == counter + 1);
            if (channel.ShouldStandardTimerStart && !channel.IsStandardTimerStart)     //应该开启且还没开启
            {
                channel.DataQueue = new Queue<StandardRow>();
                string fileName = $"{Name}-{channel.Name}-{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";
                channel.DataLogger = new DataLogger(channel.Chamber, counter + 1, fileName);
                channel.TempFileList.Add(channel.DataLogger.FilePath);
                Executor.SpecifyChannel(counter + 1);
                channel.CurrentStep = channel.FullStepsForOneTempPoint.First();
                Executor.SpecifyTestStep(channel.CurrentStep);
                Executor.Start();
                //channel.StandardTimer.Change(980,Timeout.Infinite);
                channel.StandardTimer.Change(980, Timeout.Infinite);
                channel.IsStandardTimerStart = true;
            }

            if (channel.ShouldWaveformTimerStart && !channel.IsWaveformTimerStart)     //应该开启且还没开启
            {
                channel.DataQueue = new Queue<StandardRow>();
                string fileName = $"{Name}-{channel.Name}-{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";
                //channel.DataLogger = new DataLogger(channel.Chamber, counter + 1, fileName);
                channel.DataLogger = new DataLogger(counter + 1, fileName);
                channel.TempFileList.Add(channel.DataLogger.FilePath);
                Executor.SpecifyChannel(counter + 1);
                channel.CurrentStep = new Step() { Action = new TesterAction(ActionMode.CC_DISCHARGE,4200,0,0), Index = 1, Id = 1, CutOffBehaviors= new List<CutOffBehavior>() { new CutOffBehavior() { Id = 1, Condition = new Condition() { Parameter=Parameter.VOLTAGE, Mark= CompareMarkEnum.SmallerThan, Value=3000}, JumpBehaviors = new List<JumpBehavior>() { new JumpBehavior() } } } };
                Executor.SpecifyTestStep(channel.CurrentStep);
                Executor.Start();
                //channel.StandardTimer.Change(980,Timeout.Infinite);
                channel.WaveformTimer.Change(980, Timeout.Infinite);
                channel.IsWaveformTimerStart = true;
            }
            _counter++;
            if (_counter == Channels.Count * 10)
            {
                _counter = 0;
            }
        }
    }
}