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
        //public int ChannelNumber { get; set; }
        //public string IpAddress { get; set; }
        //public int Port { get; set; }
        //public string SessionStr { get; set; }
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
            //ChannelNumber = channelNumber;
            //IpAddress = ipAddress;
            //Port = port;
            //SessionStr = sessionStr;
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
            if (channel.ShouldTimerStart && !channel.IsTimerStart)     //应该开启且还没开启
            {
                channel.DataQueue = new Queue<StandardRow>();
                string fileName = $"{Name}-{channel.Name}-{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";
                channel.DataLogger = new DataLogger(channel.Chamber, counter + 1, fileName);
                channel.TempFileList.Add(channel.DataLogger.FilePath);
                Executor.SpecifyChannel(counter + 1);
                channel.CurrentStep = channel.FullStepsForOneTempPoint.First();
                Executor.SpecifyTestStep(channel.CurrentStep);
                Executor.Start();
                channel.Timer.Change(980, 0);
                channel.IsTimerStart = true;
            }
            _counter++;
            if (_counter == Channels.Count * 10)
            {
                _counter = 0;
            }
        }

        //public void Start(int index)
        //{
        //    var channel = Channels.SingleOrDefault(ch => ch.Index == index);
        //    channel.ShouldTimerStart = true;
        //    channel.Status = ChannelStatus.RUNNING;
        //}

        //public void Stop(int index)
        //{
        //    var channel = Channels.SingleOrDefault(ch => ch.Index == index);
        //    channel.ShouldTimerStart = false;

        //    Console.WriteLine($"Stop channel {index - 1 + 1}");
        //    Executor.SpecifyChannel(index);
        //    Executor.Stop();
        //    channel.Timer.Change(Timeout.Infinite, Timeout.Infinite);
        //    channel.IsTimerStart = false;
        //}
    }
}