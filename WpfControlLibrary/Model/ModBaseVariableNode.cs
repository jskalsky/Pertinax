using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    public abstract class ModBaseVariableNode : ModNodeWithNodeId
    {
        protected List<string> _flags;
        public ModBaseVariableNode(string name, ModNodeId nodeId, basic_type type, access access) : base(name, nodeId)
        {
            Type = type;
            Access = access;
            _flags = new List<string>();
        }

        public basic_type Type { get; }
        public access Access { get; }
        public IList<string> Flags => _flags;
        public abstract void CreateFlags(string path);
    }
}
