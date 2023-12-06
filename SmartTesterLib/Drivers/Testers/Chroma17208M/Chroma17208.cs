//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Threading.Thread;
namespace SmartTesterLib
{
    public static class StepTolerance
    {
        public static double Current { get { return 10; } } //mA
        public static double Temperature { get { return 3.5; } }    //deg
        public static double Voltage { get { return 5; } } //mV
        public static double Power { get { return 100; } }
        public static double Time { get { return 3000; } }     //mS
    }
    public class Chroma17208 : ITester
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
        public Chroma17208(int id, string name, int channelNumber, string ipAddress, int port, string sessionStr)
        {
            Id = id;
            Name = name;
            //ChannelNumber = channelNumber;
            //IpAddress = ipAddress;
            //Port = port;
            //SessionStr = sessionStr;
            Scheduler = new TimerSliceScheduler(channelNumber);
            Executor = new Chroma17208Executor();
            Channels = new List<IChannel>();

            for (int i = 1; i <= channelNumber; i++)
            {
                Token token;
                Chroma17208Channel ch = new Chroma17208Channel($"Ch{i}", i, this, out token);
                Scheduler.RegisterToken(token);
                Channels.Add(ch);
            }
            if (!Executor.Init(ipAddress, port, sessionStr))
            {
                Utilities.WriteLine("Error");
                return;
            }
            Scheduler.Activate();
        }

        //public Chroma17208(int id, string name, int channelNumber)
        //{
        //    Id = id;
        //    Name = name;
        //    Channels = new List<IChannel>();
        //    for (int i = 1; i <= channelNumber; i++)
        //    {
        //        Token token;
        //        Chroma17208Channel ch = new Chroma17208Channel($"Ch{i}", i, this, out token);
        //        Scheduler.RegisterToken(token);
        //        Channels.Add(ch);
        //    }
        //}
    }
}
