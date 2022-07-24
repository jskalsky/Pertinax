using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    internal class VmNodeObject : VmNodeWithNodeId
    {
        internal VmNodeObject(string name, Model.ModNodeId nodeId, string objectType, bool isExpanded = false, bool isEditable = false) :
            base(name, nodeId, isExpanded, isEditable)
        {
            ObjectType = objectType;
        }

        internal string ObjectType { get; }
    }
}
