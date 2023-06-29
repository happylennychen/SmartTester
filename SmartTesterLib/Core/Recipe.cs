using System.Collections.Generic;
using System.Linq;

namespace SmartTester
{
    public class Recipe
    {
        public string Name { get; set; }
        public List<Step> Steps { get; set; }

        public List<TargetTemperature> GetTemperaturePoints()
        {
            var temps = Steps.Select(st => st.Temperature).ToList();
            List<TargetTemperature> uniqueTemps = new List<TargetTemperature>();    //去掉连续重复的温度点
            TargetTemperature lastTemp = null;
            foreach (var temp in temps)
            {
                if (uniqueTemps.Count == 0)
                {
                    uniqueTemps.Add(temp);
                    lastTemp = temp;
                }
                else
                {
                    if (temp.IsCritical != lastTemp.IsCritical || temp.Temperature == lastTemp.Temperature)
                    {
                        uniqueTemps.Add(temp);
                        lastTemp = temp;
                    }
                }
            }
            return uniqueTemps;
        }
        public IChamber Chamber { get; set; }   //尝试去掉
        public IChannel Channel { get; set; }   //尝试去掉
        //public double DischargeTemperature { get; set; }
    }
}