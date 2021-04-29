using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Model
{
    public abstract class Manager
    {
        public string ActualDirectory { get; protected  set; }
        public abstract DriveInfo[] GetAllDrives();
        public abstract DirectoryItem[] GetDirectory();
        public abstract byte[] Upload(string fileName);
        public abstract void Download(string fileName, byte[] file);
        public abstract string GetDefaultDirectory();
        public abstract void SetActualDirectory(string actualDirectory);
    }
}
