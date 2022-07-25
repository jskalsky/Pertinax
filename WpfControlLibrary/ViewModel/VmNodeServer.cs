using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    public class VmNodeServer : VmNode
    {
        public VmNodeServer(string name, bool encrypt, bool isExpanded = false, bool isEditable = false) : base(name, isExpanded, isEditable)
        {
            Encrypt = encrypt;
            ImagePath = "pack://application:,,,/WpfControlLibrary;component/Icons/MultiView_6035.png";
        }

        public bool Encrypt { get; }
    }
}
