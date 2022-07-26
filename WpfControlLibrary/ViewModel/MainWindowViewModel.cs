using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfControlLibrary.Model;

namespace WpfControlLibrary.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public const int MaxNrOfVars = 50000;
        public const int MaxArrayLength = 20000;

        private object _selectedVmNode;
        bool _isEnabledFolder;
        bool _isEnabledSimpleVar;
        bool _isEnabledArrayVar;
        bool _isEnabledObjectType;
        bool _isEnabledObjectVar;
        bool _isEnabledClient;
        bool _isEnabledClientGroup;
        bool _isEnabledClientVar;
        string _folderName;
        int _nrOfAddedVars;
        int _maxNrOfVars;
        int _arrayLength;
        int _maxArrayLength;
        string _clientIpAddress;
        int _clientGroupPeriod;
        string _clientVarNodeId;
        int _clientVarCount;
        public MainWindowViewModel()
        {
            Nodes = new ObservableCollection<VmNode>();

            IsEnabledArrayVar = IsEnabledClientGroup = IsEnabledClientVar = IsEnabledFolder = IsEnabledObjectType = IsEnabledObjectVar = IsEnabledSimpleVar = false;
            IsEnabledClient = true;
            MaximumArrayLength = MaxArrayLength;
            MaximumNrOfVars = MaxNrOfVars;
            NrOfAddedVars = 1;
            ArrayLength = 1;
            SelectedVmNode = null;
            ClientIpAddress = "10.10.13.252";
        }

        public ObservableCollection<VmNode> Nodes { get; }
        public object SelectedVmNode
        {
            get { return _selectedVmNode; }
            set { _selectedVmNode = value; OnPropertyChanged(nameof(SelectedVmNode)); }
        }
        public bool IsEnabledFolder
        {
            get { return _isEnabledFolder; }
            set { _isEnabledFolder = value; OnPropertyChanged(nameof(IsEnabledFolder)); }
        }
        public bool IsEnabledSimpleVar
        {
            get { return _isEnabledSimpleVar; }
            set { _isEnabledSimpleVar = value; OnPropertyChanged(nameof(IsEnabledSimpleVar)); }
        }
        public bool IsEnabledArrayVar
        {
            get { return _isEnabledArrayVar; }
            set { _isEnabledArrayVar = value; OnPropertyChanged(nameof(IsEnabledArrayVar)); }
        }
        public bool IsEnabledObjectType
        {
            get { return _isEnabledObjectType; }
            set { _isEnabledObjectType = value; OnPropertyChanged(nameof(IsEnabledObjectType)); }
        }
        public bool IsEnabledObjectVar
        {
            get { return _isEnabledObjectVar; }
            set { _isEnabledObjectVar = value; OnPropertyChanged(nameof(IsEnabledObjectVar)); }
        }
        public bool IsEnabledClient
        {
            get { return _isEnabledClient; }
            set { _isEnabledClient = value; OnPropertyChanged(nameof(IsEnabledClient)); }
        }
        public bool IsEnabledClientGroup
        {
            get { return _isEnabledClientGroup; }
            set { _isEnabledClientGroup = value; OnPropertyChanged(nameof(IsEnabledClientGroup)); }
        }
        public bool IsEnabledClientVar
        {
            get { return _isEnabledClientVar; }
            set { _isEnabledClientVar = value; OnPropertyChanged(nameof(IsEnabledClientVar)); }
        }
        public string FolderName
        {
            get { return _folderName; }
            set { _folderName = value; OnPropertyChanged(nameof(FolderName)); }
        }
        public int NrOfAddedVars
        {
            get { return _nrOfAddedVars; }
            set { _nrOfAddedVars = value; OnPropertyChanged(nameof(NrOfAddedVars)); }
        }
        public int MaximumNrOfVars
        {
            get { return _maxNrOfVars; }
            set { _maxNrOfVars = value; OnPropertyChanged(nameof(MaxNrOfVars)); }
        }
        public int ArrayLength
        {
            get { return _arrayLength; }
            set { _arrayLength = value; OnPropertyChanged(nameof(ArrayLength)); }
        }
        public int MaximumArrayLength
        {
            get { return _maxArrayLength; }
            set { _maxArrayLength = value; OnPropertyChanged(nameof(MaxArrayLength)); }
        }
        public string ClientIpAddress
        {
            get { return _clientIpAddress; }
            set { _clientIpAddress = value; OnPropertyChanged(nameof(ClientIpAddress)); }
        }
        public int ClientGroupPeriod
        {
            get { return _clientGroupPeriod; }
            set { _clientGroupPeriod = value; OnPropertyChanged(nameof(ClientGroupPeriod)); }
        }
        public string ClientVarNodeId
        {
            get { return _clientVarNodeId; }
            set { _clientVarNodeId = value; OnPropertyChanged(nameof(ClientVarNodeId)); }
        }
        public int ClientVarCount
        {
            get { return _clientVarCount; }
            set { _clientVarCount = value; OnPropertyChanged(nameof(ClientVarCount)); }
        }
        private void LoadModNode(ModNode modNode, VmNode vmNode)
        {
            VmNode vmN = null;
            if (modNode is ModNodeNs modNs)
            {
                vmN = new VmNodeNs(modNs.Name, modNs.NsIndex, true, false);
                vmNode.AddVmNode(vmN);
            }
            else
            {
                if (modNode is ModNodeFolder modFolder)
                {
                    vmN = new VmNodeFolder(modFolder.Name, modFolder.NodeId.GetText(), false, true);
                    vmNode.AddVmNode(vmN);
                }
                else
                {
                    if (modNode is ModNodeVariable modVar)
                    {
                        vmN = new VmNodeSimpleVariable(modVar.Name, modVar.NodeId.GetText(), modVar.Type, modVar.Access, false, true);
                        vmNode.AddVmNode(vmN);
                    }
                    else
                    {
                        if (modNode is ModNodeArrayVariable arrayVar)
                        {
                            vmN = new VmNodeArrayVariable(arrayVar.Name, arrayVar.NodeId.GetText(), arrayVar.Type, arrayVar.Access, arrayVar.ArrayLength, false, true);
                            vmNode.AddVmNode(vmN);
                        }
                        else
                        {
                            if (modNode is ModNodeObjectType modOt)
                            {
                                vmN = new VmNodeObjectType(modOt.Name, modOt.NodeId.GetText(), true, true);
                                vmNode.AddVmNode(vmN);
                            }
                            else
                            {
                                if (modNode is ModNodeObject modO)
                                {
                                    vmN = new VmNodeObject(modO.Name, modO.NodeId.GetText(), modO.ObjectType, true, true);
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
                if(modNode is ModNodeClient modClient)
                {
                    VmNodeClient vmClient = new VmNodeClient(modClient.Name,modClient.IpAddress,modClient.Encrypt, true, true);
                    Nodes.Add(vmClient);
                    foreach(ModNodeClientGroup modGroup in modClient.SubNodes)
                    {
                        VmNodeClientGroup vmGroup = new VmNodeClientGroup(modGroup.Name,modGroup.Period,modGroup.Service,true,true);
                        vmClient.AddVmNode(vmGroup);
                        foreach(ModNodeClientVar modVar in modGroup.SubNodes)
                        {
                            VmNodeClientVar vmVar = new VmNodeClientVar(modVar.Name, modVar.NodeId, modVar.Type, false, false);
                            vmGroup.AddVmNode(vmVar);
                        }
                    }
                }
            }
        }
        private void SaveVmNode(ModNode modNode, VmNode vmNode)
        {
            ModNode modN = null;
            if (vmNode is VmNodeNs vmNs)
            {
                modN = new ModNodeNs(vmNs.Name, vmNs.NsIndex);
                modNode.AddSubNode(modN);
            }
            else
            {
                if (vmNode is VmNodeFolder vmFolder)
                {
                    modN = new ModNodeFolder(vmFolder.Name, ModNodeId.GetModNodeId(vmFolder.NodeIdString));
                    modNode.AddSubNode(modN);
                }
                else
                {
                    if (vmNode is VmNodeSimpleVariable vmVar)
                    {
                        modN = new ModNodeVariable(vmVar.Name, ModNodeId.GetModNodeId(vmVar.NodeIdString), vmVar.Type, vmVar.Access);
                        modNode.AddSubNode(modN);
                    }
                    else
                    {
                        if (vmNode is VmNodeArrayVariable arrayVar)
                        {
                            modN = new ModNodeArrayVariable(arrayVar.Name, ModNodeId.GetModNodeId(arrayVar.NodeIdString), arrayVar.Type, arrayVar.Access, arrayVar.ArrayLength);
                            modNode.AddSubNode(modN);
                        }
                        else
                        {
                            if (vmNode is VmNodeObjectType vmOt)
                            {
                                modN = new ModNodeObjectType(vmOt.Name, ModNodeId.GetModNodeId(vmOt.NodeIdString));
                                modNode.AddSubNode(modN);
                            }
                            else
                            {
                                if (vmNode is VmNodeObject vmO)
                                {
                                    modN = new ModNodeObject(vmO.Name, ModNodeId.GetModNodeId(vmO.NodeIdString), vmO.ObjectType);
                                    modNode.AddSubNode(modN);
                                }
                            }
                        }
                    }
                }
            }
            if (modN != null)
            {
                foreach (VmNode vN in vmNode.SubNodes)
                {
                    SaveVmNode(modN, vN);
                }
            }
        }
        public void SaveXml(string fileName)
        {
            ModOpcUa opcUa = new Model.ModOpcUa();
            opcUa.Nodes.Clear();
            foreach (VmNode vmNode in Nodes)
            {
                if (vmNode is VmNodeServer vmServer)
                {
                    ModNodeServer modServer = new ModNodeServer(vmServer.Name, vmServer.Encrypt);
                    foreach (VmNode sub in vmServer.SubNodes)
                    {
                        SaveVmNode(modServer, sub);
                    }
                    opcUa.Nodes.Add(modServer);
                }
                if(vmNode is VmNodeClient vmClient)
                {
                    ModNodeClient modClient = new ModNodeClient(vmClient.Name, vmClient.Encrypt, vmClient.IpAddress);
                    foreach(VmNodeClientGroup vmGroup in vmClient.SubNodes)
                    {
                        ModNodeClientGroup modGroup = new ModNodeClientGroup(vmGroup.Name, vmGroup.Period, vmGroup.Service);
                        modClient.AddSubNode(modGroup);
                        foreach(VmNodeClientVar vmVar in vmGroup.SubNodes)
                        {
                            ModNodeClientVar modVar = new ModNodeClientVar(vmVar.Name, vmVar.NodeId, vmVar.Type);
                            modGroup.AddSubNode(modVar);
                        }
                    }
                    opcUa.Nodes.Add(modClient);
                }
            }
            opcUa.WriteXml(fileName);
        }
    }
}
