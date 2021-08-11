using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;

namespace OpcUaExplorer.ViewModel
{
    public class SetupVm : ViewModelBase
    {
        private List<IPAddress> _iPs;
        private string _part1;
        private string _part2;
        private string _part3;
        private string _part4;
        private RelayCommand<SelectionChangedEventArgs> _ipSelectionChanged;
        private IPAddress _selectedIp;
        private RelayCommand _addIp;
        private RelayCommand _eraseIp;

        public SetupVm()
        {
            _iPs = new List<IPAddress>();
            foreach (string ip in Properties.Settings.Default.Servers)
            {
                _iPs.Add(IPAddress.Parse(ip));
            }
            RaisePropertyChanged("Ips");
            SelectedIp = _iPs[0];
            byte[] bytes = SelectedIp.GetAddressBytes();
            Part1 = bytes[0].ToString();
            Part2 = bytes[1].ToString();
            Part3 = bytes[2].ToString();
            Part4 = bytes[3].ToString();
        }

        public List<IPAddress> Ips
        {
            get { return _iPs; }
        }

        public IPAddress SelectedIp
        {
            get { return _selectedIp; }
            set { _selectedIp = value; RaisePropertyChanged(); }
        }
        public string Part1
        {
            get { return _part1; }
            set { _part1 = value; RaisePropertyChanged(); }
        }
        public string Part2
        {
            get { return _part2; }
            set { _part2 = value; RaisePropertyChanged(); }
        }
        public string Part3
        {
            get { return _part3; }
            set { _part3 = value; RaisePropertyChanged(); }
        }
        public string Part4
        {
            get { return _part4; }
            set { _part4 = value; RaisePropertyChanged(); }
        }

        private void AddIpAddress()
        {
            byte[] address = new byte[] { byte.Parse(_part1), byte.Parse(_part2), byte.Parse(_part3), byte.Parse(_part4) };
            IPAddress ip = new IPAddress(address);
            bool found = false;
            foreach(IPAddress iPAddress in _iPs)
            {
                if(iPAddress.Equals(ip))
                {
                    found = true;
                    break;
                }
            }
            if(!found)
            {
                _iPs.Add(ip);
                RaisePropertyChanged("Ips");
            }
        }
        public RelayCommand<SelectionChangedEventArgs> OnIpSelectionChanged => _ipSelectionChanged ??
                                                                                    (_ipSelectionChanged =
                                                                                      new RelayCommand<SelectionChangedEventArgs>(
                                                                                        (args) => IpSelectionChanged(args)));

        private void IpSelectionChanged(SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count == 1)
            {
                if (args.AddedItems[0] is IPAddress ip)
                {
                    byte[] bytes = ip.GetAddressBytes();
                    Part1 = bytes[0].ToString();
                    Part2 = bytes[1].ToString();
                    Part3 = bytes[2].ToString();
                    Part4 = bytes[3].ToString();
                }
            }
        }

        public RelayCommand AddIpCommand => _addIp ?? (_addIp = new RelayCommand(AddIp));
        private void AddIp()
        {
            AddIpAddress();
        }

        public RelayCommand EraseIpCommand => _eraseIp ?? (_eraseIp = new RelayCommand(EraseIp));
        private void EraseIp()
        {
            byte[] address = new byte[] { byte.Parse(_part1), byte.Parse(_part2), byte.Parse(_part3), byte.Parse(_part4) };
            IPAddress ip = new IPAddress(address);
            _iPs.Remove(ip);
        }
    }
}
