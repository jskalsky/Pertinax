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

namespace WpfControlLibrary
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
                OpcObject oo = new OpcObject(true, false, false, false, false);
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
                        mvm.SelectedOpcObject.Items.Remove(ooi);
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
                OpcObject oo = new OpcObject(false,true,false,false,false);
                vm.Objects.Add(oo);
                vm.SelectedOpcObject = oo;
                Objects.ScrollIntoView(oo);

                Debug.Print($"Selected {vm.SelectedOpcObject}, {vm.SelectedOpcObject.Name}");
                vm.ClientObjects.Add(new ClientItem(string.Empty, vm.SelectedOpcObject.Name, "XXX.XXX.XXX.XXX", vm.SelectedOpcObject, true, 100, true));
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

        private void TextBox_LostFocus()
        {

        }
    }
}
