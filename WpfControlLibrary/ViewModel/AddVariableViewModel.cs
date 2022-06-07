using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfControlLibrary.DataModel;

namespace WpfControlLibrary.ViewModel
{
    public class AddVariableViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly string[] _basicTypes = new string[] { "Boolean", "UInt8", "Int8", "UInt16", "Int16", "UInt32", "Int32", "Float", "Double" };
        private string _selectedBasicType;
        private readonly string[] _access = new string[] { "Read", "Write", "ReadWrite" };
        private string _selectedAccess;
        private readonly string[] _kind = new string[] { "Jednoduchá proměnná", "Pole", "Objekt" };
        private string _selectedKind;
        private string _varName;
        private string _varId;
        private int _varCount;
        private int _arrayLength;

        private bool _enableArrayLength;
        private bool _enableObjectName;
        private bool _enableVarName;
        private bool _enableVarId;
        private bool _enableBasicType;
        private bool _enableAccess;

        private Visibility _visSimple;
        private Visibility _visArray;
        private Visibility _visObject;
        private Visibility _visId;
        public AddVariableViewModel()
        {
            SelectedBasicType = _basicTypes[0];
            SelectedAccess = _access[0];
            SelectedKind = _kind[0];
            ArrayLength = 0;
            VarCount = 1;
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
        public string[] Kind
        {
            get { return _kind; }
        }
        public string SelectedKind
        {
            get { return _selectedKind; }
            set { _selectedKind = value; OnPropertyChanged("SelectedKind"); }
        }

        public bool EnableArrayLength
        {
            get { return _enableArrayLength; }
            set { _enableArrayLength = value; OnPropertyChanged("EnableArrayLength"); }
        }

        public bool EnableObjectName
        {
            get { return _enableObjectName; }
            set { _enableObjectName = value; OnPropertyChanged("EnableObjectName"); }
        }

        public string VarName
        {
            get { return _varName; }
            set { _varName = value; OnPropertyChanged("VarName"); }
        }

        public string VarId
        {
            get { return _varId; }
            set { _varId = value; OnPropertyChanged("VarId"); }
        }

        public bool EnableVarName
        {
            get { return _enableVarName; }
            set { _enableVarName = value; OnPropertyChanged("EnableVarName"); }
        }

        public bool EnableVarId
        {
            get { return _enableVarId; }
            set { _enableVarId = value; OnPropertyChanged("EnableVarId"); }
        }

        public bool EnableBasicType
        {
            get { return _enableBasicType; }
            set { _enableBasicType = value; OnPropertyChanged("EnableBasicType"); }
        }

        public bool EnableAccess
        {
            get { return _enableAccess; }
            set { _enableAccess = value; OnPropertyChanged("EnableAccess"); }
        }

        public int VarCount
        {
            get { return _varCount; }
            set
            {
                _varCount = value;
            }
        }

        public int ArrayLength
        {
            get { return _arrayLength; }
            set
            {
                _arrayLength = value;
            }
        }

        public Visibility VisSimple
        {
            get { return _visSimple; }
            set { _visSimple = value; OnPropertyChanged("VisSimple"); }
        }

        public Visibility VisArray
        {
            get { return _visArray; }
            set { _visArray = value; OnPropertyChanged("VisArray"); }
        }
        public Visibility VisObject
        {
            get { return _visObject; }
            set { _visObject = value; OnPropertyChanged("VisObject"); }
        }
        public Visibility VisId
        {
            get { return _visId; }
            set { _visId = value; OnPropertyChanged("VisId"); }
        }

        public DataModelNode ParentNode { get; set; }
        public ushort Namespace { get; set; }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
