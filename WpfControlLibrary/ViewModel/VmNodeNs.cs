using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    internal class VmNodeNs : VmNode
    {
        internal VmNodeNs(string name, ushort nsIndex, bool isExpanded = false, bool isEditable = false) :
            base(name, isExpanded, isEditable)
        {
            NsIndex = nsIndex;
        }
        internal ushort NsIndex { get; }
    }
}
