using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigOpcUaNet
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
        private int _repetitionRateValue;
        private OpcObject _selectedOpcObject;
        private string _itemName;

        private int _nextItemIndex;
        public ViewModel()
        {
            _objectName = "Pertinax";
            Objects = new ObservableCollection<OpcObject>();
            _selectedBasicType = _basicTypes[0];
            _selectedAccess = _access[0];
            _selectedRank = _rank[0];
            _nextItemIndex = GetMaxItemIndex() + 1;
            ItemName = $"Item{_nextItemIndex}";
        }

        private int GetMaxItemIndex()
        {
            int maxIndex = 0;
            foreach(OpcObject oo in Objects)
            {
                foreach(OpcObjectItem ooi in oo.Items)
                {
                    int i = ooi.Name.IndexOf("Item");
                    if(i==0)
                    {
                        string index = ooi.Name.Remove(0, 4);
                        if(index.Length > 0)
                        {
                            if(int.TryParse(index,out int result))
                            {
                                if(result > maxIndex)
                                {
                                    maxIndex = result;
                                }
                            }
                        }
                    }
                }
            }
            return maxIndex;
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

        public int RepetitionRateValue
        {
            get { return _repetitionRateValue; }
            set { _repetitionRateValue = value; OnPropertyChanged("RepetitionRateValue"); }
        }

        public OpcObject SelectedOpcObject
        {
            get { return _selectedOpcObject; }
            set { _selectedOpcObject = value; OnPropertyChanged("SelectedOpcObject"); }
        }

        public string ItemName
        {
            get { return _itemName; }
            set { _itemName = value;OnPropertyChanged("ItemName"); }
        }

        public int NextItemIndex
        {
            get { return _nextItemIndex; }
            set { _nextItemIndex = value; }
        }
        public ObservableCollection<OpcObject> Objects { get; private set; }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void Open()
        {
            ArraySizeValue = 1;
            RepetitionRateValue = 1;
        }
        public OpcObject AddObject()
        {
            foreach(OpcObject opcObject in Objects)
            {
                if(opcObject.Name == _objectName)
                {
                    return null;
                }
            }
            OpcObject oo = new OpcObject(_objectName);
            Objects.Add(oo);
            return oo;
        }
    }
}
