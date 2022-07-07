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

        private static LinkedList<string> _folders = new LinkedList<string>();
        private static LinkedList<string> _objectTypes = new LinkedList<string>();
        private static LinkedList<string> _simpleVars = new LinkedList<string>();
        private static LinkedList<string> _arrayVars = new LinkedList<string>();
        private static LinkedList<string> _objects = new LinkedList<string>();
        public static string[] GetNames(ushort ns, string nameBase, int count = 1)
        {
            Debug.Print("GetNames");
            switch (nameBase)
            {
                case NameFolder:
                    return GetFreeNames(ns, _folders, nameBase, MaxFolders, count);
                case NameObjectType:
                    return GetFreeNames(ns, _objectTypes, nameBase, MaxObjectTypes, count);
                case NameSimpleVar:
                    return GetFreeNames(ns, _simpleVars, nameBase, MaxSimpleVars, count);
                case NameArrayVar:
                    return GetFreeNames(ns, _arrayVars, nameBase, MaxArrayVars, count);
                case NameObjectVar:
                    return GetFreeNames(ns, _objects, nameBase, MaxObjects, count);
            }
            return null;
        }
        private static string[] GetFreeNames(ushort ns, LinkedList<string> names, string nameBase, int max, int count)
        {
            if (names.Count == 0)
            {
                for (int i = 1; i < max; i++)
                {
                    names.AddLast($"{nameBase}{i}");
                }
            }
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
    }
}
