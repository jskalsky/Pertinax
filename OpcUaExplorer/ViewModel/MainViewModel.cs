using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

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
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ServerIpAddress = Properties.Settings.Default.ServerIpAddress;
        }

        public string ServerIpAddress
        {
            get { return _serverIpAddress; }
            set { _serverIpAddress = value; RaisePropertyChanged(); }
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