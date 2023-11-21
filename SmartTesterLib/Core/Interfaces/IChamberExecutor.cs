namespace SmartTesterLib
{
    public interface IChamberExecutor
    {
        bool Start(double temperature);
        bool Stop();
        bool Init(string ipAddress, int port);
        bool ReadTemperature(out double temp);
    }
}