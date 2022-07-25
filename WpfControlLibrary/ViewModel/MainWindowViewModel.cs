﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
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
    public class MainWindowViewModel : ObservableRecipient
    {
        private object _selectedNode;
        public MainWindowViewModel()
        {
            Nodes = new ObservableCollection<VmNode>();
            OkCommand = new AsyncRelayCommand(OkCommandAsync);
            CancelCommand = new AsyncRelayCommand(CancelCommandAsync);
            TreeSelectedItemChanged = new RelayCommand<RoutedPropertyChangedEventArgs<object>>(OnTreeSelectedItemChanged);

            SelectedNode = null;
        }

        private void OnTreeSelectedItemChanged(RoutedPropertyChangedEventArgs<object> obj)
        {
            SelectedNode = obj.NewValue;
        }

        public IAsyncRelayCommand OkCommand { get; }
        public IAsyncRelayCommand CancelCommand { get; }
        public RelayCommand<RoutedPropertyChangedEventArgs<object>> TreeSelectedItemChanged { get; }
        public ObservableCollection<VmNode> Nodes { get; }
        public object SelectedNode
        {
            get { return _selectedNode; }
            set => SetProperty(ref _selectedNode, value);
        }
        private void LoadModNode(ModNode modNode, VmNode vmNode)
        {
            VmNode vmN = null;
            if (modNode is ModNodeNs modNs)
            {
                vmN = new VmNodeNs(modNs.Name, modNs.NsIndex);
                vmNode.AddVmNode(vmN);
            }
            else
            {
                if (modNode is ModNodeFolder modFolder)
                {
                    vmN = new VmNodeFolder(modFolder.Name, modFolder.NodeId, false, true);
                    vmNode.AddVmNode(vmN);
                }
                else
                {
                    if (modNode is ModNodeVariable modVar)
                    {
                        vmN = new VmNodeSimpleVariable(modVar.Name, modVar.NodeId, modVar.Type, modVar.Access, false, true);
                        vmNode.AddVmNode(vmN);
                    }
                    else
                    {
                        if (modNode is ModNodeArrayVariable arrayVar)
                        {
                            vmN = new VmNodeArrayVariable(arrayVar.Name, arrayVar.NodeId, arrayVar.Type, arrayVar.Access, arrayVar.ArrayLength, false, true);
                            vmNode.AddVmNode(vmN);
                        }
                        else
                        {
                            if (modNode is ModNodeObjectType modOt)
                            {
                                vmN = new VmNodeObjectType(modOt.Name, modOt.NodeId, true, true);
                                vmNode.AddVmNode(vmN);
                            }
                            else
                            {
                                if (modNode is ModNodeObject modO)
                                {
                                    vmN = new VmNodeObject(modO.Name, modO.NodeId, modO.ObjectType, true, true);
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
                    modN = new ModNodeFolder(vmFolder.Name, vmFolder.NodeId);
                    modNode.AddSubNode(modN);
                }
                else
                {
                    if (vmNode is VmNodeSimpleVariable vmVar)
                    {
                        modN = new ModNodeVariable(vmVar.Name, vmVar.NodeId, vmVar.Type, vmVar.Access);
                        modNode.AddSubNode(modN);
                    }
                    else
                    {
                        if (vmNode is VmNodeArrayVariable arrayVar)
                        {
                            modN = new ModNodeArrayVariable(arrayVar.Name, arrayVar.NodeId, arrayVar.Type, arrayVar.Access, arrayVar.ArrayLength);
                            modNode.AddSubNode(modN);
                        }
                        else
                        {
                            if (vmNode is VmNodeObjectType vmOt)
                            {
                                modN = new ModNodeObjectType(vmOt.Name, vmOt.NodeId);
                                modNode.AddSubNode(modN);
                            }
                            else
                            {
                                if (vmNode is VmNodeObject vmO)
                                {
                                    modN = new ModNodeObject(vmO.Name, vmO.NodeId, vmO.ObjectType);
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
                if(vmNode is VmNodeServer vmServer)
                {
                    ModNodeServer modServer = new ModNodeServer(vmServer.Name, vmServer.Encrypt);
                    foreach(VmNode sub in vmServer.SubNodes)
                    {
                        SaveVmNode(modServer, sub);
                    }
                    opcUa.Nodes.Add(modServer);
                }
            }
            opcUa.WriteXml(fileName);
        }
        private async Task OkCommandAsync()
        {

        }
        private async Task CancelCommandAsync()
        {

        }
    }
}
