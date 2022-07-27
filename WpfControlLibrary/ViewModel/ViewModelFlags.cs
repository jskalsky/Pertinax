using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfControlLibrary.Model;

namespace WpfControlLibrary.ViewModel
{
    public class ViewModelFlags : ViewModelBase
    {
        public ViewModelFlags()
        {

        }
        public ObservableCollection<PortsNode> Ports { get; }
        private IList<string> GetFlags(Model.basic_type bt, Model.access acc, string name, int arrayLength)
        {

        }
        private void LoadModNode(ModNode modNode, PortsNode portsNode)
        {
            PortsNode pN = null;
            if (modNode is ModNodeNs modNs)
            {
                pN = new PortsNode(modNs.Name);
                portsNode.Add(pN);
                portsNode.IsExpanded = true;
            }
            else
            {
                if (modNode is ModNodeFolder modFolder)
                {
                    pN = new PortsNode(modFolder.Name);
                    portsNode.Add(pN);
                    portsNode.IsExpanded = true;
                }
                else
                {
                    if (modNode is ModNodeVariable modVar)
                    {
                        pN = new PortsNode(modVar.Name);
                        portsNode.Add(pN);
                        portsNode.IsExpanded = true;
                    }
                    else
                    {
                        if (modNode is ModNodeArrayVariable arrayVar)
                        {
                            vmN = new VmNodeArrayVariable(arrayVar.Name, arrayVar.NodeId.GetText(), arrayVar.Type, arrayVar.Access, arrayVar.ArrayLength, false, true);
                            NodeIdFactory.SetNextNodeId(arrayVar.NodeId.GetText());
                            NameFactory.SetName(arrayVar.NodeId.Ns, arrayVar.Name);
                            vmNode.AddVmNode(vmN);
                        }
                        else
                        {
                            if (modNode is ModNodeObjectType modOt)
                            {
                                vmN = new VmNodeObjectType(modOt.Name, modOt.NodeId.GetText(), true, true);
                                NodeIdFactory.SetNextNodeId(modOt.NodeId.GetText());
                                NameFactory.SetName(modOt.NodeId.Ns, modOt.Name);
                                vmNode.AddVmNode(vmN);
                            }
                            else
                            {
                                if (modNode is ModNodeObject modO)
                                {
                                    vmN = new VmNodeObject(modO.Name, modO.NodeId.GetText(), modO.ObjectType, true, true);
                                    NodeIdFactory.SetNextNodeId(modO.NodeId.GetText());
                                    NameFactory.SetName(modO.NodeId.Ns, modO.Name);
                                    vmNode.AddVmNode(vmN);
                                }
                            }
                        }
                    }
                }
            }
            if (vmN != null)
            {
                foreach (ModNode mN in modNode.SubNodes)
                {
                    LoadModNode(mN, vmN);
                }
            }
        }
        public void LoadXml(string fileName)
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
                    Nodes.Add(vmServer);
                }
                if (modNode is ModNodeClient modClient)
                {
                    VmNodeClient vmClient = new VmNodeClient(modClient.Name, modClient.IpAddress, modClient.Encrypt, true, true);
                    NameFactory.SetName(0, modClient.Name);
                    Nodes.Add(vmClient);
                    foreach (ModNodeClientGroup modGroup in modClient.SubNodes)
                    {
                        VmNodeClientGroup vmGroup = new VmNodeClientGroup(modGroup.Name, modGroup.Period, modGroup.Service, true, true);
                        NameFactory.SetName(0, modGroup.Name);
                        vmClient.AddVmNode(vmGroup);
                        foreach (ModNodeClientVar modVar in modGroup.SubNodes)
                        {
                            VmNodeClientVar vmVar = new VmNodeClientVar(modVar.Name, modVar.NodeId, modVar.Type, false, false);
                            NodeIdFactory.SetNextNodeId(modVar.NodeId);
                            NameFactory.SetName(0, modVar.Name);
                            vmGroup.AddVmNode(vmVar);
                        }
                    }
                }
            }
        }

    }
}
