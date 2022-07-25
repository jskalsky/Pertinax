using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    public class VmNodeNs : VmNode
    {
        public VmNodeNs(string name, ushort nsIndex, bool isExpanded = false, bool isEditable = false) :
            base(name, isExpanded, isEditable)
        {
            ImagePath = "pack://application:,,,/WpfControlLibrary;component/Icons/Enum_582.png";
            NsIndex = nsIndex;
        }
        public ushort NsIndex { get; }
    }
}
