using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    public class VmNodeClient : VmNode
    {
        public VmNodeClient(string name, string ipAddress, bool encrypt, bool isExpanded = false, bool isEditable = false) : base(name, isExpanded, isEditable)
        {
            ImagePath = "pack://application:,,,/WpfControlLibrary;component/Icons/WebBrowser_6242.png";
            IpAddress = ipAddress;
            Encrypt = encrypt;
        }

        public string IpAddress { get; set; }
        public bool Encrypt { get; set; }
    }
}
