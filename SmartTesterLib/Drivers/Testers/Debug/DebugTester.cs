//using Newtonsoft.Json;
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
        //[JsonIgnore]
        public List<IChannel> Channels { get; set; }
        public string Name { get; set; }
        //public int ChannelNumber { get; set; }
        //public string IpAddress { get; set; }
        //public int Port { get; set; }
        //public string SessionStr { get; set; }
        //[JsonIgnore]
        public ITesterExecutor Executor { get; set; }
        private TimerSliceScheduler Scheduler { get; set; }

        //[JsonConstructor]
        public DebugTester(int id, string name, int channelNumber, string ipAddress, int port, string sessionStr)
        {
            Id = id;
            Name = name;
            //ChannelNumber = channelNumber;
            //IpAddress = ipAddress;
            //Port = port;
            //SessionStr = sessionStr;
            Scheduler = new TimerSliceScheduler(channelNumber);
            Executor = new DebugTesterExecutor(Name);
            Channels = new List<IChannel>();

            for (int i = 1; i <= channelNumber; i++)
            {
                Token token;
                DebugChannel ch = new DebugChannel($"Ch{i}", i, this, out token);
                Scheduler.RegisterToken(token);
                Channels.Add(ch);
            }
            if (!Executor.Init(ipAddress, port, sessionStr))
            {
                Console.WriteLine("Error");
                return;
            }
            Scheduler.Activate();
        }
    }
}