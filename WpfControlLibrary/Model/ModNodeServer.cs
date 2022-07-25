using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    public class ModNodeServer : ModNode
    {
        public ModNodeServer(string name, bool encrypt) : base(name)
        {
            Encrypt = encrypt;
        }
        public bool Encrypt { get; }
    }
}
