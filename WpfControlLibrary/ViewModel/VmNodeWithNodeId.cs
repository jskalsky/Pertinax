using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            NodeIdString = nodeId.GetText();
        }
        [Browsable(false)]
        public Model.ModNodeId NodeId { get; }
        public string NodeIdString { get; set; }
    }
}
