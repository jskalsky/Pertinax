using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.DataModel
{
    public class DataModelNamespace : DataModelNode
    {
        public DataModelNamespace(ushort ns) : base($"Namespace {ns}", "Icons/Namespace.png", null, null)
        {
            Namespace = ns;
            DataModelType = DataModelType.Namespace;
        }

        public ushort Namespace { get; }
    }
}
