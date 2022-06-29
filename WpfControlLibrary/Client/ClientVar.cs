using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Client
{
    public class ClientVar : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ushort _nsIndex;
        private string _id;

        public ClientVar()
        {
            ImagePath = "pack://application:,,,/WpfControlLibrary;component/Icons/Constant_495.png";
        }
        public ushort NsIndex
        {
            get { return _nsIndex; }
            set { _nsIndex = value; OnPropertyChanged(nameof(NsIndex)); }
        }

        public string Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }
        public string ImagePath { get; }

        public string Error => throw new NotImplementedException();
        public string this[string columnName]
        {
            get { return Validate(columnName); }
        }

        private string Validate(string propertyName)
        {
            string error = string.Empty;
            return error;
        }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
