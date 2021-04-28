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
            throw new NotImplementedException();
        }

        public override DirectoryItem[] GetDirectory()
        {
            throw new NotImplementedException();
        }

        public override byte[] Upload(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
