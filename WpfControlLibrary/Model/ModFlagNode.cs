using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    public class ModFlagNode
    {
        private List<ModFlagNode> _subNodes;
        public ModFlagNode(string name)
        {
            Name = name;
            _subNodes = new List<ModFlagNode>();
        }
        public string Name { get; }
        public IList<ModFlagNode> SubNodes => _subNodes;
        public void AddNode(ModFlagNode mfn)
        {
            _subNodes.Add(mfn);
        }
    }
}
