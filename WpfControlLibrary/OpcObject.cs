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
        private const int IndexBase = 1;
        private readonly ObservableCollection<OpcObjectItem> _items = new ObservableCollection<OpcObjectItem>();
        private string _name;
        private OpcObjectItem _selectedItem;

        private static HashSet<string> _objectNames = new HashSet<string>();
        private HashSet<string> _itemNames = new HashSet<string>();

        public OpcObject(bool serverObject, bool clientObject, bool publisherObject, bool subscriberObject, bool isImported)
        {
            ServerObject = serverObject;
            ClientObject = clientObject;
            PublisherObject = publisherObject;
            SubscriberObject = subscriberObject;
            IsImported = isImported;
            Name = GetNewName(_objectNames, DefaultName);
            Id = NodeId.GetNextObjectNumericId();
            Namespace = 0;
        }
        public OpcObject(string name, bool serverObject, bool clientObject, bool publisherObject, bool subscriberObject, bool isImported, ushort ns, string nodeId)
        {
            Name = name;
            _objectNames.Add(name);
            Namespace = ns;
            Id = nodeId;
            NodeId.AddId(nodeId);

            ServerObject = serverObject;
            ClientObject = clientObject;
            PublisherObject = publisherObject;
            SubscriberObject = subscriberObject;
            IsImported = isImported;
            Id = NodeId.GetNextObjectNumericId();
        }

        public ushort Namespace { get; }
        public string Id { get; }
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

        public OpcObjectItem AddItem()
        {
            string itemName = GetNewName(_itemNames, DefaultItemName);
            string nodeId = NodeId.GetNextNumericId();
            OpcObjectItem ooi = new OpcObjectItem(itemName,0,nodeId);
            _items.Add(ooi);
            return ooi;
        }
        public void AddItem(OpcObjectItem ooi)
        {
            _itemNames.Add(ooi.Name);
            NodeId.AddId(ooi.Id);
            _items.Add(ooi);
        }

        public void Clear()
        {
            _items.Clear();
        }

        private string GetNewName(HashSet<string> names, string defaultName)
        {
            SortedSet<int> idxs = new SortedSet<int>();
            foreach (string oname in names)
            {
                int index = oname.IndexOf(defaultName);
                if (index >= 0)
                {
                    string es = oname.Remove(index, defaultName.Length);
                    if (int.TryParse(es, out int idx))
                    {
                        idxs.Add(idx);
                    }
                }
            }

            int newIndex = IndexBase;
            foreach (int i in idxs)
            {
                if (newIndex != i)
                {
                    break;
                }

                ++newIndex;
            }

            string newName = $"{defaultName}{newIndex}";
            names.Add(newName);
            return newName;
        }
    }
}
