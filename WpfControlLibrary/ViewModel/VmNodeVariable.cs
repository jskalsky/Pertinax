using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    internal abstract class VmNodeVariable: VmNodeWithNodeId
    {
        internal VmNodeVariable(string name, Model.ModNodeId nodeId, string type, string access, bool isExpanded, bool isEditable) :
            base(name, nodeId, isExpanded, isEditable)
        {
            Type = type;
            Access = access;
        }

        internal string Type { get; }
        internal string Access { get; }
    }
}
