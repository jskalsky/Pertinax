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
        public const int NumericIdObjectBase = 80000;
        public const int NumericIdBase = 85000;
        private static HashSet<string> _idsString = new HashSet<string>();
        private static readonly SortedSet<uint> _idsNumeric = new SortedSet<uint>();
        private static readonly SortedSet<uint> _idsNumericObjects = new SortedSet<uint>();
        public static NodeIdType NodeIdType { get; private set; } = NodeIdType.Unknown;
        public static string GetNextNumericId()
        {
            uint id = NumericIdBase;
            foreach (uint numeric in _idsNumeric)
            {
                if (id != numeric)
                {
                    break;
                }

                ++id;
            }

            if (!_idsNumeric.Add(id))
            {
                return string.Empty;
            }

            return $"{id}";
        }

        public static string GetNextObjectNumericId()
        {
            uint id = NumericIdObjectBase;
            foreach (uint numeric in _idsNumericObjects)
            {
                if (id != numeric)
                {
                    break;
                }

                ++id;
            }

            if (!_idsNumericObjects.Add(id))
            {
                return string.Empty;
            }

            return $"{id}";
        }

        public static bool AddNumericId(string idS)
        {
            if(uint.TryParse(idS, out var id))
            {
                return _idsNumeric.Add(id);
            }

            return false;
        }

        public static bool AddObjectNumericId(string idS)
        {
            if (uint.TryParse(idS, out var id))
            {
                return _idsNumericObjects.Add(id);
            }

            return false;
        }

        public static bool AddId(string idS)
        {
            if (uint.TryParse(idS, out uint id))
            {
                return _idsNumeric.Add(id);
            }

            return _idsString.Add(idS);
        }
    }
}
