using NationalInstruments.Visa;



namespace SmartTesterLib
{
    public class Load
    {
        private static MessageBasedSession MessageBased;
        private ActionMode CurrentStepActionMode { get; set; }

        public bool Init()
        {
            try
            {
                ResourceManager resourceManager = new ResourceManager();
                MessageBased = (MessageBasedSession)resourceManager.Open("COM17");
                SetRemoteMode();
            }
            catch { 
                return false;
            }
            return true;
        }

        private void SetRemoteMode()
        {
            MessageBased.RawIO.Write("SYST:REM\n");
        }

        private void SetLocalMode() 
        {
            MessageBased.RawIO.Write("SYST:LOC\n");
        }

        public bool ReadData(out UInt32 current,out UInt32 voltage)
        {
            current = 0;
            voltage = 0;
            try
            {
                MessageBased.RawIO.Write("MEAS:CURR?\n");
                current = Convert.ToUInt32(MessageBased.RawIO.ReadString()) * 1000;
                MessageBased.RawIO.Write("MEAS:VOLT?\n");
                voltage = Convert.ToUInt32(MessageBased.RawIO.ReadString()) * 1000;
            }
            catch { return false; }
            return true;
        }

        private static string GetActionModeString(ActionMode mode)
        {
            string output;
            switch (mode)
            {
                case ActionMode.CC_DISCHARGE:
                    output = "CURR";
                    break;
                case ActionMode.CP_DISCHARGE:
                    output = "POW";
                    break;
                case ActionMode.CC_CV_CHARGE:
                    output = "";
                    break;
                case ActionMode.REST:
                    output = "POW";
                    break;
                default:
                    output = "";
                    break;
            }
            return output;
        }

        public void PowerOff()
        {
            MessageBased.RawIO.Write("INP 0\n");
            SetLocalMode();
        }

        public void PowerOn() 
        {
            MessageBased.RawIO.Write("INP 1\n");
        }

        public void SetDischargeParameters(SmartTesterStep step)
        {
            CurrentStepActionMode = step.Action.Mode;
            string actionMode = GetActionModeString(step.Action.Mode);
            if (step.Action.Mode == SmartTesterLib.ActionMode.CC_DISCHARGE)
            {
                MessageBased.RawIO.Write($"MODE {actionMode}\n");
                MessageBased.RawIO.Write($"{actionMode} {(decimal)step.Action.Current / 1000}\n");
            }
            else if (step.Action.Mode == SmartTesterLib.ActionMode.CP_DISCHARGE)
            {
                MessageBased.RawIO.Write($"MODE {actionMode}\n");
                MessageBased.RawIO.Write($"{actionMode} {(decimal)step.Action.Power / 1000}\n");
            }
            else if (step.Action.Mode == SmartTesterLib.ActionMode.REST)
            {
                MessageBased.RawIO.Write($"MODE POW\n");
                MessageBased.RawIO.Write($"POW 0\n");
            }
        }
    }
}
