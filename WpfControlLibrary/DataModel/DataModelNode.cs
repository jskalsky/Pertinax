using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.DataModel
{
    public abstract class DataModelNode
    {
        protected readonly string[] _basicTypes = new string[] { "Boolean", "UInt8", "Int8", "UInt16", "Int16", "UInt32", "Int32", "Float", "Double" };
        protected readonly string[] _access = new string[] { "Read", "Write", "ReadWrite" };
        public const ushort DefaultNamespaceIndex = 1;
        protected DataModelNode(string name, string imagePath, NodeIdBase nodeId)
        {
            Name = name;
            ImagePath = imagePath;
            NodeId = nodeId;
            Children = new ObservableCollection<DataModelNode>();
        }
        public string Name { get; }
        public string ImagePath { get; }
        public NodeIdBase NodeId { get; }
        public ObservableCollection<DataModelNode> Children { get; }

        public static DataModelFolder GetFolder(string name)
        {
            DataModelFolder node = new DataModelFolder(name, NodeIdNumeric.GetNextNodeIdNumeric(DefaultNamespaceIndex, false));
            return node;
        }

        public void AddChildren(DataModelNode node)
        {
            Children.Add(node);
        }
    }
}
