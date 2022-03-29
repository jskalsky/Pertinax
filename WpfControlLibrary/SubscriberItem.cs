using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public class SubscriberItem : INotifyPropertyChanged
    {
        public SubscriberItem(string path, string name, int publisherId, int writerId, int datasetId)
        {
            ObjectName = name;
            ConfigurationPath = path;
            ConfigurationName = Path.GetFileName(path);
            PublisherId = publisherId;
            WriterGroupId = writerId;
            DataSetWriterId = datasetId;
        }
        public string ConfigurationPath { get; }
        public string ObjectName { get; }
        public string ConfigurationName { get; }

        public int PublisherId { get; private set; }
        public int WriterGroupId { get; private set; }
        public int DataSetWriterId { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
