using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    public class VmNodeFolder : VmNodeWithNodeId
    {
        public VmNodeFolder(string name, Model.ModNodeId nodeId, bool isExpanded = false, bool isEditable = false) : 
            base(name, nodeId,isExpanded,isEditable)
        {
            ImagePath = "pack://application:,,,/WpfControlLibrary;component/Icons/Folder_6222.png";
        }
    }
}
