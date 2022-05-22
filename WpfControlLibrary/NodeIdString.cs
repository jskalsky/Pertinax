using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public class NodeIdString : NodeIdBase
    {
        public NodeIdString(ushort namespaceIndex, string id) : base(namespaceIndex, IdentifierType.String)
        {
            IdentifierString = id;
            Add($"{namespaceIndex}:{id}");
        }
        public string IdentifierString { get; }

        public static NodeIdString GetNextNodeIdString(ushort namespaceIndex)
        {
            for (uint i = 0; i < EndVarsId - BaseVarsId; ++i)
            {
                string id = $"{namespaceIndex}:NodeIdString{i}";
                if (!ExistsNodeId(id))
                {
                    return new NodeIdString(namespaceIndex, id);
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
