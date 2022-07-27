using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    public class VmNodeArrayVariable : VmNodeVariable
    {
        public VmNodeArrayVariable(string name, string nodeId, Model.basic_type type, Model.access access, int arrayLength, bool isExpanded = false, 
            bool isEditable = false) : base(name, nodeId, type, access, isExpanded, isEditable)
        {
            ImagePath = "pack://application:,,,/WpfControlLibrary;component/Icons/Type_527.png";
            ArrayLength = arrayLength;
        }

        public int ArrayLength { get; set; }
    }
}
