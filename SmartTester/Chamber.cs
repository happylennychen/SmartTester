//#define debug
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
#if debug
        public DebugChamberExecutor Executor { get; set; }
#else
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
        public Chamber(int id, string manufacturer, string name, int highestTemperature, int lowestTemperature, string ipAddress, int port)
        {
            Id = id;
            Manufacturer = manufacturer;
            Name = name;
            HighestTemperature = highestTemperature;
            LowestTemperature = lowestTemperature;
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