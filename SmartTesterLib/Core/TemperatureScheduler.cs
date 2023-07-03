using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartTester
{
    public class TemperatureScheduler
    {
        private List<TemperatureUnit> TemperatureUintList { get; set; }
        public bool IsCompleted { get { return TemperatureUintList.All(tu => tu.Status != TemperatureStatus.WAITING); } }
        public TemperatureScheduler()
        {
            TemperatureUintList = new List<TemperatureUnit>();
        }

        public bool IsTemperatureSchedulerCompatible(List<TemperatureTarget> ts)    //目前是1.完全一致2.原来未空 才兼容
        {
            if (TemperatureUintList.Count == 0)
                return true;
            for (int i = 0; i < TemperatureUintList.Count; i++)
            {
                if (TemperatureUintList[i].Target.IsCritical != ts[i].IsCritical || TemperatureUintList[i].Target.Value != ts[i].Value)
                    return false;
            }
            return true;
        }

        public void UpdateTemperatureScheduler(ref SmartTesterRecipe recipe)                   //同时绑定了Recipe.Steps中的TemperatureUnit到TemperatureUnitList中的TemperatureUnit
        {
            if (TemperatureUintList.Count == 0)
            {
                var tu = new TemperatureUnit();
                tu.Target = recipe.Steps[0].Target;
                tu.Status = TemperatureStatus.WAITING;
                TemperatureUintList.Add(tu);
            }
            int j = 0;
            for (int i = 0; i < recipe.Steps.Count; i++)
            {
                if (recipe.Steps[i].Target.EqualsTo(TemperatureUintList[j].Target))
                {
                    recipe.Steps[i].TemperatureUint = TemperatureUintList[j];
                }
                else//当前的j跟i对不上
                {
                    if (j < TemperatureUintList.Count - 1)  //j还没到最后
                        j++;                            //试试下一个
                    else//已经是最后一个了，还对不上 
                    {
                        //创建一个
                        var tu = new TemperatureUnit();
                        tu.Target = recipe.Steps[i].Target;
                        tu.Status = TemperatureStatus.WAITING;
                        TemperatureUintList.Add(tu);
                        recipe.Steps[i].TemperatureUint = tu;
                        j++;
                    }
                }
            }
        }

        public TemperatureUnit GetNextTemp()
        {
            if (TemperatureUintList.All(tu => tu.Status != TemperatureStatus.WAITING))
                return null;
            return TemperatureUintList.First(tu => tu.Status == TemperatureStatus.WAITING);
        }

        public TemperatureUnit GetCurrentTemp()
        {
            if (TemperatureUintList.All(tu => tu.Status != TemperatureStatus.REACHING && tu.Status != TemperatureStatus.REACHED))
                return null;
            return TemperatureUintList.First(tu => tu.Status == TemperatureStatus.REACHING || tu.Status == TemperatureStatus.REACHED);
        }
        //public void SetCurrentTempStatusToReached()
        //{
        //    var tu = GetCurrentTemp();
        //    tu.Status = TemperatureStatus.REACHED;
        //}
    }
}