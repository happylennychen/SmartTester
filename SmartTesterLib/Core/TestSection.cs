using System.Collections.Generic;

namespace SmartTester
{
    public class TestSection
    {
        public TargetTemperature TargetTemperature { get; set; }
        public Dictionary<IChannel, List<Step>> ChannelStepsForOneTemp { get; set; }
    }
}