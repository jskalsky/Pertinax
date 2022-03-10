using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
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
    /// Interaction logic for IpAddressBox.xaml
    /// </summary>
    public partial class IpAddressBox : UserControl
    {
        public IpAddressBox()
        {
            InitializeComponent();
            File.WriteAllText("e:\\zat15.log", $"NumericUpDown");
            //            (this.Content as FrameworkElement).DataContext = this;
        }

        public static readonly DependencyProperty IpAddressProperty = DependencyProperty.Register("IpAddress", typeof(IPAddress), typeof(IpAddressBox));

        public IPAddress IpAddress
        {
            get { return (IPAddress)GetValue(IpAddressProperty); }
            set { SetValueIpAddress(IpAddressProperty, value); DivideToParts(value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void SetValueIpAddress(DependencyProperty property,object value,[System.Runtime.CompilerServices.CallerMemberName] string p=null)
        {
            SetValue(property, value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }
        private void DivideToParts(IPAddress address)
        {
            byte[] addr = address.GetAddressBytes();
            Part1.Text = addr[0].ToString();
            Part2.Text = addr[1].ToString();
            Part3.Text = addr[2].ToString();
            Part4.Text = addr[3].ToString();
        }
        private void Part1_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                tb.SelectAll();
            }
        }

        private void Part2_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                tb.SelectAll();
            }
        }

        private void Part3_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                tb.SelectAll();
            }
        }

        private void Part4_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                tb.SelectAll();
            }
        }

        private void Part1_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
