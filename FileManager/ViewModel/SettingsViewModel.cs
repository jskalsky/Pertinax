using FileManager.Properties;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private string _startupFilename;
        private int _repetitiveRate;
        private bool _isTls;
        private RelayCommand _browse;

        private string _server;
        private string _selectedServer;

        private RelayCommand _add;
        public SettingsViewModel()
        {
            Servers = new ObservableCollection<string>();
            if (!string.IsNullOrEmpty(Settings.Default.Startup))
            {
                StartupFileName = Settings.Default.Startup;
            }
            RepetitiveRate = Settings.Default.RepetitiveRate;
            IsTls = Settings.Default.IsTls;
            foreach(string server in Properties.Settings.Default.Servers)
            {
                Servers.Add(server);
            }
            if(string.IsNullOrEmpty(Properties.Settings.Default.SelectedServer))
            {
                SelectedServer = Servers[0];
                Properties.Settings.Default.SelectedServer = SelectedServer;
            }
            else
            {
                SelectedServer = Properties.Settings.Default.SelectedServer;
            }
        }

        public string StartupFileName
        {
            get { return _startupFilename; }
            set { _startupFilename = value; RaisePropertyChanged(); }
        }

        public int RepetitiveRate
        {
            get { return _repetitiveRate; }
            set { _repetitiveRate = value; RaisePropertyChanged(); }
        }

        public bool IsTls
        {
            get { return _isTls; }
            set { _isTls = value;RaisePropertyChanged(); }
        }

        public string Server
        {
            get { return _server; }
            set { _server = value;RaisePropertyChanged(); }
        }

        public string SelectedServer
        {
            get { return _selectedServer; }
            set { _selectedServer = value;RaisePropertyChanged(); }
        }
        public ObservableCollection<string> Servers { get; }
        public RelayCommand BrowseCommand => _browse ?? (_browse = new RelayCommand(BrowseDialog));
        private void BrowseDialog()
        {
            OpenFileDialog ofn = new OpenFileDialog() { Multiselect = false, Filter = Resources.FilterStartup };
            if (ofn.ShowDialog() == true)
            {
                StartupFileName = ofn.FileName;
            }
        }
        public RelayCommand OnAdd => _add ?? (_add = new RelayCommand(Add));
        private void Add()
        {
            Debug.Print($"Server={Server}");
            if(IPAddress.TryParse(Server, out IPAddress ip))
            {
                Servers.Add(Server);
                Properties.Settings.Default.Servers.Clear();
                foreach(string server in Servers)
                {
                    Properties.Settings.Default.Servers.Add(server);
                }
                Properties.Settings.Default.SelectedServer = SelectedServer;
                Properties.Settings.Default.Save();
            }
        }
    }
}
