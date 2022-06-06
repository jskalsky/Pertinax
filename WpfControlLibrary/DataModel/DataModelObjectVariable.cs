using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.DataModel
{
    public class DataModelObjectVariable : DataModelNode
    {
        public DataModelObjectVariable(string name, NodeIdBase nodeId, string objectTypeName):base(name, "Icons/Object_554.png", nodeId)
        {
            ObjectTypeName = objectTypeName;
        }

        public string ObjectTypeName { get; }
    }
}
