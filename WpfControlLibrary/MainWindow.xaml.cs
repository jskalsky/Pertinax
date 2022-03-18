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
                int maxWriter = 0;
                int maxDataSet = 0;
                int maxIndex = 0;
                foreach (OpcObject opcObject in vm.Objects)
                {
                    if (opcObject.WriterGroupId > maxWriter)
                    {
                        maxWriter = opcObject.WriterGroupId;
                    }
                    if (opcObject.DataSetWriterId > maxDataSet)
                    {
                        maxDataSet = opcObject.DataSetWriterId;
                    }
                    int index = GetIndex(opcObject.Name);
                    if(index > maxIndex)
                    {
                        maxIndex = index;
                    }
                }
                ++maxWriter;
                ++maxDataSet;
                ++maxIndex;
                OpcObject oo = vm.AddObject($"Pertinax{maxIndex}", maxWriter, maxDataSet);
                if (oo != null)
                {
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
            Debug.Print($"ButtonAddItem_Click");
            MainViewModel vm = DataContext as MainViewModel;
            if (vm != null)
            {
                Debug.Print($"ButtonAddItem_Click 1");
                if (vm.SelectedOpcObject != null)
                {
                    Debug.Print($"ButtonAddItem_Click 2");
                    vm.NextItemIndex = vm.GetMaxItemIndex() + 1;
                    Debug.Print($"ButtonAddItem_Click max= {vm.NextItemIndex}");
                    for (int i = 0; i < vm.RepetitionRateValue; ++i)
                    {
                        vm.ItemName = $"Item{vm.NextItemIndex}";
                        ++vm.NextItemIndex;
                        vm.SelectedOpcObject.AddItem(vm.ItemName);
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

        private void CheckBoxPublish_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                if(vm.SelectedOpcObject != null)
                {
                    if (vm.SelectedOpcObject.EnablePublish)
                    {
                        vm.SelectedOpcObject.EnableInterval = true;
                        vm.SelectedOpcObject.PublishingInterval = 100;
                    }
                    else
                    {
                        vm.SelectedOpcObject.EnableInterval = false;
                        vm.SelectedOpcObject.PublishingInterval = 0;
                    }
                }
            }
        }
    }
}
