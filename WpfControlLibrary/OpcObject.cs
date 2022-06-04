using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfControlLibrary.DataModel;

namespace WpfControlLibrary
{
    public class OpcObject : INotifyPropertyChanged, IDataErrorInfo
    {
        private const string DefaultName = "Pertinax";
        private const string DefaultItemName = "Var";
        private const int IndexBase = 1;
        private readonly ObservableCollection<OpcObjectItem> _items = new ObservableCollection<OpcObjectItem>();
        private string _name;
        private string _lastName = string.Empty;
        private OpcObjectItem _selectedItem;
        private string _nodeIdString;
        private NodeIdBase _lastNodeId = null;

        public static Dictionary<string, int> _objectNames = new Dictionary<string, int>();
        public Dictionary<string, int> _itemNames = new Dictionary<string, int>();

        public OpcObject(bool serverObject, bool clientObject, bool publisherObject, bool subscriberObject, bool isImported)
        {
            Debug.Print($"OpcObject Constr");
            ServerObject = serverObject;
            ClientObject = clientObject;
            PublisherObject = publisherObject;
            SubscriberObject = subscriberObject;
            IsImported = isImported;
            string name = GetNewName(_objectNames, DefaultName);
            AddName(_objectNames, name);
            Name = name;
            NodeId = NodeIdNumeric.GetNextNodeIdNumeric(0);
            NodeIdString = NodeId.NodeIdString;
        }
        public OpcObject(string name, bool serverObject, bool clientObject, bool publisherObject, bool subscriberObject, bool isImported, string nodeId)
        {
            string n = name;
            AddName(_objectNames, n);
            Name = name;
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
                                if (char.IsLetterOrDigit(ch) || ch == '_' || ch == '.')
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
                            if (char.IsLetterOrDigit(ch) || ch == '_' || ch == '.')
                            {
                                sbId.Append(ch);
                                status = 3;
                            }
                        }
                        break;
                    case 3:
                        if (char.IsLetterOrDigit(ch) || ch == '_' || ch == '.')
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
                    Debug.Print($"Validate Object NodeIdString= {NodeIdString}, _nodeIdString= {_nodeIdString}");
                    if (_lastNodeId != null)
                    {
                        Debug.Print($"_lastNodeId= {_lastNodeId.NodeIdString}, {_lastNodeId.Guid}");
                    }
                    if (_lastNodeId != null && NodeIdString == _lastNodeId.NodeIdString)
                    {
                        Debug.Print($"==");
                        return error;
                    }

                    error = ValidateNodeId(NodeIdString);
                    if (error.Contains("formát"))
                    {
                        return error;
                    }
                    if (_lastNodeId != null)
                    {
                        NodeIdBase.Remove(_lastNodeId);
                    }
                    NodeId = NodeIdBase.GetNodeIdBase(NodeIdString);
                    _lastNodeId = NodeId;
                    Debug.Print($"Novy _lastNodeId= {_lastNodeId.NodeIdString}");
                    NodeIdBase.Add(NodeId);
                    NodeIdBase.PrintIds();
                    Debug.Print($"Validate result= {MainViewModel.IsError}, error= {error}");
                    int nr = NodeIdBase.GetNrOfErrors();
                    if (nr == 0)
                    {
                        foreach (OpcObject oo in MainViewModel.GetObjects())
                        {
                            oo.NodeIdString = oo.NodeIdString;
                        }
                        MainViewModel.IsError = false;
                    }
                    else
                    {
                        MainViewModel.IsError = true;
                    }
                    break;
                case "Name":
                    Debug.Print($"Validate Object Name {Name}, _lastName= {_lastName}");
                    if (string.IsNullOrEmpty(_lastName))
                    {
                        _lastName = Name;
                        Debug.Print($"Empty");
                        return error;
                    }
                    if (_lastName == Name)
                    {
                        Debug.Print($"== {Name}, {_lastName}");
                        return error;
                    }

                    DecrementNames(_objectNames, _lastName);
                    _lastName = Name;
                    if (AddName(_objectNames, Name))
                    {
                        error = $"Jméno proměnné {Name} již existuje";
                    }
                    else
                    {
                        if (GetNrOfNamesErrors(_objectNames) == 0)
                        {
                            foreach (OpcObject oo in MainViewModel.GetObjects())
                            {
                                oo.Name = oo.Name;
                            }
                        }
                    }
                    MainViewModel.IsError = (!string.IsNullOrEmpty(error));
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
            AddName(_itemNames, ooi.Name);
            _items.Add(ooi);
        }

        public void Clear()
        {
            _items.Clear();
        }

        private string GetNewName(Dictionary<string, int> names, string defaultName)
        {
            SortedSet<int> idxs = new SortedSet<int>();
            foreach (string oname in names.Keys)
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
            return newName;
        }

        public bool AddName(Dictionary<string, int> names, string name)
        {
            Debug.Print($"AddName {name}");
            foreach (KeyValuePair<string, int> pair in names)
            {
                Debug.Print($"A {pair.Key}-{pair.Value}");
            }
            if (names.TryGetValue(name, out int nr))
            {
                names.Remove(name);
                names[name] = nr + 1;
                foreach (KeyValuePair<string, int> pair in names)
                {
                    Debug.Print($"A1 {pair.Key}-{pair.Value}");
                }
                return true;
            }

            names[name] = 1;
            foreach (KeyValuePair<string, int> pair in names)
            {
                Debug.Print($"A2 {pair.Key}-{pair.Value}");
            }
            return false;
        }

        public bool DecrementNames(Dictionary<string, int> names, string name)
        {
            Debug.Print($"DecrementNames {name}");
            foreach (KeyValuePair<string, int> pair in names)
            {
                Debug.Print($"D {pair.Key}-{pair.Value}");
            }
            if (names.TryGetValue(name, out int nr))
            {
                names.Remove(name);
                if (nr - 1 > 0)
                {
                    names[name] = nr - 1;
                }
                foreach (KeyValuePair<string, int> pair in names)
                {
                    Debug.Print($"D1 {pair.Key}-{pair.Value}");
                }

                return true;
            }
            foreach (KeyValuePair<string, int> pair in names)
            {
                Debug.Print($"D2 {pair.Key}-{pair.Value}");
            }

            return false;
        }

        public int GetNrOfNamesErrors(Dictionary<string, int> names)
        {
            int nr = 0;
            foreach (KeyValuePair<string, int> pair in names)
            {
                if (pair.Value > 1)
                {
                    ++nr;
                }
            }

            return nr;
        }
    }
}
