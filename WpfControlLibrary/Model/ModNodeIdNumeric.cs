using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    public class ModNodeIdNumeric : ModNodeId
    {
        public ModNodeIdNumeric(ushort ns, uint numeric) : base(ns)
        {
            NumericId = numeric;
        }
        public uint NumericId { get; }

        public override string GetText()
        {
            return $"N:{Ns}:{NumericId}";
        }
    }
}
