namespace SmartTester
{
    public interface IChamber
    {
        IChamberExecutor Executor { get; set; }
        double HighestTemperature { get; set; }
        int Id { get; set; }
        double LowestTemperature { get; set; }
        string Manufacturer { get; set; }
        string Name { get; set; }
    }
}