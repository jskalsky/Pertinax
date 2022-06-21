using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfControlLibrary.DataModel;

namespace WpfControlLibrary.ViewModel
{
    public class OpcUaViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public OpcUaViewModel()
        {
            SelectedNode= null;
            DataModel = new ObservableCollection<DataModelNode>();
        }

        public DataModelNode SelectedNode { get; set; }
        public DataModelNamespace DataModelNamespace0 { get; set; }
        public DataModelNamespace DataModelNamespace1 { get; set; }

        public ObservableCollection<DataModelNode> DataModel { get; }



        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
