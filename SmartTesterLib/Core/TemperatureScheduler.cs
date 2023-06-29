using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartTester
{
    public class TemperatureScheduler
    {
        private List<TemperatureUnit> TemperatureUintList { get; set; }

        public bool IsTemperatureSchedulerCompatible(List<TargetTemperature> ts)
        {
            throw new NotImplementedException();
        }

        public void UpdateTemperatureScheduler(List<TargetTemperature> ts)
        {
            throw new NotImplementedException();
        }

        public TemperatureUnit GetNextTemp()
        {
            return TemperatureUintList.First(tu => tu.Status == TemperatureStatus.WAITING);
        }
    }
}