using System.Collections.Generic;

namespace SmartTester
{
    public class CutOffBehavior
    {
        public int Id { get; set; }
        public Condition Condition { get; set; }
        public List<JumpBehavior> JumpBehaviors { get; set; }
        public CutOffBehavior()
        {
            JumpBehaviors = new List<JumpBehavior>();
        }
    }
}