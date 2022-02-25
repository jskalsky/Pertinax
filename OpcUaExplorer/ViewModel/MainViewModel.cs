using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpcUaExplorer.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private RelayCommand _settings;
        private RelayCommand _treeContextMenuRead;
        private RelayCommand<RoutedPropertyChangedEventArgs<object>> _addressSpaceSelectionChanged;
        private RelayCommand<SelectionChangedEventArgs> _serverSelectionChanged;
        private Brush _ipForeground;
        private Brush _ipBackground;
        private Model.Client _client;
        private ObservableCollection<Model.TreeViewItem> _addressSpace;
        private Model.TreeViewItem _selectedTreeItem;
        private List<string> _servers;
        private string _selectedServer;
        private ObservableCollection<Model.TreeViewItem> _readItems;

        private uint _connectionError;
        private uint _connectionOk;
        private uint _readError;
        private uint _readOk;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ConfigurationManager.RefreshSection("OpcUaExplorer.Properties.Settings");
            _servers = new List<string>();
            foreach (string server in Properties.Settings.Default.Servers)
            {
                _servers.Add(server);
            }
            RaisePropertyChanged("Servers");
            SelectedServer = Properties.Settings.Default.SelectedServer;

            IpBackground = new SolidColorBrush(Colors.Red);
            IpForeground = new SolidColorBrush(Colors.White);
            _client = new Model.Client(SelectedServer);
            _client.PropertyChanged += _client_PropertyChanged;
            _client.Open(0);
            ReadItems = new ObservableCollection<Model.TreeViewItem>();
//            if(Properties.Settings.Default.Variables == null)
//            {
//                Properties.Settings.Default.Variables = new System.Collections.Specialized.StringCollection();
//            }
            ConnectionError = 0;
            ConnectionOk = 0;
            ReadError = 0;
            ReadOk = 0;
        }

        private void _client_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Connected":
                    if (_client.Connected)
                    {
                        IpBackground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        IpBackground = new SolidColorBrush(Colors.Red);
                    }
                    break;
                case "AddressSpace":
                    AddressSpace = _client.AddressSpace;
                    break;
                case "ReadItems":
                    ReadItems = _client.ReadItems;
                    break;
                case "ConnectionOk":
                    ConnectionOk = _client.ConnectionOk;
                    break;
                case "ConnectionError":
                    ConnectionError = _client.ConnectionError;
                    break;
                case "ReadOk":
                    ReadOk = _client.ReadOk;
                    break;
                case "ReadError":
                    ReadError = _client.ReadError;
                    break;
            }
        }

        public List<string> Servers
        {
            get { return _servers; }
        }

        public string SelectedServer
        {
            get { return _selectedServer; }
            set { _selectedServer = value; RaisePropertyChanged(); }
        }
        public Brush IpForeground
        {
            get { return _ipForeground; }
            set { _ipForeground = value; RaisePropertyChanged(); }
        }
        public Brush IpBackground
        {
            get { return _ipBackground; }
            set { _ipBackground = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<Model.TreeViewItem> AddressSpace
        {
            get { return _addressSpace; }
            set { _addressSpace = value; RaisePropertyChanged(); }
        }

        public Model.TreeViewItem SelectedTreeItem
        {
            get { return _selectedTreeItem; }
            set { _selectedTreeItem = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<Model.TreeViewItem> ReadItems
        {
            get { return _readItems; }
            set { _readItems = value;RaisePropertyChanged(); }
        }
        public uint ConnectionError
        {
            get { return _connectionError; }
            set { _connectionError = value; RaisePropertyChanged(); }
        }
        public uint ConnectionOk
        {
            get { return _connectionOk; }
            set { _connectionOk = value; RaisePropertyChanged(); }
        }
        public uint ReadError
        {
            get { return _readError; }
            set { _readError = value; RaisePropertyChanged(); }
        }
        public uint ReadOk
        {
            get { return _readOk; }
            set { _readOk = value; RaisePropertyChanged(); }
        }


        public RelayCommand SettingsCommand => _settings ?? (_settings = new RelayCommand(SettingsDialog));
        private void SettingsDialog()
        {
            View.Setup settings = new View.Setup();
            if (settings.ShowDialog() == true)
            {
                Properties.Settings.Default.Servers.Clear();
                SetupVm svm = ServiceLocator.Current.GetInstance<SetupVm>();
                foreach (IPAddress ip in svm.Ips)
                {
                    Properties.Settings.Default.Servers.Add(ip.ToString());
                }
            }
        }

        public RelayCommand<RoutedPropertyChangedEventArgs<object>> OnAddressSpaceSelectionChanged => _addressSpaceSelectionChanged ??
          (_addressSpaceSelectionChanged = new RelayCommand<RoutedPropertyChangedEventArgs<object>>((args) => AddressSpaceSelectionChanged(args)));
        public void AddressSpaceSelectionChanged(RoutedPropertyChangedEventArgs<object> args)
        {
            Debug.Print($"Selected {args.NewValue}");
            SelectedTreeItem = (Model.TreeViewItem)args.NewValue;
        }

        public RelayCommand<SelectionChangedEventArgs> OnServerSelectionChanged => _serverSelectionChanged ?? (_serverSelectionChanged = new RelayCommand<SelectionChangedEventArgs>(
                                                                              (args) => ServerSelectionChanged(args)));

        public void ServerSelectionChanged(SelectionChangedEventArgs args)
        {
            Debug.Print($"ServerSelectionChanged {args.AddedItems.Count}");
            if (args.AddedItems.Count != 0)
            {
                _selectedServer = (string)args.AddedItems[0];
//                Properties.Settings.Default.SelectedServer = _selectedServer;
            }
        }
        public RelayCommand TreeContextMenuReadCommand => _treeContextMenuRead ?? (_treeContextMenuRead = new RelayCommand(TreeContextMenuRead));
        private void TreeContextMenuRead()
        {
            Debug.Print($"TreeContextMenuRead {SelectedTreeItem}");
            if(SelectedTreeItem != null)
            {
                Debug.Print($"NodeClass= {SelectedTreeItem.Tag.NodeClass}");
                if(SelectedTreeItem.Tag.NodeClass == Model.NodeClass.Variable || SelectedTreeItem.Tag.NodeClass == Model.NodeClass.Object)
                {
                    Debug.Print($"Ukladam {SelectedTreeItem.Tag.DisplayName}");
                    bool found = false;
                    foreach(string dn in Properties.Settings.Default.Variables)
                    {
                        if(dn == SelectedTreeItem.Tag.DisplayName)
                        {
                            found = true;
                            break;
                        }
                    }
                    if(!found)
                    {
                        Properties.Settings.Default.Variables.Add(SelectedTreeItem.Tag.DisplayName);
                        Properties.Settings.Default.Save();
                    }
                }
            }
        }
    }
}