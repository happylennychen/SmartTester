﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTesterLib.Drivers.Testers.PackTester
{

    public class LibErrorCode
    {
        #region Private Member
        private static Dictionary<UInt32, string> m_dynamicErrorLib_dic = new Dictionary<UInt32, string>();
        private static ObservableCollection<string> m_error_info_list = new ObservableCollection<string>();
        #endregion

        #region Public Error Code Definition
        ///// <summary>
        ///// Error code definition
        ///// </summary>
        public const UInt32 IDS_ERR_SUCCESSFUL = 0x00000000;

        //#region Communication Layer Error code definition, IDS_ERR_SECTION_COMMUNICATE = 0x00010000
        //private const UInt32 IDS_ERR_SECTION_COMMUNICATE = 0x00010000;
        //public const UInt32 IDS_ERR_MGR_UNABLE_LOAD_DRIVER = IDS_ERR_SECTION_COMMUNICATE + 0x01;
        //public const UInt32 IDS_ERR_MGR_UNABLE_LOAD_FUNCTION = IDS_ERR_SECTION_COMMUNICATE + 0x02;
        //public const UInt32 IDS_ERR_MGR_UNABLE_FIND_DEVICE = IDS_ERR_SECTION_COMMUNICATE + 0x03;
        //public const UInt32 IDS_ERR_MGR_INVALID_INTERFACE_TYPE = IDS_ERR_SECTION_COMMUNICATE + 0x04;
        //public const UInt32 IDS_ERR_MGR_INVALID_INTERFACE_CONFIG = IDS_ERR_SECTION_COMMUNICATE + 0x05;
        //public const UInt32 IDS_ERR_MGR_INVALID_INTERFACE_HANDLER = IDS_ERR_SECTION_COMMUNICATE + 0x06;
        //public const UInt32 IDS_ERR_MGR_INVALID_INPUT_BUFFER = IDS_ERR_SECTION_COMMUNICATE + 0x10;
        //public const UInt32 IDS_ERR_MGR_INVALID_OUTPUT_BUFFER = IDS_ERR_SECTION_COMMUNICATE + 0x11;
        //public const UInt32 IDS_ERR_MGR_CONFIG_SVID_BUSTYPE = IDS_ERR_SECTION_COMMUNICATE + 0x12;
        //public const UInt32 IDS_ERR_MGR_CONFIG_BUFFER_NOT_ENOUGH = IDS_ERR_SECTION_COMMUNICATE + 0x13;
        //public const UInt32 IDS_ERR_MGR_CONFIG_SVID_NOT_SUPPORT = IDS_ERR_SECTION_COMMUNICATE + 0x14;
        //public const UInt32 IDS_ERR_MGR_NULL_PORT_NODE = IDS_ERR_SECTION_COMMUNICATE + 0x15;

        //private const UInt32 IDS_ERR_SECTOR_I2C = IDS_ERR_SECTION_COMMUNICATE + 0x00001000;
        //public const UInt32 IDS_ERR_I2C_BB_TIMEOUT = IDS_ERR_SECTOR_I2C + 0x01;
        //public const UInt32 IDS_ERR_I2C_PIN_TIMEOUT = IDS_ERR_SECTOR_I2C + 0x02;
        //public const UInt32 IDS_ERR_I2C_PROTOCOL_TIMEOUT = IDS_ERR_SECTOR_I2C + 0x03;
        //public const UInt32 IDS_ERR_I2C_EPP_TIMEOUT = IDS_ERR_SECTOR_I2C + 0x04;
        //public const UInt32 IDS_ERR_I2C_DRIVER_TIMEOUT = IDS_ERR_SECTOR_I2C + 0x05;
        //public const UInt32 IDS_ERR_I2C_NMS_TIMEOUT = IDS_ERR_SECTOR_I2C + 0x06;
        //public const UInt32 IDS_ERR_I2C_CMD_DISMATCH = IDS_ERR_SECTOR_I2C + 0x07;
        //public const UInt32 IDS_ERR_I2C_INVALID_HANDLE = IDS_ERR_SECTOR_I2C + 0x11;
        //public const UInt32 IDS_ERR_I2C_INVALID_PARAMETER = IDS_ERR_SECTOR_I2C + 0x12;
        //public const UInt32 IDS_ERR_I2C_INVALID_LENGTH = IDS_ERR_SECTOR_I2C + 0x13;
        //public const UInt32 IDS_ERR_I2C_INVALID_COMMAND = IDS_ERR_SECTOR_I2C + 0x14;
        //public const UInt32 IDS_ERR_I2C_INVALID_BLOCK_SIZE = IDS_ERR_SECTOR_I2C + 0x15;
        //public const UInt32 IDS_ERR_I2C_INVALID_BUFFER = IDS_ERR_SECTOR_I2C + 0x16;
        //public const UInt32 IDS_ERR_I2C_INVALID_INDEX = IDS_ERR_SECTOR_I2C + 0x17;
        //public const UInt32 IDS_ERR_I2C_INVALID_HARDWARE = IDS_ERR_SECTOR_I2C + 0x18;
        //public const UInt32 IDS_ERR_I2C_BUS_ERROR = IDS_ERR_SECTOR_I2C + 0x21;
        //public const UInt32 IDS_ERR_I2C_SLA_ACK = IDS_ERR_SECTOR_I2C + 0x22;
        //public const UInt32 IDS_ERR_I2C_SLA_NACK = IDS_ERR_SECTOR_I2C + 0x23;
        //public const UInt32 IDS_ERR_I2C_LOST_ARBITRATION = IDS_ERR_SECTOR_I2C + 0x24;
        //public const UInt32 IDS_ERR_I2C_CFG_INVALID_CONFIG_TYPE = IDS_ERR_SECTOR_I2C + 0x30;
        //public const UInt32 IDS_ERR_I2C_CFG_FREQUENCY_LIMIT = IDS_ERR_SECTOR_I2C + 0x31;
        //public const UInt32 IDS_ERR_I2C_CFG_FREQUENCY_ERROR = IDS_ERR_SECTOR_I2C + 0x32;

        //private const UInt32 IDS_ERR_SECTOR_SPI = IDS_ERR_SECTION_COMMUNICATE + 0x00002000;
        //public const UInt32 IDS_ERR_SPI_BB_TIMEOUT = IDS_ERR_SECTOR_SPI + 0x01;
        //public const UInt32 IDS_ERR_SPI_PIN_TIMEOUT = IDS_ERR_SECTOR_SPI + 0x02;
        //public const UInt32 IDS_ERR_SPI_PROTOCOL_TIMEOUT = IDS_ERR_SECTOR_SPI + 0x03;
        //public const UInt32 IDS_ERR_SPI_EPP_TIMEOUT = IDS_ERR_SECTOR_SPI + 0x04;
        //public const UInt32 IDS_ERR_SPI_DRIVER_TIMEOUT = IDS_ERR_SECTOR_SPI + 0x05;
        //public const UInt32 IDS_ERR_SPI_NMS_TIMEOUT = IDS_ERR_SECTOR_SPI + 0x06;
        //public const UInt32 IDS_ERR_SPI_INVALID_HANDLE = IDS_ERR_SECTOR_SPI + 0x11; //new
        //public const UInt32 IDS_ERR_SPI_INVALID_PARAMETER = IDS_ERR_SECTOR_SPI + 0x12;
        //public const UInt32 IDS_ERR_SPI_INVALID_LENGTH = IDS_ERR_SECTOR_SPI + 0x13;
        //public const UInt32 IDS_ERR_SPI_INVALID_COMMAND = IDS_ERR_SECTOR_SPI + 0x14;
        //public const UInt32 IDS_ERR_SPI_INVALID_BLOCK_SIZE = IDS_ERR_SECTOR_SPI + 0x15;
        //public const UInt32 IDS_ERR_SPI_INVALID_BUFFER = IDS_ERR_SECTOR_SPI + 0x16;
        //public const UInt32 IDS_ERR_SPI_INVALID_INDEX = IDS_ERR_SECTOR_SPI + 0x17;
        //public const UInt32 IDS_ERR_SPI_INVALID_HARDWARE = IDS_ERR_SECTOR_SPI + 0x18;
        //public const UInt32 IDS_ERR_SPI_CFG_INVALID_CONFIG_TYPE = IDS_ERR_SECTOR_SPI + 0x30;    //new
        //public const UInt32 IDS_ERR_SPI_CFG_CONFIG_ERROR = IDS_ERR_SECTOR_SPI + 0x31;
        //public const UInt32 IDS_ERR_SPI_CFG_CONFIG_VALUE_ERROR = IDS_ERR_SECTOR_SPI + 0x32;
        //public const UInt32 IDS_ERR_SPI_CFG_BAURATE_ERROR = IDS_ERR_SECTOR_SPI + 0x33;
        //public const UInt32 IDS_ERR_SPI_CFG_BAURATE_LIMIT = IDS_ERR_SECTOR_SPI + 0x34;
        //public const UInt32 IDS_ERR_SPI_CRC_CHECK = IDS_ERR_SECTOR_SPI + 0x35;
        //public const UInt32 IDS_ERR_SPI_DATA_MISMATCH = IDS_ERR_SECTOR_SPI + 0x36;
        //public const UInt32 IDS_ERR_SPI_CMD_MISMATCH = IDS_ERR_SECTOR_SPI + 0x37;

        //private const UInt32 IDS_ERR_SECTOR_SVID = IDS_ERR_SECTION_COMMUNICATE + 0x00003000;
        //public const UInt32 IDS_ERR_SVID_INVALID_PARAMETER = IDS_ERR_SECTOR_SVID + 0x01;
        //public const UInt32 IDS_ERR_SVID_OPEN_FAILED = IDS_ERR_SECTOR_SVID + 0x02;
        //public const UInt32 IDS_ERR_SVID_READ_BUFFER_NOT_ENOUGH = IDS_ERR_SECTOR_SVID + 0x03;
        //public const UInt32 IDS_ERR_SVID_IN_PARAMETER_INVALID = IDS_ERR_SECTOR_SVID + 0x04;
        //public const UInt32 IDS_ERR_SVID_INDEX_OUT = IDS_ERR_SECTOR_SVID + 0x05;
        //public const UInt32 IDS_ERR_SVID_NULL_COM_HANDLER = IDS_ERR_SECTOR_SVID + 0x06;
        //public const UInt32 IDS_ERR_SVID_COM_NOT_EXIST = IDS_ERR_SECTOR_SVID + 0x07;
        //public const UInt32 IDS_ERR_SVID_INVALID_READI2CSINGLE = IDS_ERR_SECTOR_SVID + 0x10;
        //public const UInt32 IDS_ERR_SVID_INVALID_READI2CBLOCK = IDS_ERR_SECTOR_SVID + 0x11;
        //public const UInt32 IDS_ERR_SVID_INVALID_READVRSINGLE = IDS_ERR_SECTOR_SVID + 0x12;
        //public const UInt32 IDS_ERR_SVID_INVALID_READVRWORD = IDS_ERR_SECTOR_SVID + 0x13;
        //public const UInt32 IDS_ERR_SVID_INVALID_READVRBLOCK = IDS_ERR_SECTOR_SVID + 0x14;
        //public const UInt32 IDS_ERR_SVID_INVALID_WRITEI2CSINGLE = IDS_ERR_SECTOR_SVID + 0x15;
        //public const UInt32 IDS_ERR_SVID_INVALID_WRITEI2CWORD = IDS_ERR_SECTOR_SVID + 0x16;
        //public const UInt32 IDS_ERR_SVID_INVALID_WRITEI2CBLOCK = IDS_ERR_SECTOR_SVID + 0x17;
        //public const UInt32 IDS_ERR_SVID_INVALID_WRITEVRSINGLE = IDS_ERR_SECTOR_SVID + 0x18;
        //public const UInt32 IDS_ERR_SVID_INVALID_WRITEVRBLOCK = IDS_ERR_SECTOR_SVID + 0x19;
        //public const UInt32 IDS_ERR_SVID_INVALID_ENUMMETHOD = IDS_ERR_SECTOR_SVID + 0x1A;
        //public const UInt32 IDS_ERR_SVID_COM_READ_ZERO = IDS_ERR_SECTOR_SVID + 0x1B;
        //public const UInt32 IDS_ERR_SVID_COM_TIMEOUT = IDS_ERR_SECTOR_SVID + 0x1C;
        //public const UInt32 IDS_ERR_SVID_READ_NOT_ENOUGH = IDS_ERR_SECTOR_SVID + 0x1D;
        //public const UInt32 IDS_ERR_SVID_READ_FAILED = IDS_ERR_SECTOR_SVID + 0x1E;

        //private const UInt32 IDS_ERR_SECTOR_COM = IDS_ERR_SECTION_COMMUNICATE + 0x00004000;
        //public const UInt32 IDS_ERR_COM_INVALID_PARAMETER = IDS_ERR_SECTOR_COM + 0x01;
        //public const UInt32 IDS_ERR_COM_OPEN_FAILED = IDS_ERR_SECTOR_COM + 0x02;
        //public const UInt32 IDS_ERR_COM_READ_BUFFER_NOT_ENOUGH = IDS_ERR_SECTOR_COM + 0x03;
        //public const UInt32 IDS_ERR_COM_IN_PARAMETER_INVALID = IDS_ERR_SECTOR_COM + 0x04;
        //public const UInt32 IDS_ERR_COM_INDEX_OUT = IDS_ERR_SECTOR_COM + 0x05;
        //public const UInt32 IDS_ERR_COM_NULL_COM_HANDLER = IDS_ERR_SECTOR_COM + 0x06;
        //public const UInt32 IDS_ERR_COM_COM_NOT_EXIST = IDS_ERR_SECTOR_COM + 0x07;
        //public const UInt32 IDS_ERR_COM_INVALID_READI2CSINGLE = IDS_ERR_SECTOR_COM + 0x10;
        //public const UInt32 IDS_ERR_COM_INVALID_READI2CBLOCK = IDS_ERR_SECTOR_COM + 0x11;
        //public const UInt32 IDS_ERR_COM_INVALID_READVRSINGLE = IDS_ERR_SECTOR_COM + 0x12;
        //public const UInt32 IDS_ERR_COM_INVALID_READVRWORD = IDS_ERR_SECTOR_COM + 0x13;
        //public const UInt32 IDS_ERR_COM_INVALID_READVRBLOCK = IDS_ERR_SECTOR_COM + 0x14;
        //public const UInt32 IDS_ERR_COM_INVALID_WRITEI2CSINGLE = IDS_ERR_SECTOR_COM + 0x15;
        //public const UInt32 IDS_ERR_COM_INVALID_WRITEI2CWORD = IDS_ERR_SECTOR_COM + 0x16;
        //public const UInt32 IDS_ERR_COM_INVALID_WRITEI2CBLOCK = IDS_ERR_SECTOR_COM + 0x17;
        //public const UInt32 IDS_ERR_COM_INVALID_WRITEVRSINGLE = IDS_ERR_SECTOR_COM + 0x18;
        //public const UInt32 IDS_ERR_COM_INVALID_WRITEVRBLOCK = IDS_ERR_SECTOR_COM + 0x19;
        //public const UInt32 IDS_ERR_COM_INVALID_ENUMMETHOD = IDS_ERR_SECTOR_COM + 0x1A;
        //public const UInt32 IDS_ERR_COM_COM_READ_ZERO = IDS_ERR_SECTOR_COM + 0x1B;
        //public const UInt32 IDS_ERR_COM_COM_TIMEOUT = IDS_ERR_SECTOR_COM + 0x1C;
        //public const UInt32 IDS_ERR_COM_READ_NOT_ENOUGH = IDS_ERR_SECTOR_COM + 0x1D;
        //public const UInt32 IDS_ERR_COM_READ_FAILED = IDS_ERR_SECTOR_COM + 0x1E;
        //#endregion

        private const UInt32 IDS_ERR_SECTION_DEM = 0x00020000;
        //public const UInt32 IDS_ERR_SECTION_DYNAMIC_DEM = IDS_ERR_SECTION_DEM + 0x8000;
        public const UInt32 IDS_ERR_BUS_DATA_PEC_ERROR = IDS_ERR_SECTION_DEM + 0x01;
        //public const UInt32 IDS_ERR_DEM_PARAMETERLIST_EMPTY = IDS_ERR_SECTION_DEM + 0x02;
        //public const UInt32 IDS_ERR_DEM_BIT_TIMEOUT = IDS_ERR_SECTION_DEM + 0x03;
        public const UInt32 IDS_ERR_DEM_FUN_TIMEOUT = IDS_ERR_SECTION_DEM + 0x04;
        public const UInt32 IDS_ERR_DEM_BETWEEN_SELECT_BOARD = IDS_ERR_SECTION_DEM + 0x05;
        public const UInt32 IDS_ERR_DEM_LOST_PARAMETER = IDS_ERR_SECTION_DEM + 0x06;
        //public const UInt32 IDS_ERR_DEM_USER_QUIT = IDS_ERR_SECTION_DEM + 0x07;
        //public const UInt32 IDS_ERR_DEM_PARAM_READ_UNABLE = IDS_ERR_SECTION_DEM + 0x08;
        //public const UInt32 IDS_ERR_DEM_PARAM_WRITE_UNABLE = IDS_ERR_SECTION_DEM + 0x09;
        //public const UInt32 IDS_ERR_DEM_PARAM_READ_WRITE_UNABLE = IDS_ERR_SECTION_DEM + 0x0A;
        //public const UInt32 IDS_ERR_DEM_PASSWORD_UNMATCH = IDS_ERR_SECTION_DEM + 0x0B;
        //public const UInt32 IDS_ERR_DEM_PARAM_CONTAINER_SIZE = IDS_ERR_SECTION_DEM + 0x0C;
        public const UInt32 IDS_ERR_DEM_BUF_CHECK_FAIL = IDS_ERR_SECTION_DEM + 0x0D;
        public const UInt32 IDS_ERR_DEM_FROZEN = IDS_ERR_SECTION_DEM + 0x0E;
        //public const UInt32 IDS_ERR_DEM_ATE_CRC_ERROR = IDS_ERR_SECTION_DEM + 0x0F;
        //public const UInt32 IDS_ERR_DEM_PASSWORD_INVALID = IDS_ERR_SECTION_DEM + 0x10;
        //public const UInt32 IDS_ERR_DEM_ADC_STOPPED = IDS_ERR_SECTION_DEM + 0x11;
        public const UInt32 IDS_ERR_DEM_DIRTYCHIP = IDS_ERR_SECTION_DEM + 0x12;
        public const UInt32 IDS_ERR_DEM_MAPPING_TIMEOUT = IDS_ERR_SECTION_DEM + 0x13;
        //public const UInt32 IDS_ERR_DEM_LOST_INTERFACE = IDS_ERR_SECTION_DEM + 0x14;
        //public const UInt32 IDS_ERR_DEM_LOAD_BIN_FILE_ERROR = IDS_ERR_SECTION_DEM + 0x15;
        //public const UInt32 IDS_ERR_DEM_BIN_LENGTH_ERROR = IDS_ERR_SECTION_DEM + 0x16;
        //public const UInt32 IDS_ERR_DEM_BIN_ADDRESS_ERROR = IDS_ERR_SECTION_DEM + 0x17;



        //private const UInt32 IDS_ERR_SECTION_DM = 0x00030000;
        //public const UInt32 IDS_ERR_PARAM_INVALID_HANDLER = IDS_ERR_SECTION_DM + 0x01;
        //public const UInt32 IDS_ERR_PARAM_HEX_DATA_OVERMAXRANGE = IDS_ERR_SECTION_DM + 0x02;
        //public const UInt32 IDS_ERR_PARAM_HEX_DATA_OVERMINRANGE = IDS_ERR_SECTION_DM + 0x03;
        //public const UInt32 IDS_ERR_PARAM_PHY_DATA_OVERMAXRANGE = IDS_ERR_SECTION_DM + 0x04;
        //public const UInt32 IDS_ERR_PARAM_PHY_DATA_OVERMINRANGE = IDS_ERR_SECTION_DM + 0x05;
        //public const UInt32 IDS_ERR_PARAM_DATA_ILLEGAL = IDS_ERR_SECTION_DM + 0x06;

        //private const UInt32 IDS_ERR_SECTION_FOLDERMAP = 0x00040000;
        //public const UInt32 IDS_ERR_SECTION_CANNOT_CREATE_FOLDER_COM = IDS_ERR_SECTION_FOLDERMAP + 0x01;
        //public const UInt32 IDS_ERR_SECTION_FOLDERS_LOST = IDS_ERR_SECTION_FOLDERMAP + 0x02;
        //public const UInt32 IDS_ERR_SECTION_CANNOT_ACCESS_COBRA_FOLDER = IDS_ERR_SECTION_FOLDERMAP + 0x03;
        //public const UInt32 IDS_ERR_SECTION_CANNOT_ACCESS_ExtRT_FOLDER = IDS_ERR_SECTION_FOLDERMAP + 0x04;
        //public const UInt32 IDS_ERR_SECTION_CANNOT_ACCESS_ExtMT_FOLDER = IDS_ERR_SECTION_FOLDERMAP + 0x05;
        //public const UInt32 IDS_ERR_SECTION_CANNOT_ACCESS_COBRA_DOC = IDS_ERR_SECTION_FOLDERMAP + 0x06;
        //public const UInt32 IDS_ERR_SECTION_CANNOT_ACCESS_LOG = IDS_ERR_SECTION_FOLDERMAP + 0x07;
        //public const UInt32 IDS_ERR_SECTION_CANNOT_ACCESS_SET_FOLDER = IDS_ERR_SECTION_FOLDERMAP + 0x08;
        //public const UInt32 IDS_ERR_SECTION_LOST_SET_FILES = IDS_ERR_SECTION_FOLDERMAP + 0x09;

        //private const UInt32 IDS_ERR_SECTION_EM = 0x00200000;
        //public const UInt32 IDS_ERR_EM_THREAD_BKWORKER_BUSY = IDS_ERR_SECTION_EM + 0x01;

        //#region IDS_ERR_SECTION_OCE = 0x00060000;  //ID:592
        //private const UInt32 IDS_ERR_SECTION_OCE = 0x00060000;
        //public const UInt32 IDS_ERR_SECTION_OCE_DIS_DEM = IDS_ERR_SECTION_OCE + 01;
        //public const UInt32 IDS_ERR_SECTION_OCE_LOSE_FILE = IDS_ERR_SECTION_OCE + 02;
        //public const UInt32 IDS_ERR_SECTION_OCE_DIS_FILE_ATTRIBUTE = IDS_ERR_SECTION_OCE + 03;
        //public const UInt32 IDS_ERR_SECTION_OCE_UNZIP = IDS_ERR_SECTION_OCE + 04;
        //public const UInt32 IDS_ERR_SECTION_OCE_NOT_EXIST = IDS_ERR_SECTION_OCE + 05;
        //public const UInt32 IDS_ERR_SECTION_OCE_NOT_LOWER = IDS_ERR_SECTION_OCE + 06; //Issue1289 Leon
        //public const UInt32 IDS_ERR_SECTION_OCE_MISMATCH_20024 = IDS_ERR_SECTION_OCE + 07;
        //#endregion

        //#region Cobra Center SFL Error code definition, IDS_ERR_SECTION_COBRACENTER = 0x00050000;
        //private const UInt32 IDS_ERR_SECTION_COBRACENTER = 0x00050000;
        //public const UInt32 IDS_ERR_SECTION_CENTER_INTERNET = IDS_ERR_SECTION_COBRACENTER + 01;
        //public const UInt32 IDS_ERR_SECTION_CENTER_IP_NACK = IDS_ERR_SECTION_COBRACENTER + 02;
        //public const UInt32 IDS_ERR_SECTION_CENTER_USERNAME = IDS_ERR_SECTION_COBRACENTER + 03;
        //public const UInt32 IDS_ERR_SECTION_CENTER_PASSWORD = IDS_ERR_SECTION_COBRACENTER + 04;
        //public const UInt32 IDS_ERR_SECTION_CENTER_UNAUTHORIZED = IDS_ERR_SECTION_COBRACENTER + 05;
        //public const UInt32 IDS_ERR_SECTION_CENTER_NOFOUND = IDS_ERR_SECTION_COBRACENTER + 06;
        //public const UInt32 IDS_ERR_SECTION_CENTER_DOWNLOAD_BUSY = IDS_ERR_SECTION_COBRACENTER + 07;
        //public const UInt32 IDS_ERR_SECTION_CENTER_OCE_VERSION_LOW = IDS_ERR_SECTION_COBRACENTER + 08;
        //#endregion

        //#region Device Configuration SFL Error code definition, IDS_ERR_SECTION_DEVICECONFSFL = 0x10000000
        //public const UInt32 IDS_ERR_SECTION_DEVICECONFSFL = 0x10000000;
        //public const UInt32 IDS_ERR_SECTION_DEVICECONFSFL_PARAM_INVALID = IDS_ERR_SECTION_DEVICECONFSFL + 0x00;
        //public const UInt32 IDS_ERR_SECTION_DEVICECONFSFL_PARAM_UNENALBE = IDS_ERR_SECTION_DEVICECONFSFL + 0x01;
        //public const UInt32 IDS_ERR_SECTION_DEVICECONFSFL_PARAM_VERIFY = IDS_ERR_SECTION_DEVICECONFSFL + 0x02;
        //#endregion

        #region Production SFL Error code definition, IDS_ERR_SECTION_PRODUCTIONSFL = 0x10030000;
        public const UInt32 IDS_ERR_SECTION_PRODUCTIONSFL = 0x10030000;
        public const UInt32 IDS_ERR_SECTION_PRODUCTIONSFL_POWERON_FAILED = IDS_ERR_SECTION_PRODUCTIONSFL + 0x0001;
        public const UInt32 IDS_ERR_SECTION_PRODUCTIONSFL_POWEROFF_FAILED = IDS_ERR_SECTION_PRODUCTIONSFL + 0x0002;
        public const UInt32 IDS_ERR_SECTION_PRODUCTIONSFL_POWERCHECK_FAILED = IDS_ERR_SECTION_PRODUCTIONSFL + 0x0003;
        public const UInt32 IDS_ERR_SECTION_PRODUCTIONSFL_LOADBIN_FAILED = IDS_ERR_SECTION_PRODUCTIONSFL + 0x0004;
        #endregion

        //#region Exper SFL Error code definition, IDS_ERR_SECTION_EXPERSFL = 0x10040000
        //public const UInt32 IDS_ERR_SECTION_EXPERSFL = 0x10040000;
        //private const UInt32 IDS_ERR_EXPSFL_GENERAL = IDS_ERR_SECTION_EXPERSFL + 0x1000;
        //private const UInt32 IDS_ERR_EXPSFL_OPREG = IDS_ERR_SECTION_EXPERSFL + 0x2000;
        //private const UInt32 IDS_ERR_EXPSFL_TESTMODE = IDS_ERR_SECTION_EXPERSFL + 0x3000;
        //public const UInt32 IDS_ERR_EXPSFL_XML = IDS_ERR_EXPSFL_GENERAL + 0x01;
        //public const UInt32 IDS_ERR_EXPSFL_DATABINDING = IDS_ERR_EXPSFL_GENERAL + 0x02;
        //public const UInt32 IDS_ERR_EXPSFL_OPREG_NOT_FOUND = IDS_ERR_EXPSFL_OPREG + 0x01;
        //#endregion

        //#region Scan SFL Error code definition, IDS_ERR_SECTION_SCANSFL = 0x10050000
        //public const UInt32 IDS_ERR_SECTION_SCANSFL = 0x10050000;
        //#endregion

        //#region SCS SFL Error code definition, IDS_ERR_SECTION_SCCSFL = 0x10060000
        //public const UInt32 IDS_ERR_SECTION_SCCSFL = 0x10060000;
        //public const UInt32 IDS_ERR_SCSSFL_INVALIDITEM = IDS_ERR_SECTION_SCCSFL + 0x01;
        //public const UInt32 IDS_ERR_SCSSFL_SCANDONE = IDS_ERR_SECTION_SCCSFL + 0x02;
        //#endregion

        //#region SBS SFL Error code definition, IDS_ERR_SECTION_SBSSFL = 0x10070000
        //public const UInt32 IDS_ERR_SECTION_SBSSFL = 0x10070000;
        //public const UInt32 IDS_ERR_SBSSFL_GG_ACCESS = IDS_ERR_SECTION_SBSSFL + 0x01;
        //public const UInt32 IDS_ERR_SBSSFL_LOAD_FILE = IDS_ERR_SECTION_SBSSFL + 0x02;
        //public const UInt32 IDS_ERR_SBSSFL_SW_FRAME_HEAD = IDS_ERR_SECTION_SBSSFL + 0x03;
        //public const UInt32 IDS_ERR_SBSSFL_SW_PEC_CHECK = IDS_ERR_SECTION_SBSSFL + 0x04;
        //public const UInt32 IDS_ERR_SBSSFL_WRITE_REGISTER = IDS_ERR_SECTION_SBSSFL + 0x10;
        //public const UInt32 IDS_ERR_SBSSFL_READ_REGISTER = IDS_ERR_SECTION_SBSSFL + 0x11;
        //public const UInt32 IDS_ERR_SBSSFL_POLLING_REGISTER = IDS_ERR_SECTION_SBSSFL + 0x12;
        //public const UInt32 IDS_ERR_SBSSFL_GGDRV_NOVOLTAGE = IDS_ERR_SECTION_SBSSFL + 0x20;
        //public const UInt32 IDS_ERR_SBSSFL_GGDRV_NOCURRENT = IDS_ERR_SECTION_SBSSFL + 0x21;
        //public const UInt32 IDS_ERR_SBSSFL_GGDRV_NOTEMPERATURE = IDS_ERR_SECTION_SBSSFL + 0x22;
        //public const UInt32 IDS_ERR_SBSSFL_GGDRV_NOCAR = IDS_ERR_SECTION_SBSSFL + 0x23;
        //public const UInt32 IDS_ERR_SBSSFL_GGDRV_NOOCVOLTAGE = IDS_ERR_SECTION_SBSSFL + 0x24;
        //public const UInt32 IDS_ERR_SBSSFL_GGDRV_NOPOOCV = IDS_ERR_SECTION_SBSSFL + 0x25;
        //public const UInt32 IDS_ERR_SBSSFL_GGDRV_NOSLEEPOCV = IDS_ERR_SECTION_SBSSFL + 0x26;
        //public const UInt32 IDS_ERR_SBSSFL_GGDRV_NOCTRLSTATUS = IDS_ERR_SECTION_SBSSFL + 0x27;

        //public const UInt32 IDS_ERR_SBSSFL_REQUIREWRITE = IDS_ERR_SECTION_SBSSFL + 0x80;
        //public const UInt32 IDS_ERR_SBSSFL_REQUIREREAD = IDS_ERR_SECTION_SBSSFL + 0x81;

        //public const UInt32 IDS_ERR_SBSSFL_FW_FRAME_HEAD = IDS_ERR_SECTION_SBSSFL + 0xF0;
        //public const UInt32 IDS_ERR_SBSSFL_FW_FRAME_CHECKSUM = IDS_ERR_SECTION_SBSSFL + 0xF1;
        //public const UInt32 IDS_ERR_SBSSFL_FW_COMMAND_NODEF = IDS_ERR_SECTION_SBSSFL + 0xF2;
        //public const UInt32 IDS_ERR_SBSSFL_FW_COMMAND_EXECU = IDS_ERR_SECTION_SBSSFL + 0xF3;
        //public const UInt32 IDS_ERR_SBSSFL_FW_COMMAND_EXECU_TO = IDS_ERR_SECTION_SBSSFL + 0xF4;
        //public const UInt32 IDS_ERR_SBSSFL_FW_I2C_BLOCKED = IDS_ERR_SECTION_SBSSFL + 0xF5;
        //public const UInt32 IDS_ERR_SBSSFL_FW_PEC_CHECK = IDS_ERR_SECTION_SBSSFL + 0xF6;
        //#endregion

        //#region Eagle DLL Error code definition, IDS_ERR_SECTION_EAGLEDLL = 0x10100000
        //public const UInt32 IDS_ERR_SECTION_EAGLEDLL = 0x10100000;
        //private const UInt32 IDS_ERR_EGDLL_PROJECT = IDS_ERR_SECTION_EAGLEDLL + 0x1000;
        //private const UInt32 IDS_ERR_EGDLL_EGFIRM = IDS_ERR_SECTION_EAGLEDLL + 0x2000;
        //public const UInt32 IDS_ERR_EGDLL_DEVICE_NULL = IDS_ERR_EGDLL_PROJECT + 0x01;
        //public const UInt32 IDS_ERR_EGDLL_TASKMSG_NULL = IDS_ERR_EGDLL_PROJECT + 0x02;
        //public const UInt32 IDS_ERR_EGDLL_GGPOLLING_NULL = IDS_ERR_EGDLL_PROJECT + 0x03;
        //public const UInt32 IDS_ERR_EGDLL_GGSETTING_NULL = IDS_ERR_EGDLL_PROJECT + 0x04;
        //public const UInt32 IDS_ERR_EGDLL_SBSREG_NULL = IDS_ERR_EGDLL_PROJECT + 0x05;
        //public const UInt32 IDS_ERR_EGDLL_TABLE_NUMBER = IDS_ERR_EGDLL_PROJECT + 0x06;
        //public const UInt32 IDS_ERR_EGDLL_PROJECT_FILE_NOEXIST = IDS_ERR_EGDLL_PROJECT + 0x07;
        //public const UInt32 IDS_ERR_EGDLL_LIBARY_NOEXIST = IDS_ERR_EGDLL_PROJECT + 0x08;
        //public const UInt32 IDS_ERR_EGDLL_OCVBYTSOC_NOEXIST = IDS_ERR_EGDLL_PROJECT + 0x09;
        //public const UInt32 IDS_ERR_EGDLL_TSOCBYOCV_NOEXIST = IDS_ERR_EGDLL_PROJECT + 0x0A;
        //public const UInt32 IDS_ERR_EGDLL_RC_NOEXIST = IDS_ERR_EGDLL_PROJECT + 0x0B;
        //public const UInt32 IDS_ERR_EGDLL_THERMAL_NOEXIST = IDS_ERR_EGDLL_PROJECT + 0x0C;
        //public const UInt32 IDS_ERR_EGDLL_SELFDSG_NOEXIST = IDS_ERR_EGDLL_PROJECT + 0x0D;
        //public const UInt32 IDS_ERR_EGDLL_RI_NOEXIST = IDS_ERR_EGDLL_PROJECT + 0x0E;
        //public const UInt32 IDS_ERR_EGDLL_CHGTABLE_NOEXIST = IDS_ERR_EGDLL_PROJECT + 0x0F;
        //public const UInt32 IDS_ERR_EGDLL_PROJECT_SET = IDS_ERR_EGDLL_PROJECT + 0x10;
        //public const UInt32 IDS_ERR_EGDLL_OCVBYTSOC_CONTENT = IDS_ERR_EGDLL_PROJECT + 0x11;
        //public const UInt32 IDS_ERR_EGDLL_TSOCBYOCV_CONTENT = IDS_ERR_EGDLL_PROJECT + 0x12;
        //public const UInt32 IDS_ERR_EGDLL_RCTABLE_CONTENT = IDS_ERR_EGDLL_PROJECT + 0x13;
        //public const UInt32 IDS_ERR_EGDLL_THERMALTABLE_CONTENT = IDS_ERR_EGDLL_PROJECT + 0x14;
        //public const UInt32 IDS_ERR_EGDLL_SELFDSGTABLE_CONTENT = IDS_ERR_EGDLL_PROJECT + 0x15;
        //public const UInt32 IDS_ERR_EGDLL_RITABLE_CONTENT = IDS_ERR_EGDLL_PROJECT + 0x16;
        //public const UInt32 IDS_ERR_EGDLL_CHGTABLE_CONTENT = IDS_ERR_EGDLL_PROJECT + 0x17;
        //public const UInt32 IDS_ERR_EGDLL_PROJSET_VALUE = IDS_ERR_EGDLL_PROJECT + 0x18;
        //public const UInt32 IDS_ERR_EGDLL_PROJSET_WRONG_FLAG = IDS_ERR_EGDLL_PROJECT + 0x19;
        //public const UInt32 IDS_ERR_EGDLL_BUS_BUSY = IDS_ERR_EGDLL_EGFIRM + 0x01;
        //public const UInt32 IDS_ERR_EGDLL_REGISTER = IDS_ERR_EGDLL_EGFIRM + 0x02;
        //public const UInt32 IDS_ERR_EGDLL_PARAMETERRW = IDS_ERR_EGDLL_EGFIRM + 0x03;
        //public const UInt32 IDS_ERR_EGDLL_GGREGISTERRW = IDS_ERR_EGDLL_EGFIRM + 0x04;
        //public const UInt32 IDS_ERR_EGDLL_GGPARAMETER = IDS_ERR_EGDLL_EGFIRM + 0x05;
        //#endregion

        //#region TableMaker code definition, IDS_ERR_SECTION_TABLEMAKER = 0x10110000
        //public const UInt32 IDS_ERR_SECTION_TABLEMAKER = 0x10110000;
        //private const UInt32 IDS_ERR_TMK_DATAMODEL = IDS_ERR_SECTION_TABLEMAKER + 0x8000;
        //private const UInt32 IDS_ERR_TMK_TABLEMODEL = IDS_ERR_TMK_DATAMODEL + 0x100;
        //private const UInt32 IDS_ERR_TMK_SOURCEDATA = IDS_ERR_TMK_DATAMODEL + 0x200;
        //private const UInt32 IDS_ERR_TMK_SOURCEHEADER = IDS_ERR_TMK_DATAMODEL + 0x300;
        //private const UInt32 IDS_ERR_TMK_OCVTABLE = IDS_ERR_TMK_DATAMODEL + 0x400;
        //private const UInt32 IDS_ERR_TMK_RCTABLE = IDS_ERR_TMK_DATAMODEL + 0x500;
        //private const UInt32 IDS_ERR_TMK_CHGTABLE = IDS_ERR_TMK_DATAMODEL + 0x600;
        //private const UInt32 IDS_ERR_TMK_ADRIVER = IDS_ERR_TMK_DATAMODEL + 0x700;
        //private const UInt32 IDS_ERR_TMK_TRTABLE = IDS_ERR_TMK_DATAMODEL + 0x800;
        //public const UInt32 IDS_ERR_TMK_TBL_FILE_FORMAT = IDS_ERR_TMK_TABLEMODEL + 0x01;
        //public const UInt32 IDS_ERR_TMK_TBL_FORMAT_MATCH = IDS_ERR_TMK_TABLEMODEL + 0x02;
        //public const UInt32 IDS_ERR_TMK_TBL_SOC_CREATE = IDS_ERR_TMK_TABLEMODEL + 0x03;
        //public const UInt32 IDS_ERR_TMK_TBL_CONFIG_NO_EXIT = IDS_ERR_TMK_TABLEMODEL + 0x04;
        //public const UInt32 IDS_ERR_TMK_TBL_BUILD_SEQUENCE = IDS_ERR_TMK_TABLEMODEL + 0x05;
        //public const UInt32 IDS_ERR_TMK_TBL_FILEPATH = IDS_ERR_TMK_TABLEMODEL + 0x06;
        //public const UInt32 IDS_ERR_TMK_SD_FILEPATH_NULL = IDS_ERR_TMK_SOURCEDATA + 0x01;
        //public const UInt32 IDS_ERR_TMK_SD_FILE_NOT_EXIST = IDS_ERR_TMK_SOURCEDATA + 0x02;
        //public const UInt32 IDS_ERR_TMK_SD_FILE_EXTENSION = IDS_ERR_TMK_SOURCEDATA + 0x03;
        //public const UInt32 IDS_ERR_TMK_SD_VOLTAGE_READ = IDS_ERR_TMK_SOURCEDATA + 0x04;
        //public const UInt32 IDS_ERR_TMK_SD_CURRENT_READ = IDS_ERR_TMK_SOURCEDATA + 0x05;
        //public const UInt32 IDS_ERR_TMK_SD_TEMPERATURE_READ = IDS_ERR_TMK_SOURCEDATA + 0x06;
        //public const UInt32 IDS_ERR_TMK_SD_ACCUMULATED_READ = IDS_ERR_TMK_SOURCEDATA + 0x07;
        //public const UInt32 IDS_ERR_TMK_SD_DATE_READ = IDS_ERR_TMK_SOURCEDATA + 0x08;
        //public const UInt32 IDS_ERR_TMK_SD_NUMBER_MATCH = IDS_ERR_TMK_SOURCEDATA + 0x09;
        //public const UInt32 IDS_ERR_TMK_SD_NUMBER_JUMP = IDS_ERR_TMK_SOURCEDATA + 0x0A;
        //public const UInt32 IDS_ERR_TMK_SD_NUMBER_BACK = IDS_ERR_TMK_SOURCEDATA + 0x0B;
        //public const UInt32 IDS_ERR_TMK_SD_VOLTAGE_JUMP = IDS_ERR_TMK_SOURCEDATA + 0x0C;
        //public const UInt32 IDS_ERR_TMK_SD_NOT_CONTINUE = IDS_ERR_TMK_SOURCEDATA + 0x0D;
        //public const UInt32 IDS_ERR_TMK_SD_NOT_REACH_EMPTY = IDS_ERR_TMK_SOURCEDATA + 0x0E;
        //public const UInt32 IDS_ERR_TMK_SD_FILE_OPEN_FAILE = IDS_ERR_TMK_SOURCEDATA + 0x0F;
        //public const UInt32 IDS_ERR_TMK_SD_CHARGE_NOT_FOUND = IDS_ERR_TMK_SOURCEDATA + 0x10;
        //public const UInt32 IDS_ERR_TMK_SD_IDLE_NOT_FOUND = IDS_ERR_TMK_SOURCEDATA + 0x11;
        //public const UInt32 IDS_ERR_TMK_SD_EXPERIMENT_NOT_FOUND = IDS_ERR_TMK_SOURCEDATA + 0x12;
        //public const UInt32 IDS_ERR_TMK_SD_EXPERIMENT_NOT_MATCH = IDS_ERR_TMK_SOURCEDATA + 0x13;
        //public const UInt32 IDS_ERR_TMK_SD_EXPERIMENT_ZERO = IDS_ERR_TMK_SOURCEDATA + 0x14;
        //public const UInt32 IDS_ERR_TMK_SD_EXPERIMENT_ZERO_CURRENT = IDS_ERR_TMK_SOURCEDATA + 0x15;
        //public const UInt32 IDS_ERR_TMK_SD_SERIAL_SAME = IDS_ERR_TMK_SOURCEDATA + 0x16;
        //public const UInt32 IDS_ERR_TMK_SD_EQUIPEMNT = IDS_ERR_TMK_SOURCEDATA + 0x17;
        //public const UInt32 IDS_ERR_TMK_SD_VOLTAGE_SEVERE = IDS_ERR_TMK_SOURCEDATA + 0x18;
        //public const UInt32 IDS_ERR_TMK_SD_EMPTY_FOLDER = IDS_ERR_TMK_SOURCEDATA + 0x19;
        //public const UInt32 IDS_ERR_TMK_HD_COLUMN = IDS_ERR_TMK_SOURCEHEADER + 0x01;
        //public const UInt32 IDS_ERR_TMK_HD_TYPE = IDS_ERR_TMK_SOURCEHEADER + 0x02;
        //public const UInt32 IDS_ERR_TMK_HD_WRITE_FAILED = IDS_ERR_TMK_SOURCEHEADER + 0x03;
        //public const UInt32 IDS_ERR_TMK_HD_ABSMAX_CAPACITY = IDS_ERR_TMK_SOURCEHEADER + 0x04;
        //public const UInt32 IDS_ERR_TMK_HD_CHARGE_VOLTAGE = IDS_ERR_TMK_SOURCEHEADER + 0x05;
        //public const UInt32 IDS_ERR_TMK_HD_CUTOFF_VOLTAGE = IDS_ERR_TMK_SOURCEHEADER + 0x06;
        //public const UInt32 IDS_ERR_TMK_OCV_CREATE_FILE = IDS_ERR_TMK_OCVTABLE + 0x01;
        //public const UInt32 IDS_ERR_TMK_OCV_SOURCE_EMPTY = IDS_ERR_TMK_OCVTABLE + 0x02;
        //public const UInt32 IDS_ERR_TMK_OCV_SOURCE_MANY = IDS_ERR_TMK_OCVTABLE + 0x03;
        //public const UInt32 IDS_ERR_TMK_OCV_VOLTAGE_MANY = IDS_ERR_TMK_OCVTABLE + 0x04;
        //public const UInt32 IDS_ERR_TMK_OCV_TSOC_POINT = IDS_ERR_TMK_OCVTABLE + 0x05;
        //public const UInt32 IDS_ERR_TMK_OCV_SOC_POINT = IDS_ERR_TMK_OCVTABLE + 0x06;
        //public const UInt32 IDS_ERR_TMK_OCVNEW_SOC_OVER5 = IDS_ERR_TMK_OCVTABLE + 0x07;
        //public const UInt32 IDS_ERR_TMK_OCVNEW_OCV_POINTS = IDS_ERR_TMK_OCVTABLE + 0x08;
        //public const UInt32 IDS_ERR_TMK_OCVNEW_SOC_POINTS = IDS_ERR_TMK_OCVTABLE + 0x09;
        //public const UInt32 IDS_ERR_TMK_RC_CREATE_FILE = IDS_ERR_TMK_RCTABLE + 0x01;
        //public const UInt32 IDS_ERR_TMK_RC_SOURCE_LESS = IDS_ERR_TMK_RCTABLE + 0x02;
        //public const UInt32 IDS_ERR_TMK_RC_VOLTAGE_LESS = IDS_ERR_TMK_RCTABLE + 0x03;
        //public const UInt32 IDS_ERR_TMK_RC_DCAP_NOT_MATCH = IDS_ERR_TMK_RCTABLE + 0x04;
        //public const UInt32 IDS_ERR_TMK_RC_CAPDIFF_NOT_MATCH = IDS_ERR_TMK_RCTABLE + 0x05;
        //public const UInt32 IDS_ERR_TMK_RC_MANUFA_NOT_MATCH = IDS_ERR_TMK_RCTABLE + 0x06;
        //public const UInt32 IDS_ERR_TMK_RC_BAT_MODEL_NOT_MATCH = IDS_ERR_TMK_RCTABLE + 0x07;
        //public const UInt32 IDS_ERR_TMK_RC_EXP_CURRENT_LESS = IDS_ERR_TMK_RCTABLE + 0x08;
        //public const UInt32 IDS_ERR_TMK_RC_EXP_TEMPERATURE_LESS = IDS_ERR_TMK_RCTABLE + 0x09;
        //public const UInt32 IDS_ERR_TMK_RC_EXP_SOURCE_NUM_NOT_MATCH = IDS_ERR_TMK_RCTABLE + 0x0A;
        //public const UInt32 IDS_ERR_TMK_RC_Y_POINTS_NOT_MATCH = IDS_ERR_TMK_RCTABLE + 0x0B;
        //public const UInt32 IDS_ERR_TMK_RC_INITIALZIED_FAILED = IDS_ERR_TMK_RCTABLE + 0x0C;
        //public const UInt32 IDS_ERR_TMK_RC_SOC_POINTS_ERROR = IDS_ERR_TMK_RCTABLE + 0x0D;
        //public const UInt32 IDS_ERR_TMK_RC_LAST_ONE_YPOINT = IDS_ERR_TMK_RCTABLE + 0x0E;
        //public const UInt32 IDS_ERR_TMK_CHG_DISCHARGE_DETECT = IDS_ERR_TMK_CHGTABLE + 0x01;
        //public const UInt32 IDS_ERR_TMK_CHG_INPUT_CURRENT_LESS = IDS_ERR_TMK_CHGTABLE + 0x02;
        //public const UInt32 IDS_ERR_TMK_CHG_INPUT_CURRENT_SMALL = IDS_ERR_TMK_CHGTABLE + 0x03;
        //public const UInt32 IDS_ERR_TMK_CHG_INPUT_CURRENT_BIG = IDS_ERR_TMK_CHGTABLE + 0x04;
        //public const UInt32 IDS_ERR_TMK_CHG_INPUT_CURRENT_NOTFOUND = IDS_ERR_TMK_CHGTABLE + 0x05;
        //public const UInt32 IDS_ERR_TMK_CHG_CREATE_FILE = IDS_ERR_TMK_CHGTABLE + 0x06;
        //public const UInt32 IDS_ERR_TMK_DRV_TYPE_NOT_SUPPORT = IDS_ERR_TMK_ADRIVER + 0x01;
        //public const UInt32 IDS_ERR_TMK_DRV_HEADER_NOT_FOUND = IDS_ERR_TMK_ADRIVER + 0x02;
        //public const UInt32 IDS_ERR_TMK_DRV_TEMP_FILE_STRING = IDS_ERR_TMK_ADRIVER + 0x03;
        //public const UInt32 IDS_ERR_TMK_DRV_TEMP_FILE_CREATE = IDS_ERR_TMK_ADRIVER + 0x04;
        //public const UInt32 IDS_ERR_TMK_DRV_H_FILE_CREATE = IDS_ERR_TMK_ADRIVER + 0x05;
        //public const UInt32 IDS_ERR_TMK_DRV_C_FILE_CREATE = IDS_ERR_TMK_ADRIVER + 0x06;
        //public const UInt32 IDS_ERR_TMK_DRV_FILE_READ = IDS_ERR_TMK_ADRIVER + 0x07;
        //public const UInt32 IDS_ERR_TMK_DRV_FILES_CRATE = IDS_ERR_TMK_ADRIVER + 0xF0;   //below are OK message, not error message
        //public const UInt32 IDS_ERR_TMK_DRV_TABLE_CRATE = IDS_ERR_TMK_ADRIVER + 0xF1;
        //public const UInt32 IDS_ERR_TMK_DRV_SHORT_TABLES = IDS_ERR_TMK_ADRIVER + 0xF2;
        //public const UInt32 IDS_ERR_TMK_TR_CREATE_FILE = IDS_ERR_TMK_TRTABLE + 01;
        //public const UInt32 IDS_ERR_TMK_TR_READ_OCV_FILE = IDS_ERR_TMK_TRTABLE + 10;
        //public const UInt32 IDS_ERR_TMK_TR_OCV_VOLT_DATA = IDS_ERR_TMK_TRTABLE + 11;
        //public const UInt32 IDS_ERR_TMK_TR_OCV_PERCENT_DATA = IDS_ERR_TMK_TRTABLE + 12;
        //public const UInt32 IDS_ERR_TMK_TR_OCV_POINTS_NOMATCH = IDS_ERR_TMK_TRTABLE + 13;
        //public const UInt32 IDS_ERR_TMK_TR_OCV_HEADER = IDS_ERR_TMK_TRTABLE + 14;
        //public const UInt32 IDS_ERR_TMK_TR_OCV_VOLT_OUTBOUND = IDS_ERR_TMK_TRTABLE + 15;
        //public const UInt32 IDS_ERR_TMK_TR_OCV_PERCENT_OUTBOUND = IDS_ERR_TMK_TRTABLE + 16;
        //public const UInt32 IDS_ERR_TMK_TR_READ_RC_FILE = IDS_ERR_TMK_TRTABLE + 20;
        //public const UInt32 IDS_ERR_TMK_TR_RC_VOLT_DATA = IDS_ERR_TMK_TRTABLE + 21;
        //public const UInt32 IDS_ERR_TMK_TR_RC_CURR_DATA = IDS_ERR_TMK_TRTABLE + 22;
        //public const UInt32 IDS_ERR_TMK_TR_RC_TEMP_DATA = IDS_ERR_TMK_TRTABLE + 23;
        //public const UInt32 IDS_ERR_TMK_TR_RC_RC_DATA = IDS_ERR_TMK_TRTABLE + 24;
        //public const UInt32 IDS_ERR_TMK_TR_RC_POINTS_NOMATCH = IDS_ERR_TMK_TRTABLE + 25;
        //public const UInt32 IDS_ERR_TMK_TR_RC_HEADER = IDS_ERR_TMK_TRTABLE + 26;
        //public const UInt32 IDS_ERR_TMK_TR_RC_FULL_CAPACITY = IDS_ERR_TMK_TRTABLE + 27;
        //public const UInt32 IDS_ERR_TMK_TR_READ_H_FILE = IDS_ERR_TMK_TRTABLE + 30;
        //public const UInt32 IDS_ERR_TMK_TR_READ_C_FILE = IDS_ERR_TMK_TRTABLE + 31;
        //public const UInt32 IDS_ERR_TMK_TR_EMPTY_CH_FILE = IDS_ERR_TMK_TRTABLE + 32;
        //public const UInt32 IDS_ERR_TMK_TR_DRV_TMP_FILE = IDS_ERR_TMK_TRTABLE + 33;
        //#endregion

        //#region Lotus code definition, IDS_ERR_SECTION_LOTUSSFL = 0x10120000
        //public const UInt32 IDS_ERR_SECTION_LOTUSSFL = 0x10120000;
        //public const UInt32 IDS_ERR_SECTION_LOTUSREGISTER = 0x10120001;
        //public const UInt32 IDS_ERR_SECTION_LOTUSTESTMODE = 0x10120002;
        //public const UInt32 IDS_ERR_SECTION_LOTUSNORMALMODE = 0x10120003;
        ////private const UInt32 IDS_ERR_TMK_DATAMODEL = IDS_ERR_SECTION_LOTUSSFL + 0x8000;
        ////private const UInt32 IDS_ERR_TMK_TABLEMODEL = IDS_ERR_SECTION_LOTUSSFL + 0x100;
        ////private const UInt32 IDS_ERR_TMK_SOURCEDATA = IDS_ERR_SECTION_LOTUSSFL + 0x200;
        //#endregion

        //#region MerlionPD flash access, IDS_ERR_SECTION_MERLIONPD_FLASH = 0x10130000
        //public const UInt32 IDS_ERR_ERASE_MAIN_FLASH_TIMEOUT = 0x10130100;
        //public const UInt32 IDS_ERR_ERASE_INFO_FLASH_TIMEOUT = 0x10130200;
        //public const UInt32 IDS_ERR_WRITE_MAIN_FLASH_CHECKSUM = 0x10130300;
        //public const UInt32 IDS_ERR_READ_MAIN_FLASH_CHECKSUM = 0x10130400;
        //public const UInt32 IDS_ERR_WRITE_INFO_FLASH_CHECKSUM = 0x10130500;
        //public const UInt32 IDS_ERR_READ_INFO_FLASH_CHECKSUM = 0x10130600;
        //public const UInt32 IDS_ERR_INVALID_INFO_FLASH_DATA = 0x10130700;
        //#endregion

        //#region Simulation code definition, IDS_ERR_SECTION_SIMULATION = 0x10140000
        //private const UInt32 IDS_ERR_SECTION_SIMULATION = 0x10140000;
        //public const UInt32 IDS_ERR_SECTION_SIMULATION_START = IDS_ERR_SECTION_SIMULATION + 0x01;
        //public const UInt32 IDS_ERR_SECTION_SIMULATION_COMPLETE = IDS_ERR_SECTION_SIMULATION + 0x02;
        //public const UInt32 IDS_ERR_SECTION_SIMULATION_FILE_LOST = IDS_ERR_SECTION_SIMULATION + 0x03;
        //public const UInt32 IDS_ERR_SECTION_SIMULATION_FILE_OPENED = IDS_ERR_SECTION_SIMULATION + 0x04;
        //public const UInt32 IDS_ERR_ATM_LOST_PARAMETER = IDS_ERR_SECTION_SIMULATION + 0x10;
        //#endregion

        //#region Simulation code definition, IDS_ERR_SECTION_TRIMSFL = 0x10150000
        //public const UInt32 IDS_ERR_SECTION_TRIMSFL = 0x10150000;
        //public const UInt32 IDS_ERR_SECTION_TRIMSFL_WORKSHEET_NOT_EXIT = IDS_ERR_SECTION_TRIMSFL + 0x01;
        //public const UInt32 IDS_ERR_SECTION_TRIMSFL_WORKSHEET_CELL_INVALID = IDS_ERR_SECTION_TRIMSFL + 0x02;
        //#endregion
        #endregion

        #region Public Description of Error code
        /// <summary>
        /// Parse Error code to error description
        /// </summary>
        /// <param name="dwErr">Error code encoding</param>
        /// <param name="strErr">Reference type, error description</param>
        public static string GetErrorDescription(UInt32 dwErr, bool bSpecific = true)
        {
            string strErr = String.Empty;

            if (m_dynamicErrorLib_dic.ContainsKey(dwErr))
                return m_dynamicErrorLib_dic[dwErr];
            switch (dwErr)
            {
                case IDS_ERR_SUCCESSFUL:
                    strErr = "Access successfully";
                    break;

                #region Communication Layer error description
                //Are generated by CommunicateManager.cs
                //case IDS_ERR_MGR_UNABLE_LOAD_DRIVER:
                //    strErr = "Unable to load driver of O2 USB adapter.";
                //    break;
                //case IDS_ERR_MGR_UNABLE_LOAD_FUNCTION:
                //    strErr = "Unable to load function from driver, driver may be broken.";
                //    break;
                //case IDS_ERR_MGR_INVALID_INTERFACE_TYPE:
                //    strErr = "Invalid interface type. The type may not be supported currently.";
                //    break;
                //case IDS_ERR_MGR_INVALID_INTERFACE_CONFIG:
                //    strErr = "Invalid configure value of interface.";
                //    break;
                //case IDS_ERR_MGR_INVALID_INTERFACE_HANDLER:
                //    strErr = "Invalid interface handler.";
                //    break;
                //case IDS_ERR_MGR_INVALID_INPUT_BUFFER:
                //    strErr = "Inpute buffer is invalid, please make sure the length of input buffer.";
                //    break;
                //case IDS_ERR_MGR_INVALID_OUTPUT_BUFFER:
                //    strErr = "Output buffere is invalid, please make sure the length of output buffer.";
                //    break;
                //case IDS_ERR_MGR_CONFIG_SVID_BUSTYPE:
                //    strErr = "Wrong BusType, you are trying to call SVID setting function with non-SVID BusType in BusOption.";
                //    break;
                //case IDS_ERR_MGR_CONFIG_BUFFER_NOT_ENOUGH:
                //    strErr = "Buffer is not enough to save SVID config.";	//should not happened
                //    break;
                //case IDS_ERR_MGR_CONFIG_SVID_NOT_SUPPORT:
                //    strErr = "Wrong BusType, this function is used only when BusType is equal to BUS_TYPE_SVID.";
                //    break;
                //case IDS_ERR_MGR_NULL_PORT_NODE:
                //    strErr = "Error in OCE!! Null node of connection Port in xml, please contact O2 for complete OCE package";
                //    break;

                ////Are generated by I2C communication protocol
                //case IDS_ERR_I2C_BB_TIMEOUT:
                //    strErr = "Byte function timeout, cannot read/write correct byte data.";
                //    break;
                //case IDS_ERR_I2C_PIN_TIMEOUT:
                //    strErr = "Word function timeout, cannot read/write correct word data.";
                //    break;
                //case IDS_ERR_I2C_PROTOCOL_TIMEOUT:
                //    strErr = "Block function timeout, cannot read/write correct block data.";
                //    break;
                //case IDS_ERR_I2C_EPP_TIMEOUT:
                //    strErr = "Driver command operation time out, cannot send command through driver function.";
                //    break;
                //case IDS_ERR_I2C_DRIVER_TIMEOUT:
                //    strErr = "Driver timeout.";
                //    break;
                //case IDS_ERR_I2C_NMS_TIMEOUT:
                //    strErr = "NMS timeout.";
                //    break;
                //case IDS_ERR_I2C_CMD_DISMATCH:
                //    strErr = "I2C adaptor command is not match, please check adaptor version or re-power it.";
                //    break;
                //case IDS_ERR_I2C_INVALID_HANDLE:
                //    strErr = "Invalid I2C handler of driver.";
                //    break;
                //case IDS_ERR_I2C_INVALID_PARAMETER:
                //    strErr = "Invalid I2C parameter, parameter is not supported in I2C protocol";
                //    break;
                //case IDS_ERR_I2C_INVALID_LENGTH:
                //    strErr = "Invalid length in I2C protocol, buffer length is too short.";
                //    break;
                //case IDS_ERR_I2C_INVALID_COMMAND:
                //    strErr = "Invalid I2C command, command is not supported in I2C protocol.";
                //    break;
                //case IDS_ERR_I2C_INVALID_BLOCK_SIZE:
                //    strErr = "Block size is too short for I2C data.";
                //    break;
                //case IDS_ERR_I2C_INVALID_BUFFER:
                //    strErr = "Invalid buffer, buffer is not valid for saving I2C data.";
                //    break;
                //case IDS_ERR_I2C_INVALID_INDEX:
                //    strErr = "Wrong index value in I2C protocol.";
                //    break;
                //case IDS_ERR_I2C_INVALID_HARDWARE:
                //    strErr = "Return length has error, target device may be in error or doesn't exist.";
                //    break;
                //case IDS_ERR_I2C_BUS_ERROR:
                //    strErr = "I2C bus error, communication is failed";
                //    break;
                //case IDS_ERR_I2C_SLA_ACK:
                //    strErr = "I2C Slave device is ACK.";
                //    break;
                //case IDS_ERR_I2C_SLA_NACK:
                //    strErr = "I2C Slave device is NACK.";
                //    break;
                //case IDS_ERR_I2C_LOST_ARBITRATION:
                //    strErr = "I2C communication arbitration is lost, data may be broken.";
                //    break;
                //case IDS_ERR_I2C_CFG_INVALID_CONFIG_TYPE:
                //    strErr = "Invalid configuration options in I2C protocol.";
                //    break;
                //case IDS_ERR_I2C_CFG_FREQUENCY_LIMIT:
                //    strErr = "I2C frequency is out of range, I2C frequency value should be between 60K~400K";
                //    break;
                //case IDS_ERR_I2C_CFG_FREQUENCY_ERROR:
                //    strErr = "Write I2C frequency error, adaptor may have something wrong.";
                //    break;

                ////Are generated by SPI communication protocol
                //case IDS_ERR_SPI_BB_TIMEOUT:
                //    strErr = "Byte function timeout, cannot read/write correct byte data.";
                //    break;
                //case IDS_ERR_SPI_PIN_TIMEOUT:
                //    strErr = "Word function timeout, cannot read/wrtie correct word data.";
                //    break;
                //case IDS_ERR_SPI_PROTOCOL_TIMEOUT:
                //    strErr = "Block function timeout, cannot read/write correct block data.";
                //    break;
                //case IDS_ERR_SPI_EPP_TIMEOUT:
                //    strErr = "Driver command operation time out, cannot send command through driver function.";
                //    break;
                //case IDS_ERR_SPI_DRIVER_TIMEOUT:
                //    strErr = "Driver timeout.";
                //    break;
                //case IDS_ERR_SPI_NMS_TIMEOUT:
                //    strErr = "NMS timeout.";
                //    break;
                //case IDS_ERR_SPI_INVALID_HANDLE:
                //    strErr = "Invalid SPI handler of driver.";
                //    break;
                //case IDS_ERR_SPI_INVALID_PARAMETER:
                //    strErr = "Invalid SPI parametre, parameter is not supported in SPI protocol.";
                //    break;
                //case IDS_ERR_SPI_INVALID_LENGTH:
                //    strErr = "Invalid  length in SPI protocol, buffer length is too short.";
                //    break;
                //case IDS_ERR_SPI_INVALID_COMMAND:
                //    strErr = "Invalid SPI command, command is not supported in SPI protocol.";
                //    break;
                //case IDS_ERR_SPI_INVALID_BLOCK_SIZE:
                //    strErr = "Block size is too short for SPI data.";
                //    break;
                //case IDS_ERR_SPI_INVALID_BUFFER:
                //    strErr = "Invalid buffer, buffer is not valid for saving SPI data.";
                //    break;
                //case IDS_ERR_SPI_INVALID_INDEX:
                //    strErr = "Wrong index value in SPI protocol.";
                //    break;
                //case IDS_ERR_SPI_INVALID_HARDWARE:
                //    strErr = "Return lengh has error, target device may be in error or doesn't exist.";
                //    break;
                //case IDS_ERR_SPI_CFG_INVALID_CONFIG_TYPE:
                //    strErr = "Invalid configuration options in SPI protocol.";
                //    break;
                //case IDS_ERR_SPI_CFG_CONFIG_ERROR:
                //    strErr = "Error on writing SPI configuration to adapter.";
                //    break;
                //case IDS_ERR_SPI_CFG_CONFIG_VALUE_ERROR:
                //    strErr = "SPI configuration value has error, adapter cannot support such configure value.";
                //    break;
                //case IDS_ERR_SPI_CFG_BAURATE_ERROR:
                //    strErr = "Error on writing SPI baud rate to adapter.";
                //    break;
                //case IDS_ERR_SPI_CFG_BAURATE_LIMIT:
                //    strErr = "SPI baud rate value is out of range.";
                //    break;
                //case IDS_ERR_SPI_CRC_CHECK:
                //    strErr = "SPI bus crc check error.";
                //    break;
                //case IDS_ERR_SPI_DATA_MISMATCH:
                //    strErr = "SPI write into and read back data section mismatch.";
                //    break;
                //case IDS_ERR_SPI_CMD_MISMATCH:
                //    strErr = "SPI write into and read back command section mismatch.";
                //    break;

                ////Are generated by SVID communication protocol
                //case IDS_ERR_SVID_INVALID_PARAMETER:
                //    strErr = "Invalid SVID parameter, parameter is not supported in I2C protocol";
                //    break;
                //case IDS_ERR_SVID_OPEN_FAILED:
                //    strErr = "SVID COM port opened failed, please check COM port setting and make sure it's not under using.";
                //    break;
                //case IDS_ERR_SVID_READ_BUFFER_NOT_ENOUGH:
                //    strErr = "yDataOut buffer is not enough to save data, please make sure buffer size is big enough.";
                //    break;
                //case IDS_ERR_SVID_IN_PARAMETER_INVALID:
                //    strErr = "yDataIn is less than 2; at leas it should have address+index in first parameter.";
                //    break;
                //case IDS_ERR_SVID_INDEX_OUT:
                //    strErr = "Index is out of array.";
                //    break;
                //case IDS_ERR_SVID_NULL_COM_HANDLER:
                //    strErr = "COM port handler is null, please go to Bus Options Windows to find your COM port.";
                //    break;
                //case IDS_ERR_SVID_COM_NOT_EXIST:
                //    strErr = "COM port is not existing in computer, plesase connect COM device and reopen it through Bus Options again.";
                //    break;
                //case IDS_ERR_SVID_INVALID_READI2CSINGLE:
                //    strErr = "I2C ReadSingle() protocol is not supported in SVID BusType.";
                //    break;
                //case IDS_ERR_SVID_INVALID_READI2CBLOCK:
                //    strErr = "I2C ReadBlock() protocol is not supported in SVID BusType.";
                //    break;
                //case IDS_ERR_SVID_INVALID_READVRSINGLE:
                //    strErr = "VR ReadSingle() protocol is not supported in SVID BustType.";
                //    break;
                //case IDS_ERR_SVID_INVALID_READVRWORD:
                //    strErr = "VR ReadWord() protocol is not supported in SVID BustType.";
                //    break;
                //case IDS_ERR_SVID_INVALID_READVRBLOCK:
                //    strErr = "VR ReadBlock() protocol is not supported in SVID BustType.";
                //    break;
                //case IDS_ERR_SVID_INVALID_WRITEI2CSINGLE:
                //    strErr = "I2C WriteSingle() protocol is not supported in SVID BusType.";
                //    break;
                //case IDS_ERR_SVID_INVALID_WRITEI2CWORD:
                //    strErr = "I2C WriteWord() protocol is not supported in SVID BusType.";
                //    break;
                //case IDS_ERR_SVID_INVALID_WRITEI2CBLOCK:
                //    strErr = "I2C WriteBlock() protocol is not supported in SVID BusType.";
                //    break;
                //case IDS_ERR_SVID_INVALID_WRITEVRSINGLE:
                //    strErr = "VR WriteSingle() protocol is not supported in SVID BusType.";
                //    break;
                //case IDS_ERR_SVID_INVALID_WRITEVRBLOCK:
                //    strErr = "VR WriteBlock() protocol is only supported to 3 bytes in SVID BusType.";
                //    break;
                //case IDS_ERR_SVID_INVALID_ENUMMETHOD:
                //    strErr = "Invalid Enum Value in AccessMethod.";     //should not happen
                //    break;
                //case IDS_ERR_SVID_COM_READ_ZERO:
                //    strErr = "Read zero data from COM port, make sure COM port device is connected.";
                //    break;
                //case IDS_ERR_SVID_COM_TIMEOUT:
                //    strErr = "COM port communication timeout, please make sure COM port is connected.";
                //    break;
                //case IDS_ERR_SVID_READ_NOT_ENOUGH:
                //    strErr = "Data read from COM port is not enough, please check your setting and command, make sure command is right.";
                //    break;
                //case IDS_ERR_SVID_READ_FAILED:
                //    strErr = "Data read from COM port is not enough, please check your setting and command, make sure command is right.";
                //    break;


                //case IDS_ERR_COM_COM_READ_ZERO:
                //    strErr = "Read zero data from COM port, make sure COM port device is connected.";
                //    break;
                //case IDS_ERR_COM_COM_TIMEOUT:
                //    strErr = "COM port communication timeout, please make sure COM port is connected.";
                //    break;
                //case IDS_ERR_COM_READ_NOT_ENOUGH:
                //    strErr = "Data read from COM port is not enough, please check your setting and command, make sure command is right.";
                //    break;
                //case IDS_ERR_COM_READ_FAILED:
                //    strErr = "Data read from COM port is not enough, please check your setting and command, make sure command is right.";
                //    break;
                #endregion

                #region DEM Error Description
                //case IDS_ERR_DEM_BIT_TIMEOUT:
                //    strErr = "Bit function timeout, cannot check bit flag free.";
                //    break;
                case IDS_ERR_DEM_FUN_TIMEOUT:
                    strErr = "Function timeout, cannot complete work in limited time.";
                    break;
                case IDS_ERR_DEM_BETWEEN_SELECT_BOARD:
                    strErr = "Chip version is mismatch between oce and board\n";
                    break;
                case IDS_ERR_BUS_DATA_PEC_ERROR:
                    strErr = "Bus Data PEC Verification Failed.";
                    break;
                //case IDS_ERR_PARAM_INVALID_HANDLER:
                //    strErr = "Parameter is invalid.";
                //    break;
                //case IDS_ERR_PARAM_HEX_DATA_OVERMAXRANGE:
                //    strErr = "Parameter hex data is out of the max value";
                //    break;
                //case IDS_ERR_PARAM_HEX_DATA_OVERMINRANGE:
                //    strErr = "Parameter hex data is out of the minimum value";
                //    break;
                //case IDS_ERR_PARAM_PHY_DATA_OVERMAXRANGE:
                //    strErr = "Parameter physical data is out of the max value.";
                //    break;
                //case IDS_ERR_PARAM_PHY_DATA_OVERMINRANGE:
                //    strErr = "Parameter physical data is out of the minimum value.";
                //    break;
                //case IDS_ERR_PARAM_DATA_ILLEGAL:
                //    strErr = "Parameter data is illegal,please check.";
                //    break;
                //case IDS_ERR_DEM_PARAMETERLIST_EMPTY:
                //    strErr = "ParameterList is empty.";
                //    break;
                //case IDS_ERR_DEM_USER_QUIT:
                //    strErr = "User select to quit the service!";
                //    break;
                //case IDS_ERR_DEM_PASSWORD_UNMATCH:
                //    strErr = "The password is incorrect!";
                //    break;
                //case IDS_ERR_DEM_PASSWORD_INVALID:
                //    strErr = "The password is invalid!";
                //    break;
                //case IDS_ERR_DEM_ADC_STOPPED:
                //    strErr = "ADC had been stopped!";
                //    break;
                //case IDS_ERR_DEM_PARAM_CONTAINER_SIZE:
                //    strErr = "The size of parameter's container error";
                //    break;
                //case IDS_ERR_DEM_ATE_CRC_ERROR:
                //    strErr = "ATE CRC Error!";
                //    break;
                case IDS_ERR_DEM_LOST_PARAMETER:
                    strErr = "Cannot find parameter information from xml";
                    break;
                case IDS_ERR_DEM_FROZEN:
                    strErr = "Chip is frozen!";
                    break;
                case IDS_ERR_DEM_DIRTYCHIP:
                    strErr = "Chip is dirty!";
                    break;
                case IDS_ERR_DEM_BUF_CHECK_FAIL:
                    strErr = "Read back check failed!";
                    break;
                case IDS_ERR_DEM_MAPPING_TIMEOUT:
                    strErr = "Mapping Timeout!";
                    break;
                //case IDS_ERR_DEM_LOST_INTERFACE:
                //    strErr = "DEM lost the interface,Please improve the DEM!";
                //    break;
                //case IDS_ERR_DEM_LOAD_BIN_FILE_ERROR:
                //    strErr = "Cannot load bin file!";
                //    break;
                //case IDS_ERR_DEM_BIN_LENGTH_ERROR:
                //    strErr = "Bin file length error!";
                //    break;
                //case IDS_ERR_DEM_BIN_ADDRESS_ERROR:
                //    strErr = "Bin file address error!";
                //    break;
                #endregion

                //#region MerlionPD Flash Error Description
                //case IDS_ERR_ERASE_MAIN_FLASH_TIMEOUT:
                //    strErr = "Erase Main Flash timeout";
                //    break;
                //case IDS_ERR_ERASE_INFO_FLASH_TIMEOUT:
                //    strErr = "Erase Information Flash timeout";
                //    break;
                //case IDS_ERR_WRITE_MAIN_FLASH_CHECKSUM:
                //    strErr = "Write main flash checksum error";
                //    break;
                //case IDS_ERR_READ_MAIN_FLASH_CHECKSUM:
                //    strErr = "Read main flash checksum error";
                //    break;
                //case IDS_ERR_WRITE_INFO_FLASH_CHECKSUM:
                //    strErr = "Write information flash checksum error";
                //    break;
                //case IDS_ERR_READ_INFO_FLASH_CHECKSUM:
                //    strErr = "Read information flash checksum error";
                //    break;
                //case IDS_ERR_INVALID_INFO_FLASH_DATA:
                //    strErr = "Invalid information flash data";
                //    break;
                //#endregion

                //#region EM Error Description
                //case IDS_ERR_EM_THREAD_BKWORKER_BUSY:
                //    strErr = "Communication Busy, Other SFLs is using bus adaptor to access device.";
                //    break;
                //#endregion

                //#region OCE Error Description  //ID:592
                //case IDS_ERR_SECTION_OCE_DIS_DEM:
                //    strErr = "DEM not found in OCE.";
                //    break;
                //case IDS_ERR_SECTION_OCE_LOSE_FILE:
                //    strErr = "File(s) missing in OCE.";
                //    break;
                //case IDS_ERR_SECTION_OCE_DIS_FILE_ATTRIBUTE:
                //    strErr = "File Attribute defective in OCE.";
                //    break;
                //case IDS_ERR_SECTION_OCE_UNZIP:
                //    strErr = "Failure when unzipping the OCE.";
                //    break;
                //case IDS_ERR_SECTION_OCE_NOT_EXIST:
                //    strErr = "OCE Not Found";
                //    break;
                //case IDS_ERR_SECTION_OCE_NOT_LOWER:
                //    strErr = "The file extension is not lower case.";
                //    break;
                //case IDS_ERR_SECTION_OCE_MISMATCH_20024:
                //    strErr = "The current OCE can only be used in COBRA v2.00.23 or before.";
                //    break;
                //#endregion

                //#region Device Configuration SFL error description
                //case IDS_ERR_SECTION_DEVICECONFSFL_PARAM_INVALID:
                //    strErr = "Some parameters' value are invalid, Please check!";
                //    break;
                //case IDS_ERR_SECTION_DEVICECONFSFL_PARAM_UNENALBE:
                //    strErr = "The parameter had been uneable and can't be changed!";
                //    break;
                //case IDS_ERR_SECTION_DEVICECONFSFL_PARAM_VERIFY:
                //    strErr = "Failed to verify the parameter written!";
                //    break;
                //#endregion

                //            #region Prodcution SFL error description
                //            case IDS_ERR_SECTION_PRODUCTIONSFL_POWERON_FAILED:
                //                strErr = "Turn on programming voltage failed!";
                //                break;
                //            case IDS_ERR_SECTION_PRODUCTIONSFL_POWEROFF_FAILED:
                //                strErr = "Turn off programming voltage failed!";
                //                break;
                //            case IDS_ERR_SECTION_PRODUCTIONSFL_POWERCHECK_FAILED:
                //                strErr = "Programming voltage check failed!";
                //                break;
                //            case IDS_ERR_SECTION_PRODUCTIONSFL_LOADBIN_FAILED:
                //                strErr = "Load bin file failed!";
                //                break; 
                //            #endregion

                //            #region Expert SFL error description
                //            case IDS_ERR_EXPSFL_XML:
                //                strErr = "Parsing information of xml has error, file maybe damage, please contact COBRA";
                //                break;
                //            case IDS_ERR_EXPSFL_DATABINDING:
                //                strErr = "Cannot find information of register, file maybe damage, please contact COBRA";
                //                break;
                //            #endregion

                //            #region SCS SFL error description
                //            case IDS_ERR_SCSSFL_INVALIDITEM:
                //                strErr = "Invalid! Please select a ADC channel first!";
                //                break;
                //            case IDS_ERR_SCSSFL_SCANDONE:
                //                strErr = "It's done!";
                //                break;
                //            #endregion

                //            #region SBS SFL error description
                //            case IDS_ERR_SBSSFL_GG_ACCESS:
                //                strErr = "Access gas gauge failed!";
                //                break;
                //            case IDS_ERR_SBSSFL_LOAD_FILE:
                //                strErr = "Load File failed!";
                //                break;
                //            case IDS_ERR_SBSSFL_SW_FRAME_HEAD:
                //                strErr = "Frame head error on SW side";
                //                break;
                //            case IDS_ERR_SBSSFL_SW_PEC_CHECK:
                //                strErr = "PEC check error on SW side";
                //                break;
                //            case IDS_ERR_SBSSFL_WRITE_REGISTER:
                //                strErr = "Cannot write register, please check communication is OK";
                //                break;
                //            case IDS_ERR_SBSSFL_READ_REGISTER:
                //                strErr = "Cannot read register, please check communication is OK";
                //                break;
                //            case IDS_ERR_SBSSFL_POLLING_REGISTER:
                //                strErr = "Cannot read physical value from chip, please check communication is OK";
                //                break;
                //            case IDS_ERR_SBSSFL_GGDRV_NOVOLTAGE:
                //                strErr = "Cannot find voltage parameter, please check xml file is correct";
                //                break;
                //            case IDS_ERR_SBSSFL_GGDRV_NOCURRENT:
                //                strErr = "Cannot find current parameter, please check xml file is correct.";
                //                break;
                //            case IDS_ERR_SBSSFL_GGDRV_NOTEMPERATURE:
                //                strErr = "Cannot find temperature parameter, please check xml file is correct.";
                //                break;
                //            case IDS_ERR_SBSSFL_GGDRV_NOCAR:
                //                strErr = "Cannot find CAR parameter, please check xml file is correct.";
                //                break;
                //            case IDS_ERR_SBSSFL_GGDRV_NOOCVOLTAGE:
                //                strErr = "Cannot find OCV parameter, please check xml file is correct.";
                //                break;
                //            case IDS_ERR_SBSSFL_GGDRV_NOPOOCV:
                //                strErr = "Cannot find PoOCV parameter, please check xml file is correct.";
                //                break;
                //            case IDS_ERR_SBSSFL_GGDRV_NOSLEEPOCV:
                //                strErr = "Cannot find SleepOCV parameter, please check xml file is correct.";
                //                break;
                //            case IDS_ERR_SBSSFL_GGDRV_NOCTRLSTATUS:
                //                strErr = "Cannot find Ctrl/Status parameter, please check xml file is correct.";
                //                break;
                //            case IDS_ERR_SBSSFL_FW_FRAME_HEAD:
                //                strErr = "Frame head error on FW side,Code:0xF0";
                //                break;
                //            case IDS_ERR_SBSSFL_FW_FRAME_CHECKSUM:
                //                strErr = "Frame Checksum error on FW side,Code:0xF1";
                //                break;
                //            case IDS_ERR_SBSSFL_FW_COMMAND_NODEF:
                //                strErr = "Command not definite on FW side,Code:0xF2";
                //                break;
                //            case IDS_ERR_SBSSFL_FW_COMMAND_EXECU:
                //                strErr = "Command executive error on FW side,Code:0xF3";
                //                break;
                //            case IDS_ERR_SBSSFL_FW_COMMAND_EXECU_TO:
                //                strErr = "Command executive Time out on FW side,Code:0xF4";
                //                break;
                //            case IDS_ERR_SBSSFL_FW_I2C_BLOCKED:
                //                strErr = "I2C is Blocked on FW side,Code:0xF5";
                //                break;
                //            case IDS_ERR_SBSSFL_FW_PEC_CHECK:
                //                strErr = "PEC check error on FW side,Code:0xF6";
                //                break;
                //            #endregion

                //            #region Eagle DLL error description
                //            case IDS_ERR_EGDLL_TABLE_NUMBER:
                //                {
                //                    strErr = "Number of table files is wrong, please check table selection";
                //                    break;
                //                }
                //            case IDS_ERR_EGDLL_PROJECT_FILE_NOEXIST:
                //                {
                //                    strErr = "Project setting file does not exist";
                //                    break;
                //                }
                //            case IDS_ERR_EGDLL_LIBARY_NOEXIST:
                //                {
                //                    strErr = "DLL libary file does not exist";
                //                    break;
                //                }
                //            case IDS_ERR_EGDLL_OCVBYTSOC_NOEXIST:
                //                {
                //                    strErr = "OCVbyTSOC  file does not exist";
                //                    break;
                //                }
                //            case IDS_ERR_EGDLL_TSOCBYOCV_NOEXIST:
                //                {
                //                    strErr = "TSOCbyOCV  file does not exist";
                //                    break;
                //                }
                //            case IDS_ERR_EGDLL_RC_NOEXIST:
                //                {
                //                    strErr = "RC table  file does not exist";
                //                    break;
                //                }
                //            case IDS_ERR_EGDLL_THERMAL_NOEXIST:
                //                {
                //                    strErr = "Thermal table  file does not exist";
                //                    break;
                //                }
                //            case IDS_ERR_EGDLL_SELFDSG_NOEXIST:
                //                {
                //                    strErr = "Self-discharge file does not exist";
                //                    break;
                //                }
                //            case IDS_ERR_EGDLL_RI_NOEXIST:
                //                {
                //                    strErr = "Resistor Internal file does not exist";
                //                    break;
                //                }
                //            case IDS_ERR_EGDLL_CHGTABLE_NOEXIST:
                //                {
                //                    strErr = "Charge table file does not exist";
                //                    break;
                //                }
                //            case IDS_ERR_EGDLL_PROJECT_SET:
                //                {
                //                    strErr = "Project setting file has error";
                //                    break;
                //                }
                //            case IDS_ERR_EGDLL_OCVBYTSOC_CONTENT:
                //                {
                //                    strErr = "OCVbyTSOC table content has wrong format";
                //                    break;
                //                }
                //            case IDS_ERR_EGDLL_TSOCBYOCV_CONTENT:
                //                {
                //                    strErr = "TSOCbyOCV table content has wrong format";
                //                    break;
                //                }
                //            case IDS_ERR_EGDLL_RCTABLE_CONTENT:
                //                {
                //                    strErr = "RC table content has wrong format";
                //                    break;
                //                }
                //            case IDS_ERR_EGDLL_THERMALTABLE_CONTENT:
                //                {
                //                    strErr = "Thermal table content has wrong format";
                //                    break;
                //                }
                //            case IDS_ERR_EGDLL_SELFDSGTABLE_CONTENT:
                //                {
                //                    strErr = "Self-Discharge table content has wrong format";
                //                    break;
                //                }
                //            case IDS_ERR_EGDLL_RITABLE_CONTENT:
                //                {
                //                    strErr = "RI table content has wrong format";
                //                    break;
                //                }
                //            case IDS_ERR_EGDLL_CHGTABLE_CONTENT:
                //                {
                //                    strErr = "Charge table content has wrong format";
                //                    break;
                //                }
                //            case IDS_ERR_EGDLL_PROJSET_VALUE:
                //                {
                //                    strErr = "Project setting table has wrong format";
                //                    break;
                //                }
                //            case IDS_ERR_EGDLL_BUS_BUSY:
                //                {
                //                    strErr = "I2C Bus is busy, other SFL is communicating with chip";
                //                    break;
                //                }
                //            case IDS_ERR_EGDLL_REGISTER:
                //                {
                //                    strErr = "Register information error.";
                //                    break;
                //                }
                //            case IDS_ERR_EGDLL_PARAMETERRW:
                //                {
                //                    strErr = "Cannot find related parameter in Read|Wrtie parameter list .";
                //                    break;
                //                }
                //            case IDS_ERR_EGDLL_GGREGISTERRW:
                //                {
                //                    strErr = "Gas Gauge register read failed.";
                //                    break;
                //                }
                //            case IDS_ERR_EGDLL_GGPARAMETER:
                //                {
                //                    strErr = "Important GasGauge parameter has zero value, please set it up through Configuration SFL.";
                //                    break;
                //                }

                //            #endregion

                //            #region TableMaker error description
                //            /* Note that: when displaying specific error description, here are common rules in TableMaker
                //// strVal01 must be saved FilePath string
                //// uVal01 must be saved SerialNumber
                //	// fVal01 must be saved Voltage value
                //// fVal02 must be saved Current value
                //*/
                //            case IDS_ERR_TMK_TBL_FILE_FORMAT:
                //                {
                //                    //brief message
                //                    strErr = string.Format("E{0}: 无法识别源文件格式", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", FilePath = {0}", strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_TBL_FORMAT_MATCH:
                //                {
                //                    //brief message
                //                    strErr = string.Format("E{0}: 源文件格式不统一", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format("\n Cannot add {0} ", strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_TBL_SOC_CREATE:
                //                {
                //                    strErr = string.Format("E{0}: 无法为SOC文件创建SOC目录", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(": ", strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_TBL_CONFIG_NO_EXIT:
                //                {
                //                    strErr = string.Format("E{0}: 缺少配置文件", dwErr.ToString("X"));
                //                    break;
                //                }
                //            case IDS_ERR_TMK_TBL_BUILD_SEQUENCE:
                //                {
                //                    strErr = string.Format("E{0}: TableMaker API usage error, please call BuildTable() first.", dwErr.ToString("X"));
                //                    break;
                //                }
                //            case IDS_ERR_TMK_TBL_FILEPATH:
                //                {
                //                    strErr = string.Format("E{0}: 打开源文件出错", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(": ", strVal01);
                //                    break;
                //                }

                //            //source data
                //            case IDS_ERR_TMK_SD_FILEPATH_NULL:
                //                {
                //                    strErr = string.Format("E{0}: 源文件路径无效", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", perhaps other source files have error and cannot consider a reasonable file, so that it is empty");
                //                    break;
                //                }
                //            case IDS_ERR_TMK_SD_FILE_NOT_EXIST:
                //                {
                //                    strErr = string.Format("E{0}: 源文件不存在", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", {0} is not existed in harddrive or cannot be opened.", strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_SD_FILE_EXTENSION:
                //                {
                //                    strErr = string.Format("E{0}: 设备类型不支持", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(".\nCurrently we only support Jinfan and AcuTech.");
                //                    break;
                //                }
                //            case IDS_ERR_TMK_SD_VOLTAGE_READ:
                //                {
                //                    strErr = string.Format("E{0}: 无法从文件中读取电压", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(".\n\"{0}\" string is not recognized in file, {1}", strVal02, strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_SD_CURRENT_READ:
                //                {
                //                    strErr = string.Format("E{0}: 无法从文件中读取电流", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(".\n\"{0}\" string is not recognized in file, {1}", strVal02, strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_SD_TEMPERATURE_READ:
                //                {
                //                    strErr = string.Format("E{0}: 无法从文件中读取温度", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(".\n\"{0}\" string is not recognized in file, {1}", strVal02, strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_SD_ACCUMULATED_READ:
                //                {
                //                    strErr = string.Format("E{0}: 无法从文件中读取电量累加值", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(".\n\"{0}\" string is not recognized in file, {1}", strVal02, strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_SD_DATE_READ:
                //                {
                //                    strErr = string.Format("E{0}: 无法从文件中读取日期记录", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(".\n\"{0}\" string is not recognized in file, {1}", strVal02, strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_SD_NUMBER_MATCH:
                //                {
                //                    strErr = string.Format("E{0}: 实验数据不完整", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(".\n In {0} source data, Column counter is {1} but SERIALNUMBER counter = {2}, data may be lost.", strVal01, uVal01.ToString(), uVal02.ToString());
                //                    break;
                //                }
                //            case IDS_ERR_TMK_SD_NUMBER_JUMP:
                //                {
                //                    strErr = string.Format("E{0}: 序列号不连续", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(" from {0} to {1} in file, .", uVal01.ToString(), uVal02.ToString(), strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_SD_NUMBER_BACK:
                //                {
                //                    strErr = string.Format("E{0}: 序列号不累加", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", last one is {0} and now is  {1}.", uVal01.ToString(), uVal02.ToString());
                //                    break;
                //                }
                //            case IDS_ERR_TMK_SD_VOLTAGE_JUMP:
                //                {
                //                    strErr = string.Format("E{0}: 电压跳跃超过 5mV", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", from {1}mV to {2}mV at SerialNumber {0}.", uVal01, fVal01, fVal02);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_SD_NOT_CONTINUE:
                //                {
                //                    strErr = string.Format("E{0}: Serial Number is not continuous", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", at SerialNumber {0}.", uVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_SD_NOT_REACH_EMPTY:
                //                {
                //                    strErr = string.Format("E{0}: 无法找到放电到空数据", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(",  in file {0}.", strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_SD_FILE_OPEN_FAILE:
                //                {
                //                    strErr = string.Format("E{0}: 无法打开文件", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", file path = {0}.", strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_SD_CHARGE_NOT_FOUND:       //basically no used
                //                {
                //                    strErr = string.Format("E{0}: 无法找到充电学习周期", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(",  in file {0}.", strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_SD_IDLE_NOT_FOUND:     //basically no used
                //                {
                //                    strErr = string.Format("E{0}: 无法找到空闲状态数据", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(",  in file {0}.", strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_SD_EXPERIMENT_NOT_FOUND:
                //                {
                //                    strErr = string.Format("E{0}: 无法找到放电数据", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(",  in file {0}.", strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_SD_EXPERIMENT_NOT_MATCH:   //basically no used
                //                {
                //                    strErr = string.Format("E{0}: 放电电流与头信息中的设置不一致", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(",  in file {0}.", strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_SD_EXPERIMENT_ZERO:        //basically no used
                //                {
                //                    strErr = string.Format("E{0}: Cannot find experiment data", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(",  in file {0}.", strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_SD_EXPERIMENT_ZERO_CURRENT:
                //                {
                //                    strErr = string.Format("E{0}: 检测到连续5个以上电流为0的实验数据", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(",  at Serial = {0} in file {1}.", uVal01, strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_SD_SERIAL_SAME:
                //                {
                //                    strErr = string.Format("E{0}: There are 2 experiment data with same serial number", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(" = {0} in file {1}.", uVal01, strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_SD_EQUIPEMNT:  //basically should not be used
                //                {
                //                    strErr = string.Format("E{0}: Cannot read equipment string correctly", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", please key in as JYxxx or JFxxx format in your header.");
                //                    break;
                //                }
                //            case IDS_ERR_TMK_SD_VOLTAGE_SEVERE:
                //                {
                //                    strErr = string.Format("E{0}: 电压跳变异常", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", at serialnumber {0} in file {1}.", uVal01, strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_SD_EMPTY_FOLDER:
                //                {
                //                    strErr = string.Format("E{0}: 目标目录不存在", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", make sure it is correct folder path {0}.", strVal01);
                //                    break;
                //                }

                //            //header
                //            case IDS_ERR_TMK_HD_COLUMN:
                //                {
                //                    strErr = string.Format("E{0}: 头信息行数错误", dwErr.ToString("X"));
                //                    break;
                //                }
                //            case IDS_ERR_TMK_HD_TYPE:
                //                {
                //                    strErr = string.Format("E{0}: Type无法识别", dwErr.ToString("X"));
                //                    break;
                //                }
                //            case IDS_ERR_TMK_HD_WRITE_FAILED:   //should not happed
                //                {
                //                    strErr = string.Format("E{0}: Create SoC file error.", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", file path = {0}.", strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_HD_ABSMAX_CAPACITY:
                //                {
                //                    strErr = string.Format("E{0}: 无法识别 Absolute Capacity", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", (AbsMaxCap={0}) in file {1}.", fVal01, strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_HD_CHARGE_VOLTAGE:
                //                {
                //                    strErr = string.Format("E{0}: 无法识别 Charge Voltage", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", (ChargeVoltage={0}) in file {1}.", fVal01, strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_HD_CUTOFF_VOLTAGE:
                //                {
                //                    strErr = string.Format("E{0}: 无法识别Cutoff Voltage", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(",  (CutoffVoltage={0}) in file {1}.", fVal01, strVal01);
                //                    break;
                //                }
                //            //OCV
                //            case IDS_ERR_TMK_OCV_CREATE_FILE:
                //                {
                //                    strErr = string.Format("E{0}: 无法生成OCV表", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(",  failed to create {0}", strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_OCV_SOURCE_EMPTY:
                //                {
                //                    strErr = string.Format("E{0}: 无法识别源文件", dwErr.ToString("X"));
                //                    break;
                //                }
                //            case IDS_ERR_TMK_OCV_SOURCE_MANY:
                //                {
                //                    strErr = string.Format("E{0}: 源文件多于2个", dwErr.ToString("X"));
                //                    break;
                //                }
                //            case IDS_ERR_TMK_OCV_VOLTAGE_MANY:      //now we don't need user to input voltage
                //                {
                //                    strErr = string.Format("E{0}: Input Voltage is more than 2 points.", dwErr.ToString("X"));
                //                    break;
                //                }
                //            case IDS_ERR_TMK_OCV_TSOC_POINT:
                //                {
                //                    strErr = string.Format("E{0}: TSOC points is not matched", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(",  {0} points are got, but matched with TSOC defined {1} points", uVal01.ToString(), uVal02.ToString());
                //                    break;
                //                }
                //            case IDS_ERR_TMK_OCV_SOC_POINT:
                //                {
                //                    strErr = string.Format("E{0}: SOC points is not matched", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(",  {0} points are got, but matched with SOC defined {1} points", uVal01.ToString(), uVal02.ToString());
                //                    break;
                //                }
                //            case IDS_ERR_TMK_OCVNEW_SOC_OVER5:
                //                {
                //                    strErr = string.Format("E{0}: 无法找到误差 5% 以内的SOC值", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(",  at SerialNumber {1}, in file {0}", strVal01, uVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_OCVNEW_OCV_POINTS:
                //                {
                //                    strErr = string.Format("E{0}: OCV points is not enough", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(",   we need {0} but only got {1} points from file.", uVal02, uVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_OCVNEW_SOC_POINTS: //not used
                //                {
                //                    strErr = string.Format("E{0}: SOC points is not enough", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(",   we need {0} but only got {1} points from file.", uVal02, uVal01);
                //                    break;
                //                }

                //            //rc table
                //            case IDS_ERR_TMK_RC_CREATE_FILE:
                //                {
                //                    strErr = string.Format("E{0}: 无法生成RC表", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", target file path is {0}", strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_RC_SOURCE_LESS:
                //                {
                //                    strErr = string.Format("E{0}: 源文件不足", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", you only select {0} files", uVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_RC_VOLTAGE_LESS:
                //                {
                //                    strErr = string.Format("E{0}: 电压点不足", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", you only input {0} voltage point", uVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_RC_DCAP_NOT_MATCH:
                //                {
                //                    strErr = string.Format("E{0}: Absolute Capacity 不一致", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", in file {0}.", strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_RC_CAPDIFF_NOT_MATCH:
                //                {
                //                    strErr = string.Format("E{0}: Capacity Difference 不一致", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", in file {0}.", strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_RC_MANUFA_NOT_MATCH:
                //                {
                //                    strErr = string.Format("E{0}: Manufacture不一致", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", in file {0}.", strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_RC_BAT_MODEL_NOT_MATCH:
                //                {
                //                    strErr = string.Format("E{0}: Battery Model 不一致", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", in file {0}.", strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_RC_EXP_CURRENT_LESS:
                //                {
                //                    strErr = string.Format("E{0}: 实验电流点不足", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", only {0} kind(s) of experiment current cannot make RC table", uVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_RC_EXP_TEMPERATURE_LESS:
                //                {
                //                    strErr = string.Format("E{0}: 实验温度点不足", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", only {0} kind(s) of experiment temperature cannot make RC table", uVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_RC_EXP_SOURCE_NUM_NOT_MATCH:
                //                {
                //                    strErr = string.Format("E{0}: 源文件不齐", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", only {0} experiment source data is selected, we detected {1} temperature and {2} current difference setting, it should have {3} files.", uVal01, uVal02, uVal03, (uVal02 * uVal03));
                //                    break;
                //                }
                //            case IDS_ERR_TMK_RC_Y_POINTS_NOT_MATCH:
                //                {
                //                    strErr = string.Format("E{0}: RC点与用户需求不匹配", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", only {0} SoC ponts are calculated but is not matched wiht user input voltage number, {1}", uVal01, uVal02);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_RC_INITIALZIED_FAILED:
                //                {
                //                    strErr = string.Format("E{0}: Initialized RC table failed, cannot make RC table", dwErr.ToString("X"));
                //                    break;
                //                }
                //            case IDS_ERR_TMK_RC_SOC_POINTS_ERROR:
                //                {
                //                    strErr = string.Format("E{0}: RC点的行数不匹配", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", only {0} line could be calculated. There are {1} temperature and {2} current and it should be having {3} lines created", uVal01, uVal02, uVal03, (uVal02 * uVal03));
                //                    break;
                //                }
                //            case IDS_ERR_TMK_RC_LAST_ONE_YPOINT:
                //                {
                //                    strErr = string.Format("E{0}: 输入电压与实验电压不匹配", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", last one input voltage is {0}, and last one experiment voltage {1} in file {2}", uVal01, fVal01, strVal01);
                //                    break;
                //                }
                //            //charge table
                //            case IDS_ERR_TMK_CHG_DISCHARGE_DETECT:
                //                {
                //                    strErr = string.Format("E{0}: 充电实验中检测到放电过程", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", current = {1} at serial number = {0}", uVal01, fVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_CHG_INPUT_CURRENT_LESS:
                //                {
                //                    strErr = string.Format("E{0}: 输入电流点数不足", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", you only input {0} points", uVal02, fVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_CHG_INPUT_CURRENT_SMALL:
                //                {
                //                    strErr = string.Format("E{0}: 输入电流小于实验充电电流", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", Input current= {0}mA, last one experiment charge current ={1}mA", uVal01, fVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_CHG_INPUT_CURRENT_BIG:
                //                {
                //                    strErr = string.Format("E{0}: 输入电流大于实验充电电流", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", Input current= {0}mA, last one experiment charge current ={1}mA", uVal01, fVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_CHG_INPUT_CURRENT_NOTFOUND:
                //                {
                //                    strErr = string.Format("E{0}: 无法从实验数据中找到输入电流", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", Input current= {0}mA, experiment is from {1}mA to {2}mA", uVal01, fVal01, fVal02);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_CHG_CREATE_FILE:
                //                {
                //                    strErr = string.Format("E{0}: 无法生成充电表", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", target full path is {0}", strVal01);
                //                    break;
                //                }
                //            //android driver
                //            case IDS_ERR_TMK_DRV_TYPE_NOT_SUPPORT:  //should not happen
                //                {
                //                    strErr = string.Format("E{0}: 无法识别源文件", dwErr.ToString("X"));
                //                    break;
                //                }
                //            case IDS_ERR_TMK_DRV_HEADER_NOT_FOUND:  //should not happen
                //                {
                //                    strErr = string.Format("E{0}: 无法找到源文件的头信息", dwErr.ToString("X"));
                //                    break;
                //                }
                //            case IDS_ERR_TMK_DRV_TEMP_FILE_STRING:
                //                {
                //                    strErr = string.Format("E{0}: 临时文件路径无效", dwErr.ToString("X"));
                //                    break;
                //                }
                //            case IDS_ERR_TMK_DRV_TEMP_FILE_CREATE:
                //                {
                //                    strErr = string.Format("E{0}: 无法生成临时文件", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", target full path is {0}", strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_DRV_H_FILE_CREATE:
                //                {
                //                    strErr = string.Format("E{0}: 无法生成.H文件", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", target full path is, {0}", strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_DRV_C_FILE_CREATE:
                //                {
                //                    strErr = string.Format("E{0}: 无法生成.C文件", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", target full path is, {0}", strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_DRV_FILES_CRATE:
                //                {
                //                    strErr = string.Format("Build Android Driver successfully");
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", please check folder, {0}, for your C & H file", strVal01);
                //                    break;
                //                }
                //            case IDS_ERR_TMK_DRV_TABLE_CRATE:
                //                {
                //                    strErr = string.Format("Build table text successfully");
                //                    break;
                //                }
                //            case IDS_ERR_TMK_DRV_SHORT_TABLES:
                //                {
                //                    strErr = string.Format("Build table text successfully, but we need other tables to generate Android Driver.\nMake sure Manufacture Factory, Battery Model, and Absolute Capacity are consistent. ");
                //                    break;
                //                }
                //            case IDS_ERR_TMK_DRV_FILE_READ: //should not happed
                //                {
                //                    strErr = string.Format("Ex{0}: Cannot read temporary file", dwErr.ToString("X"));
                //                    if (bSpecific)          //detail description
                //                        strErr += string.Format(", {0}", strVal01);
                //                    break;
                //                }

                //            case IDS_ERR_TMK_TR_CREATE_FILE:
                //                {
                //                    strErr = string.Format("E{0}: 无法生成TR表", dwErr.ToString("X"));
                //                    if (bSpecific)			//detail description
                //                        strErr += string.Format(",  failed to create {0}", strVal01);
                //                    break;
                //                }

                //            case IDS_ERR_TMK_TR_READ_OCV_FILE:
                //                {
                //                    strErr = string.Format("E{0}: 无法打开OCV表", dwErr.ToString("X"));
                //                    if (bSpecific)			//detail description
                //                        strErr += string.Format(",  failed to open {0}", strVal01);
                //                    break;
                //                }

                //            case IDS_ERR_TMK_TR_OCV_VOLT_DATA:
                //                {
                //                    strErr = string.Format("E{0}: OCV表內电压无法识别", dwErr.ToString("X"));
                //                    if (bSpecific)			//detail description
                //                        strErr += string.Format(",  cannot read voltage value in {0}", strVal01);
                //                    break;
                //                }

                //            case IDS_ERR_TMK_TR_OCV_PERCENT_DATA:
                //                {
                //                    strErr = string.Format("E{0}: OCV表內容量无法识别", dwErr.ToString("X"));
                //                    if (bSpecific)			//detail description
                //                        strErr += string.Format(",  cannot regnize value of percentage in {0}", strVal01);
                //                    break;
                //                }

                //            case IDS_ERR_TMK_TR_OCV_POINTS_NOMATCH:
                //                {
                //                    strErr = string.Format("E{0}: OCV表總點數不一致", dwErr.ToString("X"));
                //                    if (bSpecific)			//detail description
                //                        strErr += string.Format(",  points number is not match in {0}", strVal01);
                //                    break;
                //                }

                //            case IDS_ERR_TMK_TR_OCV_HEADER:
                //                {
                //                    strErr = string.Format("E{0}: 无法识别OCV表內註解", dwErr.ToString("X"));
                //                    if (bSpecific)			//detail description
                //                        strErr += string.Format(",  cannot read header information in {0}", strVal01);
                //                    break;
                //                }

                //            case IDS_ERR_TMK_TR_OCV_VOLT_OUTBOUND:
                //                {
                //                    strErr = string.Format("E{0}: OCV表內电压超出合理範圍", dwErr.ToString("X"));
                //                    if (bSpecific)			//detail description
                //                        strErr += string.Format(",  voltage value is out of range in {0}", strVal01);
                //                    break;
                //                }

                //            case IDS_ERR_TMK_TR_OCV_PERCENT_OUTBOUND:
                //                {
                //                    strErr = string.Format("E{0}: OCV表內容量超出合理範圍", dwErr.ToString("X"));
                //                    if (bSpecific)			//detail description
                //                        strErr += string.Format(",  percentage value is out of range in {0}", strVal01);
                //                    break;
                //                }

                //            case IDS_ERR_TMK_TR_READ_RC_FILE:
                //                {
                //                    strErr = string.Format("E{0}: 无法打开RC表", dwErr.ToString("X"));
                //                    if (bSpecific)			//detail description
                //                        strErr += string.Format(",  failed to open {0}", strVal01);
                //                    break;
                //                }

                //            case IDS_ERR_TMK_TR_RC_VOLT_DATA:
                //                {
                //                    strErr = string.Format("E{0}: RC表內电压无法识别", dwErr.ToString("X"));
                //                    if (bSpecific)			//detail description
                //                        strErr += string.Format(",  cannot read voltage value in {0}", strVal01);
                //                    break;
                //                }

                //            case IDS_ERR_TMK_TR_RC_CURR_DATA:
                //                {
                //                    strErr = string.Format("E{0}: RC表內电流无法识别", dwErr.ToString("X"));
                //                    if (bSpecific)			//detail description
                //                        strErr += string.Format(",  cannot read current value in {0}", strVal01);
                //                    break;
                //                }

                //            case IDS_ERR_TMK_TR_RC_TEMP_DATA:
                //                {
                //                    strErr = string.Format("E{0}: RC表內溫度无法识别", dwErr.ToString("X"));
                //                    if (bSpecific)			//detail description
                //                        strErr += string.Format(",  cannot read temperature value in {0}", strVal01);
                //                    break;
                //                }

                //            case IDS_ERR_TMK_TR_RC_RC_DATA:
                //                {
                //                    strErr = string.Format("E{0}: RC表內容量无法识别", dwErr.ToString("X"));
                //                    if (bSpecific)			//detail description
                //                        strErr += string.Format(",  cannot read remaining capacity value in {0}", strVal01);
                //                    break;
                //                }

                //            case IDS_ERR_TMK_TR_RC_POINTS_NOMATCH:
                //                {
                //                    strErr = string.Format("E{0}: RC表數值點數不匹配", dwErr.ToString("X"));
                //                    if (bSpecific)			//detail description
                //                        strErr += string.Format(",  points number of voltage, current, temperature, and remaining capacity is not match in {0}", strVal01);
                //                    break;
                //                }

                //            case IDS_ERR_TMK_TR_RC_HEADER:
                //                {
                //                    strErr = string.Format("E{0}: 无法识别RC表內註解", dwErr.ToString("X"));
                //                    if (bSpecific)			//detail description
                //                        strErr += string.Format(",  cannot read header information in {0}", strVal01);
                //                    break;
                //                }

                //            case IDS_ERR_TMK_TR_RC_FULL_CAPACITY:
                //                {
                //                    strErr = string.Format("E{0}: 无法识别RC表總容量", dwErr.ToString("X"));
                //                    if (bSpecific)			//detail description
                //                        strErr += string.Format(",  cannot read fully charged capacity in {0}", strVal01);
                //                    break;
                //                }

                //            case IDS_ERR_TMK_TR_READ_H_FILE:
                //                {
                //                    strErr = string.Format("E{0}: 无法打开.h文件", dwErr.ToString("X"));
                //                    if (bSpecific)			//detail description
                //                        strErr += string.Format(",  failed to open {0}", strVal01);
                //                    break;
                //                }

                //            case IDS_ERR_TMK_TR_READ_C_FILE:
                //                {
                //                    strErr = string.Format("E{0}: 无法打开.c文件", dwErr.ToString("X"));
                //                    if (bSpecific)			//detail description
                //                        strErr += string.Format(",  failed to open {0}", strVal01);
                //                    break;
                //                }

                //            case IDS_ERR_TMK_TR_EMPTY_CH_FILE:
                //                {
                //                    strErr = string.Format("E{0}: .c/.h文件名為空", dwErr.ToString("X"));
                //                    if (bSpecific)			//detail description
                //                        strErr += string.Format(",  failed to open {0}, \n {1}", strVal01, strVal02);
                //                    break;
                //                }

                //            case IDS_ERR_TMK_TR_DRV_TMP_FILE:
                //                {
                //                    strErr = string.Format("E{0}: 无法新增暫存文件", dwErr.ToString("X"));
                //                    if (bSpecific)			//detail description
                //                        strErr += string.Format(",  failed to create temporary file, {0}", strVal01);
                //                    break;
                //                }

                //            #endregion

                //            #region Simulation error description
                //            case IDS_ERR_SECTION_SIMULATION_START:
                //                {
                //                    strErr = "Warning:COBRA will enter into automation mode.";
                //                    break;
                //                }
                //            case IDS_ERR_SECTION_SIMULATION_COMPLETE:
                //                {
                //                    strErr = " Congratulations:Successful to complete the simulation.";
                //                    break;
                //                }
                //            case IDS_ERR_SECTION_SIMULATION_FILE_LOST:
                //                {
                //                    strErr = "The file had been lost,please check.";
                //                    break;
                //                }
                //            case IDS_ERR_SECTION_SIMULATION_FILE_OPENED:
                //                {
                //                    strErr = "The file had been open,please check.";
                //                    break;
                //                }
                //            case IDS_ERR_ATM_LOST_PARAMETER:
                //                {
                //                    strErr = string.Format("Cannot find parameter information from xml by Register Index = 0x{0:X2}", uVal01);
                //                    break;
                //                }
                //            case IDS_ERR_SECTION_TRIMSFL_WORKSHEET_NOT_EXIT:
                //                {
                //                    strErr = "The worksheet does not exit,please check!";
                //                    break;
                //                }
                //            case IDS_ERR_SECTION_TRIMSFL_WORKSHEET_CELL_INVALID:
                //                {
                //                    strErr = "The cell value is invalid,please check!";
                //                    break;
                //                }

                //            #endregion
                //            #region Folder Operation error description
                //            case IDS_ERR_SECTION_CANNOT_CREATE_FOLDER_COM:
                //                {
                //                    strErr = "Error: Cannot create folder, please make sure you have privilege or make sure COBRA running on correct Drive.";
                //                    break;
                //                }
                //            case IDS_ERR_SECTION_FOLDERS_LOST:
                //                {
                //                    strErr = "Error: Some important folder lost,please contact with developers.";
                //                    break;
                //                }
                //            case IDS_ERR_SECTION_CANNOT_ACCESS_COBRA_FOLDER:
                //                {
                //                    strErr = "Error: Cannot access COBRA folder, please make sure you have privilege or make sure COBRA running on correct Drive.";
                //                    break;
                //                }
                //            case IDS_ERR_SECTION_CANNOT_ACCESS_ExtRT_FOLDER:
                //                {
                //                    strErr = "Error: Cannot access Extension Runtime folder, please make sure you have privilege or make sure COBRA running on correct Drive.";
                //                    break;
                //                }
                //            case IDS_ERR_SECTION_CANNOT_ACCESS_ExtMT_FOLDER:
                //                {
                //                    strErr = "Error: Cannot access Extension Monitor folder, please make sure you have privilege or make sure COBRA running on correct Drive.";
                //                    break;
                //                }
                //            case IDS_ERR_SECTION_CANNOT_ACCESS_COBRA_DOC:
                //                {
                //                    strErr = "Error: Cannot access COBRA Document folder, please make sure you have privilege or make sure COBRA running on correct Drive.";
                //                    break;
                //                }
                //            case IDS_ERR_SECTION_CANNOT_ACCESS_LOG:
                //                {
                //                    strErr = "Error: Cannot access log folder, please make sure you have privilege or make sure COBRA running on correct Drive.";
                //                    break;
                //                }
                //            case IDS_ERR_SECTION_CANNOT_ACCESS_SET_FOLDER:
                //                {
                //                    strErr = "Error: Cannot access settings folder, please make sure you have privilege or make sure COBRA running on correct Drive.";
                //                    break;
                //                }
                //            #endregion

                //            #region Cobra Center error description
                //            case IDS_ERR_SECTION_CENTER_IP_NACK:
                //                strErr = "No response from website, please check network and IP address/port are correct.";
                //                break;
                //            case IDS_ERR_SECTION_CENTER_USERNAME:
                //                strErr = "User name is wrong, please check email correct.";
                //                break;
                //            case IDS_ERR_SECTION_CENTER_PASSWORD:
                //                strErr = "Password incorrect, please check password.";
                //                break;
                //            case IDS_ERR_SECTION_CENTER_UNAUTHORIZED:
                //                strErr = "Unauthorized client access, please contact O2Micor for further help.";
                //                break;
                //            case IDS_ERR_SECTION_CENTER_NOFOUND:
                //                strErr = "Not found of items.";
                //                break;
                //            case IDS_ERR_SECTION_CENTER_DOWNLOAD_BUSY:
                //                strErr = "File is downloading,please wait!";
                //                break;
                //            case IDS_ERR_SECTION_CENTER_OCE_VERSION_LOW:
                //                strErr = "This Extension File (OCE) is not eligible for auto-update feature. Please upgrade manually!";
                //                break;
                //            #endregion
                default:
                    {
                        if (m_error_info_list.Count != 0)
                        {
                            for (int i = 0; i < m_error_info_list.Count; i++)
                                strErr += string.Format("{0}:{1}\n", i, m_error_info_list[i]);
                        }
                        else
                            strErr = "Un-recognize error code,Communication should be disconnect,Please check";
                    }
                    break;
            }
            return strErr;
        }
        #endregion
    }
}
