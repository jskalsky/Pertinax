using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    public class VmNodeObject : VmNodeWithNodeId
    {
        public VmNodeObject(string name, Model.ModNodeId nodeId, string objectType, bool isExpanded = false, bool isEditable = false) :
            base(name, nodeId, isExpanded, isEditable)
        {
            ObjectType = objectType;
            ImagePath = "pack://application:,,,/WpfControlLibrary;component/Icons/Object_554.png";
        }

        public string ObjectType { get; }
    }
}
