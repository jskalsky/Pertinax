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
        private bool _isExpanded;

        public event PropertyChangedEventHandler PropertyChanged;

        public ClientConnection(string ipAddress, bool crypto)
        {
            Groups = new ObservableCollection<Group>();
            ImagePath = "pack://application:,,,/WpfControlLibrary;component/Icons/ClassIcon.png";
            Crypto = crypto;
            IpAddress = ipAddress;
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
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; OnPropertyChanged("IsExpanded"); }
        }
        public string ImagePath { get; }

        public ObservableCollection<Group> Groups { get; }

        public void AddGroup(Group group)
        {
            Groups.Add(group);
        }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
