namespace SmartTesterLib
{
    public class JumpBehavior
    {
        public int Id { get; set; }
        public Condition Condition { get; set; }
        public JumpType JumpType { get; set; }
        public int Index { get; set; }

        internal JumpBehavior Clone()
        {
            JumpBehavior output = new JumpBehavior();
            output.Condition = this.Condition.Clone();
            output.JumpType = this.JumpType;
            output.Index = this.Index;
            return output;
        }
    }
}