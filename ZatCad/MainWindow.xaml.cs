﻿using ConfigOpcUaNet;
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

namespace ZatCad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ConfigOpcUa.ConfigOpcUa configOpcUa;
        private string FileName = "e:\\test.xml";
        public MainWindow()
        {
            InitializeComponent();
            configOpcUa = new ConfigOpcUa.ConfigOpcUa();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            configOpcUa.LoadConfig(FileName);
            configOpcUa.MakeConfig(null, FileName);
        }

        private void ButtonPort_Click(object sender, RoutedEventArgs e)
        {
            configOpcUa.LoadConfig(FileName);
            string port = configOpcUa.CreatePort();
        }
    }
}
