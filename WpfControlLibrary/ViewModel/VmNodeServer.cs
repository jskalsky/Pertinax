using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    internal class VmNodeServer : VmNode
    {
        internal VmNodeServer(string name, bool encrypt, bool isExpanded = false, bool isEditable = false) : base(name, isExpanded, isEditable)
        {
            Encrypt = encrypt;
        }

        internal bool Encrypt { get; }
    }
}
