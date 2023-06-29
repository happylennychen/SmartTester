using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SmartTester
{
    public class Chamber : IChamber
    {
        public int Id { get; set; }
        public string Manufacturer { get; set; }
        public string Name { get; set; }
        public double LowestTemperature { get; set; }
        public double HighestTemperature { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        [JsonIgnore]
        public IChamberExecutor Executor { get; set; }
        public List<IChannel> Channels { get; set; }
        public TestPlanScheduler TestScheduler { get; set; }

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
            Executor = new PUL80Executor();
            if (!Executor.Init(ipAddress, port))
            {
                Console.WriteLine("Error");
                return;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}