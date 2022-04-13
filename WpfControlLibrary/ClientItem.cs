using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public class ClientItem : ImportedItem, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int _rxtxPeriod;
        private bool _validity;
        private string _ipAddress;
        public ClientItem(string path, string name, string ipAddress, OpcObject opcObject, bool validity, int rxtxPeriod) : base(path,name)
        {
            Debug.Print($"Constructor ClientItem {rxtxPeriod}");
            IpAddress = ipAddress;
            RxTxPeriod = rxtxPeriod;
            Validity = validity;
            OpcObject = opcObject;
        }

        public OpcObject OpcObject { get; }

        public int RxTxPeriod
        {
            get { return _rxtxPeriod; }
            set { _rxtxPeriod = value; OnPropertyChanged("RxTxPeriod"); }
        }

        public bool Validity
        {
            get { return _validity; }
            set { _validity = value; OnPropertyChanged("Validity"); }
        }

        public string IpAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; OnPropertyChanged("IpAddress"); }
        }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
