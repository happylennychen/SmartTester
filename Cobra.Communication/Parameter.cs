using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Cobra.Communication
{

    public class Parameter : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        //参数唯一标识符
        private UInt32 m_Guid;
        public UInt32 guid
        {
            get { return m_Guid; }
            set { m_Guid = value; }
        }

        //参数在Section参数列表中位置
        private Int32 m_SectionPos;
        public Int32 sectionpos
        {
            get { return m_SectionPos; }
            set { m_SectionPos = value; }
        }

        private Double m_Key;
        public Double key
        {
            get { return m_Key; }
            set { m_Key = value; }
        }

        private UInt16 m_SubSection;
        public UInt16 subsection
        {
            get { return m_SubSection; }
            set { m_SubSection = value; }
        }

        private UInt16 m_SubType;
        public UInt16 subtype
        {
            get { return m_SubType; }
            set { m_SubType = value; }
        }

        private double m_PhyRef;
        public double phyref
        {
            get { return m_PhyRef; }
            set { m_PhyRef = value; }
        }

        private double m_RegRef;
        public double regref
        {
            get { return m_RegRef; }
            set { m_RegRef = value; }
        }

        private double m_Offset;
        public double offset
        {
            get { return m_Offset; }
            set { m_Offset = value; }
        }

        //add by Thitara 2023-2-28
        private bool m_bUpdateCheck;
        public bool bUpdateCheck
        {
            get { return m_bUpdateCheck; }
            set { m_bUpdateCheck = value; }
        }

        /*(D151224)Francis, looks no used anymore
        private double m_MinValue;
        public double minvalue
        {
            get { return m_MinValue; }
            set { m_MinValue = value; }
        }

        private double m_MaxValue;
        public double maxvalue
        {
            get { return m_MaxValue; }
            set { m_MaxValue = value; }
        }
         */

        //(A151224)Francis, add for HexMin, HexMax, PhyMin, and PhyMax
        private Int64 m_dbHexMin;		//Support DWord in KALL17 project
        public Int64 dbHexMin
        {
            get { return m_dbHexMin; }
            set { m_dbHexMin = value; }
        }

        private Int64 m_dbHexMax;		//Support DWord in KALL17 project
        public Int64 dbHexMax
        {
            get { return m_dbHexMax; }
            set { m_dbHexMax = value; }
        }

        private double m_dbPhyMin;
        public double dbPhyMin
        {
            get { return m_dbPhyMin; }
            set
            {
                {
                    m_dbPhyMin = value;
                    OnPropertyChanged("dbPhyMin");
                }
            }
        }

        private double m_dbPhyMax;
        public double dbPhyMax
        {
            get { return m_dbPhyMax; }
            set
            {

                {
                    m_dbPhyMax = value;
                    OnPropertyChanged("dbPhyMax");
                }
            }
        }


        private Dictionary<string, Reg> m_RegList = new Dictionary<string, Reg>();
        public Dictionary<string, Reg> reglist
        {
            get { return m_RegList; }
            set { m_RegList = value; }
        }

        private double m_PhyData;
        public double phydata
        {
            get { return m_PhyData; }
            set
            {
                //if (m_PhyData != value)
                {
                    m_PhyData = value;
                    OnPropertyChanged("phydata");
                }
            }
        }

        private UInt16 m_HexData;
        public UInt16 hexdata
        {
            get { return m_HexData; }
            set { m_HexData = value; }
        }

        private UInt32 m_U32HexData;
        public UInt32 u32hexdata
        {
            get { return m_U32HexData; }
            set
            {
                m_U32HexData = value;
                OnPropertyChanged("u32hexdata");
            }
        }

        private string m_sPhyData;
        public string sphydata
        {
            get { return m_sPhyData; }
            set
            {
                m_sPhyData = value;
                OnPropertyChanged("sphydata");
            }
        }

        //Add 20170301
        private bool m_bShow = true; //用于设定此参数是否需要在UI上隐藏之功能
        public bool bShow
        {
            get { return m_bShow; }
            set
            {
                m_bShow = value;
                OnPropertyChanged("bShow");
            }
        }

        //Add 20220126
        private string m_sMessage = string.Empty; //用于传输参数详细信息
        public string sMessage
        {
            get { return m_sMessage; }
            set
            {
                m_sMessage = value;
                OnPropertyChanged("sMessage");
            }
        }

        private TSMBbuffer m_TsmbBuffer = new TSMBbuffer();
        public TSMBbuffer tsmbBuffer
        {
            get { return m_TsmbBuffer; }
            set { m_TsmbBuffer = value; }
        }

        private Dictionary<string, SFL> m_SFLList = new Dictionary<string, SFL>();
        public Dictionary<string, SFL> sfllist
        {
            get { return m_SFLList; }
            set { m_SFLList = value; }
        }

        private AsyncObservableCollection<string> m_ItemList = new AsyncObservableCollection<string>();
        public AsyncObservableCollection<string> itemlist
        {
            get { return m_ItemList; }
            set
            {
                m_ItemList = value;
                OnPropertyChanged("itemlist");
            }
        }

        private UInt32 m_ErrorCode;
        public UInt32 errorcode
        {
            get { return m_ErrorCode; }
            set
            {
                m_ErrorCode = value;
                OnPropertyChanged("errorcode");
            }
        }

        /// <summary>
        /// 通过参数名检查节点是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetXElementValueByName(XElement node, string name)
        {
            if (node.Element(name) == null) return null;
            else if (String.IsNullOrEmpty(node.Element(name).Value)) return null;
            else return node.Element(name).Value;
        }

        /// <summary>
        /// 通过参数属性名检查节点是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetXElementValueByAttribute(XElement node, string name)
        {
            if (node.Attribute(name) == null) return null;
            else if (String.IsNullOrEmpty(node.Attribute(name).Value)) return null;
            else return node.Attribute(name).Value;
        }
    }
    public class Reg
    {
        private UInt16 m_Address;
        public UInt16 address
        {
            get { return m_Address; }
            set { m_Address = value; }
        }
        private UInt32 m_U32Address;
        public UInt32 u32Address
        {
            get { return m_U32Address; }
            set { m_U32Address = value; }
        }

        private UInt16 m_StartBit;
        public UInt16 startbit
        {
            get { return m_StartBit; }
            set { m_StartBit = value; }
        }

        private UInt16 m_BitsNumber;
        public UInt16 bitsnumber
        {
            get { return m_BitsNumber; }
            set { m_BitsNumber = value; }
        }
    }
    public class TSMBbuffer
    {
        public ushort length;
        public byte[] bdata; //byte0:type(op,efuse) byte1:reg addr byte2~31:data

        public TSMBbuffer()
        {
            length = 2;
            bdata = new byte[32];
        }
    }
    public class SFL : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        private Parameter m_Parent;
        public Parameter parent
        {
            get { return m_Parent; }
            set { m_Parent = value; }
        }

        private string m_SFLName;
        public string sflname
        {
            get { return m_SFLName; }
            set { m_SFLName = value; }
        }

        private Hashtable m_node_table = new Hashtable();
        public Hashtable nodetable
        {
            get { return m_node_table; }
            set { m_node_table = value; }
        }
    }
}
