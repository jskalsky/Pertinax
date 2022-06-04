using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.DataModel
{
    public enum IdentifierType { Unknown, Numeric, String }
    public abstract class NodeIdBase
    {
        protected const uint BaseObjectsId = 80000;
        protected const uint BaseVarsId = 85000;
        protected const uint EndVarsId = 100000;

        private static Dictionary<string, List<NodeIdBase>> _ids = new Dictionary<string, List<NodeIdBase>>();
        protected NodeIdBase(ushort namespaceIndex, IdentifierType identifierType)
        {
            NamespaceIndex = namespaceIndex;
            IdentifierType = identifierType;
        }
        public ushort NamespaceIndex { get; set; }
        public IdentifierType IdentifierType { get; }
        public Guid Guid { get; protected set; }
        public string NodeIdString
        {
            get { return $"{NamespaceIndex}:{GetIdentifier()}"; }
        }
        public static NodeIdBase GetNodeIdBase(string nodeId)
        {
            string[] items = nodeId.Split(':');
            ushort ns = 0;
            if (ushort.TryParse(items[0], out ushort namespaceIndex))
            {
                ns = namespaceIndex;
            }

            if (uint.TryParse(items[1], out uint numeric))
            {
                return new NodeIdNumeric(ns, numeric);
            }
            return new NodeIdString(ns, items[1]);
        }

        public static void PrintIds()
        {
            Debug.Print("IDS");
            foreach (KeyValuePair<string, List<NodeIdBase>> pair in _ids)
            {
                Debug.Print($"{pair.Key}");
                foreach (NodeIdBase nib in pair.Value)
                {
                    Debug.Print($"     {nib.NodeIdString}, {nib.Guid}");
                }
            }
        }
        public static bool ExistsNodeId(string nodeId)
        {
            Debug.Print($"ExistsNodeId {nodeId}");
            PrintIds();
            return _ids.ContainsKey(nodeId);
        }

        public static void Clear()
        {
            Debug.Print("NodeIdBase.Clear()");
            _ids.Clear();
        }

        public static bool Remove(NodeIdBase nodeId)
        {
            Debug.Print($"Remove {nodeId.NodeIdString}, {nodeId.Guid}");
            if (_ids.TryGetValue(nodeId.NodeIdString, out List<NodeIdBase> lnid))
            {
                Debug.Print($"Nasel {nodeId.NodeIdString}");
                bool founded = false;
                foreach (NodeIdBase nib in lnid)
                {
                    Debug.Print($"nib= {nib.Guid}");
                    if (nib.Guid == nodeId.Guid)
                    {
                        Debug.Print($"2 Nasel {nib.NodeIdString}");
                        founded = true;
                        break;
                    }
                }

                if (founded)
                {
                    lnid.Remove(nodeId);
                    if (lnid.Count == 0)
                    {
                        _ids.Remove(nodeId.NodeIdString);
                    }
                    return true;
                }
            }
            return false;
        }

        public static void Add(NodeIdBase nodeId)
        {
            Debug.Print($"NodeIdBase.Add({nodeId.NodeIdString}, {nodeId.Guid})");
            if (!_ids.TryGetValue(nodeId.NodeIdString, out List<NodeIdBase> lnib))
            {
                lnib = new List<NodeIdBase>();
                _ids[nodeId.NodeIdString] = lnib;
            }
            lnib.Add(nodeId);
        }

        public static int GetNrOfErrors()
        {
            int nr = 0;
            foreach (KeyValuePair<string, List<NodeIdBase>> kv in _ids)
            {
                if (kv.Value.Count > 1)
                {
                    ++nr;
                }
            }
            return nr;
        }
        public abstract string GetIdentifier();
    }
}
