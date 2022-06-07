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
            if(e.AddedItems.Count == 1)
            {
                if (e.AddedItems[0] is string selectedKind)
                {
                    Debug.Print($"selectedKind= {selectedKind}");
                    if(DataContext is AddVariableViewModel vm)
                    {
                        vm.EnableBasicType= false;
                        vm.EnableAccess = false;
                        vm.EnableArrayLength = false;
                        vm.EnableObjectName = false;
                        vm.EnableVarName = false;
                        vm.EnableVarId = false;

                        if (selectedKind == vm.Kind[0])
                        {
                            vm.EnableBasicType = true;
                            vm.EnableAccess = true;
                            if (vm.VarCount == 1)
                            {
                                vm.EnableVarName= true;
                                vm.EnableVarId = true;
                            }
                        }
                        else
                        {
                            if(selectedKind == vm.Kind[1])
                            {
                                vm.EnableBasicType = true;
                                vm.EnableAccess = true;
                                vm.EnableArrayLength = true;
                                if (vm.VarCount == 1)
                                {
                                    vm.EnableVarName = true;
                                    vm.EnableVarId = true;
                                }

                            }
                            else
                            {
                                if(selectedKind == vm.Kind[2])
                                {
                                    vm.EnableObjectName = true;
                                }
                            }
                        }
                    }
                }
            }
            e.Handled = true;
        }
    }
}
