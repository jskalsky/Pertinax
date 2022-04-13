using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public static class NodeId
    {
        public static string Id { get; private set; } = "80000";

        public static string NextId()
        {
            if (uint.TryParse(Id, out uint id))
            {
                ++id;
                Id = $"{id}";
            }
            return Id;
        }

        public static void SetId(string oldId)
        {
            if (uint.TryParse(oldId, out uint oldid))
            {
                if (uint.TryParse(Id, out uint id))
                {
                    if (oldid > id)
                    {
                        Id = $"{oldid}";
                    }
                }
            }
            Debug.Print($"SetId= {oldId}, {Id}");
        }
    }
}
