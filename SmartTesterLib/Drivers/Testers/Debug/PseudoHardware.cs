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
        public Timer Timer { get; set; }

        public PseudoHardware(int interval, double ccs, double cvs, double dvs, double dts, double ndts)
        {
            Stopwatch = new Stopwatch();
            DataLength = 20;
            DataQueues = new Queue<StandardRow>();
            Battery = new PseudoBattery(3600, 0, 0, 3600, 20);
            Battery.ChargeCurrentSlope = ccs;
            Battery.ChargeVoltageSlope = cvs;
            Battery.DischargeVoltageSlope = dvs;
            Battery.EnvTemperature = 0;
            Battery.DischargeTemperatureSlope = dts;
            Battery.NonDischargeTemperatureSlope = ndts;
            TimerIntervalInMS = interval;
            //var timer = new Timer(_ => TimerCallback(), null, 100, TimerIntervalInMS);
            Timer = new Timer(_ => TimerCallback());
        }

        private void TimerCallback()
        {
            UpdateBatteryParameters();
        }

        private void UpdateBatteryParameters()
        {
            Random random = new Random();

            double voltageOffset = random.Next(-10, 10);
            double currentOffset = random.Next(-5, 5);
            double temperatureOffset = random.Next(-1, 1);
            switch (Step.Action.Mode)
            {
                case ActionMode.CC_CV_CHARGE:       //
                    if (Battery.Voltage < Step.Action.Voltage)
                    {
                        Battery.Current = Step.Action.Current;
                        Battery.Voltage += Battery.ChargeVoltageSlope * TimerIntervalInMS / 1000.0 + voltageOffset;
                        if (Battery.Voltage > Step.Action.Voltage)
                            Battery.Voltage = Step.Action.Voltage;
                    }
                    else
                        Battery.Current -= Battery.ChargeCurrentSlope * TimerIntervalInMS / 1000.0 + currentOffset;

                    Battery.RemainCapacity += Battery.Current / 1000 * (TimerIntervalInMS / 1000.0) / 3600;

                    if (Battery.Temperature > Battery.EnvTemperature)
                        Battery.Temperature -= Battery.NonDischargeTemperatureSlope * TimerIntervalInMS / 1000.0 + temperatureOffset;
                    else
                        Battery.Temperature = Battery.EnvTemperature;
                    break;
                case ActionMode.CC_DISCHARGE:
                    //if (Battery.Voltage > Step.CutOffBehaviors.SingleOrDefault(cob => cob.Condition.Parameter == Parameter.VOLTAGE).Condition.Value)
                    {
                        Battery.Voltage -= Battery.DischargeVoltageSlope * TimerIntervalInMS / 1000.0 + voltageOffset;
                        Battery.Current = Step.Action.Current;
                        Battery.Temperature += Battery.DischargeTemperatureSlope * TimerIntervalInMS / 1000.0 + temperatureOffset;
                    }
                    //else
                    //{
                    //    Battery.Voltage = Step.CutOffBehaviors.SingleOrDefault(cob => cob.Condition.Parameter == Parameter.VOLTAGE).Condition.Value;
                    //    Battery.Current = 0;
                    //}
                    Battery.RemainCapacity -= Battery.Current / 1000 * (TimerIntervalInMS / 1000.0) / 3600;
                    break;
                case ActionMode.CP_DISCHARGE:
                    if (Battery.Voltage > Step.CutOffBehaviors.SingleOrDefault(cob => cob.Condition.Parameter == Parameter.VOLTAGE).Condition.Value)
                    {
                        Battery.Voltage -= Battery.DischargeVoltageSlope * TimerIntervalInMS / 1000.0 + voltageOffset;
                        Battery.Current = Step.Action.Power / Battery.Voltage * 1000;
                        Battery.Temperature += Battery.DischargeTemperatureSlope * TimerIntervalInMS / 1000.0 + temperatureOffset;
                    }
                    else
                    {
                        Battery.Voltage = Step.CutOffBehaviors.SingleOrDefault(cob => cob.Condition.Parameter == Parameter.VOLTAGE).Condition.Value;
                        Battery.Current = 0;
                    }
                    Battery.RemainCapacity -= Battery.Current / 1000 * (TimerIntervalInMS / 1000.0) / 3600;
                    break;
                case ActionMode.REST:       //
                    Battery.Current = 0;

                    if (Battery.Temperature > Battery.EnvTemperature)
                        Battery.Temperature -= Battery.NonDischargeTemperatureSlope * TimerIntervalInMS / 1000.0 + temperatureOffset;
                    else
                        Battery.Temperature = Battery.EnvTemperature;
                    break;
            }
            foreach (var cob in Step.CutOffBehaviors)
            {
                double leftValue, rightValue;
                switch (cob.Condition.Parameter)
                {
                    case Parameter.CURRENT:
                        leftValue = Battery.Current;
                        rightValue = cob.Condition.Value;
                        break;
                    case Parameter.POWER:
                        leftValue = Battery.Voltage * Battery.Current / 1000;
                        rightValue = cob.Condition.Value;
                        break;
                    case Parameter.TEMPERATURE:
                        leftValue = Battery.Temperature;
                        rightValue = cob.Condition.Value;
                        break;
                    case Parameter.TIME:
                        leftValue = Stopwatch.Elapsed.TotalMilliseconds;
                        rightValue = cob.Condition.Value * 1000;
                        break;
                    case Parameter.VOLTAGE:
                        leftValue = Battery.Voltage;
                        rightValue = cob.Condition.Value;
                        break;
                    default:
                        return; //不可能
                }
                if (ConditionCheck(leftValue, rightValue, cob.Condition.Mark))
                {
                    Stop();
                }
            }
        }

        private bool ConditionCheck(double leftValue, double rightValue, CompareMarkEnum mark)
        {
            switch (mark)
            {
                case CompareMarkEnum.LargerThan:
                    return leftValue >= rightValue;
                case CompareMarkEnum.SmallerThan:
                    return leftValue <= rightValue;
                case CompareMarkEnum.EqualTo:
                    return leftValue == rightValue;
            }
            return false;
        }

        internal StandardRow GetStandardRow()
        {
            StandardRow output = new StandardRow();
            output.TimeInMS = (uint)Stopwatch.ElapsedMilliseconds;
            output.Current = Battery.Current;
            output.Mode = Step.Action.Mode;
            output.Temperature = Battery.Temperature;
            output.Voltage = Battery.Voltage;
            output.Capacity = Battery.RemainCapacity;
            RowStatus rowStatus;
            foreach (var cob in Step.CutOffBehaviors)
            {
                double leftValue, rightValue;
                switch (cob.Condition.Parameter)
                {
                    case Parameter.CURRENT:
                        leftValue = Battery.Current;
                        rightValue = cob.Condition.Value;
                        rowStatus = RowStatus.CUT_OFF_BY_CURRENT;
                        break;
                    case Parameter.POWER:
                        leftValue = Battery.Voltage * Battery.Current / 1000;
                        rightValue = cob.Condition.Value;
                        rowStatus = RowStatus.CUT_OFF_BY_POWER;
                        break;
                    case Parameter.TEMPERATURE:
                        leftValue = Battery.Temperature;
                        rightValue = cob.Condition.Value;
                        rowStatus = RowStatus.CUT_OFF_BY_TEMPERATURE;
                        break;
                    case Parameter.TIME:
                        leftValue = output.TimeInMS;
                        rightValue = cob.Condition.Value * 1000;
                        rowStatus = RowStatus.CUT_OFF_BY_TIME;
                        break;
                    case Parameter.VOLTAGE:
                        leftValue = Battery.Voltage;
                        rightValue = cob.Condition.Value;
                        rowStatus = RowStatus.CUT_OFF_BY_VOLTAGE;
                        break;
                    default:
                        return null; //不可能
                }
                if (ConditionCheck(leftValue, rightValue, cob.Condition.Mark))
                {
                    output.Status = rowStatus;
                }
            }
            return output;
        }

        internal void Start()
        {
            Stopwatch.Restart();
            Timer.Change(TimerIntervalInMS, TimerIntervalInMS);
        }

        private void Stop()
        {
            Stopwatch.Stop();
            Timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}