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
        private string _nodeIdString;
        private NodeIdBase _lastNodeId = null;

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
            NodeIdString = NodeId.NodeIdString;
        }
        public OpcObject(string name, bool serverObject, bool clientObject, bool publisherObject, bool subscriberObject, bool isImported, string nodeId)
        {
            Name = name;
            _objectNames.Add(name);
            NodeId = NodeIdBase.GetNodeIdBase(nodeId);
            NodeIdString = NodeId.NodeIdString;
            ServerObject = serverObject;
            ClientObject = clientObject;
            PublisherObject = publisherObject;
            SubscriberObject = subscriberObject;
            IsImported = isImported;
        }

        public NodeIdBase NodeId { get; private set; }
        public string NodeIdString
        {
            get { return _nodeIdString; }
            set
            {
                _nodeIdString = value;
                OnPropertyChanged("NodeIdString");
            }
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

        public static string ValidateNodeId(string nodeId)
        {
            Debug.Print($"ValidateNodeId {nodeId}");
            int status = 0;
            string error = string.Empty;
            StringBuilder sbNs = new StringBuilder();
            StringBuilder sbId = new StringBuilder();
            foreach (char ch in nodeId)
            {
                switch (status)
                {
                    case 0:
                        if (char.IsDigit(ch))
                        {
                            sbNs.Append(ch);
                        }
                        else
                        {
                            if (ch == ' ')
                            {
                                status = 1;
                            }
                            else
                            {
                                if (ch == ':')
                                {
                                    status = 2;
                                }
                                else
                                {
                                    error = $"Chybný formát Id uzlu";
                                    return error;
                                }
                            }
                        }
                        break;
                    case 1:
                        if (ch == ' ')
                        {
                            break;
                        }
                        else
                        {
                            if (ch == ':')
                            {
                                status = 2;
                            }
                            else
                            {
                                if (char.IsLetterOrDigit(ch) || ch == '_')
                                {
                                    sbId.Append(ch);
                                    status = 3;
                                }
                                else
                                {
                                    error = $"Chybný formát Id uzlu";
                                    return error;
                                }
                            }
                        }
                        break;
                    case 2:
                        if (ch == ' ')
                        {
                            break;
                        }
                        else
                        {
                            if (char.IsLetterOrDigit(ch) || ch == '_')
                            {
                                sbId.Append(ch);
                                status = 3;
                            }
                        }
                        break;
                    case 3:
                        if (char.IsLetterOrDigit(ch) || ch == '_')
                        {
                            sbId.Append(ch);
                        }
                        else
                        {
                            error = $"Chybný formát Id uzlu";
                            return error;
                        }
                        break;
                }
            }

            string ns = sbNs.ToString();
            string id = sbId.ToString();
            if (string.IsNullOrEmpty(ns) || string.IsNullOrEmpty(id))
            {
                error = $"Chybný formát Id uzlu";
                return error;
            }

            string ni = $"{ns}:{id}";
            Debug.Print($"ni= {ni}");
            if (NodeIdBase.ExistsNodeId(ni))
            {
                error = $"Identifikátor uzlu {ni} již existuje";
            }

            return error;
        }
        private string Validate(string propertyName)
        {
            string error = string.Empty;
            switch (propertyName)
            {
                case "NodeIdString":
                    if (_lastNodeId != null && _lastNodeId.NodeIdString == NodeIdString)
                    {
                        return error;
                    }

                    error = ValidateNodeId(NodeIdString);
                    if (!string.IsNullOrEmpty(error))
                    {
                        MainViewModel.IsError = true;
                        return error;
                    }

                    if (_lastNodeId != null)
                    {
                        NodeIdBase.Remove(_lastNodeId);
                    }
                    _lastNodeId = NodeId;
                    NodeId = NodeIdBase.GetNodeIdBase(NodeIdString);
                    MainViewModel.IsError = false;
                    break;
            }

            return error;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public OpcObjectItem AddItem()
        {
            string itemName = GetNewName(_itemNames, DefaultItemName);
            OpcObjectItem ooi = new OpcObjectItem(itemName, this);
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
