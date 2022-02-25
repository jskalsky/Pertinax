using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace OpcUaExplorer.Model
{
    public class Client : INotifyPropertyChanged
    {
        private const ushort DefaultNamespace = 0;
        private const uint RootNode = 85;

        private DispatcherTimer _timer;
        private bool _connected;
        private bool _browse;
        private bool _read;
        private ObservableCollection<TreeViewItem> _addressSpace;
        private ObservableCollection<TreeViewItem> _readItems;
        private object _lock = new object();

        private uint _connectionError;
        private uint _connectionOk;
        private uint _readError;
        private uint _readOk;
        public Client(string ip)
        {
            ServerIpAddress = ip;
            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 10);
            Connected = false;
            _browse = true;
            _read = false;
            _addressSpace = new ObservableCollection<TreeViewItem>();
            _readItems = new ObservableCollection<TreeViewItem>();
            ConnectionError = 0;
            ConnectionOk = 0;
            ReadError = 0;
            ReadOk = 0;
        }

        string ServerIpAddress { get; }
        public bool Connected
        {
            get { return _connected; }
            set { _connected = value; OnPropertyChanged("Connected"); }
        }

        public ObservableCollection<TreeViewItem> AddressSpace
        {
            get { return _addressSpace; }
        }

        public uint ConnectionError
        {
            get { return _connectionError; }
            set { _connectionError = value; OnPropertyChanged("ConnectionError"); }
        }
        public uint ConnectionOk
        {
            get { return _connectionOk; }
            set { _connectionOk = value; OnPropertyChanged("ConnectionOk"); }
        }
        public uint ReadError
        {
            get { return _readError; }
            set { _readError = value; OnPropertyChanged("ReadError"); }
        }
        public uint ReadOk
        {
            get { return _readOk; }
            set { _readOk = value; OnPropertyChanged("ReadOk"); }
        }

        public ObservableCollection<TreeViewItem> ReadItems
        {
            get { return _readItems; }
            set { _readItems = value; OnPropertyChanged("ReadItems"); }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            string ip = $"opc.tcp://{ServerIpAddress}:4840";
            //            Debug.Print($"Tick {ip}");
            int result = OpcUa.Connect(ip);
            Debug.Print($"Connect {ip} result= {result:X}");
            Connected = (result != 0) ? false : true;
            if (!Connected)
            {
                _browse = true;
                _read = false;
                ++ConnectionError;
            }
            else
            {
                ++ConnectionOk;
                if (_browse)
                {
                    AddressSpace.Clear();
                    BrowseItem[] items = OpcUa.Browse(DefaultNamespace, RootNode);
                    //                    Debug.Print($"items= {items}");
                    foreach (BrowseItem bi in items)
                    {
                        if (bi.NodeClass == NodeClass.Object || bi.NodeClass == NodeClass.Variable)
                        {
                            //                            Debug.Print($"browse= {bi.BrowseName}, {bi.DisplayName}");
                            string name = bi.BrowseName;
                            if (name == string.Empty)
                            {
                                name = bi.DisplayName;
                            }
                            if (name == string.Empty)
                            {
                                name = "Unknown";
                            }
                            TreeViewItem tvi = new TreeViewItem(name);
                            tvi.Tag = bi;
                            _addressSpace.Add(tvi);
                            if(bi.NodeClass != NodeClass.Variable)
                            {
                                BrowseNode(tvi, bi);
                            }
                        }
                    }
                    ReadItems.Clear();
                    foreach(string name in Properties.Settings.Default.Variables)
                    {
                        Debug.Print($"Hledam {name}");
                        foreach (TreeViewItem item in _addressSpace)
                        {
                            FindReadItems(item, name);
                        }
                    }
                    _browse = false;
                    _read = true;
                    OnPropertyChanged("AddressSpace");
                    OnPropertyChanged("ReadItems");
                }
                if (_read)
                {
                    lock (_lock)
                    {
                        List<uint> ids = new List<uint>();
                        foreach (TreeViewItem tvi in ReadItems)
                        {
                            BrowseItem bi = tvi.Tag as BrowseItem;
                            if(bi != null)
                            {
                                if (bi.DisplayName == "Z1xx")
                                {
                                    foreach(TreeViewItem child in tvi.Children)
                                    {
                                        BrowseItem biChild = child.Tag as BrowseItem;
                                        if(biChild != null)
                                        {
                                            if(biChild.NodeClass == NodeClass.Object)
                                            {
                                                Debug.Print($"Skupina {child.Children.Count}");
                                                foreach(TreeViewItem objectChild in child.Children)
                                                {
                                                    BrowseItem biObjectChild = objectChild.Tag as BrowseItem;
                                                    if(biObjectChild.NodeClass == NodeClass.Variable)
                                                    {
                                                        Debug.Print($"name= {biObjectChild.Numeric}, {biObjectChild.DisplayName}");
                                                        ids.Add(biObjectChild.Numeric);
                                                        if(ids.Count > 400)
                                                        {
                                                            Debug.Print($"1 count= {ids.Count}, {DateTime.Now.TimeOfDay}");
                                                            OpcUa.ServiceReadItems(0, ids.ToArray());
                                                            ids.Clear();
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if(biChild.NodeClass == NodeClass.Variable)
                                                {
                                                    ids.Add(biChild.Numeric);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if(ids.Count != 0)
                        {
                            Debug.Print($"2 count= {ids.Count}, {DateTime.Now.TimeOfDay}");
                            OpcUa.ServiceReadItems(0, ids.ToArray());
                            ids.Clear();
                        }
                    }
                }
            }
        }

        private void BrowseNode(TreeViewItem parent, BrowseItem node)
        {
            //            Debug.Print($"parent= {parent.Name}, {node.NodeIdType}");
            if (node.NodeIdType == NodeIdType.Numeric && node.NodeClass != NodeClass.Variable)
            {
                //                Debug.Print($"BR {node.NamespaceIndex}, {node.Numeric}");
                BrowseItem[] items = OpcUa.Browse(node.NamespaceIndex, node.Numeric);
                //               foreach(BrowseItem bit in items)
                //               {
                //                   Debug.Print($"  bit= {bit.BrowseName}, {bit.DisplayName}, {bit.NamespaceIndex}, {bit.NodeClass}, {bit.NodeIdType}, {bit.Numeric}");
                //               }
                foreach (BrowseItem bi in items)
                {
                    string name = bi.BrowseName;
                    if (name == string.Empty)
                    {
                        name = bi.DisplayName;
                    }
                    if (name == string.Empty)
                    {
                        name = "Unknown";
                    }
                    TreeViewItem child = parent.AddChild(name, bi);
                    BrowseNode(child, bi);
                }
            }
        }

        private void FindReadItems(TreeViewItem parent, string name)
        {
            Debug.Print($"FindReadItems {parent.Name}, {name}");
            if(parent.Name == name)
            {
                ReadItems.Add(parent);
                Debug.Print($"Pridavam {parent.Name}");
                return;
            }
            foreach (TreeViewItem child in parent.Children)
            {
                FindReadItems(child, name);
            }
        }
        public int Open(int security)
        {
            int result = OpcUa.OpenClient(security, 1460, 1460);
            if (result != 0)
            {
                return result;
            }
            _timer.Start();
            return 0;
        }

        private void DecodeData(byte[] buffer, int type, int arrayLength)
        {
            int count = (arrayLength == -1) ? 1 : arrayLength;
            int bufferOffset = 0;
            for (int i = 0; i < count; ++i)
            {
                switch (type)
                {
                    case 9:
                        float valFloat = BitConverter.ToSingle(buffer, bufferOffset);
                        Debug.Print($"     {valFloat}");
                        bufferOffset += 4;
                        break;
                    case 0:
                        bool valBool = (buffer[bufferOffset] != 0) ? true : false;
                        Debug.Print($"     {valBool}");
                        bufferOffset += 1;
                        break;
                }
            }
        }
    }
}
