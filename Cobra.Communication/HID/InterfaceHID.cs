using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
////using Cobra.Common;

namespace Cobra.Communication.HID
{
	public abstract class CInterfaceHID : CInterfaceBase
	{
        #region Public Member Declaration
        public const ushort HIDI2C_MAX_SIZE = 58;
        public const ushort HIDRS232_MAX_SIZE = 60;
        public const ushort HIDSPI_MAX_SIZE = 60;


        public enum AdaptorReturn : byte
		{
			ES_DRIVER = 0,
			ES_CONTROLLER = 1,
			ES_HID = 2,
		}

		public enum AdaptorErrCode : byte
		{
			O2_HID_STATUS_OK = 0,
			O2_HID_STATUS_BUS_ERROR = 1,
			O2_HID_STATUS_SLA_ACK = 2,
			O2_HID_STATUS_SLA_NACK = 3,
			O2_HID_STATUS_DATA_NACK = 4,
			O2_HID_STATUS_ARB_LOST = 5,
			O2_HID_STATUS_BUS_BUSY = 6,
			O2_HID_STATUS_LAST_DATA_ACK = 7,
			O2_HID_STATUS_INVALID_PEC = 8,
			O2_HID_NOT_AVAILABLE = 0x9C,//(byte)-100,
			O2_HID_NOT_ENABLED = 0x9B,//(byte)-101,
			O2_HID_READ_ERROR = 0x9A,//(byte)-102,
			O2_HID_WRITE_ERROR = 0x99,//(byte)-103,
			O2_HID_SLAVE_BAD_CONFIG = 0x98,//(byte)-104,
			O2_HID_SLAVE_READ_ERROR = 0x97,//(byte)-105,
			O2_HID_SLAVE_TIMEOUT = 0x96,//(byte)-106,
			O2_HID_DROPPED_EXCESS_BYTES = 0x95,//(byte)-107,
			O2_HID_BUS_ALREADY_FREE = 0x94,//(byte)-108,
			O2_HID_INVALID_HANDLE = 0x93,//(byte)-109,
			O2_HID_INVALID_PARAMETER = 0x92,//(byte)-110,
			O2_HID_INVALID_LENGTH = 0x91,//(byte)-111,
			O2_HID_INVALID_COMMAND = 0x90,//(byte)-112,
			O2_HID_COMMAND_DISMATCH = 0x8F,//(byte)-113,
		}

		// <summary>
		// HID frequence value
		// </summary>
		private UInt16 m_Frequence;
		public UInt16 HIDFrequence { get { return m_Frequence; } set { m_Frequence = value; } }
		#endregion

		#region Public Method
		public bool GetHIDLastErr(byte yAdptorCmd, byte[] yDataArry)
		{
			//byte yReturn = (byte)AdaptorErrCode.O2_I2C_STATUS_OK;
			bool bReturn = true;

			ErrorCode = LibErrorCode.IDS_ERR_SUCCESSFUL;
			if (yAdptorCmd != yDataArry[1])
			{
				bReturn = false;
				ErrorCode = LibErrorCode.IDS_ERR_I2C_CMD_DISMATCH;
			}
			return bReturn;// yReturn;
		}
		#endregion
	}
}
