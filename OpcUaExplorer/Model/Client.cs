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
        private const uint RootNode = 84;

        private DispatcherTimer _timer;
        private bool _connected;
        private bool _browse;
        private ObservableCollection<TreeViewItem> _addressSpace;
        public Client(string ip)
        {
            ServerIpAddress = ip;
            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 10);
            Connected = false;
            _browse = true;
            _addressSpace = new ObservableCollection<TreeViewItem>();
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            string ip = $"opc.tcp://{ServerIpAddress}";
            Debug.Print($"Tick {ip}");
            int result = OpcUa.Connect(ip);
            Debug.Print($"result= {result}");
            Connected = (result != 0) ? false : true;
            if (Connected && _browse)
            {
                BrowseItem[] items = OpcUa.Browse(DefaultNamespace, RootNode);
                Debug.Print($"items= {items.Length}");
                foreach(BrowseItem bi in items)
                {
                    Debug.Print($"browse= {bi.BrowseName}, {bi.DisplayName}");
                    string name = bi.BrowseName;
                    if(name == string.Empty)
                    {
                        name = bi.DisplayName;
                    }
                    if(name == string.Empty)
                    {
                        name = "Unknown";
                    }
                    TreeViewItem tvi = new TreeViewItem(name);
                    tvi.Tag = bi;
                    _addressSpace.Add(tvi);
                    BrowseNode(tvi, bi);
                }
                _browse = false;
                OnPropertyChanged("AddressSpace");
            }
        }

        private void BrowseNode(TreeViewItem parent, BrowseItem node)
        {
            if(node.NodeIdType == NodeIdType.Numeric)
            {
                BrowseItem[] items = OpcUa.Browse(node.NamespaceIndex, node.Numeric);
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
    }
}
