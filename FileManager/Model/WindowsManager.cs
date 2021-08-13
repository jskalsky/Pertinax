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
            byte[] file = File.ReadAllBytes(fileName);
            return file;
        }

        public override void RefreshDrives()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            List<string> drvs = new List<string>();
            foreach (DriveInfo di in drives)
            {
                if (di.IsReady)
                {
                    drvs.Add(di.RootDirectory.FullName);
                }
            }
            Drives = drvs.ToArray();
        }

        public override void SelectDrive(string drive, string actualDirectory)
        {
            if (Drives == null)
            {
                return;
            }
            if (Drives.Length == 0)
            {
                return;
            }
            if(string.IsNullOrEmpty(drive))
            {
                drive = Drives[0];
            }
            SelectedDrive = drive;
            if (string.IsNullOrEmpty(actualDirectory))
            {
                ActualDirectory = drive;
            }
            else
            {
                if (actualDirectory.Contains(drive))
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
            if (!string.IsNullOrEmpty(_actualDirectory))
            {
                string ad = _actualDirectory;
                if (dir == "..")
                {
                    int index = ad.LastIndexOf('\\');
                    if (index > 0)
                    {
                        ad = ad.Remove(index + 1);
                        if(ad != Path.GetPathRoot(ad))
                        {
                            ad = ad.Remove(index);
                        }
                        ActualDirectory = ad;
                    }
                }
                else
                {
                    if(ad[ad.Length - 1] != Path.DirectorySeparatorChar)
                    {
                        ad += Path.DirectorySeparatorChar;
                    }
                    ad += dir;
                    ActualDirectory = ad;
                }
            }
        }

        public override void RefreshDirectory()
        {
            if (!string.IsNullOrEmpty(_actualDirectory))
            {
                string[] files = Directory.GetFiles(_actualDirectory);
                string[] folders = Directory.GetDirectories(_actualDirectory);
                string root = Path.GetPathRoot(_actualDirectory);
                if (!string.IsNullOrEmpty(root))
                {
                    List<string> flds = new List<string>();
                    if (root != _actualDirectory)
                    {
                        flds.Add("..");
                        foreach(string folder in folders)
                        {
                            int index = folder.LastIndexOf('\\');
                            if(index > 0)
                            {
                                string fol = folder.Substring(index + 1);
                                flds.Add($"[{fol}]");
                            }
                        }
                        Folders = flds.ToArray();
                    }
                    else
                    {
                        foreach (string folder in folders)
                        {
                            int index = folder.LastIndexOf('\\');
                            if (index > 0)
                            {
                                string fol = folder.Substring(index + 1);
                                flds.Add($"[{fol}]");
                            }
                        }
                        Folders = flds.ToArray();
                    }
                }
                List<string> fs = new List<string>();
                foreach(string file in files)
                {
                    fs.Add(Path.GetFileName(file));
                }
                Files = fs.ToArray();
            }
        }

        public override string MakeFilename(string selectedItem)
        {
            string filename = ActualDirectory + Path.DirectorySeparatorChar + selectedItem;
            return filename;
        }
    }
}
