using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public class SubscriberItem : ImportedItem, INotifyPropertyChanged
    {
        private bool _receive;
        public SubscriberItem(string path, string name, int publisherId, int writerId, int datasetId) : base(path, name)
        {
            PublisherId = publisherId;
            WriterGroupId = writerId;
            DataSetWriterId = datasetId;
        }
        public int PublisherId { get; private set; }
        public int WriterGroupId { get; private set; }
        public int DataSetWriterId { get; private set; }

        public bool Receive
        {
            get { return _receive; }
            set { _receive = value; OnPropertyChanged("Receive"); }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
