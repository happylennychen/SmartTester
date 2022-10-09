using System;
using System.Diagnostics;

namespace SmartTester
{
    public class DebugChamberExecutor : IChamberExecutor
    {
        private Stopwatch stopwatch = new Stopwatch();
        private double StartTemperature { get; set; }
        private double TargetTemperature { get; set; }
        public bool Init(string ipAddress, int port)
        {
            return true;
        }

        public bool ReadTemperature(out double temp)
        {
            temp = StartTemperature + (TargetTemperature - StartTemperature) * (stopwatch.Elapsed.TotalSeconds > 20 ? 1 : stopwatch.Elapsed.TotalSeconds / 20.0);
            return true;
        }

        public bool Start(double temperature)
        {
            TargetTemperature = temperature;
            Random rand = new Random();
            StartTemperature = rand.NextDouble() * 10 + 20;
            stopwatch.Start();
            return true;
        }

        public bool Stop()
        {
            stopwatch.Reset();
            return true;
        }
    }
}