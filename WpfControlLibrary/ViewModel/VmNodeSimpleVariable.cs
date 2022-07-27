using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    public class VmNodeSimpleVariable : VmNodeVariable
    {
        public VmNodeSimpleVariable(string name, string nodeId, Model.basic_type type, Model.access access, bool isExpanded = false, bool isEditable = false) :
            base(name, nodeId, type, access, isExpanded, isEditable)
        {
            ImagePath = "pack://application:,,,/WpfControlLibrary;component/Icons/Constant_495.png";
        }
    }
}
