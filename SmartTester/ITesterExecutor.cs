namespace SmartTester
{
    public interface ITesterExecutor
    {
        bool ReadRow(int channelIndex, out StandardRow stdRow, out uint channelEvents);
        double ReadTemperarture(int channelIndex);
        bool SpecifyChannel(int channelIndex);
        bool SpecifyTestStep(Step step);
        bool Start();
        bool Stop();
    }
}