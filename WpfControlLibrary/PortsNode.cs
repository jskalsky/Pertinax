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

        public PortsNode()
        {
            _children = new ObservableCollection<PortsNode>();
            Text = string.Empty;
        }

        public PortsNode(string text)
        {
            _children = new ObservableCollection<PortsNode>();
            Text = text;
        }

        public IList<PortsNode> Children => _children;
        public string Text { get; }

        public PortsNode Add(string text)
        {
            PortsNode result = new PortsNode(text);
            _children.Add(result);
            return result;
        }
    }
}
