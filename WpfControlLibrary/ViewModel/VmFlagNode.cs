using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    public class VmFlagNode
    {
        public VmFlagNode(string text, bool isExpanded)
        {
            Children = new ObservableCollection<VmFlagNode>();
            Text = string.Empty;
            IsExpanded = isExpanded;
        }

        public ObservableCollection<VmFlagNode> Children;
        public string Text { get; }
        public bool IsExpanded { get; set; }
        public void Add(VmFlagNode pn)
        {
            Children.Add(pn);
        }
    }
}
