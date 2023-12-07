//using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTesterLib
{
    public class DebugChamber : IChamber
    {
        public int Id { get; set; }
        public string Manufacturer { get; set; }
        public string Name { get; set; }
        public double LowestTemperature { get; set; }
        public double HighestTemperature { get; set; }
        ////[JsonIgnore]
        [NotMapped]
        public IChamberExecutor Executor { get; set; }
        [NotMapped]
        public TestPlanScheduler TestScheduler { get; set; }
        [NotMapped]
        public List<IChannel> PairedChannels { get; set; }
        [NotMapped]
        public TemperatureScheduler TempScheduler { get; set; }
        //private Timer timer { get; set; }
        private byte TempInRangeCounter { get; set; } = 0;
        public DebugChamber()
        {
            
        }

        //[JsonConstructor]
        public DebugChamber(int id, string manufacturer, string name, double highestTemperature, double lowestTemperature)
        {
            Id = id;
            Manufacturer = manufacturer;
            Name = name;
            HighestTemperature = highestTemperature;
            LowestTemperature = lowestTemperature;
            Executor = new DebugChamberExecutor();
            TestScheduler = new TestPlanScheduler(this);
            TempScheduler = new TemperatureScheduler();
        }
        public void Assamble()  //从DB中Load上来后，只有部分属性。还需要组装其他属性
        {
            Executor = new DebugChamberExecutor();
            TestScheduler = new TestPlanScheduler(this);
            TempScheduler = new TemperatureScheduler();
        }

        public bool UpdateStatus()
        {
            double temp;
            bool ret = false;
            ret = Executor.ReadTemperature(out temp);
            if (!ret)
            {
                Utilities.WriteLine($"Read Temperature failed! Please check chamber cable.");
                return false;
            }
            var currentTemp = TempScheduler.GetCurrentTemp();
            if (Math.Abs(temp - currentTemp.Target.Value) < 5)
            {
                TempInRangeCounter++;
                Utilities.WriteLine($"Temperature reach target. Counter: {TempInRangeCounter}");
            }
            else
            {
                TempInRangeCounter = 0;
                Utilities.WriteLine($"Temperature leave target. Counter: {TempInRangeCounter}");
            }
            if (TempInRangeCounter < 30)
            {
                currentTemp.Status = TemperatureStatus.REACHING;
            }
            else
            {
                currentTemp.Status = TemperatureStatus.REACHED;
                //timer.Change(Timeout.Infinite, 1000);
            }
            return true;
        }

        public bool StartNextUnit()
        {
            bool ret;
            var ctu = TempScheduler.GetCurrentTemp();
            if (ctu != null)
                ctu.Status = TemperatureStatus.PASSED;
            var tUnit = TempScheduler.GetNextTemp();
            if (tUnit == null)
            {
                Utilities.WriteLine($"There's no waiting temperature.");
                return false;
            }

            ret = Executor.Start(tUnit.Target.Value);
            if (!ret)
            {
                Utilities.WriteLine($"Start chamber failed! Please check chamber cable.");
                return ret;
            }
            tUnit.Status = TemperatureStatus.REACHING;

            //timer.Change(0, 1000);
            return true;
        }

        public bool Stop()
        {
            bool ret;
            //var tUnit = TempScheduler.GetCurrentTemp();

            ret = Executor.Stop();
            if (!ret)
            {
                Utilities.WriteLine($"Stop chamber failed! Please check chamber cable.");
                return ret;
            }
            //tUnit.Status = TemperatureStatus.PASSED;

            //timer.Change(Timeout.Infinite, 1000);
            return true;
        }
        public override string ToString()
        {
            return this.Name;
        }
    }
}