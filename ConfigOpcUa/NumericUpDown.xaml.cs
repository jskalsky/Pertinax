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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConfigOpcUa
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        public NumericUpDown()
        {
            Debug.Print("NumericUpDown");
            InitializeComponent();
        }

        public static readonly DependencyProperty UpImageProperty = DependencyProperty.Register("UpImage", typeof(object), typeof(NumericUpDown));

        public object UpImage
        {
            get => (object)GetValue(UpImageProperty);
            set { Debug.Print($"UpImage= {value}"); SetValue(UpImageProperty, value); }
        }
    }
}
