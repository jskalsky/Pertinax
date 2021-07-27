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

namespace OpcUaExplorer.View
{
    /// <summary>
    /// Interaction logic for Setup.xaml
    /// </summary>
    public partial class Setup : Window
    {
        public Setup()
        {
            InitializeComponent();
            Debug.Print($"Context= {DataContext}");
            Ip.DataContext = new ViewModel.IpAddressBoxVm();
            Mask.DataContext = new ViewModel.IpAddressBoxVm();
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            Debug.Print($"Context= {DataContext}");
            if (DataContext != null)
            {
                ViewModel.SetupVm sv = DataContext as ViewModel.SetupVm;
                if (sv != null)
                {
                    sv.Ip = new System.Net.IPAddress(new byte[] { byte.Parse(Ip.Part1.Text), byte.Parse(Ip.Part2.Text), byte.Parse(Ip.Part3.Text), byte.Parse(Ip.Part4.Text) });
                }
            }
            DialogResult = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                ViewModel.SetupVm sv = DataContext as ViewModel.SetupVm;
                if (sv != null)
                {
                    byte[] ip = sv.Ip.GetAddressBytes();
                    Ip.Part1.Text = ip[0].ToString();
                    Ip.Part2.Text = ip[1].ToString();
                    Ip.Part3.Text = ip[2].ToString();
                    Ip.Part4.Text = ip[3].ToString();
                }
            }
            Ip.Part1.SelectAll();
            Ip.Part1.Focus();
        }
    }
}
