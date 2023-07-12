using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Cobra.Common
{
    public interface IOutPut
    {
        void Print(HoundInfo sInfo);
    }

    public class Hound
    {
        private static IOutPut output;
        #region 公有方法
        public static void Register(IOutPut target)
        {
            if (target != null)
                output = target;
        }

        public static void Log(HoundInfo info)
        {
            if (info != null)
            {
                output.Print(info);
            }
        }
        #endregion
    }

}
