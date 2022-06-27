using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Client
{
    public class ClientVar : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ushort _nsIndex;
        private string _id;

        public ushort NsIndex
        {
            get { return _nsIndex; }
            set { _nsIndex = value; OnPropertyChanged(nameof(NsIndex)); }
        }

        public string Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
