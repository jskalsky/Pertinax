using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.DataModel
{
    public class DataModelArrayVariable : DataModelNode
    {
        public DataModelArrayVariable(string name, NodeIdBase nodeId, string basicType, string varAccess, int arrayLength) :
            base(name, "Icons/Type_527.png", nodeId)
        {
            BasicType = basicType;
            VarAccess = varAccess;
            ArrayLength = arrayLength;
        }
        public string BasicType { get; }
        public string VarAccess { get; }
        public int ArrayLength { get; }

    }
}
