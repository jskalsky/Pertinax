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

namespace ConfigOpcUaNet
{
    /// <summary>
    /// Interaction logic for PortDialog.xaml
    /// </summary>
    public partial class PortDialog : Window
    {
        public PortDialog()
        {
            InitializeComponent();
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void TreeView_Loaded(object sender, RoutedEventArgs e)
        {
            if(DataContext is ViewModel vm)
            {
                TreePorts.Items.Add(vm.RootNode);
            }
        }
    }
}
