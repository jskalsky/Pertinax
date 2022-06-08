using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
using WpfControlLibrary.DataModel;
using WpfControlLibrary.View;
using WpfControlLibrary.ViewModel;

namespace WpfControlLibrary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private int GetIndex(string text)
        {
            int result = 0;
            if (text.Length > 0)
            {
                LinkedList<char> str = new LinkedList<char>();
                for (int i = text.Length - 1; i >= 0; --i)
                {
                    if (char.IsDigit(text[i]))
                    {
                        str.AddFirst(text[i]);
                    }
                    else
                    {
                        break;
                    }
                }
                if (str.Count != 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (char ch in str)
                    {
                        sb.Append(ch);
                    }
                    if (int.TryParse(sb.ToString(), out int index))
                    {
                        result = index;
                    }
                }
            }
            return result;
        }
        private void AddObject_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                Debug.Print($"AddObject_Click");
                OpcObject oo = new OpcObject(true, false, false, false, false);
                Debug.Print($"End");
                vm.Objects.Add(oo);
                vm.SelectedOpcObject = oo;
                Objects.ScrollIntoView(oo);
                ServerItem si = new ServerItem(vm.SelectedOpcObject);
                vm.ServerObjects.Add(si);
            }
        }

        private void ListViewObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                if (vm.SelectedOpcObject != null)
                {
                }
            }
        }

        private void ButtonAddItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                if (vm.SelectedOpcObject != null)
                {
                    for (int i = 0; i < vm.RepetitionRateValue; ++i)
                    {
                        OpcObjectItem ooi = vm.SelectedOpcObject.AddItem();
                        ooi.SelectedBasicType = vm.SelectedSetupItem;
                        ooi.SelectedRank = vm.SelectedSetupRank;
                        ooi.ArraySizeValue = vm.SelectedSetupLength;
                        ListViewItems.ScrollIntoView(ooi);
                    }
                }
            }
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel mvm)
            {
                if (MainViewModel.IsError)
                {
                    MessageBox.Show("Opravte chybu nebo stiskněte Zrušit", "OpcUa", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                foreach (ClientItem ci in mvm.ClientObjects)
                {
                    if (!IPAddress.TryParse(ci.IpAddress, out IPAddress ip))
                    {
                        MessageBox.Show($"Chybně zadaná Ip adresa {ci.IpAddress}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                        e.Handled = true;
                        return;
                    }
                }
            }
            e.Handled = true;
            DialogResult = true;
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.RepetitionRateValue = 1;
                vm.GroupAddressString = "224.0.0.22";
                vm.LocalIpAddressString = "10.10.13.253";
                vm.PublisherId = 1;
            }
        }

        private void AddSubscriber_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "OpcUa cfg files(*.OPCUA)|*.OPCUA|All files(*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                FileName = string.Empty
            };
            if (ofd.ShowDialog() != true) return;
            Debug.Print("AddSubscriber_Click");
            if (DataContext is MainViewModel vm)
            {
                vm.SubscriberPath = ofd.FileName;
            }
        }

        private void PublishObject_Click(object sender, RoutedEventArgs e)
        {
            Debug.Print("Publish");
            if (DataContext is MainViewModel vm)
            {
                OpcObject oo = new OpcObject(false, false, true, false, false);
                vm.Objects.Add(oo);
                Objects.ScrollIntoView(oo);
                Debug.Print("Object added");
                vm.SelectedOpcObject = oo;
                Debug.Print("Je to publish");
                WpfControlLibrary.PublisherItem pi = new PublisherItem(oo, vm.PublisherId);
                vm.PublisherObjects.Add(pi);
                Debug.Print("Pridano");
            }
        }

        private void MenuItemPublish_Click(object sender, RoutedEventArgs e)
        {
            Debug.Print($"sender= {sender}");
            if (DataContext is MainViewModel vm)
            {
                Debug.Print($"Selected {vm.SelectedOpcObject}, {vm.SelectedOpcObject.Name}");
                vm.PublisherObjects.Add(new PublisherItem(vm.SelectedOpcObject, vm.PublisherId));
            }
        }

        private void Objects_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            Debug.Print($"1 {sender}");
        }

        private void Rank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                string rank = (string)e.AddedItems[0];
                if (DataContext is MainViewModel vm)
                {
                    if (sender is ComboBox cb)
                    {
                        if (cb.Tag is OpcObjectItem ooi)
                        {
                            if (rank == "Pole")
                            {
                                ooi.EnableArraySize = true;
                                ooi.ArraySizeValue = 0;
                            }
                            else
                            {
                                ooi.EnableArraySize = false;
                                ooi.ArraySizeValue = -1;
                            }
                        }
                    }
                }
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "OpcUa cfg files(*.OPCUA)|*.OPCUA|All files(*.*)|*.*",
                InitialDirectory = string.IsNullOrEmpty(WpfControlLibrary.Properties.Settings.Default.LastConfigFolder)
                    ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                    : Properties.Settings.Default.LastConfigFolder,
                FileName = string.Empty
            };
            if (ofd.ShowDialog() != true) return;
            Properties.Settings.Default.LastConfigFolder = System.IO.Path.GetDirectoryName(ofd.FileName);
            Properties.Settings.Default.Save();
            Debug.Print("AddSubscriber_Click");
            if (DataContext is MainViewModel vm)
            {
                vm.SubscriberPath = ofd.FileName;
            }
        }

        private void MenuItemChange_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                EditObjectItemDialog editObjectItemDialog = new EditObjectItemDialog();
                Debug.Print($"vmei= {editObjectItemDialog.DataContext}");
                Debug.Flush();
                if (editObjectItemDialog.DataContext is ViewModelEditItems vmei)
                {
                    int selectedCount = 0;
                    OpcObjectItem selectedItem = null;
                    foreach (OpcObjectItem ooi in vm.SelectedOpcObject.Items)
                    {
                        if (ooi.Selected)
                        {
                            ++selectedCount;
                            selectedItem = ooi;
                        }
                    }
                    Debug.Print($"Sel= {selectedCount}");
                    Debug.Flush();
                    if (selectedCount == 1)
                    {
                        if (selectedItem != null) vmei.SelectedBasicType = selectedItem.SelectedBasicType;
                        Debug.Print($"s {vmei.SelectedBasicType}, {selectedItem.SelectedBasicType}");
                        if (selectedItem != null) vmei.SelectedRank = selectedItem.SelectedRank;
                        Debug.Print($"s {vmei.SelectedRank}, {selectedItem.SelectedRank}");
                        if (selectedItem != null) vmei.WriteOutside = selectedItem.WriteOutside;
                        if (selectedItem != null) vmei.ArraySizeValue = selectedItem.ArraySizeValue;
                    }
                    if (editObjectItemDialog.ShowDialog() == true)
                    {
                        foreach (OpcObjectItem ooi in vm.SelectedOpcObject.Items)
                        {
                            if (ooi.Selected)
                            {
                                ooi.SelectedBasicType = vmei.SelectedBasicType;
                                ooi.SelectedRank = vmei.SelectedRank;
                                ooi.WriteOutside = vmei.WriteOutside;
                                ooi.ArraySizeValue = vmei.ArraySizeValue;
                                ooi.Selected = false;
                            }
                        }
                    }
                }
            }
            e.Handled = true;
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbr = MessageBox.Show("Opravdu zrušit vybranou položku(položky) ?", "OpcUa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (mbr == MessageBoxResult.Yes)
            {
                List<OpcObjectItem> erase = new List<OpcObjectItem>();
                if (DataContext is MainViewModel mvm)
                {
                    foreach (OpcObjectItem ooi in mvm.SelectedOpcObject.Items)
                    {
                        if (ooi.Selected)
                        {
                            erase.Add(ooi);
                        }
                    }
                    foreach (OpcObjectItem ooi in erase)
                    {
                        NodeIdBase.Remove(ooi.NodeId);
                        mvm.SelectedOpcObject.Items.Remove(ooi);
                    }

                    if (NodeIdBase.GetNrOfErrors() == 0)
                    {
                        MainViewModel.IsError = false;
                    }
                }
            }
        }

        private void ButtonObjectDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbr = MessageBox.Show("Opravdu zrušit vybraný objekt ?", "OpcUa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (mbr == MessageBoxResult.Yes)
            {
                if (DataContext is MainViewModel mvm)
                {
                    mvm.Objects.Remove(mvm.SelectedOpcObject);
                }
            }
        }

        private void RankSetup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                if (e.AddedItems.Count == 1)
                {
                    if (e.AddedItems[0] is string s)
                    {
                        if (s == "Pole")
                        {
                            vm.EnableSetupLength = true;
                        }
                        else
                        {
                            vm.EnableSetupLength = false;
                        }
                    }
                }
            }
        }

        private void ButtonObjectToClientClick(object sender, RoutedEventArgs e)
        {
            Debug.Print($"ButtonObjectToClientClick");
            if (DataContext is MainViewModel vm)
            {
                OpcObject oo = new OpcObject(false, true, false, false, false);
                vm.Objects.Add(oo);
                vm.SelectedOpcObject = oo;
                Objects.ScrollIntoView(oo);

                Debug.Print($"Selected {vm.SelectedOpcObject}, {vm.SelectedOpcObject.Name}");
                vm.ClientObjects.Add(new ClientItem(string.Empty, vm.SelectedOpcObject.Name, "XXX.XXX.XXX.XXX", vm.SelectedOpcObject, true, 100, false, true));
            }
        }

        private void Ip_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (!IPAddress.TryParse(tb.Text, out IPAddress ip))
                {
                    MessageBox.Show($"Chybně zadaná Ip adresa {tb.Text}", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            e.Handled = true;
        }

        private void Ip_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.SelectAll();
                e.Handled = true;
            }
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

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.SelectedDataModelNode = e.NewValue as DataModelNode;
                e.Handled = true;
            }
        }

        private void MiAddFolder_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                if (vm.SelectedDataModelNode != null)
                {

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
                if (DataContext is MainViewModel mvm)
                {
                    DataModelNamespace ns = mvm.SelectedDataModelNode.GetNamespace();
                    if (ns != null)
                    {
                        vm.ParentNode = mvm.SelectedDataModelNode;
                        vm.Namespace = ns.Namespace;
                        IList<string> names = IdFactory.GetNextName(ns.Namespace, IdFactory.NameSimpleVar);
                        if (names != null && names.Count == 1)
                        {
                            vm.VarName = names[0];
                        }
                        names = IdFactory.GetNextIndex(ns.Namespace);
                        if(names != null && names.Count == 1)
                        {
                            vm.VarId = names[0];
                        }
                        Debug.Print("502");
                        vm.SelectedKind = vm.Kind[2];
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

        }

        private void MiRemove_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
