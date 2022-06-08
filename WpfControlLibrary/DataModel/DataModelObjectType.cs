using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.DataModel
{
    public class DataModelObjectType : DataModelNode
    {
        public DataModelObjectType(string name, NodeIdBase nodeId, DataModelNode parent) : base(name, "pack://application:,,,/WpfControlLibrary;component/Icons/ClassIcon.png", nodeId, parent)
        {
            DataModelType = DataModelType.ObjectType;
        }
    }
}
