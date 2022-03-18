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
        private readonly ObservableCollection<OpcObjectItem> _items;
        private string _name;
        private int _publishingInterval;
        private int _writerGroupId;
        private int _dataSetWriterId;
        private bool _subscribe;
        private bool _publish;
        private bool _enableInterval;
        private bool _enablePublish;
        private bool _enableSubscribe;
        private int _publisherId;
        public OpcObject(string name, int publisherId, int writer, int dataSet, int interval, bool subscribe, bool publish)
        {
            Name = name;
            _items = new ObservableCollection<OpcObjectItem>();
            WriterGroupId = writer;
            DataSetWriterId = dataSet;
            PublishingInterval = interval;
            PublisherId = publisherId;
            Publish = publish;
            Subscribe = subscribe;
            EnableInterval = EnableSubscribe = EnablePublish = false;
            if(!Subscribe)
            {
                EnablePublish = true;
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }
        public int PublishingInterval
        {
            get { return _publishingInterval; }
            set { _publishingInterval = value; OnPropertyChanged("PublishingInterval"); }
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
        public bool Subscribe
        {
            get { return _subscribe; }
            set { _subscribe = value; OnPropertyChanged("Subscribe"); }
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
        public bool EnableInterval
        {
            get { return _enableInterval; }
            set { _enableInterval = value; OnPropertyChanged("EnableInterval"); }
        }

        public bool EnablePublish
        {
            get { return _enablePublish; }
            set { _enablePublish = value; OnPropertyChanged("EnablePublish"); }
        }

        public bool EnableSubscribe
        {
            get { return _enableSubscribe; }
            set { _enableSubscribe = value; OnPropertyChanged("EnableSubscribe"); }
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
    }
}
