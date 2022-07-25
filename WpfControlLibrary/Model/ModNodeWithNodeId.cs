using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    public abstract class ModNodeWithNodeId : ModNode
    {
        public ModNodeWithNodeId(string name, ModNodeId nodeId) : base(name)
        {
            NodeId = nodeId;
        }

        public ModNodeId NodeId { get; }
    }
}
