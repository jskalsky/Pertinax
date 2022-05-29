using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public class OpcObjectItem : INotifyPropertyChanged, IDataErrorInfo
    {
        private readonly string[] _basicTypes = new string[] { "Boolean", "UInt8", "Int8", "UInt16", "Int16", "UInt32", "Int32", "Float", "Double" };
        private string _selectedBasicType;
        private readonly string[] _access = new string[] { "Read", "Write", "ReadWrite" };
        private string _selectedAccess;
        private readonly string[] _rank = new string[] { "Jednoduchá proměnná", "Pole" };
        private string _selectedRank;
        private int _arraySizeValue;
        private bool _writeOutside;
        private string _name;
        private object _rankTag;
        private bool _selected;

        private bool _enableBasicTypes;
        private bool _enableAccess;
        private bool _enableRank;
        private bool _enableArraySize;
        private bool _enableWriteOutside;

        private string _nodeIdString;
        private NodeIdBase _lastNodeId = null;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public OpcObjectItem(string name, OpcObject parent)
        {
            Debug.Print($"");
            Name = name;
            SelectedAccess = _access[0];
            SelectedRank = _rank[0];
            ArraySizeValue = 0;
            SelectedBasicType = _basicTypes[7];
            EnableBasicTypes = true;
            EnableRank = true;
            WriteOutside = false;
            EnableWriteOutside = false;
            RankTag = this;
            NodeId = NodeIdNumeric.GetNextNodeIdNumeric(1, false);
            _lastNodeId = NodeId;
            NodeIdBase.Add(NodeId);
            NodeIdString = NodeId.NodeIdString;
            Parent = parent;
        }

        public OpcObjectItem(string name, string access, string rank, int arraySizeValue, string basicType,
            bool writeOutside, string nodeId, OpcObject parent)
        {
            Debug.Print($"Construktor {nodeId}");
            Name = name;
            SelectedAccess = access;
            SelectedRank = rank;
            ArraySizeValue = arraySizeValue;
            SelectedBasicType = basicType;
            EnableBasicTypes = true;
            EnableRank = true;
            WriteOutside = writeOutside;
            EnableWriteOutside = false;
            RankTag = this;
            NodeId = NodeIdBase.GetNodeIdBase(nodeId);
            _lastNodeId = NodeId;
            NodeIdBase.Add(NodeId);
            NodeIdString = NodeId.NodeIdString;
            Parent = parent;
        }

        public OpcObject Parent { get; }
        public NodeIdBase NodeId { get; private set; }
        public string NodeIdString
        {
            get { return _nodeIdString; }
            set
            {
                Debug.Print($"set NodeIdString _nodeIdString= {_nodeIdString}, value= {value}");
                _nodeIdString = value;
                OnPropertyChanged("NodeIdString");
            }
        }
        public string[] BasicTypes
        {
            get { return _basicTypes; }
        }

        public string SelectedBasicType
        {
            get { return _selectedBasicType; }
            set { _selectedBasicType = value; OnPropertyChanged("SelectedBasicType"); }
        }

        public string[] Access
        {
            get { return _access; }
        }

        public string SelectedAccess
        {
            get { return _selectedAccess; }
            set { _selectedAccess = value; }
        }

        public string[] Rank
        {
            get { return _rank; }
        }

        public string SelectedRank
        {
            get { return _selectedRank; }
            set { _selectedRank = value; OnPropertyChanged("SelectedRank"); }
        }

        public int ArraySizeValue
        {
            get { return _arraySizeValue; }
            set { _arraySizeValue = value; OnPropertyChanged("ArraySizeValue"); }
        }

        public bool WriteOutside
        {
            get { return _writeOutside; }
            set { _writeOutside = value; OnPropertyChanged("WriteOutside"); }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }

        public object RankTag
        {
            get { return _rankTag; }
            set { _rankTag = value; OnPropertyChanged("RankTag"); }
        }
        public bool EnableBasicTypes
        {
            get { return _enableBasicTypes; }
            set { _enableBasicTypes = value; OnPropertyChanged("EnableBasicTypes"); }
        }
        public bool EnableAccess
        {
            get { return _enableAccess; }
            set { _enableAccess = value; OnPropertyChanged("EnableAccess"); }
        }
        public bool EnableRank
        {
            get { return _enableRank; }
            set { _enableRank = value; OnPropertyChanged("EnableRank"); }
        }
        public bool EnableArraySize
        {
            get { return _enableArraySize; }
            set { _enableArraySize = value; OnPropertyChanged("EnableArraySize"); }
        }
        public bool EnableWriteOutside
        {
            get { return _enableWriteOutside; }
            set { _enableWriteOutside = value; OnPropertyChanged("EnableWriteOutside"); }
        }

        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; OnPropertyChanged("Selected"); }
        }

        public string Error => throw new NotImplementedException();

        private string Validate(string propertyName)
        {
            Debug.Print($"OpcObjectItem Validate {propertyName}");
            string error = string.Empty;
            switch (propertyName)
            {
                case "NodeIdString":
                    Debug.Print($"NodeIdString= {NodeIdString}, _nodeIdString= {_nodeIdString}");
                    if (_lastNodeId != null)
                    {
                        Debug.Print($"_lastNodeId= {_lastNodeId.NodeIdString}, {_lastNodeId.Guid}");
                    }
                    if (_lastNodeId != null && NodeIdString == _lastNodeId.NodeIdString)
                    {
                        Debug.Print($"==");
                        return error;
                    }

                    error = OpcObject.ValidateNodeId(NodeIdString);
                    if (error.Contains("formát"))
                    {
                        return error;
                    }
                    if (_lastNodeId != null)
                    {
                        NodeIdBase.Remove(_lastNodeId);
                    }
                    NodeId = NodeIdBase.GetNodeIdBase(NodeIdString);
                    _lastNodeId = NodeId;
                    Debug.Print($"Novy _lastNodeId= {_lastNodeId.NodeIdString}");
                    NodeIdBase.Add(NodeId);
                    NodeIdBase.PrintIds();
                    MainViewModel.IsError = (!string.IsNullOrEmpty(error));
                    Debug.Print($"Validate result= {MainViewModel.IsError}, error= {error}");
                    int nr = NodeIdBase.GetNrOfErrors();
                    if(nr == 0)
                    {
                        foreach(OpcObjectItem ooi in Parent.Items)
                        {
                            ooi.NodeIdString = ooi.NodeIdString;
                        }
                    }
                    break;
            }

            return error;
        }
        public string this[string columnName]
        {
            get
            {
                return Validate(columnName);
            }
        }
    }
}
