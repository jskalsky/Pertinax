using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BupLst
{
    public enum IoClass { Unknown, BlockOut, ExtIn, ExtOut, UnconnectedIn, UnconnectedOut}
    public class Io
    {
        public Io(IoClass ioClass, ushort valueType)
        {
            IoClass = ioClass;
            ValueType = valueType;
        }
        public IoClass IoClass { get; }
        public ushort ValueType { get; }
    }
}
