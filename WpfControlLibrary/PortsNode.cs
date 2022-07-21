using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
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
        public bool IsExpanded { get; }
        public PortsNode Add(string text)
        {
            PortsNode result = new PortsNode(text, IsExpanded);
            _children.Add(result);
            return result;
        }

        public void AddFlag(string flag)
        {
            _flags.Add(flag);
        }
    }
}
