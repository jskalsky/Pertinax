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
        private static readonly Dictionary<ushort, LinkedList<int>> _freeIndexes = new Dictionary<ushort, LinkedList<int>>();
        private static readonly Dictionary<ushort, HashSet<string>> _names = new Dictionary<ushort, HashSet<string>>();
        private static readonly Dictionary<ushort, LinkedList<int>> _freeNumericIds = new Dictionary<ushort, LinkedList<int>>();
        private static readonly Dictionary<ushort, HashSet<string>> _ids = new Dictionary<ushort, HashSet<string>>();

        private static readonly Dictionary<ushort, HashSet<string>> _publishedNames = new Dictionary<ushort, HashSet<string>>();
        private static readonly Dictionary<ushort, HashSet<string>> _publishedIds = new Dictionary<ushort, HashSet<string>>();

        public const string NameNamespace = "Ns";
        public const string NameFolder = "Folder";
        public const string NameObjectType = "ObjectType";
        public const string NameSimpleVar = "Var";
        public const string NameArrayVar = "VarArray";
        public const string NameObjectVar = "VarObject";
        private static readonly List<string> _baseNames = new List<string>() { NameNamespace, NameFolder, NameObjectType, NameSimpleVar, NameArrayVar, NameObjectVar };

        //        private const int IndexesAmountPlus = 10000;
        private const int IndexesAmountPlus = 10;

        private static int _maxNameIndex = 0;
        private static int _maxNumericId = 0;
        public static string[] GetNames(ushort ns, string nameBase, int count = 1)
        {
            Debug.Print("GetNames");
            string[] names = GetFreeN(ns, _freeIndexes, _names, nameBase, count, ref _maxNameIndex).ToArray();
            foreach (string name in names)
            {
                if (!_publishedNames.TryGetValue(ns, out HashSet<string> nameSet))
                {
                    nameSet = new HashSet<string>();
                    _publishedNames.Add(ns, nameSet);
                }
                nameSet.Add(name);
            }
            return names;
        }

        public static string[] GetNumericIds(ushort ns, int count = 1)
        {
            Debug.Print("GetNumericIds");
            string[] ids = GetFreeN(ns, _freeNumericIds, _ids, null, count, ref _maxNumericId).ToArray();
            foreach (string id in ids)
            {
                if (!_publishedIds.TryGetValue(ns, out HashSet<string> idSet))
                {
                    idSet = new HashSet<string>();
                    _publishedIds.Add(ns, idSet);
                }
                idSet.Add(id);
            }
            return ids;
        }

        public static void RemovePublishedNames(ushort ns, string[] names)
        {
            foreach (string name in names)
            {
                if (_publishedNames.TryGetValue(ns, out HashSet<string> nameSet))
                {
                    nameSet.Remove(name);
                }
            }
        }

        public static void RemoveAllPublishedNames(ushort ns)
        {
            Debug.Print($"RemoveAllPublishedNames");
            if (_publishedNames.TryGetValue(ns, out HashSet<string> nameSet))
            {
                List<string> names = new List<string>();
                foreach(string name in nameSet)
                {
                    names.Add(name);
                }
                ReturnN(ns, _freeIndexes, _names, names);
                nameSet.Clear();
            }
        }

        public static void RemovePublishedIds(ushort ns, string[] ids)
        {
            foreach (string name in ids)
            {
                if (_publishedIds.TryGetValue(ns, out HashSet<string> idSet))
                {
                    idSet.Remove(name);
                }
            }
        }

        public static void RemoveAllPublishedIds(ushort ns)
        {
            Debug.Print($"RemoveAllPublishedIds");
            if (_publishedIds.TryGetValue(ns, out HashSet<string> idSet))
            {
                List<string> names = new List<string>();
                foreach (string name in idSet)
                {
                    names.Add(name);
                }
                ReturnN(ns, _freeNumericIds, _ids, names);
                idSet.Clear();
            }
        }

        private static IList<string> GetFreeN(ushort ns, Dictionary<ushort, LinkedList<int>> idxs, Dictionary<ushort, HashSet<string>> names, string nameBase, int count,
            ref int maxIndex)
        {
            if (!idxs.TryGetValue(ns, out LinkedList<int> ll))
            {
                ll = new LinkedList<int>();
                for (int i = 1; i < IndexesAmountPlus; i++)
                {
                    ll.AddLast(i);
                    Debug.Print($"1AddLast= {i}, ns= {ns}");
                    maxIndex = i;
                }
                idxs[ns] = ll;
            }
            Debug.Print($"indexesCount= {ll.Count}, count= {count}");
            while (ll.Count < count)
            {
                int mi = maxIndex + 1;
                for (int i = 0; i < IndexesAmountPlus; ++i)
                {
                    ll.AddLast(mi + i);
                    Debug.Print($"2AddLast= {i}");
                    maxIndex = mi + i;
                }
            }
            if (!names.TryGetValue(ns, out HashSet<string> existingNames))
            {
                existingNames = new HashSet<string>();
                names[ns] = existingNames;
            }
            List<string> result = new List<string>();
            for (int i = 0; i < count; ++i)
            {
                Debug.Print($"ll.Count= {ll.Count}");
                string s = (string.IsNullOrEmpty(nameBase)) ? $"{ll.First.Value}" : $"{nameBase}{ll.First.Value}";
                result.Add(s);
                ll.RemoveFirst();
                names[ns].Add(s);
            }
            foreach (int node in ll)
            {
                Debug.Print($"ll= {node}");
            }
            foreach (string s in result)
            {
                Debug.Print($"GetFreeN= {s}");
            }
            return result;
        }

        private static void ReturnN(ushort ns, Dictionary<ushort, LinkedList<int>> idxs, Dictionary<ushort, HashSet<string>> names, IList<string> returningNames)
        {
            foreach (string s in returningNames)
            {
                Debug.Print($"Returning {s}");
                bool founded = false;
                foreach(string baseName in _baseNames)
                {
                    int idx = s.IndexOf(baseName);
                    if (idx == 0)
                    {
                        string index = s.Remove(0, baseName.Length);
                        if (int.TryParse(index, out int indexInt))
                        {
                            if (idxs.TryGetValue(ns, out LinkedList<int> idxList))
                            {
                                AddSort(idxList, indexInt);
                                founded = true;
                                Debug.Print($"founded {s}, {baseName}, {indexInt}");
                                break;
                            }
                        }
                    }

                }
                if (!founded)
                {
                    Debug.Print($"Nenasel {s}");
                    if (int.TryParse(s, out int indexInt))
                    {
                        Debug.Print($"indexInt= {indexInt}");
                        if (idxs.TryGetValue(ns, out LinkedList<int> idxList))
                        {
                            AddSort(idxList, indexInt);
                        }
                    }
                }
                if (names.TryGetValue(ns, out HashSet<string> namesList))
                {
                    namesList.Remove(s);
                }
            }
        }

        private static void AddSort(LinkedList<int> ll, int val)
        {
            if (val < ll.First.Value)
            {
                ll.AddFirst(val);
            }
            else
            {
                if (val > ll.Last.Value)
                {
                    ll.AddLast(val);
                }
                else
                {
                    for (LinkedListNode<int> node = ll.First; node != null; node = node.Next)
                    {
                        if (val > node.Value)
                        {
                            ll.AddAfter(node, val);
                            break;
                        }
                    }
                }
            }
        }
    }
}
