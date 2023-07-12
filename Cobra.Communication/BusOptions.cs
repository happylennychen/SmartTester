using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Cobra.Communication
{
    public enum BUS_TYPE
    {
        BUS_TYPE_I2C = 0,
        BUS_TYPE_I2C2,
        BUS_TYPE_SPI,
        BUS_TYPE_SVID,
        BUS_TYPE_RS232
    }

    public enum DEVICE_TYPE
    {
        DEV_Default,
        DEV_O2Adapter,
        DEV_Aadvark,
        DEV_O2Link
    }

    public enum BUS_CONFIG
    {
        CONFIG_I2C = 0x00,
        CONFIG_SPI = 0x01,
        CONFIG_SVID = 0x02,
        CONFIG_RS232 = 0x03,
    }
    public class BusOptions : INotifyPropertyChanged
    {
        #region GUID definition, used in Communication Layer to identify which Options
        public const UInt32 BusOptionsElement = 0x00100000;
        public const UInt32 I2CBusOptionsElement = 0x00100000;
        public const UInt32 ConnectPort_GUID = I2CBusOptionsElement + 0x0000;
        public const UInt32 I2CFrequency_GUID = I2CBusOptionsElement + 0x0001;
        public const UInt32 I2CAddress_GUID = I2CBusOptionsElement + 0x0002;
        public const UInt32 I2CPECMODE_GUID = I2CBusOptionsElement + 0x0003;
        public const UInt32 I2C2Address_GUID = I2CBusOptionsElement + 0x0004;
        public const UInt32 I2C2PECMODE_GUID = I2CBusOptionsElement + 0x0005;

        public const UInt32 SPIBusOptionsElement = 0x00100020;
        public const UInt32 SPIBaudRate_GUID = SPIBusOptionsElement + 0x0001;
        public const UInt32 SPISSPolarity_GUID = SPIBusOptionsElement + 0x0002;
        public const UInt32 SPIBitOrder_GUID = SPIBusOptionsElement + 0x0003;
        public const UInt32 SPIPolarity_GUID = SPIBusOptionsElement + 0x0004;
        public const UInt32 SPIPhase_GUID = SPIBusOptionsElement + 0x0005;
        public const UInt32 SPIWire_GUID = SPIBusOptionsElement + 0x0006;

        public const UInt32 RS232BusOptionsElement = 0x00100040;
        public const UInt32 RS232ConnectPort_GUID = RS232BusOptionsElement + 0x0000;
        public const UInt32 RS232BaudRate_GUID = RS232BusOptionsElement + 0x0001;
        public const UInt32 RS232DataBits_GUID = RS232BusOptionsElement + 0x0002;
        public const UInt32 RS232Stopbit_GUID = RS232BusOptionsElement + 0x0003;
        public const UInt32 RS232Parity_GUID = RS232BusOptionsElement + 0x0004;

        public const UInt32 SVIDBusOptionsElement = 0x00100060;
        public const UInt32 SVIDI2CFrequency_GUID = SVIDBusOptionsElement + 0x01;
        public const UInt32 SVIDI2CAddress_GUID = SVIDBusOptionsElement + 0x02;
        public const UInt32 SVIDBaudRate_GUID = SVIDBusOptionsElement + 0x0003;
        public const UInt32 SVIDDataBits_GUID = SVIDBusOptionsElement + 0x0004;
        public const UInt32 SVIDStopbit_GUID = SVIDBusOptionsElement + 0x0005;
        public const UInt32 SVIDParity_GUID = SVIDBusOptionsElement + 0x0006;
        #endregion

        private ObservableCollection<Options> m_OptionsList = new ObservableCollection<Options>();
        public ObservableCollection<Options> optionsList
        {
            get { return m_OptionsList; }
            set { m_OptionsList = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        private bool m_deviceischeck;
        public bool DeviceIsCheck
        {
            get { return m_deviceischeck; }
            set
            {
                m_deviceischeck = value;
                OnPropertyChanged("DeviceIsCheck");
            }
        }

        private BUS_TYPE m_bustype;
        public BUS_TYPE BusType
        {
            get { return m_bustype; }
            set
            {
                m_bustype = value;
                OnPropertyChanged("BusType");
            }
        }

        public int DeviceIndex { get; set; }

        private string m_devicename;
        public string DeviceName
        {
            get { return String.Format("{0} Connection Setting", m_devicename); }
            set
            {
                m_devicename = value;
                OnPropertyChanged("DeviceName");
            }
        }

        public Options GetOptionsByGuid(UInt32 guid)
        {
            foreach (Options op in optionsList)
            {
                if (op.guid.Equals(guid))
                    return op;
            }
            return null;
        }

        public string Name { get; set; }

        public BusOptions()
        {
            DeviceIsCheck = true;
            BusType = 0;
            DeviceIndex = 0;
            DeviceName = null;
            Name = null;
        }
    }
}
