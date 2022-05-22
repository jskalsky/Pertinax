using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public class SecondaryViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private const ushort MaxNamespaceIndex = 5;
        private string _ns;
        private string _id;

        public string Ns
        {
            get { return _ns; }
            set { _ns = value; OnPropertyChanged("Ns"); }
        }

        public string Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged("Id"); }
        }

        public string Error
        {
            get { return "....."; }
        }

        public string this[string columnName]
        {
            get { return Validate(columnName); }
        }

        private string Validate(string propertyName)
        {
            string error = string.Empty;
            switch (propertyName)
            {
                case "Ns":
                    Debug.Print($"Ns= {Ns}");
                    if (!ushort.TryParse(Ns, out ushort ns))
                    {
                        error = $"Chybný formát indexu, {ns}";
                        break;
                    }

                    if (ns > MaxNamespaceIndex)
                    {
                        error = $"Index jmenného prostoru mimo rozsah, 0 - {MaxNamespaceIndex}";
                    }
                    break;
                case "Id":
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
