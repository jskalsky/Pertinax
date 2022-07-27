using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    public class PortsNode
    {
        private readonly ObservableCollection<PortsNode> _children;
        private readonly List<string> _flags;

        public PortsNode(bool isExpanded = false)
        {
            _children = new ObservableCollection<PortsNode>();
            _flags = new List<string>();
            Text = string.Empty;
            IsExpanded = isExpanded;
        }

        public PortsNode(string text, bool isExpanded = false)
        {
            _children = new ObservableCollection<PortsNode>();
            _flags = new List<string>();
            Text = text;
            IsExpanded = isExpanded;
        }

        public IList<PortsNode> Children => _children;
        public string Text { get; }
        public IList<string> Flags => _flags;
        public bool IsExpanded { get; set; }
        public PortsNode Add(string text, bool isExpanded = false)
        {
            PortsNode result = new PortsNode(text, isExpanded);
            _children.Add(result);
            return result;
        }
        public void Add(PortsNode pn)
        {
            _children.Add(pn);
        }

        public void AddFlag(string flag)
        {
            _flags.Add(flag);
        }
    }
}
