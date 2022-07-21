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
            if(sender is ListBoxItem tvi)
            {
                if(tvi.IsSelected)
                {
                    if (tvi.Content is string s)
                    {
                        SelectedPort = s;
                        Debug.Print($"SelectedPort= {SelectedPort}, Handled= {e.Handled}");
                        Close();
                    }
                }
            }
            e.Handled = true;
        }

        private void TreePorts_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Debug.Print($"ItemChanged {sender}, {e.NewValue}");
            if(e.NewValue is PortsNode pn)
            {
                Debug.Print($"pn Flags= {pn.Flags.Count}");
                Flags.Items.Clear();
                foreach (string s in pn.Flags)
                {
                    Flags.Items.Add(s);
                }

                if (Flags.Items.Count != 0)
                {
                    Flags.SelectedIndex = 0;
                }
            }

            e.Handled = true;
        }

        private void Flags_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0] is string s)
                {
                    SelectedPort = s;
                }
            }

            e.Handled = true;
        }
    }
}
