using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Model
{
    public abstract class Manager : INotifyPropertyChanged
    {
        protected string[] _drives;
        protected string _actualDirectory;
        protected string[] _dirItems;
        protected string _selectedDrive;
        protected string _lastError;

        public event PropertyChangedEventHandler PropertyChanged;
        protected Manager()
        {
            _actualDirectory = string.Empty;
            _selectedDrive = string.Empty;
            _lastError = string.Empty;
        }

        public string LastError
        {
            get { return _lastError; }
            set { _lastError = value; OnPropertyChanged("LastError"); }
        }
        public string[] Drives
        {
            get { return _drives; }
            protected set { _drives = value; OnPropertyChanged("Drives"); }
        }

        public string ActualDirectory
        {
            get { return _actualDirectory; }
            protected set { _actualDirectory = value; OnPropertyChanged("ActualDirectory"); }
        }

        public string[] DirItems
        {
            get { return _dirItems; }
            protected set { _dirItems = value; OnPropertyChanged("DirItems"); }
        }

        public string SelectedDrive
        {
            get { return _selectedDrive; }
            set { _selectedDrive = value; OnPropertyChanged("SelectedDrive"); }
        }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public abstract byte[] Upload(string fileName);
        public abstract void Download(string fileName, byte[] file);
        public abstract void RefreshDrives();
        public abstract void SelectDrive(string drive, string actualDirectory);
        public abstract void ChangeDirectory(string dir);
        public abstract void RefreshDirectory();
        public abstract string MakeFilename(string selectedItem);
        public abstract void SymLink(string old, string newPath);
        public abstract void Remove(string path);
    }
}
