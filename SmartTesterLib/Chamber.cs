#define debug
using Newtonsoft.Json;
using System;

namespace SmartTester
{
    public class Chamber
    {
        public int Id { get; set; }
        public string Manufacturer { get; set; }
        public string Name { get; set; }
        public double LowestTemperature { get; set; }
        public double HighestTemperature { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
#if debug
        [JsonIgnore]
        public DebugChamberExecutor Executor { get; set; }
#else
        [JsonIgnore]
        public PUL80Executor Executor { get; set; }
#endif

        public Chamber()
        { }
        public Chamber(int Id, string Manufacturer, string Name, int HighestTemperature, int LowestTemperature)
        {
            this.Id = Id;
            this.Manufacturer = Manufacturer;
            this.Name = Name;
            this.HighestTemperature = HighestTemperature;
            this.LowestTemperature = LowestTemperature;
        }
        [JsonConstructor]
        public Chamber(int id, string manufacturer, string name, double highestTemperature, double lowestTemperature, string ipAddress, int port)
        {
            Id = id;
            Manufacturer = manufacturer;
            Name = name;
            HighestTemperature = highestTemperature;
            LowestTemperature = lowestTemperature;
            IpAddress = ipAddress;
            Port = port;
#if debug
            Executor = new DebugChamberExecutor();
#else
            Executor = new PUL80Executor();
#endif
#if !debug
            if (!Executor.Init(ipAddress, port))
            {
                Console.WriteLine("Error");
                return;
            }
#endif
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}