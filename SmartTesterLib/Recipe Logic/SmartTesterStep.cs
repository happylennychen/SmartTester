using System.Collections.Generic;

namespace SmartTesterLib
{
    public class SmartTesterStep
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public TesterAction Action { get; set; }
        public TemperatureTarget Target { get; set; }
        public List<CutOffBehavior> CutOffBehaviors { get; set; }
        public TemperatureUnit TemperatureUint { get; set; }

        public SmartTesterStep()
        {
            CutOffBehaviors = new List<CutOffBehavior>();
        }
        public override string ToString()
        {
            string output = $"STEP Index={Index} Mode={Action.Mode} Voltage={Action.Voltage} Current={Action.Current} Power={Action.Power} Temperature={Target.Value}";
            //foreach (var cob in CutOffBehaviors)
            //{
            //    output += $"\tCOB {cob.Condition.Parameter} {cob.Condition.Mark} {cob.Condition.Value}\n";
            //    foreach (var jpb in cob.JumpBehaviors)
            //    {
            //        string jpbDescriptor;
            //        if (jpb.JumpType == JumpType.INDEX)
            //            jpbDescriptor = $"\t\tJPB JumpType={jpb.JumpType} Index={jpb.Index}\n";
            //        else
            //            jpbDescriptor = $"\t\tJPB JumpType={jpb.JumpType}\n";
            //        output += jpbDescriptor;
            //    }
            //}
            return output;
        }
    }
}