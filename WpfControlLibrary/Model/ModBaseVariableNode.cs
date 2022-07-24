using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    internal abstract class ModBaseVariableNode : ModNodeWithNodeId
    {
        internal ModBaseVariableNode(string name, ModNodeId nodeId, string type, string access) : base(name, nodeId)
        {
            Type = type;
            Access = access;
        }

        internal string Type { get; }
        internal string Access { get; }
    }
}
