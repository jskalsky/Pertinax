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
        private bool _browse;
        public Client(string ip)
        {
            ServerIpAddress = ip;
            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 10);
            Connected = false;
            _browse = true;
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
            if (Connected && _browse)
            {
                int nr = 0;
                BrowseResponse[] br = null;
                result = OpcUa.Browse(85, ref nr, br);
                Debug.Print($"Browse {nr}");
                br = new BrowseResponse[nr];
                for (int i = 0; i < nr; ++i)
                {
                    br[i] = new BrowseResponse(0, 0, new byte[32], 0, new byte[32], new byte[32]);
                }
                Debug.Print($"br {br[0]}, {br[1]}, {nr}");
                result = OpcUa.Browse(85, ref nr, br);
                for (int i = 0; i < nr; ++i)
                {
                    Debug.Print($"{br[i]}");
                    Debug.Print($"i= {i}, {br[i].numeric}");
                }
            }
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
