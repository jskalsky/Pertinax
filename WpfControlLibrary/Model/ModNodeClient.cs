using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    public class ModNodeClient : ModNode
    {
        public ModNodeClient(string name, bool encrypt, string ipAddress) : base(name)
        {
            Encrypt = encrypt;
            IpAddress = ipAddress;
        }

        public bool Encrypt { get; }
        public string IpAddress { get; }
    }
}
