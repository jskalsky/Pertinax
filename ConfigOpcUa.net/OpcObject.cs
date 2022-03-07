using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigOpcUaNet
{
    public class OpcObject
    {
        private readonly ObservableCollection<OpcObjectItem> _items;
        public OpcObject(string name)
        {
            Name = name;
            PublishingInterval = 0;
            _items = new ObservableCollection<OpcObjectItem>();
        }
        public string Name { get; }
        public int PublishingInterval { get; }
        public ObservableCollection<OpcObjectItem> Items => _items;

        public OpcObjectItem AddItem(string name, string basicType, string access, string rank, string arraySize)
        {
            OpcObjectItem ooi = new OpcObjectItem(name, basicType, access, rank, arraySize);
            _items.Add(ooi);
            return ooi;
        }
    }
}
