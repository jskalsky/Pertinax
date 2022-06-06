using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.DataModel
{
    public class DataModelSimpleVariable : DataModelNode
    {
        public DataModelSimpleVariable(string name, NodeIdBase nodeId, string basicType, string varAccess, DataModelNode parent) : base(name, "Icons/Constant_495.png", nodeId, parent)
        {
            BasicType = basicType;
            VarAccess = varAccess;
            DataModelType = DataModelType.SimpleVariable;
        }

        public string BasicType { get; }
        public string VarAccess { get; }
    }
}
