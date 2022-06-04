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
            Guid = Guid.NewGuid();
            Debug.Print($"NodeIdString {namespaceIndex}:{id}:{Guid}");
        }
        public string IdentifierString { get; }

        public static NodeIdString GetNextNodeIdString(ushort namespaceIndex)
        {
            for (uint i = 0; i < EndVarsId - BaseVarsId; ++i)
            {
                string id = $"{namespaceIndex}:NodeIdString{i}";
                if (!ExistsNodeId(id))
                {
                    return new NodeIdString(namespaceIndex, $"NodeIdString{i}");
                }
            }

            return null;
        }

        public override string GetIdentifier()
        {
            return IdentifierString;
        }
    }
}
