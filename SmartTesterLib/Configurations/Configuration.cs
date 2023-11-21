//#define debug
//using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SmartTesterLib
{
    public class Configuration
    {
        public List<IChamber> Chambers;
        public List<ITester> Testers;
        public Configuration()
        {
            Chambers = new List<IChamber>();
            Testers = new List<ITester>();
        }
    }
}