namespace SmartTester
{
    public class PseudoBattery
    {
        public double FullCapacity { get; set; }
        public double RemainCapacity { get; set; }
        public double Current { get; set; }
        public double Voltage { get; set; }
        public double Temperature { get; set; }
        public double ChargeCurrentSlope { get; internal set; }
        public double ChargeVoltageSlope { get; internal set; }
        public double EnvTemperature { get; internal set; }
        public double NonDischargeTemperatureSlope { get; internal set; }
        public double DischargeTemperatureSlope { get; internal set; }
        public double DischargeVoltageSlope { get; internal set; }

        public PseudoBattery(double fullCapacity, double remainCapacity, double current, double voltage, double temperature)
        {
            FullCapacity = fullCapacity;
            RemainCapacity = remainCapacity;
            Current = current;
            Voltage = voltage;
            Temperature = temperature;
        }
    }
}