using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTester
{
    public class TesterAction
    {
        public int Id { get; set; }
        public ActionMode Mode { get; set; }
        public int Voltage { get; set; }
        public int Current { get; set; }
        public int Power { get; set; }
        public TesterAction()
        {

        }
        public TesterAction(ActionMode action, int voltage, int current, int power)
        {
            Mode = action;
            Voltage = voltage;
            Current = current;
            Power = power;
        }
        internal TesterAction Clone()
        {
            TesterAction output = new TesterAction();
            output.Current = this.Current;
            output.Mode = this.Mode;
            output.Power = this.Power;
            output.Voltage = this.Voltage;
            return output;
        }

        public override string ToString()
        {
            return $"Mode:{Mode}, Voltage:{Voltage}, Current:{Current}, Power:{Power}";
        }
    }
}
