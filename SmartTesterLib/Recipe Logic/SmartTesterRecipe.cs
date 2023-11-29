using System.Collections.Generic;
using System.Linq;

namespace SmartTesterLib
{
    public class SmartTesterRecipe
    {
        public string Name { get; set; }
        public List<SmartTesterStep> Steps { get; set; }

        public List<TemperatureTarget> GetUniqueTemperaturePoints()
        {
            var temps = Steps.Select(st => st.Target).ToList();
            List<TemperatureTarget> uniqueTemps = new List<TemperatureTarget>();    //去掉连续重复的温度点
            TemperatureTarget lastTemp = null;
            foreach (var temp in temps)
            {
                if (uniqueTemps.Count == 0)
                {
                    uniqueTemps.Add(temp);
                    lastTemp = temp;
                }
                else
                {
                    if (temp.IsCritical != lastTemp.IsCritical || temp.Value != lastTemp.Value)
                    {
                        uniqueTemps.Add(temp);
                        lastTemp = temp;
                    }
                }
            }
            return uniqueTemps;
        }
        //public IChamber Chamber { get; set; }   //尝试去掉
        //public IChannel Channel { get; set; }   //尝试去掉
        //public double DischargeTemperature { get; set; }
        public override string ToString()
        {
            return this.Name;
        }
    }
}