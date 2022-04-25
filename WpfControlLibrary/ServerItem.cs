using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public class ServerItem : INotifyPropertyChanged
    {
        private OpcObject _opcObject;

        public event PropertyChangedEventHandler PropertyChanged;

        public ServerItem(OpcObject oo)
        {
            OpcObject = oo;
        }

        public OpcObject OpcObject
        {
            get { return _opcObject; }
            set { _opcObject = value; OnPropertyChanged("OpcObject"); }
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
