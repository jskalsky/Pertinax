using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public class OpcObject : INotifyPropertyChanged
    {
        private const string DefaultName = "Pertinax";
        private const string DefaultItemName = "Var";
        private readonly ObservableCollection<OpcObjectItem> _items = new ObservableCollection<OpcObjectItem>();
        private string _name;
        private OpcObjectItem _selectedItem;
        private static int _nextDefaultNameIndex = 1;

        public OpcObject(string name, bool serverObject, bool clientObject, bool publisherObject, bool subscriberObject, bool isImported,
            string nodeId)
        {
            Name = name;
            int index = GetDefaultIndex(name, DefaultName);
            if (index > 0 && index >= _nextDefaultNameIndex)
            {
                _nextDefaultNameIndex = index + 1;
            }

            ServerObject = serverObject;
            ClientObject = clientObject;
            PublisherObject = publisherObject;
            SubscriberObject = subscriberObject;
            IsImported = isImported;
            Id = nodeId;
        }

        public OpcObject(bool serverObject, bool clientObject, bool publisherObject, bool subscriberObject, bool isImported) :
            this(string.Empty, serverObject, clientObject, publisherObject, subscriberObject, isImported, string.Empty)
        {
            Name = $"{DefaultName}{_nextDefaultNameIndex++}";
            Id = NodeId.GetNextNumericId();
        }

        public string Id { get; }
        public int NextItemIndex { get; set; } = 1;
        public bool ServerObject { get; private set; }
        public bool ClientObject { get; private set; }
        public bool PublisherObject { get; private set; }
        public bool SubscriberObject { get; private set; }
        public bool IsImported { get; private set; }
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }

        public OpcObjectItem SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; OnPropertyChanged("SelectedItem"); }
        }
        public ObservableCollection<OpcObjectItem> Items => _items;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public OpcObjectItem AddItem(string id = "")
        {
            return AddItem($"{DefaultItemName}{NextItemIndex}", id);
        }
        public OpcObjectItem AddItem(string name, string id = "")
        {
            int index = GetDefaultIndex(name, DefaultItemName);
            if (index > 0 && index >= NextItemIndex)
            {
                NextItemIndex = index + 1;
            }
            OpcObjectItem ooi = new OpcObjectItem(name, PublisherObject, id);
            _items.Add(ooi);
            return ooi;
        }

        public void AddItem(OpcObjectItem ooi)
        {
            int index = GetDefaultIndex(ooi.Name, DefaultItemName);
            if (index > 0 && index >= NextItemIndex)
            {
                NextItemIndex = index + 1;
            }
            _items.Add(ooi);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public string GetDefaultName()
        {
            return $"{DefaultName}{_nextDefaultNameIndex++}";
        }

        public int GetDefaultIndex(string name, string defaultName)
        {
            LinkedList<char> ll = new LinkedList<char>();
            string text = string.Empty;
            for (int i = name.Length - 1; i >= 0; --i)
            {
                if (char.IsDigit(name[i]))
                {
                    ll.AddFirst(name[i]);
                }
                else
                {
                    text = name.Substring(0, i + 1);
                    break;
                }
            }
            if (ll.Count != 0)
            {
                if (string.IsNullOrEmpty(text))
                {
                    return -1;
                }
                if (text == defaultName)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (char ch in ll)
                    {
                        sb.Append(ch);
                    }
                    return int.Parse(sb.ToString());
                }
            }
            return -1;
        }
    }
}
