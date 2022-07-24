using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    internal class ModNodeObject : ModNodeWithNodeId
    {
        internal ModNodeObject(string name, ModNodeId nodeId, string objectType) : base(name, nodeId)
        {
            ObjectType = objectType;
        }
        internal string ObjectType { get; }
    }
}
