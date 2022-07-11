#define debug
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
        public PUL80Executor Executor { get; set; }

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

            Executor = new PUL80Executor();
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