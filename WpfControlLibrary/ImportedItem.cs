using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public abstract class ImportedItem
    {
        public ImportedItem(string path, string name)
        {
            ObjectName = name;
            ConfigurationPath = path;
            ConfigurationName = Path.GetFileNameWithoutExtension(path);
        }
        public string ConfigurationPath { get; }
        public string ObjectName { get; }
        public string ConfigurationName { get; }
    }
}
