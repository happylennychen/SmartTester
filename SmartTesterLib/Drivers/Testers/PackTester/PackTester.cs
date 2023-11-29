using Newtonsoft.Json;

namespace SmartTesterLib
{
    public class PackTester : ITester
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
        private TimerSliceScheduler Scheduler { get; set; }

        [JsonConstructor]
        public PackTester(int id, string name, int channelNumber, string ipAddress, int port, string sessionStr)
        {
            Id = id;
            Name = name;
            ChannelNumber = channelNumber;
            IpAddress = ipAddress;
            Port = port;
            SessionStr = sessionStr;
            Scheduler = new TimerSliceScheduler(channelNumber);
            Executor = new PackTesterExecutor();
            Channels = new List<IChannel>();

            for (int i = 1; i <= channelNumber; i++)
            {
                Token token;
                PackChannel ch = new PackChannel($"Ch{i}", i, this, out token);
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
    }
}