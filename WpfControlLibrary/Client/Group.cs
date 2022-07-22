using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Client
{
    public class Group : INotifyPropertyChanged
    {
        private ushort _period;
        private string _service;
        private static readonly string[] _services = new string[] { "Read", "Write" };
        private bool _isExpanded;

        public event PropertyChangedEventHandler PropertyChanged;

        public Group(ushort period, string service)
        {
            Period = period;
            Service = service;
            Vars = new ObservableCollection<ClientVar>();
        }
        public ObservableCollection<ClientVar> Vars { get; }
        public string Service
        {
            get { return _service; }
            set { _service = value; OnPropertyChanged(nameof(Service)); }
        }
        public ushort Period
        {
            get { return _period; }
            set { _period = value; OnPropertyChanged(nameof(Period)); }
        }
        public static string[] Services
        {
            get { return _services; }
        }
        public void AddVar(ClientVar var)
        {
            Vars.Add(var);
        }
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; OnPropertyChanged("IsExpanded"); }
        }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
