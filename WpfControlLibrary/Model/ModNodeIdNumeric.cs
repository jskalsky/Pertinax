using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    internal class ModNodeIdNumeric : ModNodeId
    {
        internal ModNodeIdNumeric(ushort ns, uint numeric) : base(ns)
        {
            NumericId = numeric;
        }
        internal uint NumericId { get; }

        internal override string GetText()
        {
            return $"N:{Ns}:{NumericId}";
        }
    }
}
