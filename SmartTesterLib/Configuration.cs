//#define debug
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SmartTester
{
    public class Configuration
    {
#if debug        
        public List<DebugChamber> Chambers { get; set; }
        public List<DebugTester> Testers { get; set; }
#else
        public List<Chamber> Chambers;
        public List<Tester> Testers;
#endif

#if debug
        public Configuration(List<IChamber> chambers, List<ITester> testers)
        {
            this.Chambers = chambers.Select(cmb=>(DebugChamber)cmb).ToList();
            this.Testers = testers.Select(tst=>(DebugTester)tst).ToList();
        }
        [JsonConstructor]
        public Configuration(List<DebugChamber> chambers, List<DebugTester> testers)
        {
            this.Chambers = chambers;
            this.Testers = testers;
        }
#else
        public Configuration(List<IChamber> chambers, List<ITester> testers)
        {
            this.Chambers = chambers.Select(cmb=>(Chamber)cmb).ToList();
            this.Testers = testers.Select(tst=>(Tester)tst).ToList();
        }
        [JsonConstructor]
        public Configuration(List<Chamber> chambers, List<Tester> testers)
        {
            this.Chambers = chambers;
            this.Testers = testers;
        }
#endif
    }
}