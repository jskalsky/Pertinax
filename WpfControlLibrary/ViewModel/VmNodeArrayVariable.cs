using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    internal class VmNodeArrayVariable : VmNodeVariable
    {
        internal VmNodeArrayVariable(string name, Model.ModNodeId nodeId, string type, string access, int arrayLength, bool isExpanded = false, 
            bool isEditable = false) : base(name, nodeId, type, access, isExpanded, isEditable)
        {
            ArrayLength = arrayLength;
        }

        internal int ArrayLength { get; }
    }
}
