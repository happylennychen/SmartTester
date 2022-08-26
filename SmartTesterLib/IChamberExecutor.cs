namespace SmartTester
{
    public interface IChamberExecutor
    {
        bool Start(double temperature);
        bool Stop();
    }
}