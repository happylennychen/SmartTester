namespace SmartTester
{
    public enum RowStatus
    {
        RUNNING,
        CUT_OFF_BY_CURRENT,
        CUT_OFF_BY_VOLTAGE,
        CUT_OFF_BY_TEMPERATURE,
        CUT_OFF_BY_POWER,
        CUT_OFF_BY_CAPACITY,
        CUT_OFF_BY_TIME,
        CUT_OFF_BY_ERROR,
        STOP,
        UNKNOWN
    }
}