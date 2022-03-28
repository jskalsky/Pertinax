using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public class OpcObject : INotifyPropertyChanged
    {
        private const string DefaultName = "Pertinax";
        private readonly ObservableCollection<OpcObjectItem> _items;
        private string _name;
        private int _writerGroupId;
        private int _dataSetWriterId;
        private bool _publish;
        private bool _enablePublish;
        private int _publisherId;
        private object _checkBoxPublish;
        private bool _isImported;
        private static int _nextWriterGroupId = 1;
        private static int _nextDataSetWriterId = 1;
        private static int _nextDefaultNameIndex = 1;
        protected OpcObject()
        {
            _items = new ObservableCollection<OpcObjectItem>();
        }
        public OpcObject(string name, int publisherId) : this()
        {
            Name = name;
            PublisherId = publisherId;
            WriterGroupId = _nextWriterGroupId++;
            DataSetWriterId = _dataSetWriterId++;
            Publish = true;
            IsImported = false;
        }

        public OpcObject(string name) : this()
        {
            Name = name;
            Publish = false;
            IsImported = false;
        }

        public OpcObject(string name, int publisherId, int writerGroupId, int dataSetWriterId) : this()
        {
            Name = name;
            PublisherId = publisherId;
            WriterGroupId = writerGroupId;
            DataSetWriterId = dataSetWriterId;
            Publish = false;
            IsImported = true;
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }
        public int WriterGroupId
        {
            get { return _writerGroupId; }
            set { _writerGroupId = value; OnPropertyChanged("WriterGroupId"); }
        }
        public int DataSetWriterId
        {
            get { return _dataSetWriterId; }
            set { _dataSetWriterId = value; OnPropertyChanged("DataSetWriterId"); }
        }

        public bool Publish
        {
            get { return _publish; }
            set { _publish = value; OnPropertyChanged("Publish"); }
        }

        public int PublisherId
        {
            get { return _publisherId; }
            set { _publisherId = value; OnPropertyChanged("PublisherId"); }
        }

        public bool EnablePublish
        {
            get { return _enablePublish; }
            set { _enablePublish = value; OnPropertyChanged("EnablePublish"); }
        }

        public object CheckBoxPublish
        {
            get { return _checkBoxPublish; }
            set { _checkBoxPublish = value; OnPropertyChanged("CheckBoxPublish"); }
        }

        public bool IsImported
        {
            get { return _isImported; }
            set { _isImported = value; OnPropertyChanged("IsImported"); }
        }
        public ObservableCollection<OpcObjectItem> Items => _items;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public OpcObjectItem AddItem(string name)
        {
            OpcObjectItem ooi = new OpcObjectItem(name);
            _items.Add(ooi);
            return ooi;
        }

        public void AddItem(OpcObjectItem ooi)
        {
            _items.Add(ooi);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public string GetDefaultName()
        {
            return $"{DefaultName}{_nextDefaultNameIndex++}";
        }

        public int GetDefaultNameIndex()
        {
            LinkedList<char> ll = new LinkedList<char>();
            string text = string.Empty;
            for(int i=Name.Length-1;i>= 0;--i)
            {
                if(char.IsDigit(Name[i]))
                {
                    ll.AddFirst(Name[i]);
                }
                else
                {
                    text = Name.Substring(0, i + 1);
                    break;
                }
            }
            if(ll.Count != 0)
            {
                if(string.IsNullOrEmpty(text))
                {
                    return -1;
                }
                if(text == DefaultName)
                {
                    return int.Parse(ll.ToString());
                }
            }
            return -1;
        }
        public static OpcObject Create(string name, string description)
        {
            string[] descrItems = description.Split(';');
            if (descrItems.Length != 6)
            {
                return null;
            }
            bool subscribe = false;
            if (!bool.TryParse(descrItems[0], out subscribe))
            {
                return null;
            }
            bool publish = false;
            if (!bool.TryParse(descrItems[1], out publish))
            {
                return null;
            }
            return Create(name, description, subscribe, publish);
        }
        public static OpcObject Create(string name, string description, bool subscribe, bool publish)
        {
            string[] descrItems = description.Split(';');
            if (descrItems.Length != 6)
            {
                return null;
            }
            int publisherId = 0;
            if (!int.TryParse(descrItems[2], out publisherId))
            {
                return null;
            }
            int writer = 0;
            if (!int.TryParse(descrItems[3], out writer))
            {
                return null;
            }
            int ds = 0;
            if (!int.TryParse(descrItems[4], out ds))
            {
                return null;
            }
            int pInterval = 0;
            if (!int.TryParse(descrItems[5], out pInterval))
            {
                return null;
            }
            return new OpcObject(name, publisherId, writer, ds, pInterval, subscribe, publish);
        }
    }
}
