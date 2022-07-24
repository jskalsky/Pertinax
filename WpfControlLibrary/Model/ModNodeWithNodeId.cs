using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    internal abstract class ModNodeWithNodeId : ModNode
    {
        internal ModNodeWithNodeId(string name, ModNodeId nodeId) : base(name)
        {
            NodeId = nodeId;
        }

        internal ModNodeId NodeId { get; }
    }
}
