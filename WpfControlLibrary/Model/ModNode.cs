using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    public abstract class ModNode
    {
        private List<ModNode> _subNodes;
        protected ModNode(string name)
        {
            Name = name;
            _subNodes = new List<ModNode>();
        }
        public IList<ModNode> SubNodes => _subNodes;
        public string Name { get; }
        public void AddSubNode(ModNode modNode)
        {
            SubNodes.Add(modNode);
        }
    }
}
