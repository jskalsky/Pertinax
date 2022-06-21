using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.DataModel
{
    public class DataModelObjectVariable : DataModelNode
    {
        public DataModelObjectVariable(string name, NodeIdBase nodeId, string objectTypeName, DataModelNode parent) : base(name, "pack://application:,,,/WpfControlLibrary;component/Icons/Object_554.png", nodeId, parent)
        {
            ObjectTypeName = objectTypeName;
            DataModelType = DataModelType.ObjectVariable;
            TreeNodeText = $"{ObjectTypeName}";
        }

        public string ObjectTypeName { get; }
    }
}
