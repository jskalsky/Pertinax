using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Model
{
    public class ModNodeClientGroup : ModNode
    {
        public ModNodeClientGroup(string name, ushort period, client_service service) : base(name)
        {
            Period = period;
            Service = service;
        }

        public ushort Period { get; }
        public client_service Service { get; }
    }
}
