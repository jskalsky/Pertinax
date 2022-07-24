using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    internal class ModNodeNs : ModNode
    {
        internal ModNodeNs(string name, ushort nsIndex) : base(name)
        {
            NsIndex = nsIndex;
        }

        internal ushort NsIndex { get; }
    }
}
