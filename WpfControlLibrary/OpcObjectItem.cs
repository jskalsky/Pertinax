using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public class OpcObjectItem : INotifyPropertyChanged
    {
        private readonly string[] _basicTypes = new string[] { "Boolean", "UInt8", "Int8", "UInt16", "Int16", "UInt32", "Int32", "Float", "Double" };
        private string _selectedBasicType;
        private readonly string[] _access = new string[] { "Read", "Write", "ReadWrite" };
        private string _selectedAccess;
        private readonly string[] _rank = new string[] { "SimpleVariable", "Array" };
        private string _selectedRank;
        private int _arraySizeValue;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public OpcObjectItem(string name)
        {
            Name = name;
            SelectedAccess = _access[0];
            SelectedRank = _rank[0];
            ArraySizeValue = 0;
            SelectedBasicType = _basicTypes[7];
        }

        public OpcObjectItem(OpcObjectItem ooi)
        {
            Name = ooi.Name;
            SelectedAccess = ooi.SelectedAccess;
            SelectedBasicType = ooi.SelectedBasicType;
            SelectedRank = ooi.SelectedRank;
            ArraySizeValue = ooi.ArraySizeValue;
        }
        public string Name { get; }

        public string[] BasicTypes
        {
            get { return _basicTypes; }
        }

        public string SelectedBasicType
        {
            get { return _selectedBasicType; }
            set { _selectedBasicType = value; }
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
            set { _selectedRank = value; }
        }

        public int ArraySizeValue
        {
            get { return _arraySizeValue; }
            set { _arraySizeValue = value; OnPropertyChanged("ArraySizeValue"); }
        }
    }
}
