using System.Collections.Generic;

namespace SmartTesterLib
{
    public interface IChamber
    {
        IChamberExecutor Executor { get; set; }
        List<IChannel> Channels { get; set; }
        TestPlanScheduler TestScheduler { get; set; }
        double HighestTemperature { get; set; }
        int Id { get; set; }
        double LowestTemperature { get; set; }
        string Manufacturer { get; set; }
        string Name { get; set; }
        TemperatureScheduler TempScheduler { get; set; }

        bool StartNextUnit();   //根据TempScheduler开始调温
        bool Stop();
        bool UpdateStatus();    //读取温度，更新TestScheduler中的CurrentTemperatureUnit的Status
    }
}