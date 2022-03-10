using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public class ViewModelPorts : INotifyPropertyChanged
    {
        private PortsNode _rootNode;

        public PortsNode RootNode
        {
            get { return _rootNode; }
            set { _rootNode = value; OnPropertyChanged("RootNode"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
