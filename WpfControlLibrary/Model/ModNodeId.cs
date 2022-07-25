using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    public abstract class ModNodeId
    {
        public ModNodeId(ushort ns)
        {
            Ns = ns;
        }

        public ushort Ns { get; }
        public abstract string GetText();
        public static ModNodeId GetModNodeId(string nodeId)
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
                    return new ModNodeIdNumeric(ns, numeric);
                }
            }
            else
            {
                if (items[0] == "S")
                {
                    return new ModNodeIdString(ns, items[2]);
                }
            }
            return null;
        }
    }
}
