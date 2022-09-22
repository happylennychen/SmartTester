using Newtonsoft.Json;

namespace SmartTester
{
    public class DebugChamber : IChamber
    {
        public int Id { get; set; }
        public string Manufacturer { get; set; }
        public string Name { get; set; }
        public double LowestTemperature { get; set; }
        public double HighestTemperature { get; set; }
        [JsonIgnore]
        public IChamberExecutor Executor { get; set; }
        [JsonConstructor]
        public DebugChamber(int id, string manufacturer, string name, double highestTemperature, double lowestTemperature)
        {
            Id = id;
            Manufacturer = manufacturer;
            Name = name;
            HighestTemperature = highestTemperature;
            LowestTemperature = lowestTemperature;
            Executor = new DebugChamberExecutor();
        }
    }
}