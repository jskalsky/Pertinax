using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    public class Variable
    {
        public Variable(string name, ushort nsIndex, string identifier, string kind, string basicType, string access, int arrayLength, string objectName)
        {
            Name = name;
            NsIndex = nsIndex;
            Identifier = identifier;
            Kind = kind;
            BasicType = basicType;
            Access = access;
            ArrayLength = arrayLength;
            ObjectName = objectName;
        }

        public string Name { get; }
        public ushort NsIndex { get; }
        public string Identifier { get; }
        public string Kind { get; }
        public string BasicType { get; }
        public string Access { get; }
        public int ArrayLength { get; }
        public string ObjectName { get; }
    }
}
