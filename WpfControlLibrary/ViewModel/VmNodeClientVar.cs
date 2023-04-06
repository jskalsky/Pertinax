using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    public class VmNodeClientVar : VmNode
    {
        public VmNodeClientVar(string name, string nodeId, string type, Model.client_service service, bool isExpanded = false, bool isEditable = false) : 
            base(name, isExpanded, isEditable)
        {
            ImagePath = "pack://application:,,,/WpfControlLibrary;component/Icons/ComplexType_12905.png";
            NodeId = nodeId;
            Type = type;
            Service = service;
            Flags = new ObservableCollection<string>();
        }

        public string NodeId { get; set; }
        public string Type { get; set; }
        public Model.client_service Service { get; }
        public ObservableCollection<string> Flags { get; }

        public void CreateFlag(string path)
        {

        }
    }
}
