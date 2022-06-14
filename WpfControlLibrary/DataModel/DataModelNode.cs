using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.DataModel
{
    public enum DataModelType { None, Namespace, Folder, ObjectType, ObjectVariable, SimpleVariable, ArrayVariable }
    public abstract class DataModelNode
    {
        protected readonly string[] _basicTypes = new string[] { "Boolean", "UInt8", "Int8", "UInt16", "Int16", "UInt32", "Int32", "Float", "Double" };
        protected readonly string[] _access = new string[] { "Read", "Write", "ReadWrite" };
        public const ushort DefaultNamespaceIndex = 1;
        protected DataModelNode(string name, string imagePath, NodeIdBase nodeId, DataModelNode parent)
        {
            Name = name;
            TreeNodeText = name;
            ImagePath = imagePath;
            NodeId = nodeId;
            Children = new ObservableCollection<DataModelNode>();
            Parent = parent;
        }
        public string TreeNodeText { get; protected set; }
        public string Name { get; }
        public string ImagePath { get; }
        public NodeIdBase NodeId { get; }
        public DataModelNode Parent { get; private set; }
        public ObservableCollection<DataModelNode> Children { get; }
        public DataModelType DataModelType { get; protected set; }
        public static DataModelFolder GetFolder(string name, DataModelNode parent)
        {
            DataModelFolder node = new DataModelFolder(name, NodeIdNumeric.GetNextNodeIdNumeric(DefaultNamespaceIndex, false), parent);
            return node;
        }

        public static DataModelSimpleVariable GetSimpleVariable(string name, NodeIdBase nodeId, string basicType, string varAccess, DataModelNode parent)
        {
            DataModelSimpleVariable node = new DataModelSimpleVariable(name, nodeId, basicType, varAccess, parent);
            return node;
        }
        public void AddChildren(DataModelNode node)
        {
            Children.Add(node);
        }

        public DataModelNamespace GetNamespace()
        {
            DataModelNode node = this;
            while (node.DataModelType != DataModelType.Namespace)
            {
                node = node.Parent;
            }
            if (node is DataModelNamespace ns)
            {
                return ns;
            }
            return null;
        }
    }
}
