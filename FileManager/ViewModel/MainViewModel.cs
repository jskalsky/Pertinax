using FileManager.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;

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
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            Debug.Print($"MainViewModel constructor");
            TargetsLeft = new ObservableCollection<string>();
            TargetsLeft.Add("Pc");
            TargetsLeft.Add("Z2xx");
            if(string.IsNullOrEmpty(Properties.Settings.Default.TargetLeft))
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
        }

        public ObservableCollection<string> TargetsLeft { get; }
        public ObservableCollection<DriveInfo> TargetDrivesLeft { get; }
        public string SelectedTargetLeft { get { return _selectedTargetLeft; } set { _selectedTargetLeft = value; RaisePropertyChanged(); } }

        public ObservableCollection<string> TargetsRight { get; }
        public ObservableCollection<DriveInfo> TargetDrivesRight { get; }
        public string SelectedTargetRight { get { return _selectedTargetRight; } set { _selectedTargetRight = value; RaisePropertyChanged(); } }
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
            }
        }
    }
}