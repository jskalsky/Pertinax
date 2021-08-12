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
                    Folders = _diagClient.Dirs;
                    break;
            }
        }

        public override void ChangeDirectory(string dir)
        {
        }

        public override void RefreshDirectory()
        {
            _diagClient.ReadDir("10.10.13.252", _actualDirectory);
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
            ActualDirectory = "/";
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
