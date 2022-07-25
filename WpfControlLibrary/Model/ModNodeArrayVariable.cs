using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    public class ModNodeArrayVariable : ModBaseVariableNode
    {
        public ModNodeArrayVariable(string name, ModNodeId nodeId, string type, string access, int arrayLength) : base(name, nodeId, type, access)
        {
            ArrayLength = arrayLength;
        }

        public int ArrayLength { get; }
    }
}
