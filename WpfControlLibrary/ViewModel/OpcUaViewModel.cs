using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfControlLibrary.DataModel;
using WpfControlLibrary.Client;

namespace WpfControlLibrary.ViewModel
{
    public class OpcUaViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _localIpAddress;
        private string _multicastIpAddress;
        public OpcUaViewModel()
        {
            SelectedNode = null;
            DataModel = new ObservableCollection<DataModelNode>();
            Connections = new ObservableCollection<ClientConnection>();
            ObjectTypes = new List<DataModelObjectType>();
            MulticastIpAddress = "224.0.0.22";
            LocalIpAddress = "10.10.13.252";
        }

        public DataModelNode SelectedNode { get; set; }
        public DataModelNamespace DataModelNamespace0 { get; set; }
        public DataModelNamespace DataModelNamespace1 { get; set; }

        public ObservableCollection<DataModelNode> DataModel { get; }
        public ObservableCollection<ClientConnection> Connections { get; }
        public List<DataModelObjectType> ObjectTypes { get; }
        public Client.ClientConnection SelectedConnection { get; set; }

        public string LocalIpAddress
        {
            get { return _localIpAddress; }
            set { _localIpAddress = value; OnPropertyChanged(nameof(LocalIpAddress)); }
        }
        public string MulticastIpAddress
        {
            get { return _multicastIpAddress; }
            set { _multicastIpAddress = value; OnPropertyChanged(nameof(MulticastIpAddress)); }
        }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
