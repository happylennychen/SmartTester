using System.Collections.Generic;

namespace SmartTester
{
    public class TestRound
    {
        //public IChamber Chamber { get; set; }
        public Dictionary<IChannel, Recipe> ChannelRecipes { get; set; }
        public RoundStatus Status { get; set; }
        public TestRound(Dictionary<IChannel, Recipe> channelRecipes)
        {
            //Chamber = chamber;
            ChannelRecipes = channelRecipes;
            Status = RoundStatus.WAITING;
        }
    }
}