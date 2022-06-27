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
        private bool _isExpanded;

        public event PropertyChangedEventHandler PropertyChanged;

        public ClientConnection()
        {
            Vars = new ObservableCollection<ClientVar>();
            ImagePath = "pack://application:,,,/WpfControlLibrary;component/Icons/ClassIcon.png";
            Period = 100;
            Crypto = false;
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
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; OnPropertyChanged("IsExpanded"); }
        }
        public string ImagePath { get; }

        public ObservableCollection<ClientVar> Vars { get; }
        public void AddVar(ushort ns, string id)
        {
            ClientVar var = new ClientVar() { NsIndex = ns, Id = id };
            Vars.Add(var);
        }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
