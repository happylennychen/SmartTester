using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTesterLib
{
    public static class GlobalSettings
    {
        public static string OutputFolder { get; set; }
        //public static Dictionary<IChamber, int> ChamberRoundIndex { get; set; } = new Dictionary<IChamber, int>();
        //public static string ConfigurationFilePath { get; internal set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration.json");
        //public static string TestPlanFolderPath { get; set; } = Path.Combine(System.Environment.CurrentDirectory, "Test Plan\\");
        public static string ConfigurationFilePath { get; internal set; } = "D:\\O2Micro\\Source Codes\\BC Lab\\ST\\SmartTester\\Configuration\\Configuration.json";
        public static string TestPlanFolderPath { get; set; } = "D:\\O2Micro\\Source Codes\\BC Lab\\ST\\SmartTester\\Configuration\\Test Plan\\";
        public static string ConsoleFolderPath { get; set; } = Path.Combine(System.Environment.CurrentDirectory, "Console\\");
    }
}
