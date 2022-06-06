using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace WpfControlLibrary.View
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        public NumericUpDown()
        {
            Debug.Print("Numeric konstruktor");
            InitializeComponent();
            //            DataContext = this;
            Debug.Print("Numeric konstruktor konec");
        }

        public static readonly DependencyProperty NudMinProperty = DependencyProperty.Register("NudMin", typeof(int), typeof(NumericUpDown), new PropertyMetadata(0));
        public int NudMin
        {
            get { return (int)GetValue(NudMinProperty); }
            set { SetValue(NudMinProperty, value); }
        }

        public static readonly DependencyProperty NudMaxProperty = DependencyProperty.Register("NudMax", typeof(int), typeof(NumericUpDown), new PropertyMetadata(100));
        public int NudMax
        {
            get { return (int)GetValue(NudMaxProperty); }
            set { SetValue(NudMaxProperty, value); Debug.Print($"Set NudMax {value}");}
        }
        public static readonly DependencyProperty NudValueProperty = DependencyProperty.Register("NudValue", typeof(int), typeof(NumericUpDown), new PropertyMetadata(0, OnNudValuePropertyChanged));
        public int NudValue
        {
            get { return (int)GetValue(NudValueProperty); }
            set { SetValue(NudValueProperty, value); TextBoxValue.Text = NudValue.ToString(); Debug.Print($"Set {value}"); }
        }

        public static readonly DependencyProperty NudWidthProperty = DependencyProperty.Register("NudWidth", typeof(double), typeof(NumericUpDown), new PropertyMetadata(80.0));

        public double NudWidth
        {
            get { return (double)GetValue(NudWidthProperty); }
            set
            {
                SetValue(NudWidthProperty, value); BorderControl.Width = value;
            }
        }
        private static void OnNudValuePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Debug.Print($"NudValuePropertyChanged {dependencyObject}, {e.NewValue}");
            if (dependencyObject is NumericUpDown nud)
            {
                nud.TextBoxValue.Text = e.NewValue.ToString();
            }
        }
        public static readonly DependencyProperty NudIncrementProperty = DependencyProperty.Register("NudIncrement", typeof(int), typeof(NumericUpDown), new PropertyMetadata(1, OnNudValuePropertyChanged));
        public int NudIncrement
        {
            get { return (int)GetValue(NudIncrementProperty); }
            set { SetValue(NudIncrementProperty, value); }
        }

        private void ButtonUp_Click(object sender, RoutedEventArgs e)
        {
            Debug.Print($"Up {NudValue}, {NudIncrement}, {NudMax}");
            if (NudValue + NudIncrement <= NudMax)
            {
                NudValue += NudIncrement;
                TextBoxValue.Text = NudValue.ToString();
            }
        }

        private void TextBoxValue_LostFocus(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TextBoxValue.Text, out int val))
            {
                if (val >= NudMin && val <= NudMax)
                {
                    NudValue = val;
                }
            }
        }

        private void TextBoxValue_Loaded(object sender, RoutedEventArgs e)
        {
            TextBoxValue.Text = NudValue.ToString();
        }

        private void ButtonDown_Click(object sender, RoutedEventArgs e)
        {
            if (NudValue - NudIncrement >= NudMin)
            {
                NudValue -= NudIncrement;
                TextBoxValue.Text = NudValue.ToString();
            }
        }
    }
}
