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
    public class Tests
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
            var files = Directory.GetFiles(@"D:\BC_Lab\SW Design\Instrument Automation\40T init(2)\", "*.txt");
            for(int i=1;i<=8;i++)
            {
                var fileList = files.Where(o => o.Contains($"Chroma17208M-Ch{i}")).OrderBy(o=>o).ToList();
                Utilities.FileConvert(fileList, CreateFullSteps(), -10);
            }
            //List<string> fileList = new List<string>();
            //fileList.Add(@"D:\BC_Lab\SW Design\Instrument Automation\File Converter\30T auto init 2\Chroma17208M-Ch1-20220630160748.txt");
            //fileList.Add(@"D:\BC_Lab\SW Design\Instrument Automation\File Converter\30T auto init\Chroma17208M-Ch1-20220630180635.txt");
        }
        private static List<Step> CreateFullSteps()
        {
            Step chargeStep = new Step() { Index = 1, Action = new TesterAction() { Mode = ActionMode.CC_CV_CHARGE, Voltage = 4200, Current = 2000, Power = 0 } };
            JumpBehavior jpb = new JumpBehavior() { JumpType = JumpType.NEXT };
            Condition cdt = new Condition() { Parameter = Parameter.CURRENT, Mark = CompareMarkEnum.SmallerThan, Value = 200 };
            CutOffBehavior cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            chargeStep.CutOffBehaviors.Add(cob);


            Step idleStep = new Step() { Index = 2, Action = new TesterAction() { Mode = ActionMode.REST, Voltage = 0, Current = 0, Power = 0 } };
            jpb = new JumpBehavior() { JumpType = JumpType.NEXT };
            cdt = new Condition() { Parameter = Parameter.TIME, Mark = CompareMarkEnum.LargerThan, Value = 1800 };
            cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            idleStep.CutOffBehaviors.Add(cob);

            Step dischargeStep = new Step() { Index = 3, Action = new TesterAction() { Mode = ActionMode.CC_DISCHARGE, Voltage = 0, Current = 4000, Power = 0 } };
            jpb = new JumpBehavior() { JumpType = JumpType.NEXT };
            cdt = new Condition() { Parameter = Parameter.VOLTAGE, Mark = CompareMarkEnum.SmallerThan, Value = 2500 };
            cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            dischargeStep.CutOffBehaviors.Add(cob);

            return new List<Step> { chargeStep, idleStep, dischargeStep};
        }

        [Fact]
        public void LoadFromFileShouldWork()
        {
            Utilities.LoadTestFromFile(@"D:\BC_Lab\SW Design\Instrument Automation\Test Plan Json\");
        }
        [Fact]
        public void GetAdjustedRowShouldWork()
        {
            Step step = new Step();
            step.Action = new TesterAction(ActionMode.CP_DISCHARGE, 0, 0, 16000);
            var cob = new CutOffBehavior();
            cob.Condition = new Condition() { Parameter = Parameter.VOLTAGE, Value = 2500};
            step.CutOffBehaviors.Add(cob);
            List<StandardRow> standardRows = new List<StandardRow>();
            //standardRows.Add(new StandardRow("0,58000,2,-3117.825,2567.09,32.15,-48.52404,0,0"));
            standardRows.Add(new StandardRow("0,59000,2,-3124.468,2562.845,32.1,-49.39121,0,0"));
            standardRows.Add(new StandardRow("0,60000,2,-3128.044,2558.552,32.14,-50.25982,0,0"));
            standardRows.Add(new StandardRow("0,60012,2,-3128.086912,2558.500484,32.11,-50.27025,0,8"));
            standardRows.Add(new StandardRow("0,334,2,-7.898484E-05,2648.892,32.14,-0.5672878,0,8"));
            Tester test = new Tester();
            PrivateObject poTest = new PrivateObject(test);
            StandardRow stdrow = (StandardRow)poTest.Invoke("GetAdjustedRow", standardRows, step);
            Assert.Equal(2500.0, stdrow.Voltage);
            Assert.Equal(16000.0 / 2500.0 * 1000, stdrow.Current);
        }
    }
}
