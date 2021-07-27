using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUaExplorer.ViewModel
{
    public class IpAddressBoxVm : ViewModelBase
    {
        private int _part1;
        private int _part2;
        private int _part3;
        private int _part4;

        public IpAddressBoxVm()
        {
            Part1 = 255;
        }

        public int Part1
        {
            get { return _part1; }
            set { _part1 = value; RaisePropertyChanged(); }
        }
        public int Part2
        {
            get { return _part2; }
            set { _part2 = value; RaisePropertyChanged(); }
        }
        public int Part3
        {
            get { return _part3; }
            set { _part3 = value; RaisePropertyChanged(); }
        }
        public int Part4
        {
            get { return _part4; }
            set { _part4 = value; RaisePropertyChanged(); }
        }
    }
}
