using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfControlLibrary
{
    /// <summary>
    /// Interaction logic for NodeIdControl.xaml
    /// </summary>
    public partial class NodeIdControl : UserControl
    {
        public NodeIdControl()
        {
            InitializeComponent();
//            DataContext = this;
        }

        public static readonly DependencyProperty NodeIdProperty = DependencyProperty.Register("NodeId", typeof(NodeIdBase), typeof(NodeIdControl),
            new PropertyMetadata(new NodeIdNumeric(0, 1), OnNodeIdPropertyChanged));

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public NodeIdBase NodeId
        {
            get { return (NodeIdBase)GetValue(NodeIdProperty); }
            set { SetValue(NodeIdProperty, value); }
        }

        private static void OnNodeIdPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Debug.Print($"NudValuePropertyChanged {dependencyObject}, {e.NewValue}");
            if (dependencyObject is WpfControlLibrary.NodeIdControl nic)
            {
                if (e.NewValue is NodeIdBase nib)
                {
                    nic.NamespaceIndex.Text = $"{nib.NamespaceIndex}";
                    nic.Identifier.Text = nib.GetIdentifier();
                }
            }
        }

        private void SelectAddress(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.SelectAll();
            }
        }

        private void KeyboardSelectAddress(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.SelectAll();
            }
        }

        private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (!tb.IsKeyboardFocusWithin)
                {
                    e.Handled = true;
                    tb.Focus();
                }
            }
        }

        private void NamespaceIndex_OnLostFocus(object sender, RoutedEventArgs e)
        {
            Debug.Print($"NamespaceIndex_OnLostFocus");
            string nodeId = $"{NamespaceIndex.Text}:{Identifier.Text}";
            Debug.Print($"nodeId= {nodeId}");
            NodeIdBase nib = NodeIdBase.GetNodeIdBase(nodeId);
            NodeId = nib;
        }

        private void Identifier_OnLostFocus(object sender, RoutedEventArgs e)
        {
            Debug.Print($"Identifier_OnLostFocus");
            string nodeId = $"{NodeId.NamespaceIndex}:{Identifier.Text}";
            Debug.Print($"nodeId= {nodeId}");
            NodeIdBase nib = NodeIdBase.GetNodeIdBase(nodeId);
            NodeId = nib;
        }
    }
}
