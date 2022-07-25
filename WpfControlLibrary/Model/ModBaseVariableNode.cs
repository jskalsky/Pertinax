using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    public abstract class ModBaseVariableNode : ModNodeWithNodeId
    {
        public ModBaseVariableNode(string name, ModNodeId nodeId, string type, string access) : base(name, nodeId)
        {
            Type = type;
            Access = access;
        }

        public string Type { get; }
        public string Access { get; }
    }
}
