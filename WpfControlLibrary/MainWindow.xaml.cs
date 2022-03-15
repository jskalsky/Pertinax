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

        private void AddObject_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel vm = DataContext as MainViewModel;
            if (vm != null)
            {
                OpcObject oo = vm.AddObject("Pertinax");
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
                if(vm.SelectedOpcObject != null)
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

        private void CheckBoxPub_Click(object sender, RoutedEventArgs e)
        {
        }

        private void MenuItemAddPub_Click(object sender, RoutedEventArgs e)
        {
            if(DataContext is MainViewModel vm)
            {
                if(vm.SelectedOpcObject != null)
                {
                    vm.PublisherObjects.Add(new PublisherItem(vm.SelectedOpcObject.Name, 100));
                }
            }
        }

        private void MenuItemAddSub_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                if (vm.SelectedOpcObject != null)
                {
                    vm.SubscriberObjects.Add(new SubscriberItem(vm.SelectedOpcObject.Name, 1));
                }
            }
        }
    }
}
