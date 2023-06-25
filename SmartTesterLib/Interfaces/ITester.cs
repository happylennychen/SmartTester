using System.Collections.Generic;

namespace SmartTester
{
    public interface ITester
    {
        int Id { get; set; }
        void SetStep(Step step, int index);//没用到
        void Start(int index);
        void Stop(int index);
        string GetData(int index);//没用到
        List<IChannel> Channels { get; set; }
        string Name { get; set; }
        ITesterExecutor Executor { get; set; }
    }
}