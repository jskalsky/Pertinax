using ConfigOpcUa;
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

namespace ZatCad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch(Exception exc)
            {
                Debug.Print($"Exception: {exc.Message}");
                StackTrace stackTrace = new StackTrace(exc, true);
                for (int i = 0; i < stackTrace.FrameCount; ++i)
                {
                    Debug.WriteLine($"  {stackTrace.GetFrame(i).GetFileName()}, {stackTrace.GetFrame(i).GetFileLineNumber()} : {stackTrace.GetFrame(i).GetMethod().Name}");
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(DataContext is MainViewModel mvm)
                {
                    string fileName = App.AppFolder + '\\' + mvm.SelectedCfg;
//                    configOpcUa.LoadConfig(fileName);
                    App.configOpcUa.MakeConfig(null, fileName);
                }
            }
            catch(Exception exc)
            {
                Debug.Print($"Exception: {exc.Message}");
                StackTrace stackTrace = new StackTrace(exc, true);
                for (int i = 0; i < stackTrace.FrameCount; ++i)
                {
                    Debug.WriteLine($"  {stackTrace.GetFrame(i).GetFileName()}, {stackTrace.GetFrame(i).GetFileLineNumber()} : {stackTrace.GetFrame(i).GetMethod().Name}");
                }
            }
        }

        private void ButtonPort_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel mvm)
            {
                string fileName = App.AppFolder + '\\' + mvm.SelectedCfg;
                App.configOpcUa.LoadConfig(fileName);
                string port = App.configOpcUa.CreatePort();
                Debug.Print($"port= {port}");
            }
        }
    }
}
