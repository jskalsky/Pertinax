using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Client
{
    public class ClientConnection : INotifyPropertyChanged
    {
        private string _ipAddress;
        private bool _crypto;
        private ushort _period;
        private string _service;

        public event PropertyChangedEventHandler PropertyChanged;

        public ClientConnection()
        {
            Vars=new ObservableCollection<ClientVar>();
        }

        public string IpAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; OnPropertyChanged(nameof(IpAddress)); }
        }

        public bool Crypto
        {
            get { return _crypto; }
            set { _crypto = value; OnPropertyChanged(nameof(Crypto)); }
        }

        public ushort Period
        {
            get { return _period; }
            set { _period = value; OnPropertyChanged(nameof(Period)); }
        }

        public string Service
        {
            get { return _service; }
            set { _service = value; OnPropertyChanged(nameof(Service)); }
        }

        public ObservableCollection<ClientVar> Vars { get; }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
