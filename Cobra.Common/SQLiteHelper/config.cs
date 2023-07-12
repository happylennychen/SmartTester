using System;
using System.Collections.Generic;
using System.Text;

namespace Cobra.Common
{
    public class config
    {
        public static string DatabaseFile = string.Empty;
        public static string DataSource
        {
            get
            {
                return string.Format("data source={0}", DatabaseFile);
            }
        }
    }
}
