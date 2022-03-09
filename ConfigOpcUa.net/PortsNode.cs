using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigOpcUaNet
{
    public class PortsNode
    {
        private readonly List<PortsNode> _children;

        public PortsNode()
        {
            _children = new List<PortsNode>();
            Text = string.Empty;
        }

        public PortsNode(string text)
        {
            _children = new List<PortsNode>();
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
