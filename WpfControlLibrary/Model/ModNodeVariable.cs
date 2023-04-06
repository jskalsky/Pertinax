using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    public class ModNodeVariable : ModBaseVariableNode
    {
        public ModNodeVariable(string name, ModNodeId nodeId, basic_type type, access access) : base(name, nodeId, type, access)
        {

        }
        public override void CreateFlags(string path)
        {
            if (ModOpcUa.PtxBasicTypes.TryGetValue(ModOpcUa.GetBasicType(Type), out char basicTypeChar))
            {
                string outFlag = $"O.OPCUA.{basicTypeChar}.{path}.{Name}";
                string inFlag = $"I.OPCUA.{basicTypeChar}.{path}.{Name}";
                if (Access == access.ReadWrite)
                {
                    _flags.Add(inFlag);
                    _flags.Add(outFlag);
                    ModFlagsCollection.ModFlags[inFlag.ToUpperInvariant()] = new ModFlag(inFlag, false);
                    ModFlagsCollection.ModFlags[outFlag.ToUpperInvariant()] = new ModFlag(outFlag, false);
                }
                else
                {
                    if (Access == access.Read)
                    {
                        _flags.Add(outFlag);
                        ModFlagsCollection.ModFlags[outFlag.ToUpperInvariant()] = new ModFlag(outFlag, false);
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
