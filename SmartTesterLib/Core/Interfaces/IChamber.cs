namespace SmartTesterLib
{
    public interface IChamber : IAssamble
    {
        int Id { get; set; }
        IChamberExecutor Executor { get; set; }
        List<IChannel> PairedChannels { get; set; }
        TestPlanScheduler TestScheduler { get; set; }
        double HighestTemperature { get; set; }
        double LowestTemperature { get; set; }
        string Manufacturer { get; set; }
        string Name { get; set; }
        TemperatureScheduler TempScheduler { get; set; }

        bool StartNextUnit();   //根据TempScheduler开始调温
        bool Stop();
        bool UpdateStatus();    //读取温度，更新TestScheduler中的CurrentTemperatureUnit的Status
    }
}