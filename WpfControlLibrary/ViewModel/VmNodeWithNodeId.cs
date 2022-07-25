using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfControlLibrary.Model;
namespace WpfControlLibrary.ViewModel
{
    public abstract class VmNodeWithNodeId : VmNode
    {
        public VmNodeWithNodeId(string name, ModNodeId nodeId, bool isExpanded, bool isEditable) : base(name, isExpanded, isEditable)
        {
            NodeId = nodeId;
        }

        public Model.ModNodeId NodeId { get; }
    }
}
