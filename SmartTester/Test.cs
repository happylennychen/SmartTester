using System.Collections.Generic;

namespace SmartTester
{
    public class Test
    {
        public List<Step> Steps { get; set; }
        public Chamber Chamber { get; set; }
        public Channel Channel { get; set; }
        public double DischargeTemperature { get; set; }
    }
}