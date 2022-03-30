using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public class ClientItem : ImportedItem, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int _interval;
        private bool _validity;
        public ClientItem(string path, string name, string ipAddress) : base(path,name)
        {
            IpAddress = ipAddress;
            Interval = 100;
            Validity = false;
        }

        public string IpAddress { get; }
        public int Interval
        {
            get { return _interval; }
            set { _interval = value; OnPropertyChanged("Interval"); }
        }

        public bool Validity
        {
            get { return _validity; }
            set { _validity = value; OnPropertyChanged("Validity"); }
        }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
