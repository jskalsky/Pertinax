using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public class PublisherItem : INotifyPropertyChanged
    {
        private OpcObject _opcObject;
        private int _interval;
        private static int _nextWriterGroupId = 0;
        private static int _nextDataSetWriterId = 0;

        public PublisherItem(OpcObject oo, int publisherId)
        {
            OpcObject = oo;
            PublisherId = publisherId;
            WriterGroupId = ++_nextWriterGroupId;
            DataSetWriterId = ++_nextDataSetWriterId;
            Interval = 100;
            Debug.Print($"OpcObject= {OpcObject.Name}");
        }

        public PublisherItem(OpcObject oo, int publisherId, int writerId, int datasetWriter, int interval)
        {
            OpcObject = oo;
            Interval = interval;
            PublisherId = publisherId;
            WriterGroupId = writerId;
            DataSetWriterId = datasetWriter;
            if(WriterGroupId > _nextWriterGroupId)
            {
                _nextWriterGroupId = WriterGroupId;
            }
            if(DataSetWriterId>_nextDataSetWriterId)
            {
                _nextDataSetWriterId = DataSetWriterId;
            }
        }
        public OpcObject OpcObject
        {
            get { return _opcObject; }
            set { _opcObject = value; OnPropertyChanged("OpcObject"); }
        }

        public int Interval
        {
            get { return _interval; }
            set { _interval = value; OnPropertyChanged("Interval"); }
        }

        public int WriterGroupId { get; private set; }
        public int DataSetWriterId { get; private set; }
        public int PublisherId { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
