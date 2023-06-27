using System.Collections.Generic;
using System.Threading;

namespace SmartTester
{
    public interface IChannel
    {
        int Index { get; set; }
        string Name { get; set; }
        Timer Timer { get; set; }
        DataLogger DataLogger { get; set; }
        Queue<StandardRow> DataQueue { get; set; }
        Step CurrentStep { get; set; }
        List<Step> FullStepsForOneTempPoint { get; set; }
        bool IsTimerStart { get; set; }
        bool ShouldTimerStart { get; set; }
        double TargetTemperature { get; set; }
        ChannelStatus Status { get; set; }
        List<string> TempFileList { get; set; }
        ITester Tester { get; set; }
        IChamber Chamber { get; set; }
        uint LastTimeInMS { get; set; }
        //uint Offset { get; set; }

        void GenerateFile(System.Collections.Generic.List<Step> fullSteps);
        void Reset();
        void Stop();
        void Start();
    }
}