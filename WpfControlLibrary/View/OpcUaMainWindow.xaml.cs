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
        private readonly Dictionary<DataModelType, List<DataModelType>> _DataModelTypes = new Dictionary<DataModelType, List<DataModelType>>()
        {
            { DataModelType.Namespace, new List<DataModelType>(){DataModelType.Folder, DataModelType.ObjectVariable, DataModelType.SimpleVariable, DataModelType.ArrayVariable} },
            { DataModelType.Folder, new List<DataModelType>(){DataModelType.Folder, DataModelType.ObjectType, DataModelType.SimpleVariable, DataModelType.ArrayVariable,
                DataModelType.ObjectVariable} },
            { DataModelType.SimpleVariable, new List<DataModelType>()},
            { DataModelType.ArrayVariable, new List<DataModelType>()},
            { DataModelType.ObjectVariable, new List<DataModelType>()},
            { DataModelType.ObjectType, new List<DataModelType>(){DataModelType.SimpleVariable, DataModelType.ArrayVariable, DataModelType.ObjectVariable} }
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
                if (DataContext is MainViewModel vm)
                {
                    if (vm.SelectedDataModelNode != null)
                    {
                        DataModelNamespace ns = vm.SelectedDataModelNode.GetNamespace();
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
                                if (_DataModelTypes.TryGetValue(vm.SelectedDataModelNode.DataModelType, out List<DataModelType> enabledTypes))
                                {
                                    foreach (DataModelType type in enabledTypes)
                                    {
                                        foreach (object mi in menu.Items)
                                        {
                                            if (mi is MenuItem menuItem)
                                            {
                                                switch (menuItem.Name)
                                                {
                                                    case "MiAddFolder":
                                                        menuItem.IsEnabled = (type == DataModelType.Folder) ? true : false;
                                                        break;
                                                    case "MiAddVar":
                                                        menuItem.IsEnabled = (type == DataModelType.SimpleVariable || type == DataModelType.ObjectVariable || type == DataModelType.ArrayVariable) ? true : false;
                                                        break;
                                                    case "MiAddObjectType":
                                                        menuItem.IsEnabled = (type == DataModelType.Folder) ? true : false;
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
                if(ns != null)
                {
                    string[] names = IdFactory.GetNames(ns.Namespace, IdFactory.NameFolder);
                    string[] ids = IdFactory.GetNumericIds(ns.Namespace);
                    if(names != null && names.Length == 1 && ids != null && ids.Length == 1)
                    {
                        DataModelFolder dmf = DataModelNode.GetFolder(names[0], NodeIdBase.GetNodeIdBase($"{ns}:{ids[0]}"), mvm.SelectedNode);
                        mvm.SelectedNode.AddChildren(dmf);
                        IdFactory.RemoveAllPublishedNames(ns.Namespace);
                        IdFactory.RemoveAllPublishedIds(ns.Namespace);
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
                }
                Debug.Print("504");
                bool? result = dialogAddVariable.ShowDialog();
                Debug.Print("505");
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
                        mvm.SelectedNode.AddChildren(dmot);
                        IdFactory.RemoveAllPublishedNames(ns.Namespace);
                        IdFactory.RemoveAllPublishedIds(ns.Namespace);
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
    }
}
