using System.Collections.Generic;

namespace SmartTester
{
    public interface IRow
    {
        public uint Index { get; set; }
        public uint TimeInMS { get; set; }
        public ActionMode Mode { get; set; }
        public double Current { get; set; } //mA，充电为正，放电为负
        public double Voltage { get; set; } //mV
        public double Temperature { get; set; } //celcius
        public RowStatus Status { get; set; }
        public void LoadFromString(string line);    //Load Properties from string
    }
}