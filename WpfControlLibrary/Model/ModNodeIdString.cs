using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    internal class ModNodeIdString : ModNodeId
    {
        internal ModNodeIdString(ushort ns, string stringId) : base(ns)
        {
            StringId = stringId;
        }
        internal string StringId { get; }

        internal override string GetText()
        {
            return $"S:{Ns}:{StringId}";
        }
    }
}
