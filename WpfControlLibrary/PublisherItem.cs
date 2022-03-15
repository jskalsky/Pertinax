using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public class PublisherItem : INotifyPropertyChanged
    {
        private int _publishingInterval;
        public PublisherItem(string objectName, ushort publishingInterval)
        {
            ObjectName = objectName;
            PublishingInterval = publishingInterval;
        }
        public string ObjectName { get; }
        public int PublishingInterval
        {
            get { return _publishingInterval; }
            set { _publishingInterval = value;OnPropertyChanged("PublishingInterval"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
