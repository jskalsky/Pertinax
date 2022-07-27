using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfControlLibrary.ViewModel;

namespace WpfControlLibrary
{
    public class ViewModelPorts : INotifyPropertyChanged
    {
        public ViewModelPorts()
        {
            RootNodes = new ObservableCollection<PortsNode>();
        }
        public ObservableCollection<PortsNode> RootNodes { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
