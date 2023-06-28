using System;

namespace SmartTester
{
    public class Token
    {
        public int Index { get; set; }
        public string Description { get; set; }
        public bool IsTimerStart { get; set; }
        public bool ShouldTimerStart { get; set; }
        public Action Callback { get; set; }
        public Token(int index, string description, Action callback)
        {
            Index = index;
            Description = description;
            Callback = callback;
        }
    }
}