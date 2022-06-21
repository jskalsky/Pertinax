using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.DataModel
{
    public class DataModelArrayVariable : DataModelNode
    {
        public DataModelArrayVariable(string name, NodeIdBase nodeId, string basicType, string varAccess, int arrayLength, DataModelNode parent) :
            base(name, "pack://application:,,,/WpfControlLibrary;component/Icons/Type_527.png", nodeId, parent)
        {
            BasicType = basicType;
            VarAccess = varAccess;
            ArrayLength = arrayLength;
            DataModelType = DataModelType.ArrayVariable;
            TreeNodeText = $"{BasicType}, {ArrayLength}, {VarAccess}, {nodeId.GetIdentifier()}";
        }
        public string BasicType { get; }
        public string VarAccess { get; }
        public int ArrayLength { get; }

    }
}
