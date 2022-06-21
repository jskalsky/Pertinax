using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.DataModel
{
    public static class IdFactory
    {
        private static readonly Dictionary<ushort, int> _nextFolderIndex = new Dictionary<ushort, int>();
        private static readonly Dictionary<ushort, int> _nextObjectTypeIndex = new Dictionary<ushort, int>();
        private static readonly Dictionary<ushort, int> _nextSimpleVarIndex = new Dictionary<ushort, int>();
        private static readonly Dictionary<ushort, int> _nextArrayVarIndex = new Dictionary<ushort, int>();
        private static readonly Dictionary<ushort, int> _nextObjectVarIndex = new Dictionary<ushort, int>();
        private static readonly Dictionary<ushort, int> _nextNumericIdIndex = new Dictionary<ushort, int>();

        private static readonly Dictionary<ushort, HashSet<string>> _names = new Dictionary<ushort, HashSet<string>>();
        private static readonly Dictionary<ushort, HashSet<string>> _ids = new Dictionary<ushort, HashSet<string>>();

        public const string NameNamespace = "Ns";
        public const string NameFolder = "Folder";
        public const string NameObjectType = "ObjectType";
        public const string NameSimpleVar = "Var";
        public const string NameArrayVar = "VarArray";
        public const string NameObjectVar = "VarObject";
        private static readonly List<string> _baseNames = new List<string>() { NameNamespace, NameFolder, NameObjectType, NameSimpleVar, NameArrayVar, NameObjectVar };

        //        private const int IndexesAmountPlus = 10000;
        public static string[] GetNames(ushort ns, string nameBase, int count = 1)
        {
            Debug.Print("GetNames");
            switch (nameBase)
            {
                case NameFolder:
                    return GetFreeNames(ns, _nextFolderIndex, nameBase, count);
                case NameObjectType:
                    return GetFreeNames(ns, _nextObjectTypeIndex, nameBase, count);
                case NameSimpleVar:
                    return GetFreeNames(ns, _nextSimpleVarIndex, nameBase, count);
                case NameArrayVar:
                    return GetFreeNames(ns, _nextArrayVarIndex, nameBase, count);
                case NameObjectVar:
                    return GetFreeNames(ns, _nextObjectVarIndex, nameBase, count);
            }
            return null;
        }

        public static string[] GetNumericIds(ushort ns, int count = 1)
        {
            Debug.Print("GetNumericIds");
            if (!_nextNumericIdIndex.TryGetValue(ns, out int idx))
            {
                idx = 1;
                _nextNumericIdIndex[ns] = idx;
            }
            if (!_ids.TryGetValue(ns, out HashSet<string> ids))
            {
                ids = new HashSet<string>();
                _ids[ns] = ids;
            }
            List<string> result = new List<string>();
            for (int i = 0; i < count; ++i)
            {
                string id = $"{idx}";
                result.Add(id);
                ++idx;
                ids.Add(id);
            }
            return result.ToArray();
        }
        private static string[] GetFreeNames(ushort ns, Dictionary<ushort, int> idxs, string nameBase, int count)
        {
            if (!idxs.TryGetValue(ns, out int idx))
            {
                idx = 1;
                idxs[ns] = idx;
            }
            if (!_names.TryGetValue(ns, out HashSet<string> names))
            {
                names = new HashSet<string>();
                _names[ns] = names;
            }
            List<string> result = new List<string>();
            for (int i = 0; i < count; i++)
            {
                string name = $"{nameBase}{idx}";
                names.Add(name);
                result.Add(name);
                ++idx;
            }
            idxs.Remove(ns);
            idxs[ns] = idx;
            return result.ToArray();
        }
        public static bool NameExists(ushort ns, string name)
        {
            if (_names.TryGetValue(ns, out HashSet<string> namesList))
            {
                return namesList.Contains(name);
            }
            return false;
        }

        public static void AddName(ushort ns, string nameBase, string name)
        {
            Debug.Print($"AddName {ns}, {nameBase}:{name}");
            switch (nameBase)
            {
                case NameFolder:
                    IncrementMaxNameIndex(ns, _nextFolderIndex, nameBase, name);
                    break;
                case NameObjectType:
                    IncrementMaxNameIndex(ns, _nextObjectTypeIndex, nameBase, name);
                    break;
                case NameSimpleVar:
                    IncrementMaxNameIndex(ns, _nextSimpleVarIndex, nameBase, name);
                    break;
                case NameArrayVar:
                    IncrementMaxNameIndex(ns, _nextArrayVarIndex, nameBase, name);
                    break;
                case NameObjectVar:
                    IncrementMaxNameIndex(ns, _nextObjectVarIndex, nameBase, name);
                    break;
            }
        }

        private static void IncrementMaxNameIndex(ushort ns, Dictionary<ushort, int> idxs, string nameBase, string name)
        {
            if (!idxs.TryGetValue(ns, out int idx))
            {
                idx = 1;
                idxs[ns] = idx;
            }
            if (!_names.TryGetValue(ns, out HashSet<string> namesList))
            {
                namesList = new HashSet<string>();
                _names[ns] = namesList;
            }
            namesList.Add(name);
            int index = name.IndexOf(nameBase);
            if (index == 0)
            {
                string indexInString = name.Substring(0, nameBase.Length);
                if (int.TryParse(indexInString, out int i))
                {
                    if (i > idx)
                    {
                        idxs.Remove(ns);
                        idxs[ns] = i;
                    }
                }
            }
        }

        public static void AddNumericId(ushort ns, string id)
        {
            if (!_nextNumericIdIndex.TryGetValue(ns, out int idx))
            {
                idx = 1;
                _nextNumericIdIndex[ns] = idx;
            }
            if (!_ids.TryGetValue(ns, out HashSet<string> ids))
            {
                ids = new HashSet<string>();
                _ids[ns] = ids;
            }
            ids.Add(id);
            if (int.TryParse(id, out int i))
            {
                if (i > idx)
                {
                    _nextNumericIdIndex.Remove(ns);
                    _nextNumericIdIndex[ns] = i;
                }
            }
        }
    }
}
