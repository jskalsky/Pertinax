using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonServiceLocator;
using FileManager.Properties;
using FileManager.ViewModel;

namespace FileManager.Model
{
    public class DownloadTest
    {
        List<string> messages = new List<string>();
        public Task Execution(uint handle, string bupName, ushort T1, ushort T2)
        {
            return Task.Run(() =>
            {
                try
                {
                    messages.Clear();
                    Kos2021.Kos.DownloadFile(handle, (byte)2, "172.31.253.1", bupName, $"/usr/pertinax/bup/{Path.GetFileName(bupName)}",
                        Process.GetCurrentProcess().MainWindowHandle);
                    //                    messages.Add($"Download file {Path.GetFileName(Settings.Default.Bup)} Ok");
                    ushort taskNo = 0;
                    Kos2021.Kos.NewTask(handle, (byte)2, "172.31.253.1", $"{ Path.GetFileNameWithoutExtension(bupName)}", T1, T2, 0, out taskNo);
//                                        messages.Add($"NewTask {Path.GetFileNameWithoutExtension(Settings.Default.Bup)} Ok, taskNo= {taskNo}");
                    Kos2021.Kos.RequestTask(handle, (byte)2, "172.31.253.1", taskNo, 4, 0);
                    //                    messages.Add($"Request task Ok");
                }
                catch (Exception exc)
                {
                    messages.Add($"Exception: {exc.Message}");
                }
            });
        }

        public async void Test()
        {
            MainViewModel mvm = ServiceLocator.Current.GetInstance<MainViewModel>();
            if (string.IsNullOrEmpty(Settings.Default.Bup))
            {
                mvm.Messages.Add($"No bup file");
                return;
            }
            if (!File.Exists(Settings.Default.Bup))
            {
                mvm.Messages.Add($"Bup file doesn't exist");
                return;
            }
            string selectedDevice = null;
            string[] devices = Kos2021.Kos.GetDevices();
            foreach (string device in devices)
            {
                Debug.Print($"DEVICE= {device}");
                if (device.Contains("Linux Remote"))
                {
                    selectedDevice = device;
                    break;
                }
            }
            if (selectedDevice == null)
            {
                mvm.Messages.Add($"Selected device == null");
            }
            uint handle = 0;
            try
            {
                handle = Kos2021.Kos.OpenDevice(selectedDevice, 5000);
                //                string bupFolder = Path.GetDirectoryName(Settings.Default.Bup);
//                string bupFolder = "e:\\Automaty\\Grati3\\30CJJ01_A20\\TEMP";
                string bupFolder = "e:\\Automaty\\FeltonKral\\1HC020_A1\\TEMP";
                string[] bups = Directory.GetFiles(bupFolder, "*.bup");
//                ushort[] T1 = new ushort[] { 1, 5, 1, 1, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 1, 1, 5 };
//                ushort[] T2 = new ushort[] { 0, 0, 0, 0, 0, 1, 1, 2, 2, 2, 3, 4, 3, 4, 0, 0, 4 };
                ushort[] T1 = new ushort[] { 0x8001, 0x8001, 0x8001, 0x8001, 0x8001, 0x8001 };
                ushort[] T2 = new ushort[] { 0, 0, 0, 0, 0, 0 };
                for (int i = 0; i < 100; ++i)
                {
                    int fileIndex = 0;
                    foreach(string bup in bups)
                    {
                        await Execution(handle, bup, T1[fileIndex], T2[fileIndex]);
                        if (messages.Count != 0)
                        {
                            foreach (string s in messages)
                            {
                                mvm.Messages.Add(s);
                            }
                        }
                        mvm.NrFiles = i + 1;
                        for (int j = 0; j < 2; ++j)
                        {
                            Thread.Sleep(1000);
                        }
//                        Kos2021.SystemInfo si;
//                        Kos2021.Kos.SystemInfo(handle, (byte)2, "172.31.253.1", out si);
//                        mvm.FreeRam = si.free_ram;
                        ++fileIndex;
                    }
                }
                Kos2021.Kos.CloseDevice(handle);
            }
            catch (Exception exc)
            {
                mvm.Messages.Add($"Exception: {exc.Message}");
                return;
            }
        }
    }
}
