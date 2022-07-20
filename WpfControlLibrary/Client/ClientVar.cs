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

        private string _identifier;
        private string _selectedBasicType;
        private string _alias;
        private ClientConnection _clientConnection;
        private readonly string[] _basicTypes = new string[] { "Boolean", "UInt8", "Int8", "UInt16", "Int16", "UInt32", "Int32", "Float", "Double" };

        public ClientVar(ClientConnection cc)
        {
            _clientConnection = cc;
            ImagePath = "pack://application:,,,/WpfControlLibrary;component/Icons/Constant_495.png";
            Alias = string.Empty;
        }
        public string Identifier
        {
            get { return _identifier; }
            set { _identifier = value; OnPropertyChanged(nameof(Identifier)); }
        }
        public string ImagePath { get; }

        public string[] BasicTypes 
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
        public string Error => throw new NotImplementedException();
        public string this[string columnName]
        {
            get { return Validate(columnName); }
        }

        private string IdExists(string id)
        {
            int count = 0;
            foreach(ClientVar cv in _clientConnection.Vars)
            {
                if(cv.Identifier == id)
                {
                    ++count;
                }
            }
            string s = (count > 1)? $"Identifikátor již existuje" : string.Empty;
            return s;
        }

        private string TestIdentifier(string id)
        {
            string[] items = id.Split(':');
            if(items.Length != 2)
            {
                return $"Chybný formát identifikátoru";
            }
            if(!ushort.TryParse(items[0], out ushort value))
            {
                return $"Index jmenného prostoru musí být číslo 0 - 65535";
            }
            return string.Empty;
        }

        private int IdsErrorsCount()
        {
            Dictionary<string, int> errors = new Dictionary<string, int>();
            foreach(ClientVar cv in _clientConnection.Vars)
            {
                if(!errors.ContainsKey(cv.Identifier))
                {
                    errors.Add(cv.Identifier, 1);
                }
                else
                {
                    errors[cv.Identifier]++;
                }
            }
            int count = 0;
            foreach(KeyValuePair<string, int> kvp in errors)
            {
                if(kvp.Value > 1)
                {
                    ++count;
                }
            }
            return count;
        }
        private string Validate(string propertyName)
        {
            string error = string.Empty;
            if(!_clientConnection.ValidateVars)
            {
                return error;
            }
            switch(propertyName)
            {
                case "Identifier":
                    error = TestIdentifier(Identifier);
                    if(error != string.Empty)
                    {
                        break;
                    }
                    error = IdExists(Identifier);
                    if(error != string.Empty)
                    {
                        break;
                    }
                    if(IdsErrorsCount() == 0)
                    {
                        _clientConnection.ValidateVars = false;
                        foreach(ClientVar cv in _clientConnection.Vars)
                        {
                            cv.Identifier=cv.Identifier;
                        }
                        _clientConnection.ValidateVars = true;
                    }
                    break;
            }
            return error;
        }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
