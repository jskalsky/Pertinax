using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfControlLibrary.ViewModel
{
    public class DataModelManagerViewModel : INotifyPropertyChanged
    {
        private readonly string[] _basicTypes = new string[] { "Boolean", "UInt8", "Int8", "UInt16", "Int16", "UInt32", "Int32", "Float", "Double" };
        private string _selectedBasicType;
        private readonly string[] _access = new string[] { "Read", "Write", "ReadWrite" };
        private string _selectedAccess;
        private readonly string[] _kind = new string[] { "Jednoduchá proměnná", "Pole", "Objekt" };
        private string _selectedKind;
        private readonly string[] _idType = new string[] { "UInt32", "String", "Guid", "ByteString" };
        private string _selectedIdType;

        private string _varName;
        private int _varCount;
        private ushort _namespace;
        private int _numericId;
        private string _stringId;

        private Visibility _visibilityArray;
        private Visibility _visibilityObject;
        private Visibility _visibilitySimpleOrArray;
        private Visibility _visibilityRemoveInsert;
        private Visibility _visibilityIdUpDown;
        private Visibility _visibilityIdText;
        private Variable _selectedVariable;
        private ObservableCollection<Variable> _variables;

        public event PropertyChangedEventHandler PropertyChanged;

        public DataModelManagerViewModel()
        {
            Debug.Print($"Construktor DataModelManagerViewModel");
            Variables = new ObservableCollection<Variable>();
            SelectedVariable = null;
            VisibilityRemoveInsert = Visibility.Collapsed;
        }
        public string[] BasicTypes
        {
            get { return _basicTypes; }
        }
        public string[] Access
        {
            get { return _access; }
        }
        public string[] Kind
        {
            get { return _kind; }
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
        public string SelectedKind
        {
            get { return _selectedKind; }
            set { _selectedKind = value; OnPropertyChanged(nameof(SelectedKind)); }
        }
        public string SelectedIdType
        {
            get { return _selectedIdType; }
            set { _selectedIdType = value; OnPropertyChanged(nameof(SelectedIdType)); }
        }
        public Visibility VisibilityArray
        {
            get { return _visibilityArray; }
            set { _visibilityArray = value; OnPropertyChanged(nameof(VisibilityArray)); }
        }
        public Visibility VisibilityObject
        {
            get { return _visibilityObject; }
            set { _visibilityObject = value; OnPropertyChanged(nameof(VisibilityObject)); }
        }
        public Visibility VisibilitySimpleOrArray
        {
            get { return _visibilitySimpleOrArray; }
            set { _visibilitySimpleOrArray = value; OnPropertyChanged(nameof(VisibilitySimpleOrArray)); }
        }

        public ObservableCollection<Variable> Variables
        {
            get { return _variables; }
            private set { _variables = value; OnPropertyChanged(nameof(Variables)); }
        }
        public Variable SelectedVariable
        {
            get { return _selectedVariable; }
            set { _selectedVariable = value; OnPropertyChanged(nameof(SelectedVariable)); }
        }
        public Visibility VisibilityRemoveInsert
        {
            get { return _visibilityRemoveInsert; }
            set { _visibilityRemoveInsert = value; OnPropertyChanged(nameof(VisibilityRemoveInsert)); }
        }
        public Visibility VisibilityIdUpDown
        {
            get { return _visibilityIdUpDown; }
            set { _visibilityIdUpDown = value; OnPropertyChanged(nameof(VisibilityIdUpDown)); }
        }
        public Visibility VisibilityIdText
        {
            get { return _visibilityIdText; }
            set { _visibilityIdText = value; OnPropertyChanged(nameof(VisibilityIdText)); }
        }
        public string VarName
        {
            get { return _varName; }
            set { _varName = value; OnPropertyChanged(nameof(VarName)); }
        }
        public int VarCount
        {
            get { return _varCount; }
            set { _varCount = value; OnPropertyChanged(nameof(VarCount)); }
        }
        public  ushort Namespace
        {
            get { return _namespace; }
            set { _namespace = value; OnPropertyChanged(nameof(Namespace)); }
        }
        public int NumericId
        {
            get { return _numericId; }
            set { _numericId = value; OnPropertyChanged(nameof(NumericId)); }
        }
        public string StringId
        {
            get { return _stringId; }
            set { _stringId = value; OnPropertyChanged(nameof(StringId)); }
        }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
