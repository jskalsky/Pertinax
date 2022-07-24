using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfControlLibrary.Model;

namespace WpfControlLibrary.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        internal MainWindowViewModel()
        {
            Nodes = new ObservableCollection<VmNode>();
        }
        internal ObservableCollection<VmNode> Nodes { get; }
        private void LoadModNode(ModNode modNode, VmNode vmNode)
        {
            if (modNode is ModNodeNs modNs)
            {
                VmNodeNs vmNs = new VmNodeNs(modNs.Name, modNs.NsIndex);
                vmNode.AddVmNode(vmNs);
            }
            else
            {
                if(modNode is ModNodeFolder modFolder)
                {
                    VmNodeFolder vmFolder = new VmNodeFolder(modFolder.Name,modFolder.NodeId, false, true);
                    vmNode.AddVmNode(vmFolder);
                }
            }
        }
        internal void LoadXml(string fileName)
        {
            ModOpcUa opcUa = new Model.ModOpcUa();
            opcUa.ReadXml(fileName);
            foreach (ModNode modNode in opcUa.Nodes)
            {
                if (modNode is ModNodeServer modServer)
                {
                    VmNodeServer vmServer = new VmNodeServer(modServer.Name, modServer.Encrypt, true, true);
                    foreach (ModNode modSubNode in modServer.SubNodes)
                    {
                        LoadModNode(modSubNode, vmServer);
                    }
                }
            }
        }
    }
}
