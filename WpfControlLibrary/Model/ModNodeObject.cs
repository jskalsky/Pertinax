using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    public class ModNodeObject : ModNodeWithNodeId
    {
        public ModNodeObject(string name, ModNodeId nodeId, string objectType) : base(name, nodeId)
        {
            ObjectType = objectType;
        }
        public string ObjectType { get; }
    }
}
