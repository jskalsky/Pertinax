using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public class NodeIdNumeric : NodeIdBase
    {
        public NodeIdNumeric(ushort namespaceIndex, uint identifier) : base(namespaceIndex, IdentifierType.Numeric)
        {
            IdentifierNumeric = identifier;
            Guid = Guid.NewGuid();
            Debug.Print($"NodeIdNumeric {namespaceIndex}:{identifier}:{Guid}");
        }
        public uint IdentifierNumeric { get; }

        public static NodeIdNumeric GetNextNodeIdNumeric(ushort namespaceIndex, bool isObject = true)
        {
            if (isObject)
            {
                for (uint i = BaseObjectsId; i < BaseVarsId; ++i)
                {
                    string id = $"{namespaceIndex}:{i}";
                    if (!ExistsNodeId(id))
                    {
                        return new NodeIdNumeric(namespaceIndex,i);
                    }
                }

                return null;
            }

            for (uint i = BaseVarsId; i <= EndVarsId; ++i)
            {
                string id = $"{namespaceIndex}:{i}";
                if (!ExistsNodeId(id))
                {
                    return new NodeIdNumeric(namespaceIndex, i);
                }
            }

            return null;
        }

        public override string GetIdentifier()
        {
            return $"{IdentifierNumeric}";
        }
    }
}
