using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigOpcUa
{
    public class Flag
    {
        public Flag(string text, ushort arrayIndex)
        {
            Text = text;
            ArrayIndex = arrayIndex;
        }
        public string Text { get; }
        public ushort ArrayIndex { get; }
    }
}
