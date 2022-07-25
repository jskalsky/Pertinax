using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    public class ModNodeNs : ModNode
    {
        public ModNodeNs(string name, ushort nsIndex) : base(name)
        {
            NsIndex = nsIndex;
        }

        public ushort NsIndex { get; }
    }
}
