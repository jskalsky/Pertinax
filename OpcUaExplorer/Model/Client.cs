using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace OpcUaExplorer.Model
{
    public class Client : INotifyPropertyChanged
    {
        private DispatcherTimer _timer;
        private bool _connected;
        public Client(string ip)
        {
            ServerIpAddress = ip;
            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 10);
            Connected = false;
        }

        string ServerIpAddress { get; }
        public bool Connected
        {
            get { return _connected; }
            set { _connected = value; OnPropertyChanged("Connected"); }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            string ip = $"opc.tcp://{ServerIpAddress}";
            Debug.Print($"Tick {ip}");
            int result = OpcUa.Connect(ip);
            Debug.Print($"result= {result}");
            Connected = (result != 0) ? false : true;
        }

        public int Open(int security)
        {
            int result = OpcUa.OpenClient(security);
            if (result != 0)
            {
                return result;
            }
            _timer.Start();
            return 0;
        }
    }
}
