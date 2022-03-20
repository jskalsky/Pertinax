using ConfigOpcUa;
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

namespace ZatCad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ConfigOpcUa.ConfigOpcUa configOpcUa;
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                configOpcUa = new ConfigOpcUa.ConfigOpcUa();
            }
            catch(Exception exc)
            {
                File.WriteAllText("c:\\Work\\zat.log", exc.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(DataContext is MainViewModel mvm)
                {
                    string fileName = App.AppFolder + '\\' + mvm.SelectedCfg;
                    configOpcUa.LoadConfig(fileName);
                    configOpcUa.MakeConfig(null, fileName);
                }
            }
            catch(Exception exc)
            {
                File.WriteAllText("c:\\Work\\zat.log", exc.Message);
            }
        }

        private void ButtonPort_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel mvm)
            {
                string fileName = App.AppFolder + '\\' + mvm.SelectedCfg;
                configOpcUa.LoadConfig(fileName);
                string port = configOpcUa.CreatePort();
            }
        }
    }
}
