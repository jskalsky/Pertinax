﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfControlLibrary.Client;
using System.Windows;
using System.Diagnostics;

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
        private Visibility _visibilityConProperty;
        private Visibility _visibilityGroupProperty;
        private Visibility _visibilityVarProperty;
        private Visibility _visibilityAddVars;
        private Visibility _visibilityChangeVar;

        private int _selectedNumeric;
        private string _selectedString;
        private string _selectedName;
        private StatusMsg _selectedStatusMsg;

        private static OpcUaViewModel _instance;

        private ObservableCollection<ClientVar> _vars;

        private string _connectionIpAddress;
        private ushort _connectionPeriod;
        private string _connectionService;
        private ushort _connectionNs;
        private string _connectionIdType;
        private uint _connectionIdNumeric;
        private string _connectionIdString;
        private int _connectionNrVars;
        public OpcUaViewModel()
        {
            Connections = new ObservableCollection<ClientConnection>();
            Status = new ObservableCollection<StatusMsg>();
            MulticastIpAddress = "224.0.0.22";
            LocalIpAddress = "10.10.13.252";
            SelectedBasicType = _basicTypes[0];
            SelectedAccess = _access[0];
            SelectedIdType = _idType[0];
            VarCount = 1;
            VisibilityAddGroup = Visibility.Collapsed;
            VisibilityConProperty = Visibility.Collapsed;
            VisibilityGroupProperty = Visibility.Collapsed;
            VisibilityVarProperty = Visibility.Collapsed;
            VisibilityAddVars = Visibility.Collapsed;
            VisibilityChangeVar = Visibility.Collapsed;
            _instance = this;
        }

        public ObservableCollection<ClientConnection> Connections { get; }
        public ObservableCollection<StatusMsg> Status { get; }
        public object SelectedConnectionObject { get; set; }
        public ClientVar SelectedClientVar { get; set; }

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
        public Visibility VisibilityConProperty
        {
            get { return _visibilityConProperty; }
            set { _visibilityConProperty = value; OnPropertyChanged(nameof(VisibilityConProperty)); }
        }
        public Visibility VisibilityGroupProperty
        {
            get { return _visibilityGroupProperty; }
            set { _visibilityGroupProperty = value; OnPropertyChanged(nameof(VisibilityGroupProperty)); }
        }
        public Visibility VisibilityVarProperty
        {
            get { return _visibilityVarProperty; }
            set { _visibilityVarProperty = value; OnPropertyChanged(nameof(VisibilityVarProperty)); }
        }
        public Visibility VisibilityAddVars
        {
            get { return _visibilityAddVars; }
            set { _visibilityAddVars = value; OnPropertyChanged(nameof(VisibilityAddVars)); }
        }
        public Visibility VisibilityChangeVar
        {
            get { return _visibilityChangeVar; }
            set { _visibilityChangeVar = value; OnPropertyChanged(nameof(VisibilityChangeVar)); }
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

        public StatusMsg SelectedStatusMsg
        {
            get { return _selectedStatusMsg; }
            set { _selectedStatusMsg = value; OnPropertyChanged(nameof(SelectedStatusMsg)); }
        }

        public ObservableCollection<ClientVar> Vars
        {
            get { return _vars; }
            set { _vars = value; OnPropertyChanged("Vars"); }
        }

        public string ConnectionIpAddress
        {
            get { return _connectionIpAddress; }
            set { _connectionIpAddress = value; OnPropertyChanged(nameof(ConnectionIpAddress)); }
        }
        public ushort ConnectionPeriod
        {
            get { return _connectionPeriod; }
            set { _connectionPeriod = value; OnPropertyChanged(nameof(ConnectionPeriod)); }
        }
        public string ConnectionService
        {
            get { return _connectionService; }
            set { _connectionService = value; OnPropertyChanged(nameof(ConnectionService)); }
        }
        public string[] ConnectionServices
        {
            get { return Client.Group.Services; }
        }
        public ushort ConnectionNs
        {
            get { return _connectionNs; }
            set { _connectionNs = value; OnPropertyChanged(nameof(ConnectionNs)); }
        }
        public string ConnectionIdType
        {
            get { return _connectionIdType; }
            set { _connectionIdType = value; OnPropertyChanged(nameof(ConnectionIdType)); }
        }
        public uint ConnectionIdNumeric
        {
            get { return _connectionIdNumeric; }
            set { _connectionIdNumeric = value; OnPropertyChanged(nameof(ConnectionIdNumeric)); }
        }
        public string ConnectionIdString
        {
            get { return _connectionIdString; }
            set { _connectionIdString = value; OnPropertyChanged(nameof(ConnectionIdString)); }
        }
        public int ConnectionNrVars
        {
            get { return _connectionNrVars; }
            set { _connectionNrVars = value; OnPropertyChanged(nameof(ConnectionNrVars)); }
        }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public static void AddStatusMessage(string type, string message, object tag = null)
        {
            Debug.Print($"AddStatusMessage {type},  {message}");
            _instance.Status.Add(new StatusMsg(type, message, tag));
        }
    }
}
