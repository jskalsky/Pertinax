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
using WpfControlLibrary.ViewModel;
using WpfControlLibrary.DataModel;

namespace WpfControlLibrary.View
{
    /// <summary>
    /// Interaction logic for DialogAddVariable.xaml
    /// </summary>
    public partial class DialogAddVariable : Window
    {
        public DialogAddVariable()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            DialogResult = true;
            Close();
        }

        private void Kind_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.Print($"Kind_SelectionChanged {sender}, {e.AddedItems.Count}");
            if (e.AddedItems.Count == 1)
            {
                if (e.AddedItems[0] is string selectedKind)
                {
                    Debug.Print($"selectedKind= {selectedKind}");
                    if (DataContext is AddVariableViewModel vm)
                    {
                        vm.VisSimple = Visibility.Collapsed;
                        vm.VisArray = Visibility.Collapsed;
                        vm.VisObject = Visibility.Collapsed;
                        vm.VisId = Visibility.Collapsed;

                        if (selectedKind == vm.Kind[0])
                        {
                            vm.VisSimple = Visibility.Visible;
                            if (vm.VarCount == 1)
                            {
                                vm.VisId = Visibility.Visible;
                            }
                        }
                        else
                        {
                            if (selectedKind == vm.Kind[1])
                            {
                                vm.VisSimple = Visibility.Visible;
                                vm.VisArray = Visibility.Visible;
                                if (vm.VarCount == 1)
                                {
                                    vm.VisId = Visibility.Visible;
                                }

                            }
                            else
                            {
                                if (selectedKind == vm.Kind[2])
                                {
                                    vm.VisObject = Visibility.Visible;
                                    if(vm.VarCount == 1)
                                    {
                                        vm.VisId = Visibility.Visible;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            e.Handled = true;
        }

        private void NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (sender is NumericUpDown nud)
            {
                if (DataContext is AddVariableViewModel vm)
                {
                    if (nud.NudValue == 1)
                    {
                        vm.VisId = Visibility.Visible;
                    }
                    else
                    {
                        vm.VisId = Visibility.Collapsed;
                    }
                }
            }
        }

        private void Write_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddVariableViewModel vm)
            {
                vm.VarWritten = 0;
                for (int i = 0; i < vm.VarCount; i++)
                {
                    if(vm.SelectedKind == vm.Kind[0])
                    {
                        DataModelSimpleVariable node = DataModelNode.GetSimpleVariable(vm.VarName, NodeIdBase.GetNodeIdBase($"{vm.Namespace}:{vm.VarId}"),
                            vm.SelectedBasicType, vm.SelectedAccess, vm.ParentNode);
                        vm.ParentNode.AddChildren(node);
                        ++vm.VarWritten;
                    }
                }
            }
        }
    }
}
