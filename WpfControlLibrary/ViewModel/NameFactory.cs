using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    public static class NameFactory
    {
        public const string NameNamespace = "Ns";
        public const string NameFolder = "Folder";
        public const string NameObjectType = "ObjectType";
        public const string NameSimpleVar = "SimpleVar";
        public const string NameArrayVar = "ArrayVar";
        public const string NameObjectVar = "ObjectVar";
        public const string NameClient = "Client";
        public const string NameClientGroup = "ClientGroup";
        public const string NameClientVar = "ClientVar";

        public static uint FirstNameIndex = 1;
        public static uint LastNameIndex = 50000;

        private static uint[] _nextNameIndex = new uint[] { FirstNameIndex, FirstNameIndex, FirstNameIndex };
        private static Dictionary<ushort, HashSet<string>> _nodeIds = new Dictionary<ushort, HashSet<string>>() { {0, new HashSet<string>() }, {1, new HashSet<string>()},
            {2, new HashSet<string>()}};
        private static string[] _prefixes = new string[] {NameArrayVar, NameObjectVar, NameClient, NameClientGroup, NameClientVar, NameFolder, NameNamespace, NameObjectType,
            NameSimpleVar};

        public static string NextName(ushort ns, string prefix)
        {
            return $"{prefix}{_nextNameIndex[ns]}";
        }
        public static void SetName(ushort ns, string name)
        {
            foreach(string prefix in _prefixes)
            {
                int idx = name.IndexOf(prefix);
                if (idx == 0)
                {
                    string indexS = name.Remove(0, prefix.Length);
                    if(uint.TryParse(indexS, out uint index))
                    {
                        if(index >= _nextNameIndex[ns])
                        {
                            _nextNameIndex[ns] = index + 1;
                        }
                    }
                }
            }
            if(_nodeIds.TryGetValue(ns, out HashSet<string> nodeIds))
            {
                nodeIds.Add(name);
            }
        }
    }
}
