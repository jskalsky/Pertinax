using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    public abstract class VmNodeVariable : VmNodeWithNodeId
    {
        public VmNodeVariable(string name, string nodeId, Model.basic_type type, Model.access access, bool isExpanded, bool isEditable) :
            base(name, nodeId, isExpanded, isEditable)
        {
            Type = type;
            Access = access;
        }

        public Model.basic_type Type { get; set; }
        public Model.access Access { get; set; }
    }
}
