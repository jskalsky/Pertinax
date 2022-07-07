using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.DataModel
{
    public enum IdentifierType { Unknown, Numeric, String, Guid, ByteString }
    // Id jako string ma format N:namespace index:id numericky,  S:namespace index:id string, G:namespace index:id Guid, B:namespace index:id Byte string
    public abstract class NodeIdBase
    {
        public const uint FirstSystemNumericId = 1;
        public const uint FirstVarNumericId = 10000;
        public const uint LastVarNumericId = 100000;
        public const ushort FirstNamespaceIndex = 0;
        public const ushort LastNamespaceIndex = 8;
        public const uint Divider = 50;

        private static HashSet<string> _ids = new HashSet<string>();
        private static uint _nextSystemNumericId = FirstSystemNumericId;
        private static uint _nextVarNumericId = FirstVarNumericId;
        protected NodeIdBase(ushort namespaceIndex, IdentifierType identifierType)
        {
            NamespaceIndex = namespaceIndex;
            IdentifierType = identifierType;
        }
        public ushort NamespaceIndex { get; }
        public IdentifierType IdentifierType { get; }
        public static NodeIdBase GetNodeIdBase(string nodeId)
        {
            string[] items = nodeId.Split(':');
            ushort ns = 0;
            if (items.Length != 3)
            {
                return null;
            }
            if (ushort.TryParse(items[0], out ushort namespaceIndex))
            {
                ns = namespaceIndex;
            }
            if (items[1] =="N")
            {
                if (uint.TryParse(items[2], out uint numeric))
                {
                    return new NodeIdNumeric(ns, numeric);
                }
            }
            else
            {
                if (items[1]=="S")
                {
                    return new NodeIdString(ns, items[2]);
                }
            }
            return null;
        }
        public static NodeIdNumeric GetNextSystemNodeId(ushort ns)
        {
            if(_nextSystemNumericId + 1 >=  FirstVarNumericId)
            {
                throw new ArgumentOutOfRangeException(nameof(ns), $"Počet systémových numerických identifikátorů uzlů mimo rozsah");
            }
            NodeIdNumeric nin = new NodeIdNumeric(ns,_nextSystemNumericId++);
            string nodeId = nin.GetNodeName();
            if(_ids.Contains(nodeId))
            {
                throw new Exceptions.OpcUaException($"Systémový identifikátor {nodeId} již existuje");
            }
            _ids.Add(nodeId);
            return nin;
        }
        public static NodeIdNumeric[] GetNextNodeIds(ushort ns, int count, bool round=true)
        {
            if(_nextVarNumericId + count >= LastVarNumericId)
            {
                throw new ArgumentOutOfRangeException(nameof(ns), $"Počet numerických identifikátorů uzlů mimo rozsah");
            }
            NodeIdNumeric[] nodes = new NodeIdNumeric[count];
            if(round)
            {
                uint remainder = _nextVarNumericId % Divider;
                _nextVarNumericId = _nextVarNumericId - remainder + Divider;
            }
            for(int i=0;i< count; i++)
            {
                nodes[i] = new NodeIdNumeric(ns,_nextVarNumericId++);
                string nodeId = nodes[i].GetNodeName();
                if (_ids.Contains(nodeId))
                {
                    throw new Exceptions.OpcUaException($"Identifikátor {nodeId} již existuje");
                }
                _ids.Add(nodeId);
            }
            return nodes;
        }
        public static bool ExistsNodeId(string nodeId)
        {
            Debug.Print($"ExistsNodeId {nodeId}");
            return false;
        }

        public static void Clear()
        {
            Debug.Print("NodeIdBase.Clear()");
            _ids.Clear();
        }
        public abstract string GetNodeName();
    }
}
