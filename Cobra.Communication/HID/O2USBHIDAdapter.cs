using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Threading;
////using Cobra.Common;
using Cobra.Communication;

namespace Cobra.Communication.HID
{
    public class CO2USBHIDAdapter : CInterfaceHID
    {
        private bool bSet = false;
        private int m_outputReportLength;//输出报告长度,包刮一个字节的报告ID
        public int OutputReportLength { get { return m_outputReportLength; } } //数据长度

        private int m_inputReportLength;//输入报告长度,包刮一个字节的报告ID   
        public int InputReportLength { get { return m_inputReportLength; } }

        private static Guid O2USBHIDGuid = new Guid();
        public CO2USBHIDAdapter()
        {
            CloseDevice();
            m_Locker = new Semaphore(0, 1);
        }

        public static Guid GetGuid()
        {
            NativeMethods.HidD_GetHidGuid(ref O2USBHIDGuid);
            return O2USBHIDGuid;
        }

        public override bool OpenDevice(ref Int16 iPortNum, byte yPortIndex = 0)
        {
            bool bReturn = true;
            return bReturn;
        }

        public override bool OpenDevice(AsyncObservableCollection<string> strName, byte yPortIndex)
        {
            bool bReturn = false;
            SafeFileHandle hFile;

            if (strName.Count <= yPortIndex)
            {
                ErrorCode = LibErrorCode.IDS_ERR_HID_INVALID_INDEX;
                return bReturn;
            }
            SymbolicLinkName = strName[yPortIndex];
            hFile = NativeMethods.CreateFile(SymbolicLinkName, NativeMethods.GENERIC_READ | NativeMethods.GENERIC_WRITE, NativeMethods.FILE_SHARE_READ | NativeMethods.FILE_SHARE_WRITE, IntPtr.Zero, NativeMethods.OPEN_EXISTING, NativeMethods.FILE_FLAG_OVERLAPPED, IntPtr.Zero);

            IntPtr ptrToPreParsedData = new IntPtr();
            bool ppdSucsess = NativeMethods.HidD_GetPreparsedData(hFile, ref ptrToPreParsedData);
            NativeMethods.HIDP_CAPS capabilities = new NativeMethods.HIDP_CAPS();
            int hidCapsSucsess = NativeMethods.HidP_GetCaps(ptrToPreParsedData, ref capabilities);
            NativeMethods.HidD_FreePreparsedData(ref ptrToPreParsedData);

            if (!hFile.IsInvalid)
            {
                try
                {
                    m_Locker.Release();                                                 //if FileHandle ok, release semaphore
                }
                catch (Exception)
                {
                }
                m_outputReportLength = capabilities.OutputReportByteLength; //赋值输出长度
                m_inputReportLength = capabilities.InputReportByteLength; //赋值输入长度
                DeviceHandler = new FileStream(hFile, FileAccess.ReadWrite, (int)capabilities.InputReportByteLength, true);
                PortIndex = yPortIndex;
                m_SendBuffer = new byte[m_outputReportLength];          //new send buffer
                m_ReceiveBuffer = new byte[m_inputReportLength];       //new receive buffer
                bReturn = true;
            }
            else
            {
                ErrorCode = LibErrorCode.IDS_ERR_I2C_INVALID_HANDLE;
                return false;
            }
            if (DeviceHandler == null)
            {
                ErrorCode = LibErrorCode.IDS_ERR_I2C_INVALID_INDEX;
                return false;
            }
            if (bReturn)
            {
                ErrorCode = LibErrorCode.IDS_ERR_SUCCESSFUL;
            }
            return bReturn;
        }

        public override bool CloseDevice(bool bClearName = true)
        {
            if (DeviceHandler != null)
            {
                try
                {
                    DeviceHandler.Close();
                }
                catch (Exception)
                {
                }
                finally
                {
                    DeviceHandler = null;
                }
            }
            if (bClearName)
            {
                SymbolicLinkName = "";
                FriendName = "";
                DisplayName = "";
                DeviceNumber = 0;
                PortIndex = 0xFF;
            }
            ErrorCode = LibErrorCode.IDS_ERR_SUCCESSFUL;

            return true;
        }

        public override bool ReadDevice(ref byte[] yDataIn, ref byte[] yDataOut, ref UInt16 wDataOutLength, UInt16 wDataInLength = 1, UInt16 wDataInWrite = 1)
        {
            bool bReturn = false;
            Array.Clear(m_SendBuffer, 0, m_SendBuffer.Length);
            Array.Clear(m_ReceiveBuffer, 0, m_ReceiveBuffer.Length);
            switch (m_busopDev.BusType)
            {
                case BUS_TYPE.BUS_TYPE_I2C:
                    {
                        m_SendBuffer[0] = 0x00; //Endpoint 0
                        m_SendBuffer[1] = 0x03;
                        m_SendBuffer[2] = 0x02;
                        m_SendBuffer[3] = 0x00;
                        m_SendBuffer[4] = yDataIn[0];
                        m_SendBuffer[5] = (byte)(wDataInLength >> 8);
                        m_SendBuffer[6] = (byte)wDataInLength;
                        m_SendBuffer[7] = (byte)(wDataInWrite >> 8);
                        m_SendBuffer[8] = (byte)wDataInWrite;
                        m_SendSize = 9 + wDataInWrite;
                        for (int i = 0; i < wDataInWrite; i++)
                        {
                            m_SendBuffer[9 + i] = yDataIn[1 + i];
                        }
                        if (DeviceHandler == null)
                        {
                            ErrorCode = LibErrorCode.IDS_ERR_HID_INVALID_HANDLE;
                            return false;
                        }

                        bReturn = O2HIDControlCommand();
                        if (bReturn == true)
                        {
                            if (m_ReceiveSize > 7)
                            {
                                wDataOutLength = (UInt16)(m_ReceiveSize - 7);
                                if (wDataOutLength < wDataInLength)
                                {
                                    ErrorCode = LibErrorCode.IDS_ERR_HID_INVALID_HARDWARE;
                                    return false;
                                }
                                Buffer.BlockCopy(m_ReceiveBuffer, 7, yDataOut, 0, yDataOut.Length);
                                ErrorCode = LibErrorCode.IDS_ERR_SUCCESSFUL;
                            }
                            else
                            {
                                ErrorCode = LibErrorCode.IDS_ERR_HID_BUS_ERROR;
                                return false;
                            }
                        }
                        else
                        {
                            wDataOutLength = 0;
                        }
                    }
                    break;
                case BUS_TYPE.BUS_TYPE_SPI:
                    {
                        m_SendBuffer[0] = 0x00; //Endpoint 0
                        m_SendBuffer[1] = 0x02;
                        m_SendBuffer[2] = 0x01;
                        m_SendBuffer[3] = 0x00;
                        m_SendBuffer[4] = (byte)yDataIn.Length;

                        if (yDataIn.GetLength(0) > (m_SendBuffer.Length - 1))
                        {
                            ErrorCode = LibErrorCode.IDS_ERR_HID_INVALID_LENGTH;
                            return bReturn;
                        }
                        if (DeviceHandler == null)
                        {
                            ErrorCode = LibErrorCode.IDS_ERR_HID_INVALID_HANDLE;
                            return false;
                        }

                        Array.Copy(yDataIn, 0, m_SendBuffer, 5, yDataIn.Length);
                        m_SendSize = 5 + yDataIn.Length;
                        bReturn = O2HIDControlCommand();
                        if (bReturn == true)
                        {
                            if (m_ReceiveSize > 4)
                            {
                                wDataOutLength = (UInt16)(m_ReceiveSize - 4);
                                if (wDataOutLength < wDataInLength)
                                {
                                    ErrorCode = LibErrorCode.IDS_ERR_HID_INVALID_HARDWARE;
                                    return false;
                                }
                                Buffer.BlockCopy(m_ReceiveBuffer, 5, yDataOut, 0, yDataOut.Length);
                                ErrorCode = LibErrorCode.IDS_ERR_SUCCESSFUL;
                            }
                            else
                            {
                                ErrorCode = LibErrorCode.IDS_ERR_HID_BUS_ERROR;
                                return false;
                            }
                        }
                        else
                        {
                            wDataOutLength = 0;
                        }
                    }
                    break;
                case BUS_TYPE.BUS_TYPE_RS232:
                    {
                        m_SendBuffer[0] = 0x00; //Endpoint 0
                        m_SendBuffer[1] = 0x01;
                        m_SendBuffer[2] = 0x01;
                        m_SendBuffer[3] = (byte)(wDataInLength >> 8);
                        m_SendBuffer[4] = (byte)wDataInLength;

                        if (yDataIn.GetLength(0) > (m_SendBuffer.Length - 1))
                        {
                            ErrorCode = LibErrorCode.IDS_ERR_HID_INVALID_LENGTH;
                            return bReturn;
                        }
                        if (DeviceHandler == null)
                        {
                            ErrorCode = LibErrorCode.IDS_ERR_HID_INVALID_HANDLE;
                            return false;
                        }

                        Array.Copy(yDataIn, 0, m_SendBuffer, 5, wDataInLength);
                        m_SendSize = 5 + wDataInLength;
                        bReturn = O2HIDControlCommand();
                        if (bReturn == true)
                        {
                            if (m_ReceiveSize > 5)
                            {
                                wDataOutLength = (UInt16)(m_ReceiveSize - 5);
                                if (wDataOutLength < wDataInLength)
                                {
                                    ErrorCode = LibErrorCode.IDS_ERR_HID_INVALID_HARDWARE;
                                    return false;
                                }
                                Buffer.BlockCopy(m_ReceiveBuffer, 5, yDataOut, 0, yDataOut.Length);//Endpoint, command
                                ErrorCode = LibErrorCode.IDS_ERR_SUCCESSFUL;
                            }
                            else
                            {
                                ErrorCode = LibErrorCode.IDS_ERR_HID_BUS_ERROR;
                                return false;
                            }
                        }
                        else
                        {
                            wDataOutLength = 0;
                        }
                        break;
                    }
            }
            return bReturn;
        }

        public override bool WriteDevice(ref byte[] yDataIn, ref byte[] yDataOut, ref UInt16 wDataOutLength, UInt16 wDataInLength = 1)
        {
            bool bReturn = false;
            Array.Clear(m_SendBuffer, 0, m_SendBuffer.Length);
            Array.Clear(m_ReceiveBuffer, 0, m_ReceiveBuffer.Length);
            switch (m_busopDev.BusType)
            {
                case BUS_TYPE.BUS_TYPE_I2C:
                    {
                        m_SendBuffer[0] = 0x00; //Endpoint 0
                        m_SendBuffer[1] = 0x03;
                        m_SendBuffer[2] = 0x01;
                        m_SendBuffer[3] = 0x00;
                        m_SendBuffer[4] = (byte)yDataIn[0];
                        m_SendBuffer[5] = (byte)((wDataInLength + 1) >> 8);
                        m_SendBuffer[6] = (byte)(wDataInLength + 1);
                        m_SendBuffer[7] = yDataIn[1];
                        m_SendSize = (byte)(wDataInLength + 8);

                        if (yDataIn.GetLength(0) > (m_SendBuffer.Length - 1))
                        {
                            ErrorCode = LibErrorCode.IDS_ERR_HID_INVALID_LENGTH;
                            return bReturn;
                        }
                        if (DeviceHandler == null)
                        {
                            ErrorCode = LibErrorCode.IDS_ERR_HID_INVALID_HANDLE;
                            return false;
                        }

                        Buffer.BlockCopy(yDataIn, 2, m_SendBuffer, 8, wDataInLength);
                        bReturn = O2HIDControlCommand();
                        if (bReturn == true)
                        {
                            if (m_ReceiveSize > 7)
                            {
                                wDataOutLength = (UInt16)(m_ReceiveSize - 7);
                                if (wDataOutLength < wDataInLength)
                                {
                                    ErrorCode = LibErrorCode.IDS_ERR_HID_INVALID_HARDWARE;
                                    return false;
                                }
                                Buffer.BlockCopy(m_ReceiveBuffer, 7, yDataOut, 0, yDataOut.Length);
                                ErrorCode = LibErrorCode.IDS_ERR_SUCCESSFUL;
                            }
                            else
                            {
                                ErrorCode = LibErrorCode.IDS_ERR_HID_BUS_ERROR;
                                return false;
                            }
                        }
                        else
                        {
                            wDataOutLength = 0;
                        }
                        break;
                    }
                case BUS_TYPE.BUS_TYPE_SPI:
                    {
                        m_SendBuffer[0] = 0x00; //Endpoint 0
                        m_SendBuffer[1] = 0x02;
                        m_SendBuffer[2] = 0x01;
                        m_SendBuffer[3] = 0x00;
                        m_SendBuffer[4] = (byte)yDataIn.Length;

                        if (yDataIn.GetLength(0) > (m_SendBuffer.Length - 1))
                        {
                            ErrorCode = LibErrorCode.IDS_ERR_HID_INVALID_LENGTH;
                            return bReturn;
                        }
                        if (DeviceHandler == null)
                        {
                            ErrorCode = LibErrorCode.IDS_ERR_HID_INVALID_HANDLE;
                            return false;
                        }
                        Array.Copy(yDataIn, 0, m_SendBuffer, 5, yDataIn.Length);
                        m_SendSize = 5 + yDataIn.Length;
                        bReturn = O2HIDControlCommand();
                        if (bReturn == true)
                        {
                            if (m_ReceiveSize > 4)
                            {
                                wDataOutLength = (UInt16)(m_ReceiveSize - 4);
                                if (wDataOutLength < wDataInLength)
                                {
                                    ErrorCode = LibErrorCode.IDS_ERR_HID_INVALID_HARDWARE;
                                    return false;
                                }
                                Buffer.BlockCopy(m_ReceiveBuffer, 5, yDataOut, 0, yDataOut.Length);//Endpoint, SPI Write command
                                ErrorCode = LibErrorCode.IDS_ERR_SUCCESSFUL;
                            }
                            else
                            {
                                ErrorCode = LibErrorCode.IDS_ERR_HID_BUS_ERROR;
                                return false;
                            }
                        }
                        else
                        {
                            wDataOutLength = 0;
                        }
                        break;
                    }
                case BUS_TYPE.BUS_TYPE_RS232:
                    {
                        m_SendBuffer[0] = 0x00; //Endpoint 0
                        m_SendBuffer[1] = 0x01;
                        m_SendBuffer[2] = 0x01;
                        m_SendBuffer[3] = (byte)(wDataInLength >> 8);
                        m_SendBuffer[4] = (byte)wDataInLength;

                        if (yDataIn.GetLength(0) > (m_SendBuffer.Length - 1))
                        {
                            ErrorCode = LibErrorCode.IDS_ERR_HID_INVALID_LENGTH;
                            return bReturn;
                        }
                        if (DeviceHandler == null)
                        {
                            ErrorCode = LibErrorCode.IDS_ERR_HID_INVALID_HANDLE;
                            return false;
                        }

                        Array.Copy(yDataIn, 0, m_SendBuffer, 5, wDataInLength);
                        m_SendSize = 5 + wDataInLength;
                        bReturn = O2HIDControlCommand();
                        if (bReturn == true)
                        {
                            if (m_ReceiveSize > 5)
                            {
                                wDataOutLength = (UInt16)(m_ReceiveSize - 5);
                                if (wDataOutLength < wDataInLength)
                                {
                                    ErrorCode = LibErrorCode.IDS_ERR_HID_INVALID_HARDWARE;
                                    return false;
                                }
                                Buffer.BlockCopy(m_ReceiveBuffer, 5, yDataOut, 0, yDataOut.Length);//Endpoint,Write command
                                ErrorCode = LibErrorCode.IDS_ERR_SUCCESSFUL;
                            }
                            else
                            {
                                ErrorCode = LibErrorCode.IDS_ERR_HID_BUS_ERROR;
                                return false;
                            }
                        }
                        else
                        {
                            wDataOutLength = 0;
                        }
                        break;
                    }
            }
            return bReturn;
        }

        public override bool ResetInf()
        {
            bool bReturn = true;
            return bReturn;
        }

        public override bool SetConfigure(List<UInt32> wConfig)
        {
            bool bReturn = false;
            if (wConfig.Count < 1)
            {
                ErrorCode = LibErrorCode.IDS_ERR_HID_INVALID_LENGTH;
                return bReturn;
            }
            if (DeviceHandler == null)
            {
                ErrorCode = LibErrorCode.IDS_ERR_HID_INVALID_HANDLE;
                return false;
            }
            switch (m_busopDev.BusType)
            {
                case BUS_TYPE.BUS_TYPE_I2C:
                    {
                        UInt32 wFreq = (wConfig[0] * 1000);
                        m_SendBuffer[0] = 0x00; //Endpoint 0
                        m_SendBuffer[1] = 0x03;
                        m_SendBuffer[2] = 0x03;
                        m_SendBuffer[3] = (byte)(wFreq >> 24);
                        m_SendBuffer[4] = (byte)(wFreq >> 16);
                        m_SendBuffer[5] = (byte)(wFreq >> 8);
                        m_SendBuffer[6] = (byte)wFreq;
                        m_SendBuffer[7] = 0;
                        bReturn = O2HIDControlCommand();
                        if (bReturn)
                            ErrorCode = LibErrorCode.IDS_ERR_SUCCESSFUL;
                        break;
                    }
                case BUS_TYPE.BUS_TYPE_SPI:
                    {
                        UInt32 wCfg = wConfig[0];
                        UInt32 wSpeed = wConfig[1];
                        if (!bSet)
                        {
                            bSet = true; //To avoid the first time
                            m_SendBuffer[0] = 0x00; //Endpoint 0
                            m_SendBuffer[1] = 0x02;
                            m_SendBuffer[2] = 0x02;
                            m_SendBuffer[3] = 0x00; //Data Length
                            m_SendBuffer[4] = 0x00; //Data Length
                            m_SendBuffer[5] = 0x00; //Data Size
                            m_SendBuffer[6] = (byte)(((wCfg & 0x01) == 0x01) ? 0 : 1); //MSB First Reversed with O2adapter
                            m_SendBuffer[9] = (byte)(((wCfg & 0x04) == 0x04) ? 1 : 0); //Clock Polarity 
                            m_SendBuffer[10] = (byte)(((wCfg & 0x02) == 0x02) ? 1 : 0); //Clock Phase 
                            bReturn = true;
                        }
                        else
                        {
                            bSet = false; //To avoid the first time
                            m_SendBuffer[7] = (byte)(wSpeed >> 8); //Speed MSB First 
                            m_SendBuffer[8] = (byte)wSpeed; //Speed LSB First 
                            bReturn = O2HIDControlCommand();
                        }
                        if (bReturn)
                            ErrorCode = LibErrorCode.IDS_ERR_SUCCESSFUL;
                        break;
                    }
                case BUS_TYPE.BUS_TYPE_RS232:
                    {
                        UInt32 wFreq = wConfig[0];
                        if (wConfig[2] != 2) wConfig[2] = 1;
                        m_SendBuffer[0] = 0x00; //Endpoint 0
                        m_SendBuffer[1] = 0x01;
                        m_SendBuffer[2] = 0x03;
                        m_SendBuffer[3] = 0x00;
                        m_SendBuffer[4] = 0x07;
                        m_SendBuffer[5] = (byte)(wFreq >> 24);
                        m_SendBuffer[6] = (byte)(wFreq >> 16);
                        m_SendBuffer[7] = (byte)(wFreq >> 8);
                        m_SendBuffer[8] = (byte)wFreq;
                        m_SendBuffer[9] = (byte)wConfig[1];
                        m_SendBuffer[10] = (byte)wConfig[3];
                        m_SendBuffer[11] = (byte)(wConfig[2]);
                        bReturn = O2HIDControlCommand();
                        if (bReturn)
                            ErrorCode = LibErrorCode.IDS_ERR_SUCCESSFUL;
                        break;
                    }
            }
            return bReturn;
        }

        public override bool GetConfigure(ref List<UInt32> wConfig)
        {
            bool bReturn = true;
            return bReturn;
        }

        public override bool SetO2DelayTime(List<UInt32> wDelay)
        {
            bool bReturn = true;
            return bReturn;
        }

        public override bool SetAdapterCommand(ref byte[] yDataIn, ref byte[] yDataOut, ref UInt16 wDataOutLength, UInt16 wDataInLength = 1)
        {
            bool bReturn = true;
            return bReturn;
        }

        #region private methods
        private void PortStreamEndOfTransaction(IAsyncResult result)
        {
            ManualResetEvent e = result.AsyncState as ManualResetEvent;
            if (e != null) e.Set();
        }

        private bool O2HIDControlCommand()
        {
            bool bReturn = false;
            IAsyncResult aRes = null;
            ManualResetEvent streamDoneEvent = new ManualResetEvent(false);
            ErrorCode = LibErrorCode.IDS_ERR_SUCCESSFUL;

            if (!m_Locker.WaitOne(1500))
            {
                ErrorCode = LibErrorCode.IDS_ERR_I2C_BB_TIMEOUT;
                return bReturn;
            }
            try
            {
                DeviceHandler.Write(m_SendBuffer, 0, m_outputReportLength);
                //aRes = DeviceHandler.BeginWrite(m_SendBuffer, 0, m_SendBuffer.Length, PortStreamEndOfTransaction, streamDoneEvent);
            }
            catch (Exception)
            {
            }
            finally
            {
                if ((DeviceHandler != null))
                {
                }
                else
                {
                    if (DeviceHandler != null)
                    {
                        DeviceHandler.Close();
                    }
                    DeviceHandler = null;
                    m_ReceiveSize = 0;
                    bReturn = false;
                }
            }
            streamDoneEvent.Reset();
            try
            {
                bReturn = true;
                aRes = DeviceHandler.BeginRead(m_ReceiveBuffer, 0, m_inputReportLength, PortStreamEndOfTransaction, streamDoneEvent);
            }
            catch (Exception)
            {
            }
            finally
            {
                if (!streamDoneEvent.WaitOne(1000, false))
                {
                    ErrorCode = LibErrorCode.IDS_ERR_I2C_EPP_TIMEOUT;
                    try
                    {
                        m_Locker.Release();
                    }
                    catch (Exception)
                    {
                    }
                    bReturn = false;
                };
                if ((DeviceHandler != null) && (aRes != null))
                {
                    try
                    {
                        m_ReceiveSize = DeviceHandler.EndRead(aRes);
                        if (m_SendBuffer[1] == 0x03)
                        {
                            if (!GetHIDLastErr(m_SendBuffer[1], m_ReceiveBuffer))
                            {
                                try
                                {
                                    m_Locker.Release();
                                }
                                catch (Exception)
                                {
                                }
                                bReturn = false;
                            }
                        }
                        else
                        {
                            bReturn = true;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    if (DeviceHandler != null)
                    {
                        DeviceHandler.Close();
                    }
                    DeviceHandler = null;
                    m_ReceiveSize = 0;
                    bReturn = false;
                }
            }
            if (bReturn)
            {
                try
                {
                    m_Locker.Release();
                }
                catch (Exception)
                {
                }
            }
            return bReturn;
        }
        #endregion
    }
}
