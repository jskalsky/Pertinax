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

                ButtonAdd.IsEnabled = false;
                if (vm.SelectedNode is DataModelFolder dmf)
                {
                    if (dmf.IsParentFolder(DefaultDataModel.FolderVariables))
                    {
                        ButtonAdd.IsEnabled = true;
                    }

                    if (dmf.NodeId.NamespaceIndex != 0)
                    {
                        vm.SelectedName = dmf.Name;
                        ButtonChange.IsEnabled = true;
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
                Client.ClientConnection connection = new Client.ClientConnection() { Crypto = false, IpAddress = "10.10.200.200" };
                vm.Connections.Add(connection);
            }
            e.Handled = true;
        }

        private void MiAddConnectionVar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OpcUaViewModel vm)
            {
                if (vm.SelectedConnection != null)
                {
                    vm.SelectedBasicType = vm.BasicTypes[0];
                    vm.SelectedConnection.AddVar(1, "1000", vm.BasicTypes[0], string.Empty);
                    vm.SelectedConnection.IsExpanded = true;
                }
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
                    if (vm.SelectedConnection != null)
                    {
                        if (vm.SelectedConnection is Client.ClientConnection)
                        {
                            foreach (object mi in menu.Items)
                            {
                                if (mi is MenuItem menuItem)
                                {
                                    if (menuItem.Name == "MiAddConnectionVar")
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
                vm.SelectedConnection = e.NewValue as Client.ClientConnection;
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

        private void ButtonChange_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OpcUaViewModel vm)
            {
                DataModelNode parent = vm.SelectedNode.Parent;
                DataModelNode selectedNode = vm.SelectedNode;
                parent?.Children.Remove(selectedNode);
                selectedNode.Name = vm.VarName;
                if (selectedNode is DataModelSimpleVariable dmsv)
                {
                    dmsv.VarAccess = vm.SelectedAccess;
                    dmsv.VarType = vm.SelectedBasicType;
                    if (vm.SelectedIdType == vm.IdType[0])
                    {
                        NodeIdNumeric nin = new NodeIdNumeric(vm.SelectedNode.NodeId.NamespaceIndex, (uint)vm.SelectedNumeric);
                        selectedNode.NodeId = nin;
                    }
                    else
                    {
                        if (vm.SelectedIdType == vm.IdType[1])
                        {
                            Debug.Print($"Change_Click string");
                            NodeIdString nis = new NodeIdString(vm.SelectedNode.NodeId.NamespaceIndex, vm.SelectedString);
                            selectedNode.NodeId = nis;
                        }
                    }
                }
                else
                {
                    if (selectedNode is DataModelArrayVariable dmav)
                    {
                        dmav.VarAccess = vm.SelectedAccess;
                        dmav.BasicType = vm.SelectedBasicType;
                        if (vm.SelectedIdType == vm.IdType[0])
                        {
                            NodeIdNumeric nin = new NodeIdNumeric(vm.SelectedNode.NodeId.NamespaceIndex, (uint)vm.SelectedNumeric);
                            selectedNode.NodeId = nin;
                        }
                        else
                        {
                            if (vm.SelectedIdType == vm.IdType[1])
                            {
                                Debug.Print($"Change_Click string");
                                NodeIdString nis = new NodeIdString(vm.SelectedNode.NodeId.NamespaceIndex, vm.SelectedString);
                                selectedNode.NodeId = nis;
                            }
                        }
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
    }
}
