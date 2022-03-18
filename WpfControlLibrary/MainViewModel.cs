using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int _repetitionRateValue;
        private OpcObject _selectedOpcObject;
        private string _itemName;
        private IPAddress _localIpAddress;
        private IPAddress _groupAddress;
        private string _localIpAddressString;
        private string _groupAddressString;
        private int _publisherId;
        private SubscriberItem _selectedSubscriberItem;
        private string _windowTitle;
        private string _subscriberPath;

        private int _nextItemIndex;
        public MainViewModel()
        {

            Objects = new ObservableCollection<OpcObject>();
            SubscriberObjects = new ObservableCollection<SubscriberItem>();
            _nextItemIndex = GetMaxItemIndex() + 1;
            ItemName = $"Item{_nextItemIndex}";
            GroupAddressString = "224.0.0.22";
            LocalIpAddressString = "10.10.13.253";
            WindowTitle = "Configurator OpcUa";
        }

        public int GetMaxItemIndex()
        {
            int maxIndex = 0;
            Debug.Print($"GetMaxItemIndex {Objects.Count}");
            foreach (OpcObject oo in Objects)
            {
                Debug.Print($"Items= {oo.Items.Count}");
                foreach (OpcObjectItem ooi in oo.Items)
                {
                    int i = ooi.Name.IndexOf("Item");
                    if (i == 0)
                    {
                        string index = ooi.Name.Remove(0, 4);
                        Debug.Print($"index= {index}");
                        if (index.Length > 0)
                        {
                            if (int.TryParse(index, out int result))
                            {
                                Debug.Print($"result= {result}");
                                if (result > maxIndex)
                                {
                                    maxIndex = result;
                                    Debug.Print($"maxIndex= {maxIndex}");
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
            set { _itemName = value; OnPropertyChanged("ItemName"); }
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
            set { _localIpAddressString = value; OnPropertyChanged("LocalIpAddressString"); }
        }

        public string GroupAddressString
        {
            get { return _groupAddressString; }
            set { _groupAddressString = value; OnPropertyChanged("GroupAddressString"); }
        }

        public int PublisherId
        {
            get { return _publisherId; }
            set { _publisherId = value; OnPropertyChanged("PublisherId"); }
        }

        public SubscriberItem SelectedSubscriberItem
        {
            get { return _selectedSubscriberItem; }
            set { _selectedSubscriberItem = value; OnPropertyChanged("SelectedSubscriberItem"); }
        }

        public string WindowTitle
        {
            get { return _windowTitle; }
            set { _windowTitle = value; OnPropertyChanged("WindowTitle"); }
        }

        public string SubscriberPath
        {
            get { return _subscriberPath; }
            set { _subscriberPath = value; OnPropertyChanged("SubscriberPath"); }
        }
        public ObservableCollection<OpcObject> Objects { get; private set; }
        public ObservableCollection<SubscriberItem> SubscriberObjects { get; private set; }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public OpcObject AddObject(string name, int writer, int dataSet)
        {
            foreach (OpcObject opcObject in Objects)
            {
                if (opcObject.Name == name)
                {
                    return null;
                }
            }
            OpcObject oo = new OpcObject(name, PublisherId, writer, dataSet, 0, false, false);
            Objects.Add(oo);
            return oo;
        }

        public void AddSubscriber(string path)
        {
            SubscriberPath = path;
        }
    }
}
