using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    public abstract class VmNodeVariable: VmNodeWithNodeId
    {
        public VmNodeVariable(string name, string nodeId, string type, string access, bool isExpanded, bool isEditable) :
            base(name, nodeId, isExpanded, isEditable)
        {
            Type = type;
            Access = access;
        }

        public string Type { get; set; }
        public string Access { get; set; }
    }
}
