namespace SmartTester
{
    public enum ChannelStatus
    {
        UNASSIGNED,     //未指派任务
        ASSIGNED,
        RUNNING,
        COMPLETED,       //Channel完成之后会设为IDLE，外部会通过这个来判断是否完成
        PAUSED,
        ERROR,
        UNKNOWN
    }
}