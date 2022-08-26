using System.Collections.Generic;

namespace SmartTester
{
    public class Configuration
    {
        public List<Chamber> Chambers;
        public List<Tester> Testers;

        public Configuration(List<Chamber> chambers, List<Tester> testers)
        {
            this.Chambers = chambers;
            this.Testers = testers;
        }
    }
}