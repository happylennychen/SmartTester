using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTester
{
    public static class GlobalSettings
    {
        public static string OutputFolder { get; set; }
        public static int RoundIndex { get; set; }
        public static string ConfigurationFilePath { get; internal set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration.json");
    }
}
