using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    public class VmNodeClientGroup : VmNode
    {
        public VmNodeClientGroup(string name, ushort period, Model.client_service service, bool isExpanded = false, bool isEditable = false) : base(name, isExpanded, isEditable)
        {
            ImagePath = "pack://application:,,,/WpfControlLibrary;component/Icons/WindowsGroups_7309.png";
            Period = period;
            Service = service;
        }

        public ushort Period { get; set; }
        public Model.client_service Service { get; set; }
    }
}
