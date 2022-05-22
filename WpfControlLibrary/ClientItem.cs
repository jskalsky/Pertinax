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
        private bool _monitoring;
        private string _ipAddress;
        private bool _encryptClient;

        public ClientItem(string path, string name, string ipAddress, OpcObject opcObject, bool validity, int rxtxPeriod, bool monitoring, bool encrypt) : base(path,name)
        {
            Debug.Print($"Constructor ClientItem {rxtxPeriod}");
            IpAddress = ipAddress;
            RxTxPeriod = rxtxPeriod;
            Validity = validity;
            OpcObject = opcObject;
            Monitoring = monitoring;
            EncryptClient = encrypt;
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

        public bool Monitoring
        {
            get { return _monitoring; }
            set { _monitoring = value;OnPropertyChanged("Monitoring"); }
        }
        public string IpAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; OnPropertyChanged("IpAddress"); }
        }

        public bool EncryptClient
        {
            get { return _encryptClient; }
            set { _encryptClient = value; OnPropertyChanged("EncryptClient"); }
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
