using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    public class ModNodeIdString : ModNodeId
    {
        public ModNodeIdString(ushort ns, string stringId) : base(ns)
        {
            StringId = stringId;
        }
        public string StringId { get; }

        public override string GetText()
        {
            return $"S:{Ns}:{StringId}";
        }
    }
}
