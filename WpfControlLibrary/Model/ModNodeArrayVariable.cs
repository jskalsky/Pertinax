using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    public class ModNodeArrayVariable : ModBaseVariableNode
    {
        public ModNodeArrayVariable(string name, ModNodeId nodeId, basic_type type, access access, int arrayLength) : base(name, nodeId, type, access)
        {
            ArrayLength = arrayLength;
        }

        public int ArrayLength { get; }

        public override void CreateFlags(string path)
        {
            for (int i = 0; i < ArrayLength; ++i)
            {
                if (ModOpcUa.PtxBasicTypes.TryGetValue(ModOpcUa.GetBasicType(Type), out char basicTypeChar))
                {
                    string outFlag = $"O.OPCUA.{basicTypeChar}.{path}.{Name}.{i}";
                    string inFlag = $"I.OPCUA.{basicTypeChar}.{path}.{Name}.{i}";
                    if (Access == access.ReadWrite)
                    {
                        _flags.Add(inFlag);
                        _flags.Add(outFlag);
                        ModFlagsCollection.ModFlags[inFlag.ToUpperInvariant()] = new ModFlag(inFlag, true);
                        ModFlagsCollection.ModFlags[outFlag.ToUpperInvariant()] = new ModFlag(outFlag, true);
                    }
                    else
                    {
                        if (Access == access.Read)
                        {
                            _flags.Add(outFlag);
                            ModFlagsCollection.ModFlags[outFlag.ToUpperInvariant()] = new ModFlag(outFlag, true);
                        }
                        else
                        {
                            if (Access == access.Write)
                            {
                                _flags.Add(inFlag);
                                ModFlagsCollection.ModFlags[inFlag.ToUpperInvariant()] = new ModFlag(inFlag, false);
                            }
                        }
                    }
                }
            }
        }
    }
}
