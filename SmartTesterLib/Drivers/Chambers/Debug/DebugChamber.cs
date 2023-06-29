using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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

        public TestPlanScheduler TestScheduler { get; set; }
        public List<IChannel> Channels { get; set; }
        public TemperatureScheduler TempScheduler { get; set; }

        [JsonConstructor]
        public DebugChamber(int id, string manufacturer, string name, double highestTemperature, double lowestTemperature)
        {
            Id = id;
            Manufacturer = manufacturer;
            Name = name;
            HighestTemperature = highestTemperature;
            LowestTemperature = lowestTemperature;
            Executor = new DebugChamberExecutor();
            TestScheduler = new TestPlanScheduler();
            TempScheduler = new TemperatureScheduler();
        }

        public bool Start()
        {
            bool ret;
            var tUnit = TempScheduler.GetNextTemp();

            ret = Executor.Start(tUnit.Target.Temperature);
            if (!ret)
            {
                Console.WriteLine($"Start chamber failed! Please check chamber cable.");
                return ret;
            }
            tUnit.Status = TemperatureStatus.REACHING;
            return true;
        }
    }
}