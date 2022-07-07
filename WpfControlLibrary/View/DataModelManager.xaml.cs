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

namespace WpfControlLibrary.View
{
    /// <summary>
    /// Interaction logic for DataModelManager.xaml
    /// </summary>
    public partial class DataModelManager : Window
    {
        public DataModelManager()
        {
            InitializeComponent();
        }

        private void Kind_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is ViewModel.DataModelManagerViewModel viewModel)
            {
                viewModel.VisibilityArray = (viewModel.SelectedKind == viewModel.Kind[1]) ? Visibility.Visible : Visibility.Collapsed;
                viewModel.VisibilityObject = (viewModel.SelectedKind == viewModel.Kind[2]) ? Visibility.Visible : Visibility.Collapsed;
                viewModel.VisibilitySimpleOrArray = (viewModel.SelectedKind == viewModel.Kind[0] || viewModel.SelectedKind == viewModel.Kind[1]) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModel.DataModelManagerViewModel viewModel)
            {
                viewModel.Variables.Add(new ViewModel.Variable("Test", 1, "80000", viewModel.Kind[0], viewModel.BasicTypes[1], viewModel.Access[1], 0, "Nic"));
                viewModel.SelectedVariable = viewModel.Variables[0];
            }
        }

        private void IdType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                if (e.AddedItems[0] is string s)
                {
                    if (DataContext is ViewModel.DataModelManagerViewModel vm)
                    {
                        vm.VisibilityIdUpDown = (s == vm.IdType[0]) ? Visibility.Visible : Visibility.Collapsed;
                        vm.VisibilityIdText = (s != vm.IdType[0]) ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
            }
        }
    }
}
