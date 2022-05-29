using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public class DataModelNode
    {
        public DataModelNode(string name, object tag, string imageKey)
        {
            Name = name;
            Tag = tag;
            ImageKey = imageKey;
            Children = new ObservableCollection<DataModelNode>();
        }
        public string Name { get; set; }
        public object Tag { get; }
        public ObservableCollection<DataModelNode> Children { get; }
        public string ImageKey { get; }

        public void AddChild(string name, object tag, string imageKey)
        {
            Children.Add(new DataModelNode(name, tag, imageKey));
        }
    }
}
