using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigOpcUa
{
    public class Flag
    {
        public Flag(string text, ushort arrayIndex, bool isArray)
        {
            Text = text;
            ArrayIndex = arrayIndex;
            IsArray = isArray;
        }
        public string Text { get; }
        public ushort ArrayIndex { get; }
        public bool IsArray { get; }
    }
}
