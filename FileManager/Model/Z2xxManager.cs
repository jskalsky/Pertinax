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
            switch(e.PropertyName)
            {
                case "LastError":
                    LastError = _diagClient.LastError;
                    break;
                case "Dirs":
                    if(_actualDirectory.Length==1 && _actualDirectory[0] == '/')
                    {
                        Folders = _diagClient.Dirs;
                    }
                    else
                    {
                        List<string> folders = new List<string>();
                        folders.Add("..");
                        folders.AddRange(_diagClient.Dirs);
                        Folders = folders.ToArray();
                    }
                    break;
                case "Files":
                    Files = _diagClient.Files;
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
            _diagClient.ReadDir("10.10.13.252", 4, _actualDirectory);
            _diagClient.ReadDir("10.10.13.252", 3, _actualDirectory);
        }

        public override void RefreshDrives()
        {
            List<string> drives = new List<string>();
            Drives = drives.ToArray();
        }

        public override void SelectDrive(string drive, string actualDirectory)
        {
            Debug.Print($"SelectDrive");
            SelectedDrive = string.Empty;
            if(string.IsNullOrEmpty(actualDirectory))
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
            _diagClient.DownloadFile("10.10.13.252", fileName, file);
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
    }
}
