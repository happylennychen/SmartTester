using System.Collections.Generic;
using System.Threading;

namespace SmartTester
{
    public interface IChannel
    {
        int Index { get; set; }
        string Name { get; set; }
        SmartTesterRecipe Recipe { get; set; }
        SmartTesterStep CurrentStep { get; set; }
        //List<Step> StepsForOneTempPoint { get; set; }
        ChannelStatus Status { get; set; }
        ITester Tester { get; set; }
        IChamber Chamber { get; set; }
        void GenerateFile();
        void Reset();
        void Stop();
        void Start();
        void SetStepsForOneTempPoint();
    }
}