//#define debug
#define debugChamber
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartTester;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

namespace UnitTest
{
    public class LeonTests
    {
        [Theory]
        [InlineData(@"D:\BC_Lab\SW Design\Instrument Automation\File Converter\30T auto init\Chroma17208M-Ch1-20220630160748.csv", 7121060, @"D:\BC_Lab\SW Design\Instrument Automation\File Converter\30T auto init\Chroma17208M-Ch1-20220630160748-20220630180629.csv")]
        public void GetNewFileFullPathShouldWork(string fileName, uint lastTimeInMS, string expected)
        {
            var actual = Utilities.GetNewFileFullPath(fileName, lastTimeInMS);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FileConvertShouldWork()
        {
            var files = Directory.GetFiles(@"D:\BC_Lab\SW Design\Instrument Automation\40T INIT txt A V W\", "*.txt");
            for (int i = 1; i <= 8; i++)
            {
                var fileList = files.Where(o => o.Contains($"Chroma17208M-Ch{i}")).OrderBy(o => o).ToList();
                Utilities.StdFileConvert(fileList, CreateFullSteps(), -10);
            }
            //List<string> fileList = new List<string>();
            //fileList.Add(@"D:\BC_Lab\SW Design\Instrument Automation\File Converter\30T auto init 2\Chroma17208M-Ch1-20220630160748.txt");
            //fileList.Add(@"D:\BC_Lab\SW Design\Instrument Automation\File Converter\30T auto init\Chroma17208M-Ch1-20220630180635.txt");
        }
        private static List<SmartTesterStep> CreateFullSteps()
        {
            SmartTesterStep chargeStep = new SmartTesterStep() { Index = 1, Action = new TesterAction() { Mode = ActionMode.CC_CV_CHARGE, Voltage = 4200, Current = 2000, Power = 0 } };
            JumpBehavior jpb = new JumpBehavior() { JumpType = JumpType.NEXT };
            Condition cdt = new Condition() { Parameter = Parameter.CURRENT, Mark = CompareMarkEnum.SmallerThan, Value = 200 };
            CutOffBehavior cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            chargeStep.CutOffBehaviors.Add(cob);


            SmartTesterStep idleStep = new SmartTesterStep() { Index = 2, Action = new TesterAction() { Mode = ActionMode.REST, Voltage = 0, Current = 0, Power = 0 } };
            jpb = new JumpBehavior() { JumpType = JumpType.NEXT };
            cdt = new Condition() { Parameter = Parameter.TIME, Mark = CompareMarkEnum.LargerThan, Value = 1800 };
            cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            idleStep.CutOffBehaviors.Add(cob);

            SmartTesterStep dischargeStep = new SmartTesterStep() { Index = 3, Action = new TesterAction() { Mode = ActionMode.CC_DISCHARGE, Voltage = 0, Current = 4000, Power = 0 } };
            jpb = new JumpBehavior() { JumpType = JumpType.NEXT };
            cdt = new Condition() { Parameter = Parameter.VOLTAGE, Mark = CompareMarkEnum.SmallerThan, Value = 2500 };
            cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            dischargeStep.CutOffBehaviors.Add(cob);

            return new List<SmartTesterStep> { chargeStep, idleStep, dischargeStep };
        }

        [Fact]
        public void GetAdjustedRowShouldWork()
        {
            SmartTesterStep step = new SmartTesterStep();
            step.Action = new TesterAction(ActionMode.CP_DISCHARGE, 0, 0, 16000);
            var cob = new CutOffBehavior();
            cob.Condition = new Condition() { Parameter = Parameter.VOLTAGE, Value = 2500 };
            step.CutOffBehaviors.Add(cob);
            List<StandardRow> standardRows = new List<StandardRow>();
            //standardRows.Add(new StandardRow("0,58000,2,-3117.825,2567.09,32.15,-48.52404,0,0"));
            standardRows.Add(new StandardRow("0,59000,2,-3124.468,2562.845,32.1,-49.39121,0,0"));
            standardRows.Add(new StandardRow("0,60000,2,-3128.044,2558.552,32.14,-50.25982,0,0"));
            standardRows.Add(new StandardRow("0,60012,2,-3128.086912,2558.500484,32.11,-50.27025,0,8"));
            standardRows.Add(new StandardRow("0,334,2,-7.898484E-05,2648.892,32.14,-0.5672878,0,8"));
            Chroma17208 test = new Chroma17208();
            PrivateObject poTest = new PrivateObject(test);
            StandardRow stdrow = (StandardRow)poTest.Invoke("GetAdjustedRow", standardRows, step);
            Assert.Equal(2500.0, stdrow.Voltage);
            Assert.Equal(16000.0 / 2500.0 * 1000, stdrow.Current);
        }

        [Fact]
        public void SaveAndLoadConfigurationShouldWork()
        {
#if debug
            var tester = new DebugTester(1, "17208Auto", 8, "192.168.1.23", 8802, "TCPIP0::192.168.1.101::60000::SOCKET");
            var chamber = new DebugChamber(1, "Hongzhan", "PUL-80", 150, -40);
            List<DebugChamber> chambers = new List<DebugChamber>();
            List<DebugTester> testers = new List<DebugTester>();
#elif debugChamber
            var tester = new PackTester(1, "17208Auto", 8, "192.168.1.23", 8802, "TCPIP0::192.168.1.101::60000::SOCKET");
            var chamber = new DebugChamber(1, "Hongzhan", "PUL-80", 150, -40);
            List<DebugChamber> chambers = new List<DebugChamber>();
            List<PackTester> testers = new List<PackTester>();
#else
            var tester = new PackTester(1, "17208Auto", 8, "192.168.1.23", 8802, "TCPIP0::192.168.1.101::60000::SOCKET");
            var chamber = new Chamber(1, "Hongzhan", "PUL-80", 150, -40, "192.168.1.102", 3000);
            List<Chamber> chambers = new List<Chamber>();
            List<PackTester> testers = new List<PackTester>();
#endif
            chambers.Add(chamber);
            testers.Add(tester);
            Configuration conf1 = new Configuration(chambers, testers);
            Configuration conf2;
            Utilities.SaveConfiguration(chambers, testers);
            Utilities.LoadConfiguration(out conf2);
            Assert.True(conf1.Testers.Count == conf2.Testers.Count);
            Assert.True(conf1.Chambers.Count == conf2.Chambers.Count);
        }

        public static IEnumerable<object[]> GetTestPlanFolderTreeAndProjectName()
        {
            List<IChamber> chambers = new List<IChamber>();
            List<ITester> testers = new List<ITester>();
            for (int i = 1; i < 10; i++)
            {
                IChamber chamber = new Chamber(i, "Hongzhan", $"Chamber{i.ToString()}", 120, -40);
                ITester tester = new Chroma17208(i, $"Tester{i.ToString()}", 8);
                chambers.Add(chamber);
                testers.Add(tester);
            }

            Dictionary<IChamber, Dictionary<int, List<IChannel>>> testPlanFolderTree1 = new Dictionary<IChamber, Dictionary<int, List<IChannel>>>();
            Dictionary<int, List<IChannel>> dic1 = new Dictionary<int, List<IChannel>>();
            dic1.Add(1, new List<IChannel>() { testers[0].Channels[0], testers[0].Channels[1], testers[0].Channels[2], testers[0].Channels[3] });
            testPlanFolderTree1.Add(chambers[0], dic1);
            yield return new object[] { testPlanFolderTree1, "Project3" };



            Dictionary<IChamber, Dictionary<int, List<IChannel>>> testPlanFolderTree2 = new Dictionary<IChamber, Dictionary<int, List<IChannel>>>();
            Dictionary<int, List<IChannel>> dic2 = new Dictionary<int, List<IChannel>>();
            testPlanFolderTree2.Add(chambers[0], dic1);
            dic2.Add(1, new List<IChannel>() { testers[0].Channels[0], testers[0].Channels[1], testers[0].Channels[2], testers[0].Channels[3] });
            dic2.Add(2, new List<IChannel>() { testers[0].Channels[0], testers[0].Channels[1], testers[0].Channels[2], testers[0].Channels[3] });
            dic2.Add(3, new List<IChannel>() { testers[0].Channels[0], testers[0].Channels[1], testers[0].Channels[2], testers[0].Channels[3] });
            dic2.Add(4, new List<IChannel>() { testers[4].Channels[2], testers[5].Channels[3], testers[6].Channels[4] });
            testPlanFolderTree2.Add(chambers[3], dic2);
            yield return new object[] { testPlanFolderTree2, "Project4" };

        }
        [Theory]
        [MemberData(nameof(GetTestPlanFolderTreeAndProjectName))]
        public void CreateTestPlanFoldersShouldWork(Dictionary<IChamber, Dictionary<int, List<IChannel>>> testPlanFolderTree, string projectName)
        {
            //Dictionary<Chamber, Dictionary<int, List<Channel>>> testPlanFolderTree = new Dictionary<Chamber, Dictionary<int, List<Channel>>>();
            //Chamber chamber = new Chamber(1, "Hongzhan", "PUL80", 120, -40);
            //Tester tester = new Tester(1, "17208M", 8);

            //Dictionary<int, List<Channel>> value = new Dictionary<int, List<Channel>>();
            //value.Add(1, new List<Channel>() { tester.Channels[0], tester.Channels[1], tester.Channels[2], tester.Channels[3] });
            //testPlanFolderTree.Add(chamber, value);
            //string projectName = "Project2";
            Utilities.CreateTestPlanFolders(projectName, testPlanFolderTree);

            List<bool> bList = new List<bool>();

            string projectPath = Path.Combine(GlobalSettings.TestPlanFolderPath, projectName);
            bList.Add(Directory.Exists(projectPath));

            foreach (var cmb in testPlanFolderTree.Keys)
            {
                string chamberPath = Path.Combine(projectPath, cmb.Name);
                bList.Add(Directory.Exists(chamberPath));

                Directory.CreateDirectory(projectPath);
                foreach (var roundIndex in testPlanFolderTree[cmb].Keys)
                {
                    string roundPath = Path.Combine(chamberPath, roundIndex.ToString());
                    bList.Add(Directory.Exists(roundPath));
                    var testers = testPlanFolderTree[cmb][roundIndex].Select(ch => ch.Tester).Distinct().ToList();
                    foreach (var tst in testers)
                    {
                        string testerPath = Path.Combine(roundPath, tst.Name);
                        bList.Add(Directory.Exists(testerPath));
                        var channels = testPlanFolderTree[cmb][roundIndex].Where(ch => ch.Tester == tst).ToList();
                        foreach (var channel in channels)
                        {
                            string channelPath = Path.Combine(testerPath, channel.Index.ToString());
                            bList.Add(Directory.Exists(channelPath));
                        }
                    }
                }
            }
            var allExisted = bList.All(b => b == true);
            Assert.True(allExisted);
        }
    }
}
