using System.Collections.Generic;

namespace SmartTester
{
    public class Test
    {
        public List<Step> Steps { get; set; }
        public IChamber Chamber { get; set; }
        public IChannel Channel { get; set; }
        public double DischargeTemperature { get; set; }
    }
}