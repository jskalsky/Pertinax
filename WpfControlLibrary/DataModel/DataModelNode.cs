using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfControlLibrary.DataModel
{
    public enum DataModelType { None, Namespace, Folder, ObjectType, ObjectVariable, SimpleVariable, ArrayVariable }
    public abstract class DataModelNode : INotifyPropertyChanged, IDataErrorInfo
    {
        public const ushort DefaultNamespaceIndex = 1;
        private bool _isExpanded;
        private bool _isEnabled;
        private string _name;
        private NodeIdBase _nodeId;
        private string _oldName = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        protected DataModelNode(string name, string imagePath, NodeIdBase nodeId, DataModelNode parent)
        {
            Name = name;
            ImagePath = imagePath;
            NodeId = nodeId;
            Children = new ObservableCollection<DataModelNode>();
            Parent = parent;
            IsEnabled = true;
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }
        public string ImagePath { get; }
        public NodeIdBase NodeId
        {
            get { return _nodeId; }
            set { _nodeId = value;OnPropertyChanged(nameof(NodeId)); }
        }
        public DataModelNode Parent { get; private set; }
        public ObservableCollection<DataModelNode> Children { get; }
        public DataModelType DataModelType { get; protected set; }
        public DataModelNode Instance
        {
            get { return this; }
        }
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; OnPropertyChanged("IsExpanded"); }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; OnPropertyChanged("IsEnabled"); }
        }
        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get
            {
                return Validate(columnName);
            }
        }

        private string Validate(string propertyName)
        {
            string error = string.Empty;
            if (_oldName == Name || _oldName == string.Empty)
            {
                return error;
            }
            return error;
        }
        public static DataModelFolder GetFolder(string name, NodeIdBase nodeId, DataModelNode parent)
        {
            DataModelFolder node = new DataModelFolder(name, nodeId, parent);
            return node;
        }

        public static DataModelSimpleVariable GetSimpleVariable(string name, NodeIdBase nodeId, string basicType, string varAccess, DataModelNode parent)
        {
            DataModelSimpleVariable node = new DataModelSimpleVariable(name, nodeId, basicType, varAccess, parent);
            return node;
        }

        public static DataModelObjectType GetObjectType(string name, NodeIdBase nodeId, DataModelNode parent)
        {
            DataModelObjectType node = new DataModelObjectType(name, nodeId, parent);
            return node;
        }

        public static DataModelArrayVariable GetArrayVariable(string name, NodeIdBase nodeId, string basicType, string varAccess, int arrayLength, DataModelNode parent)
        {
            DataModelArrayVariable node = new DataModelArrayVariable(name, nodeId, basicType, varAccess, arrayLength, parent);
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

        public bool IsParentFolder(DataModelFolder folder)
        {
            DataModelNode node = this;
            while (node.DataModelType != DataModelType.Namespace)
            {
                if (node.Name == folder.Name)
                {
                    return true;
                }
                node = node.Parent;
            }

            return false;
        }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
