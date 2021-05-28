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
        private string _selectedDownloadType;
        private string _bupFilename;

        private RelayCommand _browse;

        public SettingsViewModel()
        {
            DownloadTypes = new string[] { "With reset", "Without reset" };
            if(string.IsNullOrEmpty(Settings.Default.DownloadType))
            {
                SelectedDownloadType = DownloadTypes[0];
            }
            else
            {
                SelectedDownloadType = Settings.Default.DownloadType;
            }
            if(!string.IsNullOrEmpty(Settings.Default.Bup))
            {
                BupFileName = Settings.Default.Bup;
            }
        }
        public string[] DownloadTypes { get; }
        public string SelectedDownloadType
        {
            get { return _selectedDownloadType; }
            set { _selectedDownloadType = value; RaisePropertyChanged(); }
        }

        public string BupFileName
        {
            get { return _bupFilename; }
            set { _bupFilename = value;RaisePropertyChanged(); }
        }

        public RelayCommand BrowseCommand => _browse ?? (_browse = new RelayCommand(BrowseDialog));
        private void BrowseDialog()
        {
            OpenFileDialog ofn = new OpenFileDialog() { Multiselect = false, Filter = Resources.FilterBup };
            if(ofn.ShowDialog() == true)
            {
                BupFileName = ofn.FileName;
            }
        }
    }
}
