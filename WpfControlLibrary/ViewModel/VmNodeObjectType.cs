using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    public class VmNodeObjectType : VmNodeWithNodeId
    {
        public VmNodeObjectType(string name, string nodeId, bool isExpanded = false, bool isEditable = false) : 
            base(name, nodeId, isExpanded, isEditable)
        {
            ImagePath = "pack://application:,,,/WpfControlLibrary;component/Icons/ClassIcon.png";
        }
    }
}
