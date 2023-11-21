using Cobra.Common;
using Cobra.Communication;

namespace SmartTesterLib
{
    public class OZ93510
    {
        private CCommunicateManager m_Interface { get; set; }
        public bool Init()
        {
            BusOptions m_busoption = new BusOptions();
            m_busoption.BusType = BUS_TYPE.BUS_TYPE_I2C;
            var op1 = new Options();
            op1.bcheck = false;
            op1.bedit = true;
            op1.berror = false;
            op1.brange = true;
            op1.catalog = "Common Configuration";
            op1.data = 0;
            op1.editortype = 1;
            op1.format = 1;
            op1.guid = BusOptions.ConnectPort_GUID;
            op1.maxvalue = 16;
            op1.minvalue = 0;
            op1.nickname = "Connect Port:";
            op1.order = 0;
            op1.sdevicename = "Device1 Connection Setting";
            m_busoption.optionsList.Add(op1);

            var op2 = new Options();
            op2.bcheck = false;
            op2.bedit = true;
            op2.berror = false;
            op2.brange = true;
            op2.catalog = "Common Configuration";
            op2.data = 400;
            op2.editortype = 0;
            op2.format = 1;
            op2.guid = BusOptions.I2CFrequency_GUID;
            op2.maxvalue = 400;
            op2.minvalue = 63;
            op2.nickname = "I2C Bus Frequency(KHZ):";
            op2.order = 0;
            op2.sdevicename = "Device1 Connection Setting";
            op2.sphydata = "100";
            m_busoption.optionsList.Add(op2);

            var op3 = new Options();
            var location3 = new ComboboxRoad() { Code = 0x30, ID = 0, Info = "0x30" };
            op3.LocationSource.Add(location3);
            op3.SelectLocation = location3;
            op3.bcheck = false;
            op3.bedit = true;
            op3.berror = false;
            op3.brange = true;
            op3.catalog = "LGE";
            op3.data = 0;
            op3.editortype = 1;
            op3.format = 1;
            op3.guid = BusOptions.I2CAddress_GUID;
            op3.maxvalue = 0;
            op3.minvalue = 0;
            op3.nickname = "I2C Address:";
            op3.order = 0;
            op3.sdevicename = "Device1 Connection Setting";
            op3.sphydata = "0x30";
            m_busoption.optionsList.Add(op3);

            var op4 = new Options();
            var location4 = new ComboboxRoad() { Code = 1, ID = 0, Info = "true" };
            op4.LocationSource.Add(location4);
            op4.SelectLocation = location4;
            op4.bcheck = false;
            op4.bedit = true;
            op4.berror = false;
            op4.brange = true;
            op4.catalog = "LGE";
            op4.data = 0;
            op4.editortype = 1;
            op4.format = 1;
            op4.guid = BusOptions.I2CPECMODE_GUID;
            op4.maxvalue = 0;
            op4.minvalue = 0;
            op4.nickname = "PEC enable";
            op4.order = 1;
            op4.sdevicename = "Device1 Connection Setting";
            op4.sphydata = "true";
            m_busoption.optionsList.Add(op4);

            bool bdevice = m_Interface.FindDevices(ref m_busoption);
            if (!bdevice) return false;
            m_busoption.optionsList[0].SelectLocation = m_busoption.optionsList[0].LocationSource[1];
            bdevice = m_Interface.FindDevices(ref m_busoption);
            if (!bdevice) return false;

            if (m_Interface.OpenDevice(ref m_busoption))
            {
                return true;
            }
            else
                return false;

        }
    }
}
