using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileManager.Properties;

namespace FileManager.Model
{
    public class DownloadTest
    {
        public void Execution()
        {
            if(string.IsNullOrEmpty(Settings.Default.Bup))
            {
                return;
            }
            if(!File.Exists(Settings.Default.Bup))
            {
                return;
            }
            string selectedDevice = null;
            string[] devices = Kos2021.Kos.GetDevices();
            foreach(string device in devices)
            {
                Debug.Print($"DEVICE= {device}");
                if(device.Contains("Linux Remote"))
                {
                    selectedDevice = device;
                    break;
                }
            }
            if(selectedDevice != null)
            {
                uint handle = Kos2021.Kos.OpenDevice(selectedDevice, 5000);
                try
                {
                    Kos2021.Kos.DownloadFile(handle, (byte)2, "172.31.253.1", Settings.Default.Bup, $"/usr/pertinax/bup/{Path.GetFileName(Settings.Default.Bup)}",
                        Process.GetCurrentProcess().MainWindowHandle);
                }
                catch(Exception exc)
                {

                }
                Kos2021.Kos.CloseDevice(handle);
            }
        }
    }
}
