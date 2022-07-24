using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfControlLibrary.Model;
namespace WpfControlLibrary.ViewModel
{
    internal abstract class VmNodeWithNodeId : VmNode
    {
        internal VmNodeWithNodeId(string name, ModNodeId nodeId, bool isExpanded, bool isEditable) : base(name, isExpanded, isEditable)
        {
            NodeId = nodeId;
        }

        internal Model.ModNodeId NodeId { get; }
    }
}
