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
        private BrowseItem[] _readItems;
        private List<BrowseItem> _readVariables;
        private object _lock = new object();

        private uint _connectionError;
        private uint _connectionOk;
        private uint _readError;
        private uint _readOk;
        public Client(string ip)
        {
            ServerIpAddress = ip;
            _readVariables = new List<BrowseItem>();
            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 10);
            Connected = false;
            _browse = true;
            _read = false;
            _addressSpace = new ObservableCollection<TreeViewItem>();
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
            set { _connectionError = value;OnPropertyChanged("ConnectionError"); }
        }
        public uint ConnectionOk
        {
            get { return _connectionOk; }
            set { _connectionOk = value;OnPropertyChanged("ConnectionOk"); }
        }
        public uint ReadError
        {
            get { return _readError; }
            set { _readError = value;OnPropertyChanged("ReadError"); }
        }
        public uint ReadOk
        {
            get { return _readOk; }
            set { _readOk = value;OnPropertyChanged("ReadOk"); }
        }

        public BrowseItem[] ReadItems
        {
            get { return _readItems; }
            set { _readItems = value;OnPropertyChanged("ReadItems"); }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            string ip = $"opc.tcp://{ServerIpAddress}";
//            Debug.Print($"Tick {ip}");
            int result = OpcUa.Connect(ip);
//            Debug.Print($"result= {result}");
            Connected = (result != 0) ? false : true;
            if(!Connected)
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
                            BrowseNode(tvi, bi);
                        }
                    }
                    List<BrowseItem> readItems = new List<BrowseItem>();
                    foreach(TreeViewItem item in _addressSpace)
                    {
                        FindReadItems(item, readItems);
                    }
                    if(readItems.Count != 0)
                    {
                        ReadItems = readItems.ToArray();
                    }
                    _browse = false;
                    _read = true;
                    OnPropertyChanged("AddressSpace");
                }
                if(_read)
                {
                    lock(_lock)
                    {
                        Debug.Print($"{DateTime.Now.TimeOfDay} Read= {_readVariables.Count}");
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        foreach(BrowseItem bi in _readVariables)
                        {
                            int t = 0;
                            int al = 0;
                            byte[] resultBuffer = OpcUa.Read(bi.NamespaceIndex, bi.Numeric, ref t, ref al);
                            if(resultBuffer != null)
                            {
                                ++ReadOk;
//                                Debug.Print($"Read result {DateTime.Now.TimeOfDay}, {bi.DisplayName}, buf= {resultBuffer.Length}, t= {t}, al= {al}");
//                                DecodeData(resultBuffer, t, al);
                            }
                            else
                            {
                                ++ReadError;
                            }
                        }
                        sw.Stop();
                        Debug.Print($"Elapsed= {sw.Elapsed}");
                    }
                }
            }
        }

        private void BrowseNode(TreeViewItem parent, BrowseItem node)
        {
//            Debug.Print($"parent= {parent.Name}, {node.NodeIdType}");
            if(node.NodeIdType == NodeIdType.Numeric)
            {
//                Debug.Print($"BR {node.NamespaceIndex}, {node.Numeric}");
                BrowseItem[] items = OpcUa.Browse(node.NamespaceIndex, node.Numeric);
 //               foreach(BrowseItem bit in items)
 //               {
 //                   Debug.Print($"  bit= {bit.BrowseName}, {bit.DisplayName}, {bit.NamespaceIndex}, {bit.NodeClass}, {bit.NodeIdType}, {bit.Numeric}");
 //               }
                foreach(BrowseItem bi in items)
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

        private void FindReadItems(TreeViewItem parent, List<BrowseItem> items)
        {
            BrowseItem bi = parent.Tag as BrowseItem;
            if(bi != null)
            {
                foreach(string s in Properties.Settings.Default.Variables)
                {
                    if(s == bi.DisplayName)
                    {
                        items.Add(bi);
                    }
                }
            }
            foreach(TreeViewItem child in parent.Children)
            {
                FindReadItems(child, items);
            }
        }
        public int Open(int security)
        {
            int result = OpcUa.OpenClient(security);
            if (result != 0)
            {
                return result;
            }
            _timer.Start();
            return 0;
        }

        public void AddReadVariable(BrowseItem bi)
        {
            lock(_lock)
            {
                _readVariables.Add(bi);
            }
        }

        private void DecodeData(byte[] buffer, int type, int arrayLength)
        {
            int count = (arrayLength == -1) ? 1 : arrayLength;
            int bufferOffset = 0;
            for(int i=0;i<count;++i)
            {
                switch(type)
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
