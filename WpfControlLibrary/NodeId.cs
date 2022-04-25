using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public enum NodeIdType { Unknown, Numeric, String }
    public static class NodeId
    {
        public const string DefaultNodeIdName = "NodeId";
        public static string NodeIdString { get; private set; }
        public static uint NodeIdNumeric { get; private set; } = 80000;
        public static NodeIdType NodeIdType { get; private set; } = NodeIdType.Unknown;
        public static int NextNodeIdIndex { get; private set; } = 1;
        public static string GetNextNumericId()
        {
            return $"{NodeIdNumeric++}";
        }

        public static string GetNextStringId()
        {
            return $"{DefaultNodeIdName}{NextNodeIdIndex++}";
        }

        public static void CorrectNumericId(string idS)
        {
            uint id = 0;
            if(uint.TryParse(idS, out id))
            {
                if(id > NodeIdNumeric)
                {
                    NodeIdNumeric = id + 1;
                }
            }
        }

        public static void CorrectStringId(string idS)
        {
            int index = idS.IndexOf(DefaultNodeIdName);
            if(index >= 0 && index + DefaultNodeIdName.Length  < idS.Length)
            {
                string nr = idS.Substring(index + 1);
                if(!string.IsNullOrEmpty(nr))
                {
                    int nnii = 0;
                    if(int.TryParse(nr, out nnii))
                    {
                        if(nnii >  NextNodeIdIndex)
                        {
                            NextNodeIdIndex = nnii + 1;
                        }
                    }
                }
            }
        }
    }
}
