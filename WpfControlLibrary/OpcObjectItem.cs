using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

        private string _id;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public OpcObjectItem(string name, bool publish, string id = "")
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
            EnableWriteOutside = !publish;
            RankTag = this;
            Id = id;
        }

        public string Id
        {
            get { return _id;}
            set { _id = value;OnPropertyChanged("Id"); }
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
    }
}
