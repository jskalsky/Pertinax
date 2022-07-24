using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    internal class VmNodeObjectType : VmNodeWithNodeId
    {
        internal VmNodeObjectType(string name, Model.ModNodeId nodeId, bool isExpanded = false, bool isEditable = false) : 
            base(name, nodeId, isExpanded, isEditable)
        {

        }
    }
}
