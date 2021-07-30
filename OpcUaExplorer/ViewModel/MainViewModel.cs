using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
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
        private RelayCommand<RoutedPropertyChangedEventArgs<object>> _addressSpaceSelectionChanged;
        private string _serverIpAddress;
        private Brush _ipForeground;
        private Brush _ipBackground;
        private Model.Client _client;
        private ObservableCollection<Model.TreeViewItem> _addressSpace;
        private Model.BrowseItem _selectedTreeItem;
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ServerIpAddress = Properties.Settings.Default.ServerIpAddress;
            IpBackground = new SolidColorBrush(Colors.Red);
            IpForeground = new SolidColorBrush(Colors.White);
            _client = new Model.Client(ServerIpAddress);
            _client.PropertyChanged += _client_PropertyChanged;
            _client.Open(0);
        }

        private void _client_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "Connected":
                    if(_client.Connected)
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
            }
        }

        public string ServerIpAddress
        {
            get { return _serverIpAddress; }
            set { _serverIpAddress = value; RaisePropertyChanged(); }
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

        public Model.BrowseItem SelectedTreeItem
        {
            get { return _selectedTreeItem; }
            set { _selectedTreeItem = value; RaisePropertyChanged(); }
        }
        public RelayCommand SettingsCommand => _settings ?? (_settings = new RelayCommand(SettingsDialog));
        private void SettingsDialog()
        {
            View.Setup settings = new View.Setup();
            if (settings.ShowDialog() == true)
            {

            }
        }

        public RelayCommand<RoutedPropertyChangedEventArgs<object>> OnAddressSpaceSelectionChanged => _addressSpaceSelectionChanged ??
          (_addressSpaceSelectionChanged = new RelayCommand<RoutedPropertyChangedEventArgs<object>>((args) => AddressSpaceSelectionChanged(args)));
        public void AddressSpaceSelectionChanged(RoutedPropertyChangedEventArgs<object> args)
        {
            Debug.Print($"Selected {args.NewValue}");
            SelectedTreeItem = ((Model.TreeViewItem)args.NewValue).Tag;
        }
    }
}