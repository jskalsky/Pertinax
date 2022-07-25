using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    public class ModNodeVariable : ModBaseVariableNode
    {
        public ModNodeVariable(string name, ModNodeId nodeId, string type, string access) : base(name, nodeId, type, access)
        {

        }
    }
}
