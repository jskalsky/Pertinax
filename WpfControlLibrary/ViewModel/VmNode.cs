using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    public abstract class VmNode
    {
        protected VmNode(string name, bool isExpanded, bool isEditable)
        {
            SubNodes = new ObservableCollection<VmNode>();
            Name = name;
            IsExpanded = isExpanded;
            IsEditable = isEditable;
            Parent = null;
        }
        [Browsable(false)]
        public ObservableCollection<VmNode> SubNodes { get; }
        public string Name { get; set; }
        [Browsable(false)]
        public bool IsExpanded { get; set; }
        [Browsable(false)]
        public bool IsEditable { get; set; }
        [Browsable(false)]
        public string ImagePath { get; protected set; }
        [Browsable(false)]
        public VmNode Parent { get; protected set; }


        public void AddVmNode(VmNode subNode)
        {
            subNode.Parent = this;
            SubNodes.Add(subNode);
        }
        public bool GetNamespace(out ushort nsIndex)
        {
            nsIndex = 0;
            VmNode node = this;
            while (node.Parent != null)
            {
                node = node.Parent;
                if(node != null)
                {
                    if(node is VmNodeNs ns)
                    {
                        nsIndex = ns.NsIndex;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
