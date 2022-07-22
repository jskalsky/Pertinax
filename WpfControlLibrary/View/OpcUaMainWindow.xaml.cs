using System;
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
using WpfControlLibrary.DataModel;
using System.Diagnostics;
using WpfControlLibrary.Client;

namespace WpfControlLibrary.View
{
    /// <summary>
    /// Interaction logic for OpcUaMainWindow.xaml
    /// </summary>
    public partial class OpcUaMainWindow : Window
    {
        public OpcUaMainWindow()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is OpcUaViewModel vm)
            {
                vm.SelectedNode = e.NewValue as DataModelNode;
                ButtonAdd.IsEnabled = false;
                vm.VisibilityAddGroup = Visibility.Collapsed;
                if (vm.SelectedNode is DataModelSimpleVariable || vm.SelectedNode is DataModelArrayVariable)
                {
                    vm.VarName = vm.SelectedNode.Name;
                    if (vm.SelectedNode.NodeId is NodeIdNumeric nodeIdNuneric)
                    {
                        vm.SelectedIdType = vm.IdType[0];
                        vm.VisibilityNumeric = Visibility.Visible;
                        vm.VisibilityString = Visibility.Collapsed;
                        vm.SelectedNumeric = (int)nodeIdNuneric.IdentifierNumeric;
                    }
                    else
                    {
                        if (vm.SelectedNode.NodeId is NodeIdString nodeIdString)
                        {
                            vm.SelectedIdType = vm.IdType[1];
                            vm.VisibilityNumeric = Visibility.Collapsed;
                            vm.VisibilityString = Visibility.Visible;
                            vm.SelectedString = nodeIdString.IdentifierString;
                        }
                    }
                }
                else
                {
                    if (vm.SelectedNode is DataModelFolder dmf)
                    {
                        if (dmf.NodeId.NamespaceIndex != 0)
                        {
                            vm.VarName = dmf.Name;
                            ButtonChange.IsEnabled = true;
                            vm.VisibilityAddGroup = Visibility.Visible;
                            ButtonAdd.IsEnabled = true;
                        }
                    }
                    else
                    {
                        if (vm.SelectedNode is DataModelObjectType)
                        {
                            ButtonAdd.IsEnabled = true;
                        }
                        else
                        {
                            if (vm.SelectedNode is DataModelObjectVariable dmov)
                            {
                                if (dmov.NodeId.NamespaceIndex != 0)
                                {
                                    ButtonChange.IsEnabled = true;
                                    vm.SelectedName = dmov.Name;
                                }
                            }
                        }
                    }
                }
            }
            e.Handled = true;
        }

        private void EnableMenuItem(ContextMenu menu, string itemName)
        {
            foreach (object mi in menu.Items)
            {
                if (mi is MenuItem menuItem)
                {
                    if (menuItem.Name == itemName)
                    {
                        menuItem.IsEnabled = true;
                        break;
                    }
                }
            }
        }
        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (sender is ContextMenu menu)
            {
                foreach (object mi in menu.Items)
                {
                    Debug.Print($"mi= {mi}");
                    if (mi is MenuItem menuItem)
                    {
                        menuItem.IsEnabled = false;
                    }
                }

                if (DataContext is OpcUaViewModel vm)
                {
                    if (vm.SelectedNode is DataModelNamespace dmns)
                    {
                        if (dmns.Namespace == 0)
                        {
                            return;
                        }

                        EnableMenuItem(menu, "MiAddFolder");
                    }
                    else
                    {
                        if (vm.SelectedNode is DataModelFolder dmf)
                        {
                            if (dmf.Name == "ObjectTypes")
                            {
                                EnableMenuItem(menu, "MiAddObjectType");
                            }
                            else
                            {
                                EnableMenuItem(menu, "MiAddFolder");
                            }
                        }
                    }
                }
            }
            e.Handled = true;
        }
        private void MiAddFolder_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OpcUaViewModel mvm)
            {
                DataModelNamespace ns = mvm.SelectedNode.GetNamespace();
                if (ns != null)
                {
                    string[] names = IdFactory.GetNames(ns.Namespace, IdFactory.NameFolder);
                    NodeIdNumeric numeric = NodeIdBase.GetNextSystemNodeId(ns.Namespace);
                    if (names != null && names.Length == 1 && numeric != null)
                    {
                        DataModelFolder dmf = DataModelNode.GetFolder(names[0], numeric, mvm.SelectedNode);
                        dmf.IsEnabled = true;
                        mvm.SelectedNode.AddChildren(dmf);
                        mvm.SelectedNode.IsExpanded = true;
                    }
                }
            }
        }

        private void MiAddObject_Click(object sender, RoutedEventArgs e)
        {
            DataModelManager dmm = new DataModelManager();
            if (dmm.DataContext is DataModelManagerViewModel vm)
            {
                if (DataContext is OpcUaViewModel mvm)
                {
                    mvm.SelectedNode.IsExpanded = true;
                }
            }
        }

        private void MiAddObjectType_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OpcUaViewModel mvm)
            {
                DataModelNamespace ns = mvm.SelectedNode.GetNamespace();
                if (ns != null)
                {
                    string[] names = IdFactory.GetNames(ns.Namespace, IdFactory.NameObjectType);
                    NodeIdNumeric numeric = NodeIdBase.GetNextSystemNodeId(ns.Namespace);
                    if (names != null && names.Length == 1 && numeric != null)
                    {
                        DataModelObjectType dmot = new DataModelObjectType(names[0], numeric, mvm.SelectedNode);
                        dmot.IsEnabled = true;
                        mvm.SelectedNode.AddChildren(dmot);
                        mvm.SelectedNode.IsExpanded = true;
                    }
                }
            }
        }

        private void MiRemove_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OpcUaViewModel mvm)
            {
                if (mvm.SelectedNode is DataModelNamespace dmns)
                {

                }
                else
                {
                    if (mvm.SelectedNode is DataModelFolder dmf)
                    {

                    }
                }
            }
        }
        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            DialogResult = true;
            Close();
        }

        private void SelectAddress(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.SelectAll();
            }
        }

        private void KeyboardSelectAddress(object sender, KeyboardFocusChangedEventArgs e)
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

        private void MiAddConnection_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OpcUaViewModel vm)
            {
                Client.ClientConnection cc = new ClientConnection("10.10.200.200", false);
                vm.Connections.Add(cc);
            }
            e.Handled = true;
        }
        private void MiConnectionRemove_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Connections_Opened(object sender, RoutedEventArgs e)
        {
            if (sender is ContextMenu menu)
            {
                if (DataContext is OpcUaViewModel vm)
                {
                    foreach (object mi in menu.Items)
                    {
                        if (mi is MenuItem menuItem)
                        {
                            menuItem.IsEnabled = false;
                            if (menuItem.Name == "MiAddConnection")
                            {
                                menuItem.IsEnabled = true;
                            }
                        }
                    }
                    if (vm.SelectedConnectionObject != null)
                    {
                        if (vm.SelectedConnectionObject is Client.ClientConnection)
                        {
                            foreach (object mi in menu.Items)
                            {
                                if (mi is MenuItem menuItem)
                                {
                                    if (menuItem.Name == "MiAddGroup")
                                    {
                                        menuItem.IsEnabled = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Connections_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is OpcUaViewModel vm)
            {
                vm.SelectedConnectionObject = e.NewValue;
                vm.VisibilityConProperty = Visibility.Collapsed;
                vm.VisibilityGroupProperty = Visibility.Collapsed;
                vm.VisibilityVarProperty = Visibility.Collapsed;
                if (vm.SelectedConnectionObject is Client.ClientConnection cc)
                {
                    vm.VisibilityConProperty = Visibility.Visible;
                    vm.ConnectionIpAddress = cc.IpAddress;
                }
                else
                {
                    if (vm.SelectedConnectionObject is Client.Group gr)
                    {
                        vm.VisibilityGroupProperty = Visibility.Visible;
                        vm.ConnectionPeriod = gr.Period;
                        vm.ConnectionService = gr.Service;
                        vm.VisibilityVarProperty = Visibility.Visible;
                        vm.ConnectionNs = 1;
                        vm.ConnectionIdType = vm.IdType[0];
                        vm.VisibilityNumeric = Visibility.Visible;
                        vm.VisibilityString = Visibility.Collapsed;
                        vm.ConnectionIdNumeric = 1000;
                        vm.ConnectionIdString = "Variable";
                        vm.SelectedBasicType = vm.BasicTypes[0];
                        vm.VisibilityAddVars = Visibility.Visible;
                        vm.VisibilityChangeVar = Visibility.Collapsed;
                        vm.ConnectionNrVars = 1;
                        vm.Vars = gr.Vars;
                    }
                }
            }
            e.Handled = true;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OpcUaViewModel vm)
            {
                if (vm.SelectedNode != null)
                {
                    DataModelNamespace ns = vm.SelectedNode.GetNamespace();
                    string nameBase = (vm.ArrayLength == 0) ? IdFactory.NameSimpleVar : IdFactory.NameArrayVar;
                    string[] names = IdFactory.GetNames(ns.Namespace, nameBase, vm.VarCount);
                    NodeIdNumeric[] ids = NodeIdBase.GetNextNodeIds(ns.Namespace, vm.VarCount);
                    for (int i = 0; i < vm.VarCount; i++)
                    {
                        if (vm.ArrayLength == 0)
                        {
                            DataModelSimpleVariable dmsv = new DataModelSimpleVariable(names[i], ids[i], vm.SelectedBasicType,
                            vm.SelectedAccess, vm.SelectedNode);
                            vm.SelectedNode.AddChildren(dmsv);
                        }
                        else
                        {
                            DataModelArrayVariable dmav = new DataModelArrayVariable(names[i], ids[i], vm.SelectedBasicType, vm.SelectedAccess,
                            vm.ArrayLength, vm.SelectedNode);
                            vm.SelectedNode.AddChildren(dmav);
                        }
                    }
                    if (vm.SelectedNode.Children.Count > 0)
                    {
                        vm.SelectedNode.IsExpanded = true;
                    }
                }
            }
        }

        private void IdType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is OpcUaViewModel vm)
            {
                if (e.AddedItems.Count == 1 && e.AddedItems[0] is string sel)
                {
                    if (sel == vm.IdType[0])
                    {
                        vm.VisibilityNumeric = Visibility.Visible;
                        vm.VisibilityString = Visibility.Collapsed;
                    }
                    else
                    {
                        vm.VisibilityNumeric = Visibility.Collapsed;
                        vm.VisibilityString = Visibility.Visible;
                    }
                }
            }
            e.Handled = true;
        }

        public bool TestNodeId(NodeIdBase newNodeId, bool isSystem, DataModelNode tag = null, NodeIdBase oldNodeId = null)
        {
            if (oldNodeId != null)
            {
                NodeIdBase.RemoveId(oldNodeId);
            }

            bool result = isSystem ? NodeIdBase.AddSystemNodeId(newNodeId.NamespaceIndex, newNodeId) : NodeIdBase.AddVarNodeId(newNodeId.NamespaceIndex, newNodeId);

            if (!result)
            {
                WpfControlLibrary.ViewModel.OpcUaViewModel.AddStatusMessage(WpfControlLibrary.ViewModel.StatusMsg._messageTypes[0],
                    $"NodeId {newNodeId.GetNodeName()} již existuje", tag);
            }

            return result;
        }
        public NodeIdBase CreateNodeId(ushort ns, uint numeric, string s, string idType, string[] idTypes)
        {
            if (idType == idTypes[0])
            {
                NodeIdNumeric nin = new NodeIdNumeric(ns, numeric);
                return nin;
            }
            else
            {
                if (idType == idTypes[1])
                {
                    NodeIdString nis = new NodeIdString(ns, s);
                    return nis;
                }
            }

            return null;
        }
        private void ButtonChange_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OpcUaViewModel vm)
            {
                DataModelNode parent = vm.SelectedNode.Parent;
                DataModelNode selectedNode = vm.SelectedNode;
                parent?.Children.Remove(selectedNode);
                selectedNode.Name = vm.VarName;
                NodeIdBase nib = null;
                bool testResult = false;
                if (selectedNode is DataModelSimpleVariable dmsv)
                {
                    dmsv.VarAccess = vm.SelectedAccess;
                    dmsv.VarType = vm.SelectedBasicType;
                    nib = CreateNodeId(selectedNode.NodeId.NamespaceIndex, (uint)vm.SelectedNumeric,
                        vm.SelectedString, vm.SelectedIdType, vm.IdType);
                    testResult = TestNodeId(nib, false, selectedNode, selectedNode.NodeId);
                    selectedNode.NodeId = nib;
                }
                else
                {
                    if (selectedNode is DataModelArrayVariable dmav)
                    {
                        dmav.VarAccess = vm.SelectedAccess;
                        dmav.BasicType = vm.SelectedBasicType;
                        dmav.ArrayLength = vm.ArrayLength;
                        nib = CreateNodeId(selectedNode.NodeId.NamespaceIndex, (uint)vm.SelectedNumeric,
                            vm.SelectedString, vm.SelectedIdType, vm.IdType);
                        testResult = TestNodeId(nib, false, selectedNode, selectedNode.NodeId);
                        selectedNode.NodeId = nib;
                    }
                    else
                    {
                        if (selectedNode is DataModelFolder dmf)
                        {
                            dmf.Name = vm.VarName;
                        }
                    }
                }
                parent?.Children.Add(selectedNode);
                vm.SelectedNode = selectedNode;
            }

            //            DataModelTree.Items.Refresh();
            //            DataModelTree.UpdateLayout();
            e.Handled = true;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OpcUaViewModel vm)
            {
                if (vm.SelectedNode != null)
                {
                    DataModelNamespace ns = vm.SelectedNode.GetNamespace();
                    string nameBase = (vm.ArrayLength == 0) ? IdFactory.NameSimpleVar : IdFactory.NameArrayVar;
                    string[] names = IdFactory.GetNames(ns.Namespace, nameBase, vm.VarCount);
                    NodeIdNumeric[] ids = NodeIdBase.GetNextNodeIds(ns.Namespace, vm.VarCount);
                    for (int i = 0; i < vm.VarCount; i++)
                    {
                        if (vm.ArrayLength == 0)
                        {
                            DataModelSimpleVariable dmsv = new DataModelSimpleVariable(names[i], ids[i], vm.SelectedBasicType,
                                vm.SelectedAccess, vm.SelectedNode);
                            vm.SelectedNode.AddChildren(dmsv);
                        }
                        else
                        {
                            DataModelArrayVariable dmav = new DataModelArrayVariable(names[i], ids[i], vm.SelectedBasicType, vm.SelectedAccess,
                                vm.ArrayLength, vm.SelectedNode);
                            vm.SelectedNode.AddChildren(dmav);
                        }
                    }
                    if (vm.SelectedNode.Children.Count > 0)
                    {
                        vm.SelectedNode.IsExpanded = true;
                    }
                }
            }

            e.Handled = true;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is OpcUaViewModel vm)
            {
                if (e.AddedItems.Count == 1)
                {
                    if (e.AddedItems[0] is StatusMsg status)
                    {
                        if (status.Tag != null)
                        {
                            if (status.Tag is DataModelSimpleVariable simple)
                            {
                                simple.IsSelected = true;
                                DataModelNode dmn = simple;
                                while (dmn.Parent != null)
                                {
                                    dmn.Parent.IsExpanded = true;
                                    dmn = dmn.Parent;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void MiAddGroup_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is OpcUaViewModel vm)
            {
                if (vm.SelectedConnectionObject is ClientConnection cc)
                {
                    Group group = new Group(100, Group.Services[0]);
                    cc.AddGroup(group);
                    cc.IsExpanded = true;
                }
            }
        }

        private void Change_Connection(object sender, RoutedEventArgs e)
        {
            if (DataContext is OpcUaViewModel vm)
            {
                if (vm.SelectedConnectionObject is ClientConnection cc)
                {
                    cc.IpAddress = vm.ConnectionIpAddress;
                }
            }
        }

        private void Change_Group(object sender, RoutedEventArgs e)
        {
            if (DataContext is OpcUaViewModel vm)
            {
                if (vm.SelectedConnectionObject is Group gr)
                {
                    gr.Period = vm.ConnectionPeriod;
                    gr.Service = vm.ConnectionService;
                }
            }
        }

        private void GroupAddVars(object sender, RoutedEventArgs e)
        {
            if (DataContext is OpcUaViewModel vm)
            {
                if (vm.SelectedConnectionObject is Group gr)
                {
                    NodeIdBase nid = null;
                    if (vm.ConnectionIdType == vm.IdType[0])
                    {
                        nid = new NodeIdNumeric(vm.ConnectionNs, vm.ConnectionIdNumeric);
                    }
                    else
                    {
                        if (vm.ConnectionIdType == vm.IdType[1])
                        {
                            nid = new NodeIdString(vm.ConnectionNs, vm.ConnectionIdString);
                        }
                    }
                    for (int i = 0; i < vm.ConnectionNrVars; ++i)
                    {
                        ClientVar cv = new ClientVar(gr, nid.GetNodeName(), vm.SelectedBasicType, String.Empty);
                        gr.AddVar(cv);
                        if (nid is NodeIdNumeric numeric)
                        {
                            ++numeric.IdentifierNumeric;
                        }
                    }
                    vm.Vars = gr.Vars;
                }
            }
        }

        private void Vars_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0] is ClientVar cv)
                {
                    if (DataContext is OpcUaViewModel vm)
                    {
                        vm.SelectedClientVar = cv;
                        vm.VisibilityChangeVar = Visibility.Visible;
                        vm.VisibilityGroupProperty = Visibility.Collapsed;
                        vm.VisibilityAddVars = Visibility.Collapsed;

                        vm.ConnectionIdString = "Variable";
                        NodeIdBase nib = NodeIdBase.GetNodeIdBase(cv.Identifier);
                        if (nib is NodeIdNumeric numeric)
                        {
                            vm.ConnectionIdType = vm.IdType[0];
                            vm.VisibilityNumeric = Visibility.Visible;
                            vm.ConnectionIdNumeric = numeric.IdentifierNumeric;
                            vm.ConnectionNs = numeric.NamespaceIndex;
                            vm.VisibilityString = Visibility.Collapsed;
                        }
                        else
                        {
                            if (nib is NodeIdString idS)
                            {
                                vm.ConnectionIdType = vm.IdType[1];
                                vm.VisibilityNumeric = Visibility.Collapsed;
                                vm.ConnectionIdNumeric = 1000;
                                vm.ConnectionNs = idS.NamespaceIndex;
                                vm.ConnectionIdString = idS.IdentifierString;
                                vm.VisibilityString = Visibility.Visible;
                            }
                        }
                        vm.SelectedBasicType = cv.SelectedBasicType;
                    }
                }
            }
            e.Handled = true;
        }

        private void ChangeVar(object sender, RoutedEventArgs e)
        {
            if (DataContext is OpcUaViewModel vm)
            {
                NodeIdBase nib = null;
                if (vm.ConnectionIdType == vm.IdType[0])
                {
                    nib = new NodeIdNumeric(vm.ConnectionNs, vm.ConnectionIdNumeric);
                }
                else
                {
                    if (vm.ConnectionIdType == vm.IdType[1])
                    {
                        nib = new NodeIdString(vm.ConnectionNs, vm.ConnectionIdString);
                    }
                }
                vm.SelectedClientVar.Identifier = nib.GetNodeName();
                vm.SelectedClientVar.SelectedBasicType = vm.SelectedBasicType;
            }
            e.Handled = true;
        }
    }
}
