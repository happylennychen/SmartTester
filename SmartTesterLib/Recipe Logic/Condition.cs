namespace SmartTesterLib
{
    public class Condition
    {
        public int Id { get; set; }
        public Parameter Parameter { get; set; }
        public CompareMarkEnum Mark { get; set; }
        public int Value { get; set; }
        public Condition()
        {
            Parameter = Parameter.NA;
            Mark = CompareMarkEnum.NA;
            Value = 0;
        }

        internal Condition Clone()
        {
            Condition output = new Condition();
            output.Parameter = this.Parameter;
            output.Mark = this.Mark;
            output.Value = this.Value;
            return output;
        }
    }
}