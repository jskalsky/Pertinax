using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUaExplorer.Model
{
    public enum NodeIdType { Numeric, String = 3, Guid, ByteString}
    [Flags]
    public enum NodeClass { Unspecified, Object, Variable, Method = 4, ObjectType = 8, VariableType = 16, ReferenceType = 32, DataType = 64, View = 128}
    public class BrowseItem
    {
        public BrowseItem(BrowseResponse br)
        {
            NamespaceIndex = br.namespaceIndex;
            NodeIdType = (NodeIdType)br.identifierType;
            if(NodeIdType == NodeIdType.Numeric)
            {
                Numeric = br.numeric;
                IdString = string.Empty;
            }
            else
            {
                if(NodeIdType == NodeIdType.String)
                {
                    IdString = Encoding.ASCII.GetString(br.str, 0, br.strLength);
                }
            }
            NodeClass = (NodeClass)br.nodeClass;
            BrowseName = (br.browseNameLength == 0) ? string.Empty : Encoding.ASCII.GetString(br.browseName, 0, br.browseNameLength);
            DisplayName = (br.displayNameLength == 0) ? string.Empty : Encoding.ASCII.GetString(br.displayName, 0, br.displayNameLength);
        }
        public ushort NamespaceIndex { get; }
        public NodeIdType NodeIdType { get; }
        public uint Numeric { get; }
        public string IdString { get; }
        public NodeClass NodeClass { get; }
        public string BrowseName { get; }
        public string DisplayName { get; }
    }
}
