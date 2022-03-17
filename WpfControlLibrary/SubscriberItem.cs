using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public class SubscriberItem : INotifyPropertyChanged
    {
        private bool _subscribe;
        public SubscriberItem(string path, string name)
        {
            ObjectName = name;
            Path = path;
        }
        public string Path { get; }
        public string ObjectName { get; }

        public bool Subscribe
        {
            get { return _subscribe; }
            set { _subscribe = value; OnPropertyChanged("Subscribe"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
