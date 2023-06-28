using System.Collections.Generic;
using System.Threading;

namespace SmartTester
{
    public interface IChannel
    {
        int Index { get; set; }
        string Name { get; set; }
        Step CurrentStep { get; set; }
        List<Step> FullStepsForOneTempPoint { get; set; }
        ChannelStatus Status { get; set; }
        ITester Tester { get; set; }
        IChamber Chamber { get; set; }
        void GenerateFile(System.Collections.Generic.List<Step> fullSteps);
        void Reset();
        void Stop();
        void Start();
    }
}