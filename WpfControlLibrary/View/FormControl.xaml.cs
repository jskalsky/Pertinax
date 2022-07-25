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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace WpfControlLibrary.View
{
    /// <summary>
    /// Interaction logic for FormControl.xaml
    /// </summary>
    public partial class FormControl : System.Windows.Controls.UserControl
    {
        public FormControl()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.Register("SelectedObject", typeof(object), typeof(FormControl), 
            new PropertyMetadata(null, OnSelectedObjectChanged));
        public int SelectedObject
        {
            get { return (int)GetValue(SelectedObjectProperty); }
            set { SetValue(SelectedObjectProperty, value); }
        }
        private static void OnSelectedObjectChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is FormControl fc)
            {
                WindowsFormsHost wfHost = fc.MyFormsHost;
                PropertyGrid pg = (PropertyGrid)wfHost.Child;
                pg.SelectedObject = e.NewValue;
            }
        }
    }
}
