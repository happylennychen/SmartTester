using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace SmartTester
{
    public class PseudoHardware
    {
        public Stopwatch Stopwatch { get; set; }   //用秒表来控制每个通道的状态
        public Queue<StandardRow> DataQueues { get; set; }    //用来记录历史数据，以便推演新数据
        public uint DataLength { get; set; }       //DataQueue的长度
        public Step Step { get; set; }
        public PseudoBattery Battery { get; set; }
        public int TimerIntervalInMS { get; set; }

        public PseudoHardware()
        {
            Stopwatch = new Stopwatch();
            DataLength = 20;
            DataQueues = new Queue<StandardRow>();
            Battery = new PseudoBattery(3600, 3100, 0, 3200, 20);
            Battery.ChargeCurrentSlope = 1;
            Battery.ChargeVoltageSlope = 1;
            Battery.DischargeVoltageSlope = 1;
            Battery.EnvTemperature = 30;
            Battery.DischargeTemperatureSlope = 1;
            Battery.NonDischargeTemperatureSlope = 1;
            TimerIntervalInMS = 500;
            //var timer = new Timer(_ => TimerCallback(), null, 100, TimerIntervalInMS);
            var timer = new Timer(_ => TimerCallback(), null, Timeout.Infinite, TimerIntervalInMS);
        }

        private void TimerCallback()
        {
            UpdateBatteryParameters();
        }

        private void UpdateBatteryParameters()
        {
            switch (Step.Action.Mode)
            {
                case ActionMode.CC_CV_CHARGE:       //
                    if (Battery.Voltage < Step.Action.Voltage)
                    {
                        Battery.Current = Step.Action.Current;
                        Battery.Voltage += Battery.ChargeVoltageSlope * TimerIntervalInMS / 1000.0;
                        if (Battery.Voltage > Step.Action.Voltage)
                            Battery.Voltage = Step.Action.Voltage;
                    }
                    else
                        Battery.Current -= Battery.ChargeCurrentSlope * TimerIntervalInMS / 1000.0;

                    Battery.RemainCapacity += Battery.Current / 1000 * (TimerIntervalInMS / 1000.0) / 3600;

                    if (Battery.Temperature > Battery.EnvTemperature)
                        Battery.Temperature -= Battery.NonDischargeTemperatureSlope * TimerIntervalInMS / 1000.0;
                    else
                        Battery.Temperature = Battery.EnvTemperature;
                    break;
                case ActionMode.CC_DISCHARGE:
                    if (Battery.Voltage > Step.CutOffBehaviors.SingleOrDefault(cob => cob.Condition.Parameter == Parameter.VOLTAGE).Condition.Value)
                    {
                        Battery.Voltage -= Battery.DischargeVoltageSlope * TimerIntervalInMS / 1000.0;
                        Battery.Current = Step.Action.Current;
                        Battery.Temperature += Battery.DischargeTemperatureSlope * TimerIntervalInMS / 1000.0;
                    }
                    else
                    {
                        Battery.Voltage = Step.CutOffBehaviors.SingleOrDefault(cob => cob.Condition.Parameter == Parameter.VOLTAGE).Condition.Value;
                        Battery.Current = 0;
                    }
                    break;
                case ActionMode.CP_DISCHARGE:
                    if (Battery.Voltage > Step.CutOffBehaviors.SingleOrDefault(cob => cob.Condition.Parameter == Parameter.VOLTAGE).Condition.Value)
                    {
                        Battery.Voltage -= Battery.DischargeVoltageSlope * TimerIntervalInMS / 1000.0;
                        Battery.Current = Step.Action.Power / Battery.Voltage * 1000;
                        Battery.Temperature += Battery.DischargeTemperatureSlope * TimerIntervalInMS / 1000.0;
                    }
                    else
                    {
                        Battery.Voltage = Step.CutOffBehaviors.SingleOrDefault(cob => cob.Condition.Parameter == Parameter.VOLTAGE).Condition.Value;
                        Battery.Current = 0;
                    }
                    break;
                case ActionMode.REST:       //
                    Battery.Current = 0;

                    if (Battery.Temperature > Battery.EnvTemperature)
                        Battery.Temperature -= Battery.NonDischargeTemperatureSlope * TimerIntervalInMS / 1000.0;
                    else
                        Battery.Temperature = Battery.EnvTemperature;
                    break;
            }
        }
    }
}