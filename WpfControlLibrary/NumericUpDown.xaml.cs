using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        public NumericUpDown()
        {
            InitializeComponent();
//            DataContext = this;
        }

        public static readonly DependencyProperty MinNumericProperty = DependencyProperty.Register("MinNumeric", typeof(int), typeof(NumericUpDown));
        public int MinNumeric
        {
            get { return (int)GetValue(MinNumericProperty); }
            set { SetValue(MinNumericProperty, value); }
        }

        public static readonly DependencyProperty MaxNumericProperty = DependencyProperty.Register("MaxNumeric", typeof(int), typeof(NumericUpDown));
        public int MaxNumeric
        {
            get { return (int)GetValue(MaxNumericProperty); }
            set { SetValue(MaxNumericProperty, value); }
        }
        public static readonly DependencyProperty NumericValueProperty = DependencyProperty.Register("NumericValue", typeof(int), typeof(NumericUpDown));
        public int NumericValue
        {
            get { return (int)GetValue(NumericValueProperty); }
            set { SetValue(NumericValueProperty, value); }
        }

        private void ButtonUp_Click(object sender, RoutedEventArgs e)
        {
            if(NumericValue + 1 <= MaxNumeric)
            {
                ++NumericValue;
                TextBoxValue.Text = NumericValue.ToString();
            }
        }

        private void TextBoxValue_LostFocus(object sender, RoutedEventArgs e)
        {
            if(int.TryParse(TextBoxValue.Text,out int val))
            {
                if(val >= MinNumeric && val <= MaxNumeric)
                {
                    NumericValue = val;
                }
            }
        }

        private void TextBoxValue_Loaded(object sender, RoutedEventArgs e)
        {
            TextBoxValue.Text = NumericValue.ToString();
        }

        private void ButtonDown_Click(object sender, RoutedEventArgs e)
        {
            if(NumericValue - 1 >= MinNumeric)
            {
                --NumericValue;
                TextBoxValue.Text = NumericValue.ToString();
            }
        }
    }
}
