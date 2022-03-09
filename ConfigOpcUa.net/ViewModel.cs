using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConfigOpcUaNet
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int _repetitionRateValue;
        private OpcObject _selectedOpcObject;
        private string _itemName;
        private IPAddress _localIpAddress;
        private IPAddress _groupAddress;
        private string _localIpAddressString;
        private string _groupAddressString;

        private int _nextItemIndex;
        public ViewModel()
        {

            Objects = new ObservableCollection<OpcObject>();
            PublisherObjects = new ObservableCollection<OpcObject>();
            SubscriberObjects = new ObservableCollection<OpcObject>();
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

        public IPAddress LocalIpAddress
        {
            get { return _localIpAddress; }
            set { _localIpAddress = value; OnPropertyChanged("LocalIpAddress"); }
        }

        public IPAddress GroupAddress
        {
            get { return _groupAddress; }
            set { _groupAddress = value; OnPropertyChanged("GroupAddress"); }
        }

        public string LocalIpAddressString
        {
            get { return _localIpAddressString; }
            set { _localIpAddressString = value;OnPropertyChanged("LocalIpAddressString"); }
        }

        public string GroupAddressString
        {
            get { return _groupAddressString; }
            set { _groupAddressString = value;OnPropertyChanged("GroupAddressString"); }
        }
        public ObservableCollection<OpcObject> Objects { get; private set; }
        public ObservableCollection<OpcObject> PublisherObjects { get; private set; }
        public ObservableCollection<OpcObject> SubscriberObjects { get; private set; }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public OpcObject AddObject(string name)
        {
            foreach(OpcObject opcObject in Objects)
            {
                if(opcObject.Name == name)
                {
                    return null;
                }
            }
            OpcObject oo = new OpcObject(name);
            Objects.Add(oo);
            return oo;
        }
    }
}
