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
        private bool _subscribe;
        private bool _subscribeClick;
        private OpcObjectItem _setupObjectItem;
        private string _selectedSetupItem;
        private string _selectedSetupRank;
        private int _selectedSetupLength;

        private bool _enableAddToPublisher;
        private bool _enableAddToSubscriber;
        private bool _enableAddToClient;
        private bool _enableSetupLength;

        private int _nextItemIndex;
        public MainViewModel()
        {

            Objects = new ObservableCollection<OpcObject>();
            SubscriberObjects = new ObservableCollection<SubscriberItem>();
            PublisherObjects = new ObservableCollection<PublisherItem>();
            ClientObjects = new ObservableCollection<ClientItem>();
            ServerObjects = new ObservableCollection<ServerItem>();
            _nextItemIndex = GetMaxItemIndex() + 1;
            ItemName = $"Item{_nextItemIndex}";
            GroupAddressString = "224.0.0.22";
            LocalIpAddressString = "10.10.13.253";
            WindowTitle = "OpcUa";
            RepetitionRateValue = 1;
            _setupObjectItem = new OpcObjectItem("Setup", false);
            SelectedSetupItem = _setupObjectItem.BasicTypes[0];
            SelectedSetupRank = _setupObjectItem.Rank[0];
            SelectedSetupLength = 0;
            EnableSetupLength = false;
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

        public OpcObjectItem SetupObjectItem
        {
            get { return _setupObjectItem; }
        }

        public string SelectedSetupItem
        {
            get { return _selectedSetupItem; }
            set { _selectedSetupItem = value; OnPropertyChanged("SelectedSetupItem"); }
        }

        public string SelectedSetupRank
        {
            get { return _selectedSetupRank; }
            set { _selectedSetupRank = value; OnPropertyChanged("SelectedSetupRank"); }
        }

        public int SelectedSetupLength
        {
            get { return _selectedSetupLength; }
            set { _selectedSetupLength = value; OnPropertyChanged("SelectedSetupLength"); }
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

        public bool Subscribe
        {
            get { return _subscribe; }
            set { _subscribe = value; OnPropertyChanged("Subscribe"); }
        }
        public bool SubscribeClick
        {
            get { return _subscribeClick; }
            set { _subscribeClick = value; OnPropertyChanged("SubscribeClick"); }
        }

        public bool EnableAddToPublisher
        {
            get { return _enableAddToPublisher; }
            set { _enableAddToPublisher = value; OnPropertyChanged("EnableAddToPublisher"); }
        }

        public bool EnableAddToSubscriber
        {
            get { return _enableAddToSubscriber; }
            set { _enableAddToSubscriber = value; OnPropertyChanged("EnableAddToSubscriber"); }
        }

        public bool EnableAddToClient
        {
            get { return _enableAddToClient; }
            set { _enableAddToClient = value; OnPropertyChanged("EnableAddToClient"); }
        }

        public bool EnableSetupLength
        {
            get { return _enableSetupLength; }
            set { _enableSetupLength = value; OnPropertyChanged("EnableSetupLength"); }
        }
        public ObservableCollection<OpcObject> Objects { get; private set; }
        public ObservableCollection<SubscriberItem> SubscriberObjects { get; private set; }
        public ObservableCollection<PublisherItem> PublisherObjects { get; private set; }
        public ObservableCollection<ClientItem> ClientObjects { get; private set; }
        public ObservableCollection<ServerItem> ServerObjects { get; private set; }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void AddSubscriber(string path)
        {
            SubscriberPath = path;
        }
    }
}
