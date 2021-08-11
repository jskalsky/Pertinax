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
        public override void ChangeDirectory(string dir)
        {
        }

        public override void RefreshDirectory()
        {
        }

        public override void RefreshDrives()
        {
        }

        public override void SelectDrive(string drive, string actualDirectory)
        {
        }
        public override void Download(string fileName, byte[] file)
        {
        }
        public override byte[] Upload(string fileName)
        {
            return null;
        }
    }
}
