using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfControlLibrary
{
    public enum IdentifierType { Unknown, Numeric, String }
    public abstract class NodeIdBase
    {
        protected const uint BaseObjectsId = 80000;
        protected const uint BaseVarsId = 85000;
        protected const uint EndVarsId = 100000;

        private static HashSet<string> _ids = new HashSet<string>();
        protected NodeIdBase(ushort namespaceIndex, IdentifierType identifierType)
        {
            NamespaceIndex = namespaceIndex;
            IdentifierType = identifierType;
        }
        public ushort NamespaceIndex { get; set; }
        public IdentifierType IdentifierType { get; }

        protected void Add(string nodeId)
        {
            Debug.Print($"Add {nodeId}, {_ids.Count}");
            if(_ids.Contains(nodeId))
            {
                Debug.Print("Existuje");
                MessageBox.Show($"Identifikátor uzlu {nodeId} již existuje", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Debug.Print("Neexistuje");
                _ids.Add(nodeId);
            }
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

        public static bool ExistsNodeId(string nodeId)
        {
            return _ids.Contains(nodeId);
        }

        public static void Clear()
        {
            Debug.Print("NodeIdBase.Clear()");
            _ids.Clear();
        }
        public abstract string GetIdentifier();
    }
}
