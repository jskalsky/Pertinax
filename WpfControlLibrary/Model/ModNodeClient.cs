using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    internal class ModNodeClient : ModNode
    {
        internal ModNodeClient(string name, bool encrypt, string ipAddress) : base(name)
        {
            Encrypt = encrypt;
            IpAddress = ipAddress;
        }

        internal bool Encrypt { get; }
        internal string IpAddress { get; }
    }
}
