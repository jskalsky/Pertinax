using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfControlLibrary
{
    public class OpcObject : INotifyPropertyChanged, IDataErrorInfo
    {
        private const string DefaultName = "Pertinax";
        private const string DefaultItemName = "Var";
        private const int IndexBase = 1;
        private readonly ObservableCollection<OpcObjectItem> _items = new ObservableCollection<OpcObjectItem>();
        private string _name;
        private OpcObjectItem _selectedItem;
        private NodeIdBase _nodeId;

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
            NodeId = NodeIdNumeric.GetNextNodeIdNumeric(0);
        }
        public OpcObject(string name, bool serverObject, bool clientObject, bool publisherObject, bool subscriberObject, bool isImported, string nodeId)
        {
            Name = name;
            _objectNames.Add(name);
            Debug.Print($"OpcObject construktor {nodeId}");
            NodeId = NodeIdBase.GetNodeIdBase(nodeId);
            ServerObject = serverObject;
            ClientObject = clientObject;
            PublisherObject = publisherObject;
            SubscriberObject = subscriberObject;
            IsImported = isImported;
        }

        public NodeIdBase NodeId
        {
            get { return _nodeId; }
            set { _nodeId = value;OnPropertyChanged("NodeId"); Debug.Print($"Set NodeId= {value.NamespaceIndex}:{value.GetIdentifier()}"); }
        }
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

        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get { return Validate(columnName); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string Validate(string propertyNane)
        {
            Debug.Print($"Validate {propertyNane}");
            string error = string.Empty;
            switch (propertyNane)
            {
                case "NodeId":
                    if(NodeId.NamespaceIndex > 5)
                    {
                        error = $"Index jmenného prostoru mimo rozsah, 0 - 5";
                    }
/*                    else
                    {
                        string ni = $"{NodeId.NamespaceIndex}:{NodeId.GetIdentifier()}";
                        if(NodeIdBase.ExistsNodeId(ni))
                        {
                            error = $"Identifikátor uzlu {ni} již existuje";
                        }
                    }*/
                    break;
            }
            return error;
        }
        public OpcObjectItem AddItem()
        {
            string itemName = GetNewName(_itemNames, DefaultItemName);
            OpcObjectItem ooi = new OpcObjectItem(itemName);
            _items.Add(ooi);
            return ooi;
        }
        public void AddItem(OpcObjectItem ooi)
        {
            _itemNames.Add(ooi.Name);
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
