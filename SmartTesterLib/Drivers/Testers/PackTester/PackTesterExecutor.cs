using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static System.Threading.Thread;

namespace SmartTester
{
    public class PackTesterExecutor : ITesterExecutor
    {
        public bool Init(string ipAddress, int port, string sessionStr)
        {
            throw new NotImplementedException();
        }

        public bool ReadRow(int channelIndex, out StandardRow stdRow, out uint channelEvents)
        {
            throw new NotImplementedException();
        }

        public bool ReadTemperarture(int channelIndex, out double temperature)
        {
            throw new NotImplementedException();
        }

        public bool SpecifyChannel(int channelIndex)
        {
            return true;
        }

        public bool SpecifyTestStep(SmartTesterStep step)
        {
            throw new NotImplementedException();
        }

        public bool Start()
        {
            throw new NotImplementedException();
        }

        public bool Stop()
        {
            throw new NotImplementedException();
        }
    }
}