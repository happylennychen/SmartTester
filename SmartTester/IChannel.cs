namespace SmartTester
{
    public interface IChannel
    {
        int Index { get; set; }
        void SetStep(Step step);
        void Start();
        void Stop();
        string GetData();
    }
}