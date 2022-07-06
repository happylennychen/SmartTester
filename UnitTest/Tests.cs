using SmartTester;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

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
            var files = Directory.GetFiles(@"D:\BC_Lab\SW Design\Instrument Automation\File Converter\30T auto init 2\", "*.txt");
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
            Step chargeStep = new Step() { Index = 1, Action = new TesterAction() { Mode = ActionMode.CC_CV_CHARGE, Voltage = 4200, Current = 1500, Power = 0 } };
            JumpBehavior jpb = new JumpBehavior() { JumpType = JumpType.NEXT };
            Condition cdt = new Condition() { Parameter = Parameter.CURRENT, Mark = CompareMarkEnum.SmallerThan, Value = 150 };
            CutOffBehavior cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            chargeStep.CutOffBehaviors.Add(cob);


            Step idleStep = new Step() { Index = 2, Action = new TesterAction() { Mode = ActionMode.REST, Voltage = 0, Current = 0, Power = 0 } };
            jpb = new JumpBehavior() { JumpType = JumpType.NEXT };
            cdt = new Condition() { Parameter = Parameter.TIME, Mark = CompareMarkEnum.LargerThan, Value = 1800 };
            cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            idleStep.CutOffBehaviors.Add(cob);

            Step dischargeStep = new Step() { Index = 3, Action = new TesterAction() { Mode = ActionMode.CC_DISCHARGE, Voltage = 0, Current = 3000, Power = 0 } };
            jpb = new JumpBehavior() { JumpType = JumpType.NEXT };
            cdt = new Condition() { Parameter = Parameter.VOLTAGE, Mark = CompareMarkEnum.SmallerThan, Value = 2500 };
            cob = new CutOffBehavior() { Condition = cdt };
            cob.JumpBehaviors.Add(jpb);
            dischargeStep.CutOffBehaviors.Add(cob);

            return new List<Step> { chargeStep, idleStep, dischargeStep};
        }
    }
}
