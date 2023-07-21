//#define debug
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SmartTester
{
    public class Configuration
    {
        public List<IChamber> Chambers;
        public List<ITester> Testers;
        public Configuration(List<IChamber> chambers, List<ITester> testers)
        {
            this.Chambers = chambers;
            this.Testers = testers;
        }
    }
}