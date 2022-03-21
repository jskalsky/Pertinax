using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public class SubscriberItem : INotifyPropertyChanged
    {
        private bool _subscribe;
        private object _checkBoxTag;
        public SubscriberItem(string path, string name, bool sub)
        {
            ObjectName = name;
            ConfigurationPath = path;
            ConfigurationName = Path.GetFileName(path);
            Subscribe = sub;
            CheckBoxtag = this;
        }
        public string ConfigurationPath { get; }
        public string ObjectName { get; }
        public string ConfigurationName { get; }

        public bool Subscribe
        {
            get { return _subscribe; }
            set { _subscribe = value; OnPropertyChanged("Subscribe"); }
        }

        public object CheckBoxtag
        {
            get { return _checkBoxTag; }
            set { _checkBoxTag = value; OnPropertyChanged("CheckBoxTag"); }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
