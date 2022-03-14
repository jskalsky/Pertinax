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
    /// Interaction logic for PortDialog.xaml
    /// </summary>
    public partial class PortDialog : Window
    {
        public PortDialog()
        {
            InitializeComponent();
            SelectedPort = null;
            Debug.Print("PortDialog");
        }

        public string SelectedPort { get; private set; }
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
        }

        private void TreeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Debug.Print($"DoubleClick {sender}");
            if(sender is TreeViewItem tvi)
            {
                if(tvi.IsSelected)
                {
                    if (tvi.Header is PortsNode pn)
                    {
                        SelectedPort = pn.Text;
                        e.Handled = true;
                        Debug.Print($"SelectedPort= {SelectedPort}, Handled= {e.Handled}");
                        Close();
                    }
                }
            }
        }
    }
}
