using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfControlLibrary;

namespace ConfigOpcUaNet
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
            ViewModel vm = DataContext as ViewModel;
            if (vm != null)
            {
                OpcObject oo = vm.AddObject("Pertinax");
                if(oo != null)
                {
                    vm.SelectedOpcObject = oo;
                }
            }
        }

        private void ListViewObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ButtonAddItem_Click(object sender, RoutedEventArgs e)
        {
            ViewModel vm = DataContext as ViewModel;
            if (vm != null)
            {
                if(vm.SelectedOpcObject != null)
                {
                    for(int i=0;i<vm.RepetitionRateValue;++i)
                    {
                        vm.SelectedOpcObject.AddItem(vm.ItemName);
                        ++vm.NextItemIndex;
                        vm.ItemName = $"Item{vm.NextItemIndex}";
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
            if (DataContext is ViewModel vm)
            {
                vm.RepetitionRateValue = 1;
                vm.GroupAddressString = "224.0.0.22";
            }
        }
    }
}
