namespace SmartTester
{
    public interface ITesterExecutor
    {
        bool ReadRow(int channelIndex, out object row, out uint channelEvents);
        bool ReadTemperarture(int channelIndex, out double temperature);
        bool SpecifyChannel(int channelIndex);
        bool SpecifyTestStep(SmartTesterStep step);
        bool Start();
        bool Stop();
        bool Init(string ipAddress, int port, string sessionStr);
    }
}