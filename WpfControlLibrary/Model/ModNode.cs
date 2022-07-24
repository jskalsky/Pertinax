using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    internal abstract class ModNode
    {
        private List<ModNode> _subNodes;
        protected ModNode(string name)
        {
            Name = name;
            _subNodes = new List<ModNode>();
        }
        internal IList<ModNode> SubNodes => _subNodes;
        internal string Name { get; }
        internal void AddSubNode(ModNode modNode)
        {
            SubNodes.Add(modNode);
        }
    }
}
