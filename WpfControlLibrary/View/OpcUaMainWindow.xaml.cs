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
        private readonly Dictionary<DataModelType, List<string>> _DataModelTypes = new Dictionary<DataModelType, List<string>>()
        {
            { DataModelType.Namespace, new List<string>(){ "MiAddFolder", "MiAddVar"} },
            { DataModelType.Folder, new List<string>(){"MiAddFolder", "MiAddObjectType", "MiAddVar" } },
            { DataModelType.SimpleVariable, new List<string>()},
            { DataModelType.ArrayVariable, new List<string>()},
            { DataModelType.ObjectVariable, new List<string>()},
            { DataModelType.ObjectType, new List<string>(){ "MiAddVar" } }
        };

        public OpcUaMainWindow()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is OpcUaViewModel vm)
            {
                vm.SelectedNode = e.NewValue as DataModelNode;
            }
            e.Handled = true;
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (sender is ContextMenu menu)
            {
                if (DataContext is OpcUaViewModel vm)
                {
                    if (vm.SelectedNode != null)
                    {
                        DataModelNamespace ns = vm.SelectedNode.GetNamespace();
                        if (ns != null)
                        {
                            if (ns.Namespace == 0)
                            {
                                foreach (object mi in menu.Items)
                                {
                                    Debug.Print($"mi= {mi}");
                                    if (mi is MenuItem menuItem)
                                    {
                                        menuItem.IsEnabled = false;
                                    }
                                }
                            }
                            else
                            {
                                if (_DataModelTypes.TryGetValue(vm.SelectedNode.DataModelType, out List<string> enabledTypes))
                                {
                                    foreach (object mi in menu.Items)
                                    {
                                        if (mi is MenuItem menuItem)
                                        {
                                            menuItem.IsEnabled = false;
                                            foreach (string type in enabledTypes)
                                            {
                                                if(menuItem.Name == type)
                                                {
                                                    menuItem.IsEnabled=true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
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
                    string[] ids = IdFactory.GetNumericIds(ns.Namespace);
                    if (names != null && names.Length == 1 && ids != null && ids.Length == 1)
                    {
                        DataModelFolder dmf = DataModelNode.GetFolder(names[0], NodeIdBase.GetNodeIdBase($"{ns}:{ids[0]}"), mvm.SelectedNode);
                        mvm.SelectedNode.AddChildren(dmf);
                        mvm.SelectedNode.IsExpanded = true;
                    }
                }
            }
        }

        private void MiAddVar_Click(object sender, RoutedEventArgs e)
        {
            Debug.Print("500");
            DialogAddVariable dialogAddVariable = new DialogAddVariable();
            Debug.Print("501");
            if (dialogAddVariable.DataContext is AddVariableViewModel vm)
            {
                if (DataContext is OpcUaViewModel mvm)
                {
                    List<string> objectTypes = new List<string>();
                    foreach(DataModelObjectType ot in mvm.ObjectTypes)
                    {
                        objectTypes.Add(ot.Name);
                    }
                    vm.ObjectTypes = objectTypes.ToArray();

                    DataModelNamespace ns = mvm.SelectedNode.GetNamespace();
                    if (ns != null)
                    {
                        vm.ParentNode = mvm.SelectedNode;
                        vm.Namespace = ns.Namespace;
                        string[] names = IdFactory.GetNames(ns.Namespace, IdFactory.NameSimpleVar);
                        if (names != null && names.Length == 1)
                        {
                            vm.VarName = names[0];
                        }
                        string[] ids = IdFactory.GetNumericIds(ns.Namespace);
                        if (ids != null && ids.Length == 1)
                        {
                            vm.VarId = ids[0];
                        }
                        Debug.Print("502");
                        vm.SelectedKind = vm.Kind[0];
                        vm.SelectedBasicType = vm.BasicTypes[0];
                        Debug.Print("503");
                    }
                    Debug.Print("504");
                    bool? result = dialogAddVariable.ShowDialog();
                    mvm.SelectedNode.IsExpanded = true;
                    Debug.Print("505");
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
                    string[] ids = IdFactory.GetNumericIds(ns.Namespace);
                    if (names != null && names.Length == 1 && ids != null && ids.Length == 1)
                    {
                        DataModelObjectType dmot = DataModelNode.GetObjectType(names[0], NodeIdBase.GetNodeIdBase($"{ns}:{ids[0]}"), mvm.SelectedNode);
                        mvm.ObjectTypes.Add(dmot);
                        mvm.SelectedNode.AddChildren(dmot);
                        mvm.SelectedNode.IsExpanded = true;
                    }
                }
            }
        }
        private void MiRemove_Click(object sender, RoutedEventArgs e)
        {

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
                    vm.SelectedConnection.AddVar(1, "1000", DataModel.DataModelNode._basicTypes[0], string.Empty);
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
    }
}
