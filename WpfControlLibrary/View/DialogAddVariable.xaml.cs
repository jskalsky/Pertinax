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
                                    if (vm.VarCount == 1)
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
                    IdFactory.RemoveAllPublishedNames(vm.Namespace);
                    IdFactory.RemoveAllPublishedIds(vm.Namespace);
                }
            }
        }

        private void Write_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddVariableViewModel vm)
            {
                vm.VarWritten = 0;
                if (vm.SelectedKind == vm.Kind[0])
                {
                    if (vm.VarCount == 1)
                    {
                        DataModelSimpleVariable node = DataModelNode.GetSimpleVariable(vm.VarName, NodeIdBase.GetNodeIdBase($"{vm.Namespace}:{vm.VarId}"),
                            vm.SelectedBasicType, vm.SelectedAccess, vm.ParentNode);
                        vm.ParentNode.AddChildren(node);
                        ++vm.VarWritten;
                    }
                    else
                    {
                        string[] names = IdFactory.GetNames(vm.Namespace, IdFactory.NameSimpleVar, vm.VarCount);
                        string[] ids = IdFactory.GetNumericIds(vm.Namespace, vm.VarCount);
                        for (int i = 0; i < vm.VarCount; i++)
                        {
                            DataModelSimpleVariable node = DataModelNode.GetSimpleVariable(names[i], NodeIdBase.GetNodeIdBase($"{vm.Namespace}:{ids[i]}"),
                                vm.SelectedBasicType, vm.SelectedAccess, vm.ParentNode);
                            vm.ParentNode.AddChildren(node);
                            ++vm.VarWritten;
                        }
                        IdFactory.RemovePublishedNames(vm.Namespace, names);
                        IdFactory.RemovePublishedIds(vm.Namespace, ids);
                    }
                }
                else
                {
                    if (vm.SelectedKind == vm.Kind[1])
                    {
                        if (vm.VarCount == 1)
                        {
                            DataModelArrayVariable node = DataModelNode.GetArrayVariable(vm.VarName, NodeIdBase.GetNodeIdBase($"{vm.Namespace}:{vm.VarId}"),
                                vm.SelectedBasicType, vm.SelectedAccess, vm.ArrayLength, vm.ParentNode);
                            vm.ParentNode.AddChildren(node);
                            ++vm.VarWritten;
                        }
                    }
                }
            }
        }

        private void VariableProperties_KindChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug.Print($"VariableProperties_KindChanged {e.PropertyName}");
            if (DataContext is AddVariableViewModel vm)
            {
                Debug.Print($"vm.SelectedKind = {vm.SelectedKind}");
                if(vm.SelectedKind == vm.Kind[0])
                {
                    vm.VisVarCount = Visibility.Visible;
                }
                else
                {
                    vm.VisVarCount = Visibility.Collapsed;
                }
            }
        }
    }
}
