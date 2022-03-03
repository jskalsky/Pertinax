using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigOpcUaNet
{
    public class OpcObjectItem
    {
        public OpcObjectItem(string name, string basicType, string access, string rank, string arraySize)
        {
            Name = name;
            BasicType = basicType;
            Access = access;
            Rank = rank;
            ArraySize = arraySize;
        }
        public string Name { get; }
        public string BasicType { get; }
        public string Access { get; }
        public string Rank { get; }
        public string ArraySize { get; }
    }
}
