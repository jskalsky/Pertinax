using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    public class VmNodeClientVar : VmNode
    {
        public VmNodeClientVar(string name, string nodeId, string type, bool isExpanded = false, bool isEditable = false) : base(name, isExpanded, isEditable)
        {
            ImagePath = "pack://application:,,,/WpfControlLibrary;component/Icons/ComplexType_12905.png";
            NodeId = nodeId;
            Type = type;
        }

        public string NodeId { get; set; }
        public string Type { get; set; }
    }
}
