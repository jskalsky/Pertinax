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
        public VmNodeWithNodeId(string name, string nodeId, bool isExpanded, bool isEditable) : base(name, isExpanded, isEditable)
        {
            NodeIdString = nodeId;
        }
        [Browsable(false)]
        public string NodeIdString { get; set; }
    }
}
