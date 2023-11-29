using System.Collections.Generic;

namespace SmartTesterLib
{
    public class TestRound
    {
        //public IChamber Chamber { get; set; }
        public Dictionary<IChannel, SmartTesterRecipe> ChannelRecipes { get; set; }
        public RoundStatus Status { get; set; }
        public TestRound()
        {
            ChannelRecipes = new Dictionary<IChannel, SmartTesterRecipe>();
        }
        public TestRound(Dictionary<IChannel, SmartTesterRecipe> channelRecipes)
        {
            //Chamber = chamber;
            ChannelRecipes = channelRecipes;
            Status = RoundStatus.WAITING;
        }
        public void AppendChannelRecipePair(IChannel ch, SmartTesterRecipe recipe)
        {
            if(ChannelRecipes == null)
                ChannelRecipes = new Dictionary<IChannel, SmartTesterRecipe>();
            ChannelRecipes.Add(ch, recipe);
        }
        public override string ToString()
        {
            string output = string.Empty;
            foreach (var item in this.ChannelRecipes)
            {
                var ch = item.Key;
                output+=($"{ch}:{item.Value}");
            }
            return output;
        }
    }
}