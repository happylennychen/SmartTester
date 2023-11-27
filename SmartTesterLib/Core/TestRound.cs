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
        public override string ToString()
        {
            string output = string.Empty;
            foreach (var item in this.ChannelRecipes)
            {
                var ch = item.Key;
                output+=($"\tChannel:{ch.Name}, Recipe:{item.Value.Name}");
            }
            return output;
        }
    }
}