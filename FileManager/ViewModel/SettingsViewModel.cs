using FileManager.Properties;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public SettingsViewModel()
        {
            if (!string.IsNullOrEmpty(Settings.Default.Startup))
            {
                StartupFileName = Settings.Default.Startup;
            }
            RepetitiveRate = Settings.Default.RepetitiveRate;
            IsTls = Settings.Default.IsTls;
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
        public RelayCommand BrowseCommand => _browse ?? (_browse = new RelayCommand(BrowseDialog));
        private void BrowseDialog()
        {
            OpenFileDialog ofn = new OpenFileDialog() { Multiselect = false, Filter = Resources.FilterStartup };
            if (ofn.ShowDialog() == true)
            {
                StartupFileName = ofn.FileName;
            }
        }
    }
}
