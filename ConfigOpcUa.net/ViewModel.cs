using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigOpcUa.net
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _objectName;
        private readonly string[] _basicTypes = new string[] { "Boolean", "UInt8", "Int8", "UInt16", "Int16", "UInt32", "Int32", "Float", "Double" };
        private string _selectedBasicType;
        private readonly string[] _access = new string[] { "Read", "Write", "ReadWrite" };
        private string _selectedAccess;
        private readonly string[] _rank = new string[] { "SimpleVariable", "Array" };
        private string _selectedRank;
        private int _arraySizeValue;
        public ViewModel()
        {
            _objectName = "Pertinax";
            Objects = new ObservableCollection<OpcObject>();
            _selectedBasicType = _basicTypes[0];
            _selectedAccess = _access[0];
            _selectedRank = _rank[0];
            _arraySizeValue = 1;
        }
        public string ObjectName
        {
            get { return _objectName; }
            set { _objectName = value; OnPropertyChanged("ObjectName"); }
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
            set { _selectedAccess = value; OnPropertyChanged("SelectedAccess"); }
        }

        public string[] Rank
        {
            get { return _rank; }
        }

        public string SelectedRank
        {
            get { return _selectedRank; }
            set { _selectedRank = value;OnPropertyChanged("SelectedRank"); }
        }

        public int ArraySizeValue
        {
            get { return _arraySizeValue; }
            set { _arraySizeValue = value; OnPropertyChanged("ArraySizeValue"); }
        }
        public ObservableCollection<OpcObject> Objects { get; }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void AddObject()
        {
            OpcObject oo = new OpcObject(_objectName);
            Objects.Add(oo);
        }
    }
}
