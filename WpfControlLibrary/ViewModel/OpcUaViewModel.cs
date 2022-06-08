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
            DataModelNamespace0 = new DataModelNamespace(0);
            DataModelNamespace1 = new DataModelNamespace(1);
            DataModel.Add(DataModelNamespace0);
            DataModel.Add(DataModelNamespace1);
        }

        public DataModelNode SelectedNode { get; set; }
        public DataModelNamespace DataModelNamespace0 { get; }
        public DataModelNamespace DataModelNamespace1 { get; }

        public ObservableCollection<DataModelNode> DataModel { get; }



        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
