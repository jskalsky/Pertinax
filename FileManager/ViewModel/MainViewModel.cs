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
        private string _selectedTarget;
        private RelayCommand<SelectionChangedEventArgs> _targetSelectionChanged;
        private Manager _leftPanel;
        private Manager _rightPanel;
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            Debug.Print($"MainViewModel constructor");
            Targets = new ObservableCollection<string>();
            Targets.Add("Pc");
            Targets.Add("Z2xx");
            SelectedTarget = Targets[0];
        }

        public ObservableCollection<string> Targets { get; }
        public ObservableCollection<DriveInfo> TargetDrives { get; }
        public string SelectedTarget { get { return _selectedTarget; } set { _selectedTarget = value; RaisePropertyChanged(); } }

        public RelayCommand<SelectionChangedEventArgs> OnTargetSelectionChanged => _targetSelectionChanged ?? (_targetSelectionChanged = new RelayCommand<SelectionChangedEventArgs>(
                                                                              (args) => TargetSelectionChanged(args)));

        public void TargetSelectionChanged(SelectionChangedEventArgs args)
        {
            Debug.Print($"TargetSelectionChanged {args.AddedItems.Count}");
            if (args.AddedItems.Count != 0)
            {
                _selectedTarget = (string)args.AddedItems[0];
            }
        }
    }
}