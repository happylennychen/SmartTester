namespace SmartTester
{
    public enum ChannelStatus
    {
        UNASSIGNED,     //未指派任务
        ASSIGNED,
        RUNNING,
        COMPLETED,       //Channel的一个温度节点的Steps完成之后会设为COMPLETED，外部会通过这个来判断是否完成
        PAUSED,
        ERROR,
        UNKNOWN
    }
}