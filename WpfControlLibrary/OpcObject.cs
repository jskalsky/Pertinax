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

        public OpcObject(string name, int writer, int dataSet, int publishingI, bool subscribe = false)
        {
            Name = name;
            _items = new ObservableCollection<OpcObjectItem>();
            WriterGroupId = writer;
            DataSetWriterId = dataSet;
            Subscribe = subscribe;
            PublishingInterval = publishingI;
        }

        public OpcObject(OpcObject oo, int writer, int dataSet, int publishingI, bool subscribe = false)
        {
            Name = oo.Name;
            _items = new ObservableCollection<OpcObjectItem>();
            WriterGroupId = writer;
            DataSetWriterId = dataSet;
            Subscribe = subscribe;
            PublishingInterval = publishingI;
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
        public int WriterGroupId { get; }
        public int DataSetWriterId { get; }
        public bool Subscribe { get; }
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
