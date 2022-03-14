using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public class SubscriberItem
    {
        public SubscriberItem(string name, int id)
        {
            ObjectName = name;
            PublisherId = id;
        }
        public string ObjectName { get; }
        public int PublisherId { get; set; }
    }
}
