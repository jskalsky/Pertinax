using FileManager.Model;
using FileManager.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;

namespace FileManager.ViewModel
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
        private string _selectedTargetLeft;
        private RelayCommand<SelectionChangedEventArgs> _targetLeftSelectionChanged;
        private string _selectedTargetRight;
        private RelayCommand<SelectionChangedEventArgs> _targetRightSelectionChanged;
        private Manager _leftPanel;
        private Manager _rightPanel;
        private RelayCommand _settings;
        private RelayCommand _start;

        private uint _freeRam;
        private int _nrFiles;

        private List<string> _leftDrives;
        private string _leftSelectedDrive;
        private string _leftActualFolder;
        private RelayCommand<MouseButtonEventArgs> _leftDoubleClick;

        private List<string> _rightDrives;
        private string _rightSelectedDrive;
        private string _rightActualFolder;
        private RelayCommand<MouseButtonEventArgs> _rightDoubleClick;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            Debug.Print($"MainViewModel constructor");
            Messages = new ObservableCollection<string>();
            TargetsLeft = new ObservableCollection<string>();
            TargetsLeft.Add("Pc");
            TargetsLeft.Add("Z2xx");
            if (string.IsNullOrEmpty(Properties.Settings.Default.TargetLeft))
            {
                SelectedTargetLeft = TargetsLeft[0];
                Properties.Settings.Default.TargetLeft = SelectedTargetLeft;
            }
            else
            {
                SelectedTargetLeft = Properties.Settings.Default.TargetLeft;
            }
            TargetsRight = new ObservableCollection<string>();
            TargetsRight.Add("Pc");
            TargetsRight.Add("Z2xx");
            if (string.IsNullOrEmpty(Properties.Settings.Default.TargetRight))
            {
                SelectedTargetRight = TargetsRight[0];
                Properties.Settings.Default.TargetRight = SelectedTargetRight;
            }
            else
            {
                SelectedTargetRight = Properties.Settings.Default.TargetRight;
            }

            TargetDrivesLeft = new ObservableCollection<DriveInfo>();
            TargetDrivesRight = new ObservableCollection<DriveInfo>();
            if (SelectedTargetLeft == "Pc")
            {
                _leftPanel = new WindowsManager();
            }
            else
            {
                if (SelectedTargetLeft == "Z2xx")
                {
                    _leftPanel = new Z2xxManager();
                }
            }
            if (SelectedTargetRight == "Pc")
            {
                _rightPanel = new WindowsManager();
            }
            else
            {
                if (SelectedTargetRight == "Z2xx")
                {
                    _rightPanel = new Z2xxManager();
                }
            }

            _leftPanel.PropertyChanged += _leftPanel_PropertyChanged;
            _rightPanel.PropertyChanged += _rightPanel_PropertyChanged;
            _leftDrives = new List<string>();
            _rightDrives = new List<string>();
            _leftPanel.RefreshDrives();
            _rightPanel.RefreshDrives();
            _leftSelectedDrive = Properties.Settings.Default.TargetDriveLeft;
            _leftActualFolder = Properties.Settings.Default.ActualDirectoryLeft;
            _leftPanel.SelectDrive(_leftSelectedDrive, _leftActualFolder);
            _rightSelectedDrive = Properties.Settings.Default.TargetDriveRight;
            _rightActualFolder = Properties.Settings.Default.ActualDirectoryRight;
            _rightPanel.SelectDrive(_rightSelectedDrive, _rightActualFolder);
            LeftDirectory = new ObservableCollection<string>();
            RightDirectory = new ObservableCollection<string>();
            _leftPanel.RefreshDirectory();
            _rightPanel.RefreshDirectory();
        }

        private void _rightPanel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "Drives":
                    _rightDrives.Clear();
                    _rightDrives.AddRange(_rightPanel.Drives);
                    RaisePropertyChanged("RightDrives");
                    break;
                case "SelectedDrive":
                    RightSelectedDrive = _rightPanel.SelectedDrive;
                    Properties.Settings.Default.TargetDriveRight = _rightSelectedDrive;
                    Debug.Print($"RightSelectedDrive= {Properties.Settings.Default.TargetDriveRight}");
                    break;
                case "ActualDirectory":
                    RightActualFolder = _rightPanel.ActualDirectory;
                    Properties.Settings.Default.ActualDirectoryRight = _rightActualFolder;
                    Debug.Print($"ActualDirectoryRight= {Properties.Settings.Default.ActualDirectoryRight}");
                    break;
                case "Folders":
                    RightDirectory.Clear();
                    foreach(string folder in _rightPanel.Folders)
                    {
                        RightDirectory.Add(folder);
                    }
                    break;
                case "Files":
                    foreach (string file in _rightPanel.Files)
                    {
                        RightDirectory.Add(file);
                    }
                    break;
                case "LastError":
                    Messages.Add(_rightPanel.LastError);
                    break;
            }
        }

        private void _leftPanel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Drives":
                    _leftDrives.Clear();
                    _leftDrives.AddRange(_leftPanel.Drives);
                    RaisePropertyChanged("LeftDrives");
                    break;
                case "SelectedDrive":
                    LeftSelectedDrive = _leftPanel.SelectedDrive;
                    Properties.Settings.Default.TargetDriveLeft = _leftSelectedDrive;
                    break;
                case "ActualDirectory":
                    LeftActualFolder = _leftPanel.ActualDirectory;
                    Properties.Settings.Default.ActualDirectoryLeft = _leftActualFolder;
                    break;
                case "Folders":
                    LeftDirectory.Clear();
                    foreach (string folder in _leftPanel.Folders)
                    {
                        LeftDirectory.Add(folder);
                    }
                    break;
                case "Files":
                    foreach (string file in _leftPanel.Files)
                    {
                        LeftDirectory.Add(file);
                    }
                    break;
                case "LastError":
                    Messages.Add(_leftPanel.LastError);
                    break;
            }
        }

        public ObservableCollection<string> TargetsLeft { get; }
        public ObservableCollection<DriveInfo> TargetDrivesLeft { get; }
        public string SelectedTargetLeft { get { return _selectedTargetLeft; } set { _selectedTargetLeft = value; RaisePropertyChanged(); } }
        public string LeftSelectedDrive { get { return _leftSelectedDrive; } set { _leftSelectedDrive = value; RaisePropertyChanged(); } }
        public List<string> LeftDrives
        {
            get { return _leftDrives; }
        }
        public string LeftActualFolder
        {
            get { return _leftActualFolder; }
            set { _leftActualFolder = value; RaisePropertyChanged(); }
        }
        public ObservableCollection<string> LeftDirectory { get; }
        public ObservableCollection<string> TargetsRight { get; }
        public ObservableCollection<DriveInfo> TargetDrivesRight { get; }
        public string SelectedTargetRight { get { return _selectedTargetRight; } set { _selectedTargetRight = value; RaisePropertyChanged(); } }
        public string RightSelectedDrive { get { return _rightSelectedDrive; } set { _rightSelectedDrive = value; RaisePropertyChanged(); } }
        public List<string> RightDrives
        {
            get { return _rightDrives; }
        }
        public string RightActualFolder
        {
            get { return _rightActualFolder; }
            set { _rightActualFolder = value; RaisePropertyChanged(); }
        }
        public ObservableCollection<string> RightDirectory { get; }

        public ObservableCollection<string> Messages { get; }
        public uint FreeRam
        {
            get { return _freeRam; }
            set { _freeRam = value;RaisePropertyChanged(); }
        }

        public int NrFiles
        {
            get { return _nrFiles; }
            set { _nrFiles = value;RaisePropertyChanged(); }
        }
        public RelayCommand<SelectionChangedEventArgs> OnTargetLeftSelectionChanged => _targetLeftSelectionChanged ?? (_targetLeftSelectionChanged = new RelayCommand<SelectionChangedEventArgs>(
                                                                              (args) => TargetLeftSelectionChanged(args)));

        public void TargetLeftSelectionChanged(SelectionChangedEventArgs args)
        {
            Debug.Print($"TargetLeftSelectionChanged {args.AddedItems.Count}");
            if (args.AddedItems.Count != 0)
            {
                _selectedTargetLeft = (string)args.AddedItems[0];
            }
        }
        public RelayCommand<SelectionChangedEventArgs> OnTargetRightSelectionChanged => _targetRightSelectionChanged ?? (_targetRightSelectionChanged = new RelayCommand<SelectionChangedEventArgs>(
                                                                              (args) => TargetRightSelectionChanged(args)));

        public void TargetRightSelectionChanged(SelectionChangedEventArgs args)
        {
            Debug.Print($"TargetRightSelectionChanged {args.AddedItems.Count}");
            if (args.AddedItems.Count != 0)
            {
                _selectedTargetRight = (string)args.AddedItems[0];
                _rightPanel.PropertyChanged -= _rightPanel_PropertyChanged;
                if (_selectedTargetRight == "Pc")
                {
                    _rightPanel = new WindowsManager();
                }
                else
                {
                    if (_selectedTargetRight == "Z2xx")
                    {
                        _rightPanel = new Z2xxManager();
                    }
                }
                _rightPanel.PropertyChanged += _rightPanel_PropertyChanged;
                Properties.Settings.Default.TargetRight = _selectedTargetRight;
                _rightPanel.RefreshDrives();
                RightSelectedDrive = string.Empty;
                RightActualFolder = string.Empty;
                _rightPanel.SelectDrive(_rightSelectedDrive, _rightActualFolder);
                RightDirectory.Clear();
                _rightPanel.RefreshDirectory();
            }
        }

        public RelayCommand SettingsCommand => _settings ?? (_settings = new RelayCommand(SettingsDialog));
        private void SettingsDialog()
        {
            Settings settings = new Settings();
            if(settings.ShowDialog() == true)
            {

            }
        }
        public RelayCommand StartCommand => _start ?? (_start = new RelayCommand(Start));
        private void Start()
        {
            DownloadTest dt = new DownloadTest();
            dt.Test(Properties.Settings.Default.Startup, Properties.Settings.Default.RepetitiveRate, Properties.Settings.Default.IsTls);
        }

        public RelayCommand<MouseButtonEventArgs> OnLeftDoubleClick => _leftDoubleClick ?? (_leftDoubleClick =
                                                                  new RelayCommand<MouseButtonEventArgs>(
                                                                    (args) => LeftDoubleClick(args)));
        public void LeftDoubleClick(MouseButtonEventArgs args)
        {
            ListView lv = (ListView)args.Source;
            if(lv.SelectedItem != null)
            {
                string selectedItem = (string)lv.SelectedItem;
                if(selectedItem[0] == '[')
                {
                    selectedItem = selectedItem.Trim(new[] { '[', ']' });
                }
                _leftPanel.ChangeDirectory(selectedItem);
                _leftPanel.RefreshDirectory();
            }
        }

        public RelayCommand<MouseButtonEventArgs> OnRightDoubleClick => _rightDoubleClick ?? (_rightDoubleClick =
                                                                  new RelayCommand<MouseButtonEventArgs>(
                                                                    (args) => RightDoubleClick(args)));
        public void RightDoubleClick(MouseButtonEventArgs args)
        {
            ListView lv = (ListView)args.Source;
            if (lv.SelectedItem != null)
            {
                string selectedItem = (string)lv.SelectedItem;
                if (selectedItem[0] == '[')
                {
                    selectedItem = selectedItem.Trim(new[] { '[', ']' });
                }
                _rightPanel.ChangeDirectory(selectedItem);
                _rightPanel.RefreshDirectory();
            }
        }
    }
}