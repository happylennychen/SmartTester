using System.Collections.Generic;

namespace SmartTesterLib
{
    public class TestRound
    {
        public int Id { get; set; }
        //public IChamber Chamber { get; set; }
        public List<IChannel> AvailableChannels { get; private set; }
        public Dictionary<IChannel, SmartTesterRecipe> ChannelRecipes { get; private set; }
        public RoundStatus Status { get; set; }
        public TestRound()
        {
            
        }
        public TestRound(List<IChannel> availableChannels)
        {
            ChannelRecipes = new Dictionary<IChannel, SmartTesterRecipe>();
            AvailableChannels = availableChannels;
        }
        public void AppendChannelRecipePair(IChannel ch, SmartTesterRecipe recipe)
        {
            if (AvailableChannels.Contains(ch))
            {
                ChannelRecipes.Add(ch, recipe);
            }
            else
            {
                Utilities.WriteLine($"{ch} is not available.");
            }
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