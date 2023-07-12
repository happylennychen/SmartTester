using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Cobra.Communication
{
    public class ComboboxRoad
    {
        public int ID { set; get; }
        public UInt16 Code { set; get; }
        public string Info { set; get; }
    }
    public class Options : INotifyPropertyChanged
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

        private string m_NickName;
        public string nickname
        {
            get { return m_NickName; }
            set { m_NickName = value; }
        }

        private double m_Data;
        public double data
        {
            get { return m_Data; }
            set
            {
                if (m_Data != value)
                {
                    m_Data = value;
                    OnPropertyChanged("data");
                }
            }
        }

        private UInt32 m_Guid;
        public UInt32 guid
        {
            get { return m_Guid; }
            set { m_Guid = value; }
        }

        //参数在SFL参数列表中位置
        private Int32 m_Order;
        public Int32 order
        {
            get { return m_Order; }
            set { m_Order = value; }
        }

        private string m_Catalog;
        public string catalog
        {
            get { return m_Catalog; }
            set { m_Catalog = value; }
        }

        private UInt16 m_EditorType;
        public UInt16 editortype
        {
            get { return m_EditorType; }
            set { m_EditorType = value; }
        }

        private UInt16 m_Format;
        public UInt16 format
        {
            get { return m_Format; }
            set { m_Format = value; }
        }

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

        private bool m_bEdit;
        public bool bedit
        {
            get { return m_bEdit; }
            set
            {
                m_bEdit = value;
                OnPropertyChanged("bedit");
            }
        }

        private bool m_bError;
        public bool berror
        {
            get { return m_bError; }
            set
            {
                m_bError = value;
                OnPropertyChanged("berror");
            }
        }

        private bool m_bRange;
        public bool brange
        {
            get { return m_bRange; }
            set
            {
                m_bRange = value;
                OnPropertyChanged("brange");
            }
        }

        private string m_sPhydata;
        public string sphydata
        {
            get { return m_sPhydata; }
            set
            {
                //if (m_sPhydata != value)
                {
                    m_sPhydata = value;
                    OnPropertyChanged("sphydata");
                }
            }
        }

        private string m_sDeviceName;
        public string sdevicename
        {
            get { return m_sDeviceName; }
            set
            {
                m_sDeviceName = value;
                OnPropertyChanged("sdevicename");
            }
        }

        private bool m_bCheck;
        public bool bcheck
        {
            get { return m_bCheck; }
            set
            {
                //if (m_bCheck != value)
                {
                    m_bCheck = value;
                    OnPropertyChanged("bcheck");
                }
            }
        }

        #region Combobox对象
        private ComboboxRoad m_selectLocation = new ComboboxRoad();
        public ComboboxRoad SelectLocation
        {
            get { return m_selectLocation; }
            set
            {
                m_selectLocation = value;
                if (this.PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectLocation"));
            }
        }

        private ObservableCollection<ComboboxRoad> m_locationSource = new ObservableCollection<ComboboxRoad>();
        public ObservableCollection<ComboboxRoad> LocationSource
        {
            get { return m_locationSource; }
            set
            {
                m_locationSource = value;
                if (this.PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("LocationSource"));
            }
        }
        #endregion
    }
}
