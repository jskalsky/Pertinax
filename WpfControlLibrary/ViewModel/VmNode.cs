using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    internal abstract class VmNode
    {
        protected VmNode(string name, bool isExpanded, bool isEditable)
        {
            SubNodes = new ObservableCollection<VmNode>();
            Name = name;
            IsExpanded = isExpanded;
            IsEditable = isEditable;
        }

        internal ObservableCollection<VmNode> SubNodes { get; }
        internal string Name { get; }
        internal bool IsExpanded { get; }
        internal bool IsEditable { get; }

        internal void AddVmNode(VmNode subNode)
        {
            SubNodes.Add(subNode);
        }
    }
}
