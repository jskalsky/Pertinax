using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    public static class NodeIdFactory
    {
        public static uint FirstNumericNodeId = 10000;
        public static uint LastNumericNodeId = 100000;

        private static uint[] _nextNodeId = new uint[] { FirstNumericNodeId, FirstNumericNodeId, FirstNumericNodeId };
        private static Dictionary<ushort, HashSet<string>> _nodeIds = new Dictionary<ushort, HashSet<string>>() { {0, new HashSet<string>() }, {1, new HashSet<string>()},
            {2, new HashSet<string>()}};

        public static string GetNextNodeId(ushort ns)
        {
            return $"N:{ns}:{_nextNodeId[ns]}";
        }
        public static void SetNextNodeId(string nodeId)
        {
            string[] items = nodeId.Split(':');
            if (items.Length == 3)
            {
                if (ushort.TryParse(items[1], out ushort ns))
                {
                    if (items[0] == "N")
                    {
                        if (uint.TryParse(items[2], out uint numeric))
                        {
                            if (numeric >= _nextNodeId[ns])
                            {
                                _nextNodeId[ns] = numeric + 1;
                            }
                        }
                    }
                    if(_nodeIds.TryGetValue(ns, out HashSet<string> nodeIds))
                    {
                        nodeIds.Add(nodeId);
                    }
                }
            }
        }
    }
}
