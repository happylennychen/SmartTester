using System;

namespace SmartTester
{
    public class DebugChamberExecutor : IChamberExecutor
    {
        public bool Start(double temperature)
        {
            return true;
        }

        public bool Stop()
        {
            return true;
        }
    }
}