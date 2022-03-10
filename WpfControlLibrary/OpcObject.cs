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
        private int _publishingInterval;
        private string _name;
        private bool _pub;
        private bool _sub;
        public OpcObject(string name)
        {
            Name = name;
            PublishingInterval = 0;
            _items = new ObservableCollection<OpcObjectItem>();
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

        public bool Pub
        {
            get { return _pub; }
            set { _pub = value; OnPropertyChanged("Pub"); }
        }
        public bool Sub
        {
            get { return _sub; }
            set { _sub = value; OnPropertyChanged("Sub"); }
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
