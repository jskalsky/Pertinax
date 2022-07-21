using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfControlLibrary.DataModel;
using WpfControlLibrary.Client;
using System.Windows;

namespace WpfControlLibrary.ViewModel
{
    public class OpcUaViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _localIpAddress;
        private string _multicastIpAddress;
        private readonly string[] _basicTypes = new string[] { "Boolean", "UInt8", "Int8", "UInt16", "Int16", "UInt32", "Int32", "Float", "Double" };
        private string _selectedBasicType;
        private readonly string[] _access = new string[] { "Read", "Write", "ReadWrite" };
        private string _selectedAccess;
        private readonly string[] _idType = new string[] { "UInt32", "String", "Guid", "ByteString" };
        private string _selectedIdType;
        private int _varCount;
        private int _arrayLength;
        private string _varName;

        private Visibility _visibilityNumeric;
        private Visibility _visibilityString;
        private Visibility _visibilityAddGroup;

        private int _selectedNumeric;
        private string _selectedString;
        private string _selectedName;
        public OpcUaViewModel()
        {
            SelectedNode = null;
            DataModel = new ObservableCollection<DataModelNode>();
            Connections = new ObservableCollection<ClientConnection>();
            ObjectTypes = new List<DataModelObjectType>();
            MulticastIpAddress = "224.0.0.22";
            LocalIpAddress = "10.10.13.252";
            SelectedBasicType = _basicTypes[0];
            SelectedAccess = _access[0];
            SelectedIdType = _idType[0];
            VarCount = 1;
            VisibilityAddGroup = Visibility.Collapsed;
        }

        public DataModelNode SelectedNode { get; set; }
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
        public string[] BasicTypes
        {
            get { return _basicTypes; }
        }
        public string[] Access
        {
            get { return _access; }
        }
        public string[] IdType
        {
            get { return _idType; }
        }
        public string SelectedBasicType
        {
            get { return _selectedBasicType; }
            set { _selectedBasicType = value; OnPropertyChanged(nameof(SelectedBasicType)); }
        }
        public string SelectedAccess
        {
            get { return _selectedAccess; }
            set { _selectedAccess = value; OnPropertyChanged(nameof(SelectedAccess)); }
        }
        public string SelectedIdType
        {
            get { return _selectedIdType; }
            set { _selectedIdType = value; OnPropertyChanged(nameof(SelectedIdType)); }
        }
        public int ArrayLength
        {
            get { return _arrayLength; }
            set { _arrayLength = value; OnPropertyChanged(nameof(ArrayLength)); }
        }
        public int VarCount
        {
            get { return _varCount; }
            set { _varCount = value; OnPropertyChanged(nameof(VarCount)); }
        }
        public string VarName
        {
            get { return _varName; }
            set { _varName = value; OnPropertyChanged(nameof(VarName)); }
        }
        public Visibility VisibilityNumeric
        {
            get { return _visibilityNumeric; }
            set { _visibilityNumeric = value; OnPropertyChanged(nameof(VisibilityNumeric)); }
        }
        public Visibility VisibilityString
        {
            get { return _visibilityString; }
            set { _visibilityString = value; OnPropertyChanged(nameof(VisibilityString)); }
        }
        public Visibility VisibilityAddGroup
        {
            get { return _visibilityAddGroup; }
            set { _visibilityAddGroup = value; OnPropertyChanged(nameof(VisibilityAddGroup)); }
        }
        public int SelectedNumeric
        {
            get { return _selectedNumeric; }
            set { _selectedNumeric = value; OnPropertyChanged(nameof(SelectedNumeric)); }
        }
        public string SelectedString
        {
            get { return _selectedString; }
            set { _selectedString = value; OnPropertyChanged(nameof(SelectedString)); }
        }

        public string SelectedName
        {
            get { return _selectedName; }
            set { _selectedName = value; OnPropertyChanged(nameof(SelectedName)); }
        }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
