using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigOpcUa.net
{
    public class OpcObject
    {
        public OpcObject(string name)
        {
            Name = name;
        }
        public string Name { get; }
    }
}
