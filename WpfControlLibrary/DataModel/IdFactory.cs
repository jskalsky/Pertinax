using System;
using System.Collections.Generic;
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

        public const string NameNamespace = "Ns";
        public const string NameFolder = "Folder";
        public const string NameObjectType = "ObjectType";
        public const string NameSimpleVar = "Var";
        public const string NameArrayVar = "VarArray";
        public const string NameObjectVar = "VarObject";

        private const int IndexesAmountPlus = 10000;

        private static int _nameIndexesCount = 0;
        private static int _numericIdsCount = 0;
        public static string[] GetNames(ushort ns, string nameBase, int count = 1)
        {
            return GetFreeN(ns, _freeIndexes, _names, nameBase, count, ref _nameIndexesCount).ToArray();
        }

        public static string[] GetNumericIds(ushort ns, int count = 1)
        {
            return GetFreeN(ns, _freeNumericIds, _ids, null, count, ref _numericIdsCount).ToArray();
        }

        public static void ReturnNames(IList<string> names, string baseName)
        {

        }
        private static IList<string> GetFreeN(ushort ns, Dictionary<ushort, LinkedList<int>> idxs, Dictionary<ushort, HashSet<string>> names, string nameBase, int count, 
            ref int indexesCount)
        {
            if (!idxs.TryGetValue(ns, out LinkedList<int> ll))
            {
                ll = new LinkedList<int>();
                for (int i = 1; i < IndexesAmountPlus; i++)
                {
                    ll.AddLast(i);
                }
                indexesCount = IndexesAmountPlus;
            }
            while(indexesCount < count)
            {
                for(int i = indexesCount; i < IndexesAmountPlus; ++i)
                {
                    ll.AddLast(i);
                }
                indexesCount += IndexesAmountPlus;
            }
            if (!names.TryGetValue(ns, out HashSet<string> existingNames))
            {
                existingNames = new HashSet<string>();
                names[ns] = existingNames;
            }
            List<string> result = new List<string>();
            for(int i=0; i<count; ++i)
            {
                string s = (string.IsNullOrEmpty(nameBase))? $"{ll.First.Value}" : $"{nameBase}{ll.First.Value}";
                result.Add(s);
                ll.RemoveFirst();
                names[ns].Add(s);
            }
            return result;
        }

        private static void ReturnN(ushort ns, Dictionary<ushort, LinkedList<int>> idxs, Dictionary<ushort, HashSet<string>> names, string nameBase, IList<string> returningNames)
        {
            foreach(string s in returningNames)
            {
                if(!string.IsNullOrEmpty(nameBase))
                {
                    int idx = s.IndexOf(nameBase);
                    if(idx == 0)
                    {
                        string index = s.Remove(0,nameBase.Length);
                        if(int.TryParse(index, out int indexInt))
                        {
                            if(idxs.TryGetValue(ns, out LinkedList<int> idxList))
                            {
                                AddSort(idxList, indexInt);
                            }
                        }
                    }
                }
                else
                {
                    if(int.TryParse(s, out int indexInt))
                    {
                        if (idxs.TryGetValue(ns, out LinkedList<int> idxList))
                        {
                            AddSort(idxList, indexInt);
                        }
                    }
                }
                if(names.TryGetValue(ns, out HashSet<string> namesList))
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
                    for(LinkedListNode<int> node = ll.First; node != null; node = node.Next)
                    {
                        if(val > node.Value)
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
