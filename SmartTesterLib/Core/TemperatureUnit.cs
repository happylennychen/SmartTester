namespace SmartTesterLib
{
    public class TemperatureUnit    //作为scheduler中的基础温度节点，描述温度点的状态
    {
        public TemperatureStatus Status { get; set; }
        public TemperatureTarget Target { get; set; }
    }
}