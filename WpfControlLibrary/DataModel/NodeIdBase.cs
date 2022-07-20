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
        public const ushort LastNamespaceIndex = 2;
        public const uint Divider = 50;

        private static Dictionary<ushort, HashSet<string>> _ids = new Dictionary<ushort, HashSet<string>>();
        private static Dictionary<ushort, uint> _nextSystemNumericId = new Dictionary<ushort, uint>();
        private static Dictionary<ushort, uint> _nextVarNumericId = new Dictionary<ushort, uint>();
        protected NodeIdBase(ushort namespaceIndex, IdentifierType identifierType)
        {
            NamespaceIndex = namespaceIndex;
            IdentifierType = identifierType;
        }
        public ushort NamespaceIndex { get; }
        public IdentifierType IdentifierType { get; }
        public static NodeIdBase GetNodeIdBase(string nodeId)
        {
            Debug.Print($"GetNodeIdBase {nodeId}");
            string[] items = nodeId.Split(':');
            Debug.Print($"items= {items.Length}");
            ushort ns = 0;
            if (items.Length != 3)
            {
                return null;
            }
            if (ushort.TryParse(items[1], out ushort namespaceIndex))
            {
                ns = namespaceIndex;
            }
            if (items[0] == "N")
            {
                if (uint.TryParse(items[2], out uint numeric))
                {
                    return new NodeIdNumeric(ns, numeric);
                }
            }
            else
            {
                if (items[0] == "S")
                {
                    return new NodeIdString(ns, items[2]);
                }
            }
            return null;
        }

        private static void Open()
        {
            if (_nextSystemNumericId.Count == 0)
            {
                for (ushort i = FirstNamespaceIndex; i <= LastNamespaceIndex; i++)
                {
                    _nextSystemNumericId[i] = FirstSystemNumericId;
                }
            }
            if (_ids.Count == 0)
            {
                for (ushort i = FirstNamespaceIndex; i <= LastNamespaceIndex; i++)
                {
                    _ids[i] = new HashSet<string>();
                }
            }
            if (_nextVarNumericId.Count == 0)
            {
                for (ushort i = FirstNamespaceIndex; i <= LastNamespaceIndex; i++)
                {
                    _nextVarNumericId[i] = FirstVarNumericId;
                }
            }
        }
        public static NodeIdNumeric GetNextSystemNodeId(ushort ns)
        {
            Open();
            if (_nextSystemNumericId[ns] + 1 >= FirstVarNumericId)
            {
                throw new ArgumentOutOfRangeException(nameof(ns), $"Počet systémových numerických identifikátorů uzlů mimo rozsah");
            }
            NodeIdNumeric nin = new NodeIdNumeric(ns, _nextSystemNumericId[ns]++);
            string nodeId = nin.GetNodeName();
            if (_ids[ns].Contains(nodeId))
            {
                throw new Exceptions.OpcUaException($"Systémový identifikátor {nodeId} již existuje");
            }
            _ids[ns].Add(nodeId);
            return nin;
        }
        public static NodeIdNumeric[] GetNextNodeIds(ushort ns, int count, bool round = false)
        {
            Open();
            if (_nextVarNumericId[ns] + count >= LastVarNumericId)
            {
                throw new ArgumentOutOfRangeException(nameof(ns), $"Počet numerických identifikátorů uzlů mimo rozsah");
            }
            NodeIdNumeric[] nodes = new NodeIdNumeric[count];
            if (round)
            {
                uint remainder = _nextVarNumericId[ns] % Divider;
                if (remainder > 0)
                {
                    _nextVarNumericId[ns] = _nextVarNumericId[ns] - remainder + Divider;
                }
            }
            for (int i = 0; i < count; i++)
            {
                nodes[i] = new NodeIdNumeric(ns, _nextVarNumericId[ns]++);
                string nodeId = nodes[i].GetNodeName();
                if (_ids[ns].Contains(nodeId))
                {
                    throw new Exceptions.OpcUaException($"Identifikátor {nodeId} již existuje");
                }
                _ids[ns].Add(nodeId);
            }
            return nodes;
        }

        public static void AddSystemNodeId(ushort ns, NodeIdBase id)
        {
            Debug.Print($"AddSystemNodeId ns= {ns}, id= {id}");
            Open();
            if (id is NodeIdNumeric num)
            {
                Debug.Print($"num.IdentifierNumeric={num.IdentifierNumeric}, _nextSystemNumericId[ns]={_nextSystemNumericId[ns]}");
                if (num.IdentifierNumeric >= _nextSystemNumericId[ns])
                {
                    _nextSystemNumericId[ns] = num.IdentifierNumeric + 1;
                    Debug.Print($"1 _nextSystemNumericId[ns]= {_nextSystemNumericId[ns]}");
                }
            }
            _ids[ns].Add(id.GetNodeName());
        }
        public static void AddVarNodeId(ushort ns, NodeIdBase id)
        {
            Open();
            if (id is NodeIdNumeric num)
            {
                if (num.IdentifierNumeric >= _nextVarNumericId[ns])
                {
                    _nextVarNumericId[ns] = num.IdentifierNumeric + 1;
                }
            }
            _ids[ns].Add(id.GetNodeName());
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
