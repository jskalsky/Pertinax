using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    public class ModNodeClientVar : ModNode
    {
        public ModNodeClientVar(string name, string nodeId, string type) : base(name)
        {
            NodeId = nodeId;
            Type = type;
        }

        public string NodeId { get; }
        public string Type { get; }
    }
}
