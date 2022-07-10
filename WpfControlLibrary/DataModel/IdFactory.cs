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
        public const string NameNamespace = "Ns";
        public const string NameFolder = "Folder";
        public const string NameObjectType = "ObjectType";
        public const string NameSimpleVar = "Var";
        public const string NameArrayVar = "VarArray";
        public const string NameObjectVar = "VarObject";
        public const int MaxFolders = 1000;
        public const int MaxObjectTypes = 1000;
        public const int MaxSimpleVars = 80000;
        public const int MaxArrayVars = 1000;
        public const int MaxObjects = 1000;
        public const int MaxNamespaces = 3;

        private static Dictionary<ushort, LinkedList<string>> _folders = new Dictionary<ushort, LinkedList<string>>();
        private static Dictionary<ushort, LinkedList<string>> _objectTypes = new Dictionary<ushort, LinkedList<string>>();
        private static Dictionary<ushort, LinkedList<string>> _simpleVars = new Dictionary<ushort, LinkedList<string>>();
        private static Dictionary<ushort, LinkedList<string>> _arrayVars = new Dictionary<ushort, LinkedList<string>>();
        private static Dictionary<ushort, LinkedList<string>> _objects = new Dictionary<ushort, LinkedList<string>>();

        static IdFactory()
        {
            for (ushort ns = 0; ns < MaxNamespaces; ns++)
            {
                _folders[ns] = new LinkedList<string>();
                for (int i = 1; i < MaxFolders; i++)
                {
                    _folders[ns].AddLast($"{NameFolder}{i}");
                }
                _objectTypes[ns] = new LinkedList<string>();
                for (int i = 1; i < MaxObjectTypes; i++)
                {
                    _objectTypes[ns].AddLast($"{NameObjectType}{i}");
                }
                _simpleVars[ns] = new LinkedList<string>();
                for (int i = 1; i < MaxSimpleVars; i++)
                {
                    _simpleVars[ns].AddLast($"{NameSimpleVar}{i}");
                }
                _arrayVars[ns] = new LinkedList<string>();
                for (int i = 1; i < MaxArrayVars; i++)
                {
                    _arrayVars[ns].AddLast($"{NameArrayVar}{i}");
                }
                _objects[ns] = new LinkedList<string>();
                for (int i = 1; i < MaxObjects; i++)
                {
                    _objects[ns].AddLast($"{NameObjectVar}{i}");
                }
            }
        }
        public static string[] GetNames(ushort ns, string nameBase, int count = 1)
        {
            Debug.Print("GetNames");
            if(ns >= MaxNamespaces)
            {
                throw new Exceptions.OpcUaException($"Index jmenného prostoru {ns} je větší než maximum {MaxNamespaces}");
            }
            switch (nameBase)
            {
                case NameFolder:
                    return GetFreeNames(ns, _folders[ns], nameBase, MaxFolders, count);
                case NameObjectType:
                    return GetFreeNames(ns, _objectTypes[ns], nameBase, MaxObjectTypes, count);
                case NameSimpleVar:
                    return GetFreeNames(ns, _simpleVars[ns], nameBase, MaxSimpleVars, count);
                case NameArrayVar:
                    return GetFreeNames(ns, _arrayVars[ns], nameBase, MaxArrayVars, count);
                case NameObjectVar:
                    return GetFreeNames(ns, _objects[ns], nameBase, MaxObjects, count);
            }
            return null;
        }
        private static string[] GetFreeNames(ushort ns, LinkedList<string> names, string nameBase, int max, int count)
        {
            if (count > names.Count)
            {
                throw new Exceptions.OpcUaException($"Počet požadovaných identifikátorů {count} je mimo rozsah, {nameBase}");
            }
            string[] result = new string[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = names.First();
                names.RemoveFirst();
            }
            return result;
        }
        public static void AddName(ushort ns, string prefix, string name)
        {
            if (ns >= MaxNamespaces)
            {
                throw new Exceptions.OpcUaException($"Index jmenného prostoru {ns} je větší než maximum {MaxNamespaces}");
            }
            LinkedList<string> names = null;
            switch(prefix)
            {
                case NameFolder:
                    names = _folders[ns];
                    break;
                case NameObjectType:
                    names = _objectTypes[ns];
                    break;
                case NameSimpleVar:
                    names = _simpleVars[ns];
                    break;
                case NameArrayVar:
                    names = _arrayVars[ns];
                    break;
                case NameObjectVar:
                    names = _objects[ns];
                    break;
            }
            if(name ==null)
            {
                throw new InvalidOperationException($"Chybný prefix jména");
            }
            foreach(string s in names)
            {
                if(s == name)
                {
                    names.Remove(s);
                    return;
                }
            }
        }
    }
}
