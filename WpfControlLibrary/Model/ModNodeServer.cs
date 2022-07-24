using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    internal class ModNodeServer : ModNode
    {
        internal ModNodeServer(string name, bool encrypt) : base(name)
        {
            Encrypt = encrypt;
        }
        internal bool Encrypt { get; }
    }
}
