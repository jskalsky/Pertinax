using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Client
{
    public class ClientVar : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _identifier;
        private string _selectedBasicType;
        private string _alias;
        private Group _group;
        private static readonly string[] _basicTypes = new string[] { "Boolean", "UInt8", "Int8", "UInt16", "Int16", "UInt32", "Int32", "Float", "Double" };

        public ClientVar(Group group, string id, string basicType, string alias)
        {
            _group = group;
            ImagePath = "pack://application:,,,/WpfControlLibrary;component/Icons/Constant_495.png";
            Alias = alias;
            Identifier = id;
            SelectedBasicType = basicType;
        }
        public string Identifier
        {
            get { return _identifier; }
            set { _identifier = value; OnPropertyChanged(nameof(Identifier)); }
        }
        public string ImagePath { get; }

        public static string[] BasicTypes 
        {
            get { return _basicTypes; }
        }
        public string SelectedBasicType
        {
            get { return _selectedBasicType; }
            set { _selectedBasicType = value; OnPropertyChanged(nameof(SelectedBasicType)); }
        }
        public string Alias
        {
            get { return _alias; }
            set { _alias = value; OnPropertyChanged(nameof(Alias)); }
        }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
