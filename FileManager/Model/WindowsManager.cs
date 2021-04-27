using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Model
{
    public class WindowsManager : Manager
    {
        public override void Download(string fileName, byte[] file)
        {
            string fileN = ActualDirectory + Path.DirectorySeparatorChar + fileName;
            File.WriteAllBytes(fileN, file);
        }

        public override DriveInfo[] GetAllDrives()
        {
            return DriveInfo.GetDrives();    
        }

        public override DirectoryItem[] GetDirectory()
        {
            string[] files = Directory.GetFiles(ActualDirectory);
            string[] dirs = Directory.GetDirectories(ActualDirectory);
            List<DirectoryItem> di = new List<DirectoryItem>();
            foreach(string dir in dirs)
            {
                di.Add(new FolderItem(Path.GetDirectoryName(dir)));
            }
            foreach(string file in files)
            {
                di.Add(new FileItem(Path.GetFileName(file)));
            }
            return di.ToArray();
        }

        public override byte[] Upload(string fileName)
        {
            string fileN = ActualDirectory + Path.DirectorySeparatorChar + fileName;
            byte[] file = File.ReadAllBytes(fileN);
            return file;
        }
    }
}
