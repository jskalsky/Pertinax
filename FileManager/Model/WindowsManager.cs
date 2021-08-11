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
        public override byte[] Upload(string fileName)
        {
            string fileN = ActualDirectory + Path.DirectorySeparatorChar + fileName;
            byte[] file = File.ReadAllBytes(fileN);
            return file;
        }

        public override void RefreshDrives()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            List<string> drvs = new List<string>();
            foreach(DriveInfo di in drives)
            {
                if(di.IsReady)
                {
                    drvs.Add(di.RootDirectory.FullName);
                }
            }
            Drives = drvs.ToArray();
        }

        public override void SelectDrive(string drive, string actualDirectory)
        {
            if(string.IsNullOrEmpty(actualDirectory))
            {
                ActualDirectory = drive;
            }
            else
            {
                if(actualDirectory.Contains(drive))
                {
                    ActualDirectory = actualDirectory;
                }
                else
                {
                    ActualDirectory = drive;
                }
            }
        }

        public override void ChangeDirectory(string dir)
        {
            if(!string.IsNullOrEmpty(_actualDirectory))
            {
                string ad = _actualDirectory;
                if (dir == "..")
                {
                    int index = ad.LastIndexOf('\\');
                    if(index > 0)
                    {
                        ad.Remove(index);
                        ActualDirectory = ad;
                    }
                }
                else
                {
                    ad += Path.DirectorySeparatorChar;
                    ad += dir;
                    ActualDirectory = ad;
                }
            }
        }

        public override void RefreshDirectory()
        {
            if(!string.IsNullOrEmpty(_actualDirectory))
            {
                string[] files = Directory.GetFiles(_actualDirectory);
                string[] folders = Directory.GetDirectories(_actualDirectory);
                string root = Path.GetPathRoot(_actualDirectory);
                if(!string.IsNullOrEmpty(root))
                {
                    if(root != _actualDirectory)
                    {
                        List<string> flds = new List<string>();
                        flds.AddRange(folders);
                        flds.Insert(0, "..");
                        Folders = flds.ToArray();
                    }
                    else
                    {
                        Folders = folders;
                    }
                }
                Files = files;
            }
        }
    }
}
