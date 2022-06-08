using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.DataModel
{
    public class DataModelFolder : DataModelNode
    {
        public DataModelFolder(string name, NodeIdBase nodeId, DataModelNode parent) : base(name, "pack://application:,,,/WpfControlLibrary;component/Icons/Folder_6222.png", nodeId, parent)
        { 
            DataModelType = DataModelType.Folder;
        }
    }
}
