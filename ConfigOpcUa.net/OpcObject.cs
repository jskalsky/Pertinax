using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigOpcUaNet
{
    public class OpcObject
    {
        private readonly List<OpcObjectItem> _items;
        public OpcObject(string name)
        {
            Name = name;
            _items = new List<OpcObjectItem>();
        }
        public string Name { get; }
        public IList<OpcObjectItem> Items => _items;

        public OpcObjectItem AddItem(string name, string basicType, string access, string rank, string arraySize)
        {
            OpcObjectItem ooi = new OpcObjectItem(name, basicType, access, rank, arraySize);
            _items.Add(ooi);
            return ooi;
        }
    }
}
