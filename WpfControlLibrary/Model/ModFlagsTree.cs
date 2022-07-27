using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    public static class ModFlagsTree
    {
        public static readonly List<ModFlagNode> FlagsTree = new List<ModFlagNode>();
        public static readonly Dictionary<string, ModFlagNodeVariable> AllFlags = new Dictionary<string, ModFlagNodeVariable>();

        private static ushort GetArrayIndex(string text)
        {
            int i = text.LastIndexOf('.');
            if (i < 0)
            {
                return 0;
            }

            string index = text.Substring(i + 1);
            if (ushort.TryParse(index, out ushort result))
            {
                return result;
            }

            return 0;
        }
        private static IList<string> GetFlags(Model.basic_type bt, Model.access acc, string name, string path, int arrayLength)
        {
            List<string> flags = new List<string>();
            bool itIsArray = (arrayLength == 0) ? true : false;
            if (arrayLength == 0)
            {
                ++arrayLength;
            }
            for (int i = 0; i < arrayLength; ++i)
            {
                if (Model.ModOpcUa.PtxBasicTypes.TryGetValue(Model.ModOpcUa.GetBasicType(bt), out char basicTypeChar))
                {
                    string inFlag = string.Empty;
                    string outFlag = string.Empty;
                    if (itIsArray)
                    {
                        outFlag = $"O.OPCUA.{basicTypeChar}.{path}.{name}.{i}";
                        inFlag = $"I.OPCUA.{basicTypeChar}.{path}.{name}.{i}";
                    }
                    else
                    {
                        outFlag = $"O.OPCUA.{basicTypeChar}.{path}.{name}";
                        inFlag = $"I.OPCUA.{basicTypeChar}.{path}.{name}";
                    }
                    if (acc == access.ReadWrite)
                    {
                        flags.Add(inFlag);
                        flags.Add(outFlag);
                        AllFlags[inFlag.ToUpperInvariant()] = new ModFlagNodeVariable(inFlag, itIsArray);
                        AllFlags[outFlag.ToUpperInvariant()] = new ModFlagNodeVariable(outFlag, itIsArray);
                    }
                    else
                    {
                        if (acc == access.Read)
                        {
                            flags.Add(outFlag);
                            AllFlags[outFlag.ToUpperInvariant()] = new ModFlagNodeVariable(outFlag, itIsArray);
                        }
                        else
                        {
                            if (acc == access.Write)
                            {
                                flags.Add(inFlag);
                                AllFlags[inFlag.ToUpperInvariant()] = new ModFlagNodeVariable(inFlag, itIsArray);
                            }
                        }
                    }
                }
            }
            return flags;
        }
        private static void LoadModNode(ModNode modNode, ModFlagNode mfNode, string path)
        {
            ModFlagNode mfN = null;
            string pathPart = string.Empty;
            if (modNode is ModNodeNs modNs)
            {
                mfN = new ModFlagNode(modNs.Name);
                pathPart = modNs.Name;
                mfNode.AddNode(mfN);
            }
            else
            {
                if (modNode is ModNodeFolder modFolder)
                {
                    mfN = new ModFlagNode(modFolder.Name);
                    pathPart = modFolder.Name;
                    mfNode.AddNode(mfN);
                }
                else
                {
                    if (modNode is ModNodeVariable modVar)
                    {
                        mfN = new ModFlagNodeVariable(modVar.Name, false);
                        mfNode.AddNode(mfN);
                        IList<string> flags = GetFlags(modVar.Type, modVar.Access, modVar.Name, path, 0);
                        foreach (string s in flags)
                        {
                            mfN.AddNode(new ModFlagNodeFlag(s));
                        }
                        return;
                    }
                    else
                    {
                        if (modNode is ModNodeArrayVariable arrayVar)
                        {
                            mfN = new ModFlagNodeVariable(arrayVar.Name, true);
                            mfNode.AddNode(mfN);
                            IList<string> flags = GetFlags(arrayVar.Type, arrayVar.Access, arrayVar.Name, path, arrayVar.ArrayLength);
                            foreach (string s in flags)
                            {
                                mfN.AddNode(new ModFlagNodeFlag(s));
                            }
                            return;
                        }
                        else
                        {
                            if (modNode is ModNodeObjectType modOt)
                            {
                                mfN = new ModFlagNode(modOt.Name);
                                pathPart = modOt.Name;
                                mfNode.AddNode(mfN);
                            }
                            else
                            {
                                if (modNode is ModNodeObject modO)
                                {
                                    mfN = new ModFlagNode(modO.Name);
                                    pathPart = modO.Name;
                                    mfNode.AddNode(mfN);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            if (mfN != null)
            {
                foreach (ModNode mN in modNode.SubNodes)
                {
                    LoadModNode(mN, mfN, path + $".{pathPart}");
                }
            }
        }
        public static void LoadXml(string fileName)
        {
            ModOpcUa opcUa = new Model.ModOpcUa();
            opcUa.ReadXml(fileName);
            AllFlags.Clear();
            FlagsTree.Clear();

            foreach (ModNode modNode in opcUa.Nodes)
            {
                if (modNode is ModNodeServer modServer)
                {
                    ModFlagNode root = new ModFlagNode(modServer.Name);
                    foreach (ModNode modSubNode in modServer.SubNodes)
                    {
                        string path = string.Empty;
                        LoadModNode(modSubNode, root, path);
                    }
                    FlagsTree.Add(root);
                }
                /*                if (modNode is ModNodeClient modClient)
                                {
                                    PortsNode root = new PortsNode(modClient.Name);
                                    string path = modClient.Name;
                                    Ports.Add(root);
                                    foreach (ModNodeClientGroup modGroup in modClient.SubNodes)
                                    {
                                        PortsNode group = new PortsNode(modGroup.Name);
                                        path += $".{modGroup.Name}";
                                        root.Add(group);
                                        foreach (ModNodeClientVar modVar in modGroup.SubNodes)
                                        {
                                            PortsNode var = new PortsNode(modVar.Name);
                                            path += $".{modVar.Name}";
                                        }
                                    }
                                }*/
            }
        }
    }
}
