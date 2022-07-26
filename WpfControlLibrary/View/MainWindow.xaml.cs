﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfControlLibrary.ViewModel;

namespace WpfControlLibrary.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MouseSelectAll(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.SelectAll();
            }
        }

        private void KeyboardSelectAll(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.SelectAll();
            }
        }

        private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (!tb.IsKeyboardFocusWithin)
                {
                    e.Handled = true;
                    tb.Focus();
                }
            }
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            DialogResult = true;
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            DialogResult = false;
            Close();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                if (DataContext is MainWindowViewModel vm)
                {
                    vm.SelectedVmNode = e.NewValue;
                    vm.IsEnabledArrayVar = vm.IsEnabledObjectVar = vm.IsEnabledClientVar = vm.IsEnabledFolder = vm.IsEnabledClientGroup = vm.IsEnabledObjectType =
                        vm.IsEnabledSimpleVar = false;
                    if (e.NewValue is VmNodeNs vmNs)
                    {
                        vm.IsEnabledFolder = vm.IsEnabledArrayVar = vm.IsEnabledSimpleVar = true;
                        vm.FolderName = NameFactory.NextName(vmNs.NsIndex, NameFactory.NameFolder);
                    }
                    else
                    {
                        if (e.NewValue is VmNodeFolder vmFolder)
                        {
                            vm.IsEnabledFolder = vm.IsEnabledArrayVar = vm.IsEnabledSimpleVar = true;
                            if (vmFolder.GetNamespace(out ushort ns))
                            {
                                vm.FolderName = NameFactory.NextName(ns, NameFactory.NameFolder);
                            }
                        }
                        else
                        {
                            if(e.NewValue is VmNodeClient vmClient)
                            {
                                vm.IsEnabledClientGroup = true;
                                vm.ClientGroupPeriod = 500;
                            }
                        }
                    }
                }
            }
        }

        private void AddFolder_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                if (vm.SelectedVmNode != null)
                {
                    VmNode vn = (VmNode)vm.SelectedVmNode;
                    ushort ns = 0;
                    if (vm.SelectedVmNode is VmNodeNs vmNs)
                    {
                        ns = vmNs.NsIndex;
                    }
                    else
                    {
                        if (!vn.GetNamespace(out ns))
                        {
                            return;
                        }
                    }
                    string nodeId = NodeIdFactory.GetNextNodeId(ns);
                    VmNodeFolder vmFolder = new VmNodeFolder(vm.FolderName, nodeId, true, true);
                    vn.AddVmNode(vmFolder);
                    NodeIdFactory.SetNextNodeId(nodeId);
                    NameFactory.SetName(ns, vm.FolderName);
                    vn.IsExpanded = true;
                }
            }
        }

        private void AddVars_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                if (vm.SelectedVmNode != null)
                {
                    VmNode vn = (VmNode)vm.SelectedVmNode;
                    ushort ns = 0;
                    if (vm.SelectedVmNode is VmNodeNs vmNs)
                    {
                        ns = vmNs.NsIndex;
                    }
                    else
                    {
                        if (!vn.GetNamespace(out ns))
                        {
                            return;
                        }
                    }
                    for (int i = 0; i < vm.NrOfAddedVars; ++i)
                    {
                        string nodeId = NodeIdFactory.GetNextNodeId(ns);
                        string name = NameFactory.NextName(ns, NameFactory.NameSimpleVar);
                        VmNodeSimpleVariable vmSimple = new VmNodeSimpleVariable(name, nodeId, Model.ModOpcUa.BasicTypes[0], Model.ModOpcUa.VarAccess[0], false, true);
                        vn.AddVmNode(vmSimple);
                        NodeIdFactory.SetNextNodeId(nodeId);
                        NameFactory.SetName(ns, name);
                    }
                    vn.IsExpanded = true;
                }
            }
        }

        private void AddArray_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                if (vm.SelectedVmNode != null)
                {
                    VmNode vn = (VmNode)vm.SelectedVmNode;
                    ushort ns = 0;
                    if (vm.SelectedVmNode is VmNodeNs vmNs)
                    {
                        ns = vmNs.NsIndex;
                    }
                    else
                    {
                        if (!vn.GetNamespace(out ns))
                        {
                            return;
                        }
                    }
                    string nodeId = NodeIdFactory.GetNextNodeId(ns);
                    string name = NameFactory.NextName(ns, NameFactory.NameArrayVar);
                    VmNodeArrayVariable vmArray = new VmNodeArrayVariable(name, nodeId, Model.ModOpcUa.BasicTypes[0], Model.ModOpcUa.VarAccess[0], vm.ArrayLength, false, true);
                    vn.AddVmNode(vmArray);
                    NodeIdFactory.SetNextNodeId(nodeId);
                    NameFactory.SetName(ns, name);
                    vn.IsExpanded = true;
                }
            }
        }

        private void AddClient_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                string name = NameFactory.NextName(0, NameFactory.NameClient);
                VmNodeClient vmClient = new VmNodeClient(name, vm.ClientIpAddress, false, true, false);
                vm.Nodes.Add(vmClient);
                NameFactory.SetName(0, name);
            }
        }

        private void AddGroup_Click(object sender, RoutedEventArgs e)
        {
            if(DataContext is MainWindowViewModel vm)
            {
                if (vm.SelectedVmNode != null)
                {
                    string name = NameFactory.NextName(0, NameFactory.NameClientGroup);
                    VmNodeClientGroup vmGroup = new VmNodeClientGroup(name, (ushort)vm.ClientGroupPeriod, "Read", true, false);
                    VmNode vn = (VmNode)vm.SelectedVmNode;
                    vn.AddVmNode(vmGroup);
                    NameFactory.SetName(0, name);
                    vn.IsExpanded=true;
                }
            }
        }
    }
}
