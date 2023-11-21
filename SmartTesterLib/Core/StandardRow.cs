using System;

namespace SmartTesterLib
{
    public class StandardRow : IRow
    {
        public uint Index { get; set; }
        public uint TimeInMS { get; set; }
        public ActionMode Mode { get; set; }
        public double Current { get; set; } //mA，充电为正，放电为负
        public double Voltage { get; set; } //mV
        public double Temperature { get; set; } //celcius
        public double Capacity { get; set; }    //mAh
        public double TotalCapacity { get; set; }   //mAh
        public RowStatus Status { get; set; }
        public override string ToString()
        {
            return $@"{Index},{TimeInMS},{(byte)Mode},{Current.ToString("f4")},{Voltage.ToString("f4")},{Temperature.ToString("f4")},{Capacity.ToString("f4")},{TotalCapacity.ToString("f4")},{(byte)Status}";
        }
        public StandardRow()
        {
        }
        public void LoadFromString(string line)
        {
            var strArray = line.Split(',');
            uint Index; uint TimeInMS; ActionMode Mode; double Current; double Voltage; double Temperature; double Capacity; double TotalCapacity; RowStatus Status;
            if (!uint.TryParse(strArray[0], out Index))
                return;
            if (!uint.TryParse(strArray[1], out TimeInMS))
                return;
            byte mode;
            if (!byte.TryParse(strArray[2], out mode))
                return;
            Mode = (ActionMode)mode;
            if (!double.TryParse(strArray[3], out Current))
                return;
            if (!double.TryParse(strArray[4], out Voltage))
                return;
            if (!double.TryParse(strArray[5], out Temperature))
                return;
            if (!double.TryParse(strArray[6], out Capacity))
                return;
            if (!double.TryParse(strArray[7], out TotalCapacity))
                return;
            byte status;
            if (!byte.TryParse(strArray[8], out status))
                return;
            Status = (RowStatus)status;
            this.Index = Index;
            this.TimeInMS = TimeInMS;
            this.Mode = Mode;
            this.Current = Current;
            this.Voltage = Voltage;
            this.Temperature = Temperature;
            this.Capacity = Capacity;
            this.TotalCapacity = TotalCapacity;
            this.Status = Status;
        }

        internal StandardRow Clone()
        {
            return (StandardRow)this.MemberwiseClone();
        }
    }
}