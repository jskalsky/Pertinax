using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.DataModel
{
    public class NodeIdNumeric : NodeIdBase
    {
        public NodeIdNumeric(ushort namespaceIndex, uint identifier) : base(namespaceIndex, IdentifierType.Numeric)
        {
            IdentifierNumeric = identifier;
        }
        public uint IdentifierNumeric { get; set; }
        public override string GetNodeName()
        {
            return $"N:{NamespaceIndex}:{IdentifierNumeric}";
        }
        public override string ToString()
        {
            return GetNodeName();
        }
    }
}
