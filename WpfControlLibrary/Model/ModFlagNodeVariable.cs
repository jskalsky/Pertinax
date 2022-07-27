using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    public class ModFlagNodeVariable : ModFlagNode
    {
        public ModFlagNodeVariable(string name, bool itIsArray) : base(name)
        {
            ItIsArray = itIsArray;
        }

        public bool ItIsArray { get; }
    }
}
