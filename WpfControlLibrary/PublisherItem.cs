using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public class PublisherItem
    {
        public PublisherItem(string objectName, ushort publishingInterval)
        {
            ObjectName = objectName;
            PublishingInterval = publishingInterval;
        }
        public string ObjectName { get; }
        public int PublishingInterval { get; set; }
    }
}
