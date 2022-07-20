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
            Debug.Print($"NodeIdString {namespaceIndex}:{id}");
            IdentifierString = id;
        }
        public string IdentifierString { get; }
        public override string GetNodeName()
        {
            return $"S:{NamespaceIndex}:{IdentifierString}";
        }

        public override string ToString()
        {
            return GetNodeName();
        }
    }
}
