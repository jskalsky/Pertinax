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
        }
        [Browsable(false)]
        public ObservableCollection<VmNode> SubNodes { get; }
        public string Name { get; set; }
        [Browsable(false)]
        public bool IsExpanded { get; }
        [Browsable(false)]
        public bool IsEditable { get; }
        [Browsable(false)]
        public string ImagePath { get; protected set; }

        public void AddVmNode(VmNode subNode)
        {
            SubNodes.Add(subNode);
        }
    }
}
