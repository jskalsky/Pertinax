using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    public abstract class ModBaseVariableNode : ModNodeWithNodeId
    {
        public ModBaseVariableNode(string name, ModNodeId nodeId, basic_type type, access access) : base(name, nodeId)
        {
            Type = type;
            Access = access;
        }

        public basic_type Type { get; }
        public access Access { get; }
    }
}
