using System.Collections.Generic;

namespace SmartTester
{
    public class TestSection
    {
        public TemperatureTarget TargetTemperature { get; set; }
        public Dictionary<IChannel, List<SmartTesterStep>> ChannelStepsForOneTemp { get; set; }
    }
}