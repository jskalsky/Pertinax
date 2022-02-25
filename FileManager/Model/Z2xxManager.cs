using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Model
{
    public class Z2xxManager : Manager
    {
        private DiagClient _diagClient;

        public Z2xxManager()
        {
            _diagClient = new DiagClient();
            _diagClient.PropertyChanged += _diagClient_PropertyChanged;
        }

        private void _diagClient_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "LastError":
                    LastError = _diagClient.LastError;
                    break;
                case "DirItems":
                    Debug.Print($"Z2xxManager DirItems= {_diagClient.DirItems.Length}");
                    DirItems = _diagClient.DirItems;
                    break;
            }
        }

        public override void ChangeDirectory(string dir)
        {
            if (!string.IsNullOrEmpty(_actualDirectory))
            {
                string ad = _actualDirectory;
                if (dir == "..")
                {
                    int index = ad.LastIndexOf('/');
                    if (index >= 0)
                    {
                        ad = ad.Remove(index + 1);
                        if (index != 0)
                        {
                            ad = ad.Remove(index);
                        }
                        ActualDirectory = ad;
                    }
                }
                else
                {
                    if (ad[ad.Length - 1] != '/')
                    {
                        ad += '/';
                    }
                    ad += dir;
                    ActualDirectory = ad;
                }
            }
        }

        public override void RefreshDirectory()
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.SelectedServer))
            {
                //                _diagClient.ReadDir(Properties.Settings.Default.SelectedServer, 4, _actualDirectory);
                //                _diagClient.ReadDir(Properties.Settings.Default.SelectedServer, 3, _actualDirectory);
                _diagClient.GetDirectory(Properties.Settings.Default.SelectedServer, _actualDirectory);
            }
        }

        public override void RefreshDrives()
        {
            List<string> drives = new List<string>();
            if (Properties.Settings.Default.Servers != null)
            {
                foreach (string drive in Properties.Settings.Default.Servers)
                {
                    drives.Add(drive);
                }
            }
            Drives = drives.ToArray();
        }

        public override void SelectDrive(string drive, string actualDirectory)
        {
            Debug.Print($"SelectDrive");
            SelectedDrive = string.Empty;
            if (string.IsNullOrEmpty(actualDirectory))
            {
                ActualDirectory = "/";
            }
            else
            {
                ActualDirectory = actualDirectory;
            }
        }
        public override void Download(string fileName, byte[] file)
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.SelectedServer))
            {
                _diagClient.DownloadFile(Properties.Settings.Default.SelectedServer, fileName, file);
            }
        }
        public override byte[] Upload(string fileName)
        {
            return null;
        }

        public override string MakeFilename(string selectedItem)
        {
            string filename = ActualDirectory + '/' + selectedItem;
            return filename;
        }

        public override void SymLink(string old, string newPath)
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.SelectedServer))
            {
                _diagClient.SymLink(Properties.Settings.Default.SelectedServer, old, newPath);
            }
        }

        public override void Remove(string path)
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.SelectedServer))
            {
                _diagClient.Remove(Properties.Settings.Default.SelectedServer, path);
            }
        }
    }
}
