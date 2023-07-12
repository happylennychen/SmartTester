using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Threading;
//using Cobra.Common;
using Cobra.Communication.I2C;
using Cobra.Communication.SPI;
using Cobra.Communication.SVID;
using Cobra.Communication.RS232;
using Microsoft.Win32.SafeHandles;
using Cobra.Communication.HID;

namespace Cobra.Communication
{
    public abstract class CInterfaceBase
    {
        #region  Public Member Declarasion, common and public members that would be used by inherited class

        // <summary>
        // Error Code
        // </summary>
        private UInt32 m_dwErrCode;
        // <summary>
        // ErrorCode get/set function
        // </summary>
        public UInt32 ErrorCode { get { return m_dwErrCode; } set { m_dwErrCode = value; } }

        // <summary>
        // Connected device number.
        // </summary>
        private Int16 m_wDevNo;
        // <summary>
        // DeviceNumber get/set funciton
        // </summary>
        public Int16 DeviceNumber { get { return m_wDevNo; } set { m_wDevNo = value; } }

        // <summary>
        // Port index of communicated target device
        // </summary>
        private byte m_yPortIndex;
        // <summary>
        // PortIndex get/set function
        // </summary>
        public byte PortIndex { get { return m_yPortIndex; } set { m_yPortIndex = value; } }

        // <summary>
        // FileStream of opened device
        // </summary>
        private FileStream m_I2CPortFS;
        // <summary>
        // FileStream get/set function
        // </summary>
        public FileStream DeviceHandler
        {
            get { return m_I2CPortFS; }
            set { m_I2CPortFS = value; }
        }

        public int session_id = -1;

        // <summary>
        // Link name of connected device
        // </summary>
        private string m_strSymbolicLinkName;
        // <summary>
        // LinkName get/set function
        // </summary>
        public string SymbolicLinkName
        {
            get { return m_strSymbolicLinkName; }
            set { m_strSymbolicLinkName = value; }
        }

        // <summary>
        // Friend name of connected device
        // </summary>
        private string m_strFriendName;
        // <summary>
        // FriendName get/set function
        // </summary>
        public string FriendName
        {
            get { return m_strFriendName; }
            set { m_strFriendName = value; }
        }

        // <summary>
        // Display name of connected device
        // </summary>
        private string m_strDisplayName;
        // <summary>
        // DisplayName get/set function
        // </summary>
        public string DisplayName
        {
            get { return m_strDisplayName; }
            set { m_strDisplayName = value; }
        }

        //(A151215)Francis, add for saving AutomationTest setting value
        // (D151228)Francis, ATMSetting value move to AutoMationTest class, save link for BusOption instead
        public bool bErrGenerate { get; set; }
        public bool bErrOutMax { get; set; }
        public bool bErrOutMin { get; set; }
        public bool bErrPEC { get; set; }
        public UInt16 u16Sensor { get; set; }
        //
        public BusOptions m_busopDev;
        public UInt16 wUARTReadDelay { get; set; }

        //(E151215)

        // <summary>
        // Buffer of send data, maximum=64
        // </summary>
        [MarshalAs(UnmanagedType.LPArray, SizeConst = CCommunicateManager.MAX_RWBUFFER)]
        protected byte[] m_SendBuffer;

        // <summary>
        // Buffer of receive data, maximum=64
        // </summary>
        [MarshalAs(UnmanagedType.LPArray, SizeConst = CCommunicateManager.MAX_RWBUFFER)]
        protected byte[] m_ReceiveBuffer;

        // <summary>
        // Send size
        // </summary>
        protected int m_SendSize;

        // <summary>
        // Recevie size
        // </summary>
        protected int m_ReceiveSize;

        // <summary>
        // Synchronized locker, use to make sure only one communicate going on device
        // </summary>
        //public Type m_Locker;
        protected Semaphore m_Locker;

        //(A141203)Francis, for SVID master board access
        public enum SVIDMethodEnum : int
        {
            SVIDI2C = 0x01,
            SVIDVR = 0x02,
        }
        private SVIDMethodEnum m_SVIDAccessMethod = SVIDMethodEnum.SVIDI2C;
        public SVIDMethodEnum SVIDAccessMethod
        {
            get { return m_SVIDAccessMethod; }
            set { m_SVIDAccessMethod = value; }
        }
        //(E141203)
        public DEVICE_TYPE m_curDevType = DEVICE_TYPE.DEV_Default;
        #endregion

        #region Public abstract Method	Declaration,	inherited class must implement them

        // <summary>
        // Open devices, function will enumerate all connected devices and save in iPortNum. 
        // After successfully opened, function will try to open indicated device by yPortIndex value
        // </summary>
        // <param name="iPortNum">after opened successfully, save how many devices is connected</param>
        // <param name="yPortIndex">index value to indicate which device to open</param>
        // <returns>true: opened successfully; false: opened failed</returns>
        public abstract bool OpenDevice(ref Int16 iPortNum, byte yPortIndex = 0);

        public abstract bool OpenDevice(AsyncObservableCollection<string> strName, byte yPortIndex);

        // <summary>
        // Close device hanlder stream
        // </summary>
        // <returns>true: close successfully; false: close failed</returns>
        public abstract bool CloseDevice(bool bClearName = true);

        // <summary>
        // Read data through device; function will send byte by byte according DataIn array to device through connected interface;
        // and save value in DataOut array after communication finished if necessary
        // </summary>
        // <param name="yDataIn">reference of Data pass in</param>
        // <param name="yDataOut">reference of Data pass out</param>
        // <param name="wDataOutLength">Out data length</param>
        // <param name="wDataInLength">In data length, default is 1</param>
        // <returns>true: read successfully; false: read failed</returns>
        public abstract bool ReadDevice(ref byte[] yDataIn, ref byte[] yDataOut, ref UInt16 wDataOutLength, UInt16 wDataInLength = 1, UInt16 wDataInWrite = 1);

        // <summary>
        // Write data through device; function will send byte by byte according DataIn array to device through connected interface;
        // and save value in DataOut array after communication finished if necessary
        // </summary>
        // <param name="yDataIn">reference of Data pass in</param>
        // <param name="yDataOut">reference of Data pass out</param>
        // <param name="wDataOutLength">Out data length</param>
        // <param name="wDataInLength">In data length, default is 1</param>
        // <returns>true: read successfully; false: read failed</returns>
        public abstract bool WriteDevice(ref byte[] yDataIn, ref byte[] yDataOut, ref UInt16 wDataOutLength, UInt16 wDataInLength = 1);

        public abstract bool SetConfigure(List<UInt32> wConfig);

        public abstract bool GetConfigure(ref List<UInt32> wConfig);

        // <summary>
        // Reset interface
        // </summary>
        // <returns></returns>
        public abstract bool ResetInf();

        public abstract bool SetO2DelayTime(List<UInt32> wDelay);

        public abstract bool SetAdapterCommand(ref byte[] yDataIn, ref byte[] yDataOut, ref UInt16 wDataOutLength, UInt16 wDataInLength = 1);

        #endregion

        #region static functions, to Finde hardware devices, like O2USBtoI2C adaptor, Aardvark adaptor, and O2 SVID master board
        public static unsafe bool FindO2USBDevice(ref Int16 iTotal, ref UInt32 dwErr, ref AsyncObservableCollection<string> strLinkName)
        {
            Guid tempGuid = CO2USBI2CAdapter.GetGuid();
            UInt16 wDevNum = 0;
            int hDevInfoList = 0;
            bool bPresent = false;

            dwErr = LibErrorCode.IDS_ERR_SUCCESSFUL;
            hDevInfoList = NativeMethods.SetupDiGetClassDevs(ref tempGuid, null, null, NativeMethods.ClassDevsFlags.DIGCF_PRESENT | NativeMethods.ClassDevsFlags.DIGCF_DEVICEINTERFACE);
            if (hDevInfoList != 0)
            {
                NativeMethods.SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new NativeMethods.SP_DEVICE_INTERFACE_DATA();
                for (int i = 0; i < CCommunicateManager.MAX_COMM_DEVICES; i++)
                {
                    deviceInterfaceData.cbSize = (UInt32)Marshal.SizeOf(deviceInterfaceData);
                    bPresent = NativeMethods.SetupDiEnumDeviceInterfaces(hDevInfoList, 0, ref tempGuid, i, ref deviceInterfaceData);
                    if (bPresent)
                    {
                        int requiredLength = 0;
                        NativeMethods.SetupDiGetDeviceInterfaceDetail(hDevInfoList,
                                                                      ref deviceInterfaceData,
                                                                      null,     // Not yet allocated
                                                                      0,            // Set output Buffer length to zero 
                                                                      ref requiredLength,   // Find out memory requirement
                                                                      null);
                        dwErr = NativeMethods.GetLastError();
                        if (dwErr != 0)
                        {
                            dwErr = LibErrorCode.IDS_ERR_I2C_INVALID_HANDLE;
                        }

                        int predictedLength = requiredLength;
                        NativeMethods.PSP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData = new NativeMethods.PSP_DEVICE_INTERFACE_DETAIL_DATA();
                        switch (sizeof(IntPtr))
                        {
                            case 8: deviceInterfaceDetailData.cbSize = 8; break;
                            default: deviceInterfaceDetailData.cbSize = 5; break;
                        }
                        NativeMethods.SP_DEVINFO_DATA devInfoData = new NativeMethods.SP_DEVINFO_DATA();
                        devInfoData.cbSize = (UInt32)Marshal.SizeOf(devInfoData);

                        // Second, get the detailed information
                        if (NativeMethods.SetupDiGetDeviceInterfaceDetail(hDevInfoList,
                                                                              ref deviceInterfaceData,
                                                                              ref deviceInterfaceDetailData,
                                                                              predictedLength,
                                                                              ref requiredLength,
                                                                              ref devInfoData) != 0)
                        {
                            NativeMethods.DATA_BUFFER friendlyNameBuffer = new NativeMethods.DATA_BUFFER();
                            string strTempFriendName = "";
                            if (NativeMethods.SetupDiGetDeviceRegistryProperty(hDevInfoList,
                                                                               ref devInfoData,
                                                                               NativeMethods.RegPropertyType.
                                                                                   SPDRP_FRIENDLYNAME,
                                                                               null,
                                                                               ref friendlyNameBuffer,
                                                                               Marshal.SizeOf(friendlyNameBuffer),
                                                                               ref requiredLength) == 0)
                            {
                                strTempFriendName = NativeMethods.SetupDiGetDeviceRegistryProperty(
                                                                                    hDevInfoList,
                                                                                    ref devInfoData,
                                                                                    NativeMethods.RegPropertyType.SPDRP_DEVICEDESC,
                                                                                    null,
                                                                                    ref friendlyNameBuffer,
                                                                                    Marshal.SizeOf(friendlyNameBuffer),
                                                                                    ref requiredLength) == 0
                                                                        ? deviceInterfaceDetailData.DevicePath
                                                                        : friendlyNameBuffer.Buffer;
                            }

                            wDevNum++;
                            if (strLinkName != null)
                            {
                                strLinkName.Add(deviceInterfaceDetailData.DevicePath);
                            }
                        }
                    }
                    else
                    {
                        dwErr = NativeMethods.GetLastError();
                        if (dwErr == NativeMethods.ERROR_NO_MORE_ITEMS)
                        {
                            if (i == 0)
                            {
                                dwErr = LibErrorCode.IDS_ERR_I2C_INVALID_HANDLE;
                            }
                            else
                            {
                                dwErr = LibErrorCode.IDS_ERR_SUCCESSFUL;
                            }
                            break;
                        }
                    }
                }

                bPresent = NativeMethods.SetupDiDestroyDeviceInfoList(hDevInfoList);
                if (!bPresent)
                {
                    dwErr = LibErrorCode.IDS_ERR_MGR_UNABLE_LOAD_FUNCTION;
                }
            }
            else
            {
                dwErr = LibErrorCode.IDS_ERR_I2C_INVALID_HANDLE;
            }
            iTotal = (Int16)wDevNum;

            return bPresent;
        }

        //Note that FindAAUSBDevice() is called following by FindO2USBDevice(), so that, iTotal may have nonzero value and strLinkName also
        public static unsafe bool FindAAUSBDevice(ref Int16 iTotal, ref UInt32 dwErr, ref AsyncObservableCollection<string> strLinkName)
        {
            bool bPresent = false;

            return bPresent;
        }

        //(A141017)Francis, find SVID master board
        public static unsafe bool FindSVIDMasterDevice(ref Int16 iTotal, ref UInt32 dwErr, ref AsyncObservableCollection<string> strLinkName)
        {
            bool bReturn = true;    //basically, it will have no chance that error happened
            List<string> strSerialNames;
            int iNum = 0;

            strSerialNames = CO2SVID2I2CMaster.GetComPortLinkName();
            iNum = (Int16)strSerialNames.Count;
            if (iNum > 0)
            {
                foreach (string strtmp in SerialPort.GetPortNames())
                {
                    if (strSerialNames.Contains(strtmp))
                    {
                        strLinkName.Add(strtmp);
                        iTotal += 1;
                    }
                }
            }
            else
            {
                //strLinkName.Clear();
            }

            return bReturn;
        }
        //(E141017)

        //(A150416)Francis, find RS232 connect line
        public static unsafe bool FindRS232Device(ref Int16 iTotal, ref UInt32 dwErr, ref AsyncObservableCollection<string> strLinkName)
        {
            bool bReturn = true;    //basically, it will have no chance that error happened
            List<string> strSerialNames;
            int iNum = 0;

            strSerialNames = CO2RS232Master.GetComPortLinkName();
            iNum = (Int16)strSerialNames.Count;
            if (iNum > 0)
            {
                foreach (string strtmp in SerialPort.GetPortNames())
                {
                    if (strSerialNames.Contains(strtmp))
                    {
                        strLinkName.Add(strtmp);
                        iTotal += 1;
                    }
                }
            }
            else
            {
                //strLinkName.Clear();
            }

            return bReturn;
        }
        //(E141017)

        //(A20220822)Guo, find HID connect line
        public static unsafe bool FindHIDDevice(ref Int16 iTotal, ref UInt32 dwErr, ref AsyncObservableCollection<string> strLinkName)
        {
            UInt16 wDevNum = 0;
            bool bPresent = false;
            int hDevInfoList = 0;
            Guid tempGuid = CO2USBHIDAdapter.GetGuid();
            IntPtr hFile;
            UInt16 m_VID = 0x0B97;
            UInt16 m_PID = 0x8073;
            string m_prefix = "O2 Micro Inc";

            dwErr = LibErrorCode.IDS_ERR_SUCCESSFUL;
            hDevInfoList = NativeMethods.SetupDiGetClassDevs(ref tempGuid, null, null, NativeMethods.ClassDevsFlags.DIGCF_PRESENT | NativeMethods.ClassDevsFlags.DIGCF_DEVICEINTERFACE);
            if (hDevInfoList != 0)
            {
                NativeMethods.SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new NativeMethods.SP_DEVICE_INTERFACE_DATA();
                for (int i = 0; i < CCommunicateManager.MAX_COMM_DEVICES; i++)
                {
                    deviceInterfaceData.cbSize = (UInt32)Marshal.SizeOf(deviceInterfaceData);
                    bPresent = NativeMethods.SetupDiEnumDeviceInterfaces(hDevInfoList, 0, ref tempGuid, i, ref deviceInterfaceData);
                    if (bPresent)
                    {
                        int requiredLength = 0;
                        NativeMethods.SetupDiGetDeviceInterfaceDetail(hDevInfoList,
                                                                      ref deviceInterfaceData,
                                                                      null,     // Not yet allocated
                                                                      0,            // Set output Buffer length to zero 
                                                                      ref requiredLength,   // Find out memory requirement
                                                                      null);
                        dwErr = NativeMethods.GetLastError();
                        if (dwErr != 0)
                        {
                            dwErr = LibErrorCode.IDS_ERR_I2C_INVALID_HANDLE;
                        }

                        int predictedLength = requiredLength;
                        NativeMethods.PSP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData = new NativeMethods.PSP_DEVICE_INTERFACE_DETAIL_DATA();
                        switch (sizeof(IntPtr))
                        {
                            case 8: deviceInterfaceDetailData.cbSize = 8; break;
                            default: deviceInterfaceDetailData.cbSize = 5; break;
                        }
                        NativeMethods.SP_DEVINFO_DATA devInfoData = new NativeMethods.SP_DEVINFO_DATA();
                        devInfoData.cbSize = (UInt32)Marshal.SizeOf(devInfoData);

                        // Second, get the detailed information
                        if (NativeMethods.SetupDiGetDeviceInterfaceDetail(hDevInfoList,
                                                                              ref deviceInterfaceData,
                                                                              ref deviceInterfaceDetailData,
                                                                              predictedLength,
                                                                              ref requiredLength,
                                                                              ref devInfoData) != 0)
                        {

                            hFile = NativeMethods.CreateFile(deviceInterfaceDetailData.DevicePath,
                                NativeMethods.GENERIC_WRITE | NativeMethods.GENERIC_READ,
                                NativeMethods.FILE_SHARE_WRITE | NativeMethods.FILE_SHARE_READ,
                                0,
                                NativeMethods.OPEN_EXISTING,
                                NativeMethods.FILE_FLAG_OVERLAPPED | NativeMethods.FILE_FLAG_NO_BUFFERING,
                                0);
                            SafeFileHandle sfh = new SafeFileHandle(hFile, true);       //creat Stream handler
                            if (sfh.IsInvalid) continue;
                            NativeMethods.HIDD_ATTRIBUTES attributes;
                            IntPtr serialBuff = Marshal.AllocHGlobal(512);
                            NativeMethods.HidD_GetAttributes(hFile, out attributes);
                            NativeMethods.HidD_GetManufacturerString(hFile, serialBuff, 512);
                            string deviceStr = Marshal.PtrToStringAuto(serialBuff);
                            Marshal.FreeHGlobal(serialBuff);
                            if (attributes.VendorID != m_VID || attributes.ProductID != m_PID || !deviceStr.Contains(m_prefix)) continue;
                            wDevNum++;
                            if (strLinkName != null)
                            {
                                strLinkName.Add(deviceInterfaceDetailData.DevicePath);
                            }
                        }
                    }
                    else
                    {
                        dwErr = NativeMethods.GetLastError();
                        if (dwErr == NativeMethods.ERROR_NO_MORE_ITEMS)
                        {
                            if (i == 0)
                            {
                                dwErr = LibErrorCode.IDS_ERR_I2C_INVALID_HANDLE;
                            }
                            else
                            {
                                dwErr = LibErrorCode.IDS_ERR_SUCCESSFUL;
                            }
                            break;
                        }
                    }
                }
                bPresent = NativeMethods.SetupDiDestroyDeviceInfoList(hDevInfoList);
                if (!bPresent)
                {
                    dwErr = LibErrorCode.IDS_ERR_MGR_UNABLE_LOAD_FUNCTION;
                }
            }
            else
            {
                dwErr = LibErrorCode.IDS_ERR_I2C_INVALID_HANDLE;
            }
            iTotal = (Int16)wDevNum;

            return bPresent;
        }
        #endregion

        #region Public functions, Public virtual function
        public CInterfaceBase()
        {
            m_busopDev = null;
        }

        public void SetSVIDAccessI2C()
        {
            SVIDAccessMethod = SVIDMethodEnum.SVIDI2C;
        }

        public void SetSVIDAccessVR()
        {
            SVIDAccessMethod = SVIDMethodEnum.SVIDVR;
        }

        public bool WriteDataToLog(byte[] yDataFromDEM, byte[] yDataFromChip, UInt16 wLengthChip, UInt16 wLengthDEM, byte yRW)
        {
            if (wLengthDEM == 1)
                return true;
            bool bReturn = true;
            string strTmp = string.Empty;
            string strdbg = string.Empty;

            #region support DBManager
            if ((m_busopDev.BusType == BUS_TYPE.BUS_TYPE_I2C) || (m_busopDev.BusType == BUS_TYPE.BUS_TYPE_I2C2))
            {
                if (yRW == 0)
                    strdbg += "R/W|Write, ";
                else
                    strdbg += "R/W|Read, ";
                if (m_curDevType == DEVICE_TYPE.DEV_O2Adapter)
                {
                    strdbg += string.Format("I2CAddr|0x{0:X2}, ", yDataFromDEM[0]);
                    strdbg += string.Format("RegIndex|0x{0:X2}, ", yDataFromDEM[1]);
                    if (yRW == 0)
                    {
                        strTmp = string.Empty;
                        if (wLengthDEM > 0)
                        {
                            for (int i = 0; i < wLengthDEM; i++)
                                strTmp += string.Format("Data|0x{0:X2}, ", yDataFromDEM[i + 2]);
                        }
                        else
                        {
                            strTmp += string.Format("Data|,");
                        }
                        strdbg += strTmp;
                    }
                    else
                    {
                        strTmp = string.Empty;
                        if (wLengthChip > 0)
                        {
                            for (int i = 0; i < wLengthChip; i++)
                                strTmp += string.Format("Data|0x{0:X2}, ", yDataFromChip[i]);
                        }
                        else
                        {
                            strTmp += string.Format("Data|,");
                        }
                        strdbg += strTmp;
                    }
                }
                else
                {
                    if (yRW == 0)
                    {
                        strTmp = string.Empty;
                        if (wLengthDEM > 0)
                        {
                            for (int i = 0; i < yDataFromDEM.Length; i++)
                                strTmp += string.Format("Data|0x{0:X2}, ", yDataFromDEM[i]);
                        }
                        else
                        {
                            strTmp += string.Format("Data|,");
                        }
                        strdbg += strTmp;
                    }
                    else
                    {
                        strTmp = string.Empty;
                        if (wLengthChip > 0)
                        {
                            for (int i = 0; i < yDataFromChip.Length; i++)
                                strTmp += string.Format("Data|0x{0:X2}, ", yDataFromChip[i]);
                        }
                        else
                        {
                            strTmp += string.Format("Data|,");
                        }
                        strdbg += strTmp;
                    }
                }
                strdbg += "ErrComments|";
            }
            else
            {
                if (yRW == 0)
                {
                    strdbg += "R/W|Write, ";
                    {
                        strTmp = string.Empty;
                        for (int i = 0; i < wLengthDEM; i++)
                        {
                            strTmp += string.Format("Data|0x{0:X2}, ", yDataFromDEM[i]);
                        }
                        strdbg += strTmp;
                    }
                    strdbg += "ErrComments|";
                }
                else
                {
                    strdbg += "R/W|Write, ";
                    {
                        strTmp = string.Format("Data|");
                        for (int i = 0; i < wLengthDEM; i++)
                        {
                            strTmp += string.Format("0x{0:X2}, ", yDataFromDEM[i]);
                        }
                        strdbg += strTmp;
                    }
                    strdbg += "ErrComments|";
                    strdbg += "R/W|Read, ";
                    {
                        strTmp = string.Format("Data|");
                        if (wLengthChip == 0)
                            strTmp += ",";
                        else
                        {
                            for (int i = 0; i < wLengthChip; i++)
                            {
                                strTmp += string.Format("0x{0:X2}, ", yDataFromChip[i]);
                            }
                        }
                        strdbg += strTmp;
                    }
                }
            }
            strdbg += LibErrorCode.GetErrorDescription(ErrorCode);
            #endregion
            return bReturn;
        }
        #endregion

    }
}
