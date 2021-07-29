using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Diagnostics;
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
        private string _serverIpAddress;
        private Brush _ipForeground;
        private Brush _ipBackground;
        private Model.Client _client;
        private Model.TreeViewItem _root;
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
            _root = null;
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
                case "Root":
                    Root = _client.Root;
                    Debug.Print($"Root children= {Root.Children.Count}, {Root.Name}");
                    foreach(Model.TreeViewItem tvi in Root.Children)
                    {
                        Debug.Print($"    {tvi.Name}");
                    }
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

        public Model.TreeViewItem Root
        {
            get { return _root; }
            set { _root = value; RaisePropertyChanged(); }
        }
        public RelayCommand SettingsCommand => _settings ?? (_settings = new RelayCommand(SettingsDialog));
        private void SettingsDialog()
        {
            View.Setup settings = new View.Setup();
            if (settings.ShowDialog() == true)
            {

            }
        }
    }
}