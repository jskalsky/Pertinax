using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    internal class VmNodeSimpleVariable : VmNodeVariable
    {
        internal VmNodeSimpleVariable(string name, Model.ModNodeId nodeId, string type, string access, bool isExpanded = false, bool isEditable = false) :
            base(name, nodeId, type, access, isExpanded, isEditable)
        {

        }
    }
}
