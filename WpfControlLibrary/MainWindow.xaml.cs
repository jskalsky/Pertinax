using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            MainViewModel vm = DataContext as MainViewModel;
            if (vm != null)
            {
                OpcObject oo = new OpcObject();
                if (oo != null)
                {
                    vm.Objects.Add(oo);
                    vm.SelectedOpcObject = oo;
                }
            }
        }

        private void ListViewObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainViewModel vm = DataContext as MainViewModel;
            if (vm != null)
            {
                if (vm.SelectedOpcObject != null)
                {
                }
            }
        }

        private void ButtonAddItem_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel vm = DataContext as MainViewModel;
            if (vm != null)
            {
                if (vm.SelectedOpcObject != null)
                {
                    for (int i = 0; i < vm.RepetitionRateValue; ++i)
                    {
                        OpcObjectItem ooi = vm.SelectedOpcObject.AddItem();
                        ooi.SelectedBasicType = vm.SelectedSetupItem;
                        ooi.SelectedRank = vm.SelectedSetupRank;
                    }
                }
            }
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
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
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "OpcUa cfg files(*.OPCUA)|*.OPCUA|All files(*.*)|*.*";
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            ofd.FileName = string.Empty;
            if (ofd.ShowDialog() == true)
            {
                Debug.Print("AddSubscriber_Click");
                if (DataContext is MainViewModel vm)
                {
                    vm.SubscriberPath = ofd.FileName;
                }
            }
        }

        private void PublishObject_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel vm = DataContext as MainViewModel;
            if (vm != null)
            {
                OpcObject oo = new OpcObject(true, false);
                if (oo != null)
                {
                    vm.Objects.Add(oo);
                    vm.SelectedOpcObject = oo;
                    if (oo.Publish)
                    {
                        WpfControlLibrary.PublisherItem pi = new PublisherItem(oo, vm.PublisherId);
                        vm.PublisherObjects.Add(pi);
                    }
                }
            }
        }

        private void MenuItemPublish_Click(object sender, RoutedEventArgs e)
        {
            Debug.Print($"sender= {sender}");
            MainViewModel vm = DataContext as MainViewModel;
            if (vm != null)
            {
                Debug.Print($"Selected {vm.SelectedOpcObject}, {vm.SelectedOpcObject.Name}");
                vm.PublisherObjects.Add(new PublisherItem(vm.SelectedOpcObject, vm.PublisherId));
            }
        }

        private void Objects_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            Debug.Print($"1 {sender}");
            MainViewModel vm = DataContext as MainViewModel;
            if (vm != null)
            {
                Debug.Print($"Selected {vm.SelectedOpcObject}");
                vm.EnableAddToPublisher = vm.SelectedOpcObject.Publish ? true : false;
            }
        }

        private void Rank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count == 1)
            {
                string rank = (string)e.AddedItems[0];
                MainViewModel vm = DataContext as MainViewModel;
                if (vm != null)
                {
                    if(sender is ComboBox cb)
                    {
                        if(cb.Tag is OpcObjectItem ooi)
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
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "OpcUa cfg files(*.OPCUA)|*.OPCUA|All files(*.*)|*.*";
            if(string.IsNullOrEmpty(WpfControlLibrary.Properties.Settings.Default.LastConfigFolder))
            {
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
            else
            {
                ofd.InitialDirectory = Properties.Settings.Default.LastConfigFolder;
            }
            ofd.FileName = string.Empty;
            if (ofd.ShowDialog() == true)
            {
                Properties.Settings.Default.LastConfigFolder = System.IO.Path.GetDirectoryName(ofd.FileName);
                Properties.Settings.Default.Save();
                Debug.Print("AddSubscriber_Click");
                if (DataContext is MainViewModel vm)
                {
                    vm.SubscriberPath = ofd.FileName;
                }
            }
        }
    }
}
