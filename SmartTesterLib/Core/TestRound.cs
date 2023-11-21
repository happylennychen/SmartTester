using System.Collections.Generic;

namespace SmartTesterLib
{
    public class TestRound
    {
        //public IChamber Chamber { get; set; }
        public Dictionary<IChannel, SmartTesterRecipe> ChannelRecipes { get; set; }
        public RoundStatus Status { get; set; }
        public TestRound(Dictionary<IChannel, SmartTesterRecipe> channelRecipes)
        {
            //Chamber = chamber;
            ChannelRecipes = channelRecipes;
            Status = RoundStatus.WAITING;
        }
    }
}