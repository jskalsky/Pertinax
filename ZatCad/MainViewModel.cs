using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZatCad
{
    public class MainViewModel : INotifyPropertyChanged

    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _selectedCfg;
        public MainViewModel()
        {
            Cfgs = new string[] { "Test1.OPCUA", "Test2.OPCUA" };
            SelectedCfg = Cfgs[0];
        }
        public string[] Cfgs
        {
            get;
        }

        public string SelectedCfg
        {
            get { return _selectedCfg; }
            set { _selectedCfg = value; OnPropertyChanged("SelectedCfg"); }
        }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
