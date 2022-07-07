using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.DataModel
{
    public class NodeIdString : NodeIdBase
    {
        public NodeIdString(ushort namespaceIndex, string id) : base(namespaceIndex, IdentifierType.String)
        {
            IdentifierString = id;
        }
        public string IdentifierString { get; }
        public override string GetNodeName()
        {
            return $"S:{NamespaceIndex}:{IdentifierString}";
        }
    }
}
