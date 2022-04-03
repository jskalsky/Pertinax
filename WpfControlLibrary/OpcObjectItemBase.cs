using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public abstract class OpcObjectItemBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected readonly string[] _basicTypes = new string[] { "Boolean", "UInt8", "Int8", "UInt16", "Int16", "UInt32", "Int32", "Float", "Double" };
        protected string _selectedBasicType;
        protected readonly string[] _access = new string[] { "Read", "Write", "ReadWrite" };
        protected string _selectedAccess;
        protected readonly string[] _rank = new string[] { "Jednoduchá proměnná", "Pole" };
        protected string _selectedRank;
        protected int _arraySizeValue;
        protected bool _writeOutside;
        protected string _name;

        protected OpcObjectItemBase()
        {
            SelectedAccess = _access[0];
            SelectedRank = _rank[0];
            ArraySizeValue = 0;
            SelectedBasicType = _basicTypes[0];
        }
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

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }
        public bool WriteOutside
        {
            get { return _writeOutside; }
            set { _writeOutside = value; OnPropertyChanged("WriteOutside"); }
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
