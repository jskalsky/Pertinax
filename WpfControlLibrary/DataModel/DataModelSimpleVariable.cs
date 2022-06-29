using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.DataModel
{
    public class DataModelSimpleVariable : DataModelNode
    {
        public DataModelSimpleVariable(string name, NodeIdBase nodeId, string basicType, string varAccess, DataModelNode parent) : base(name, 
            "pack://application:,,,/WpfControlLibrary;component/Icons/Constant_495.png", nodeId, parent)
        {
            SelectedString = basicType;
            VarAccess = varAccess;
            DataModelType = DataModelType.SimpleVariable;
            TreeNodeText = $"{SelectedString}, {VarAccess}, {nodeId.GetIdentifier()}";
            BasicTypesVisibility = System.Windows.Visibility.Visible;
        }

        public string VarAccess { get; }
    }
}
