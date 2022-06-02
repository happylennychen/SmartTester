namespace SmartTester
{
    public interface IChannel
    {
        int Index { get; set; }
        void SetStep(Step step);
        void Start();
        void Stop();
        StandardRow GetData();
        void LogData(string log);
    }
}