using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Model
{
    public class Z2xxManager : Manager
    {
        public override void Download(string fileName, byte[] file)
        {
            throw new NotImplementedException();
        }

        public override DriveInfo[] GetAllDrives()
        {
            return null;
        }

        public override string GetDefaultDirectory()
        {
            return string.Empty;
        }

        public override DirectoryItem[] GetDirectory()
        {
            return null;
        }

        public override bool IsWindowsManager()
        {
            return false;
        }

        public override void SetActualDirectory(string actualDirectory)
        {
        }

        public override byte[] Upload(string fileName)
        {
            return null;
        }
    }
}
