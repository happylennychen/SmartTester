namespace SmartTester
{
    public class TemperatureTarget      //recipe中带有的实验条件
    {
        public double Value { get; set; }
        public bool IsCritical { get; set; }
        public bool EqualsTo(TemperatureTarget target)
        {
            return (target.Value == Value) && (target.IsCritical == IsCritical);
        }
    }
}