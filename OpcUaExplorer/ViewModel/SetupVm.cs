using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace OpcUaExplorer.ViewModel
{
    public class SetupVm : ViewModelBase
    {
        private IPAddress _ip;

        public SetupVm()
        {
            Ip = IPAddress.Parse(Properties.Settings.Default.ServerIpAddress);
        }
        public IPAddress Ip
        {
            get { return _ip; }
            set { _ip = value; }
        }
    }
}
