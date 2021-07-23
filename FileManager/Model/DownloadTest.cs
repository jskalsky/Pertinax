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
        private Task Execution(uint handle, byte id, string ip, string bupName, ushort T1, ushort T2)
        {
            return Task.Run(() =>
            {
                try
                {
                    messages.Clear();
                    Kos2021.Kos.DownloadFile(handle, id, ip, bupName, $"/usr/pertinax/bup/{Path.GetFileName(bupName)}", Process.GetCurrentProcess().MainWindowHandle);
                    ushort taskNo = 0;
                    Kos2021.Kos.NewTask(handle, id, ip, $"{ Path.GetFileNameWithoutExtension(bupName)}", T1, T2, 0, out taskNo);
                    Kos2021.Kos.GetTaskNo(handle, id, ip, $"{ Path.GetFileNameWithoutExtension(bupName)}", out taskNo);
                    Kos2021.Kos.RequestTask(handle, id, ip, taskNo, 4, 0);
                }
                catch (Exception exc)
                {
                    messages.Add($"Exception: {exc.Message}");
                }
            });
        }

        private Task ExecutionTls(uint handle, byte id, string ip, string bupName, ushort T1, ushort T2, string hsaName)
        {
            return Task.Run(() =>
            {
                try
                {
                    messages.Clear();
                    Kos2021.Kos.DownloadFile(handle, id, ip, hsaName, $"/usr/pertinax/prj/sync.hsa", Process.GetCurrentProcess().MainWindowHandle);
                    Kos2021.Kos.DownloadFile(handle, id, ip, bupName, $"/usr/pertinax/bup/{Path.GetFileName(bupName)}", Process.GetCurrentProcess().MainWindowHandle);
                    ushort taskNo = 0;
                    Kos2021.Kos.NewTask(handle, id, ip, $"{ Path.GetFileNameWithoutExtension(bupName)}", T1, T2, 0, out taskNo);
                    Kos2021.Kos.GetTaskNo(handle, id, ip, $"{ Path.GetFileNameWithoutExtension(bupName)}", out taskNo);
                    Kos2021.Kos.RequestTask(handle, id, ip, taskNo, 0x60, 0);
                }
                catch (Exception exc)
                {
                    messages.Add($"Exception: {exc.Message}");
                }
            });
        }

        private Task UploadHsa(uint handle, byte id, string ip)
        {
            return Task.Run(() =>
            {
                string winFileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + "Pertinax" + Path.DirectorySeparatorChar + "sync.hsa";
                string linuxFileName = "usr/pertinax/prj/sync.hsa";
                try
                {
                    Kos2021.Kos.UploadFile(handle, id, ip, linuxFileName, winFileName, Process.GetCurrentProcess().MainWindowHandle);
                }
                catch (Exception exc)
                {
                    messages.Add($"Exception: {exc.Message}");
                }
            });
        }
        public Task<Bup[]> GetBups(string startup)
        {
            return Task<Bup[]>.Run<Bup[]>(() =>
            {
                List<Bup> bups = new List<Bup>();
                string dir = Path.GetDirectoryName(startup);
                string[] bupFiles = Directory.GetFiles(dir, "*.bup");
                if (bupFiles.Length == 0)
                {
                    string[] folders = Directory.GetDirectories(dir);
                    if (folders.Length != 0)
                    {
                        foreach (string sub in folders)
                        {
                            bupFiles = Directory.GetFiles(sub, "*.bup");
                            if (bupFiles.Length != 0)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        int index = dir.LastIndexOf('\\');
                        dir = dir.Remove(index);
                        bupFiles = Directory.GetFiles(dir, "*.bup");
                        if (bupFiles.Length == 0)
                        {
                            folders = Directory.GetDirectories(dir);
                            foreach (string folder in folders)
                            {
                                bupFiles = Directory.GetFiles(folder, "*.bup");
                                if (bupFiles.Length != 0)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                if (bupFiles.Length == 0)
                {
                    return null;
                }
                foreach (string bup in bupFiles)
                {
                    Bup b = new Bup(bup);
                    bups.Add(b);
                }
                string[] lines = File.ReadAllLines(startup);
                foreach (string line in lines)
                {
                    if (line.Contains("manager.inserttask"))
                    {
                        string[] items = line.Split(' ');
                        if (items.Length == 5)
                        {
                            foreach (Bup bup in bups)
                            {
                                if (bup.Name.Contains(items[1]))
                                {
                                    bup.T1 = ushort.Parse(items[2]);
                                    bup.T2 = ushort.Parse(items[3]);
                                    break;
                                }
                            }
                        }
                    }
                }
                return bups.ToArray();
            });
        }
        public async void Test(string startup, int repetitiveRate, bool isTls)
        {
            MainViewModel mvm = ServiceLocator.Current.GetInstance<MainViewModel>();
            if (string.IsNullOrEmpty(startup))
            {
                mvm.Messages.Add($"No startup file");
                return;
            }
            if (!File.Exists(startup))
            {
                mvm.Messages.Add($"Startup file doesn't exist");
                return;
            }

            Bup[] bups = await GetBups(startup);
            if (bups.Length == 0)
            {
                mvm.Messages.Add($"No bups");
                return;
            }

            string selectedDevice = null;
            string[] devices = Kos2021.Kos.GetDevices();
            foreach (string device in devices)
            {
                Debug.Print($"DEVICE= {device}");
                if (!isTls && device.Contains("Linux Remote"))
                {
                    selectedDevice = device;
                    break;
                }
                if (isTls && device.Contains("SslStream"))
                {
                    selectedDevice = device;
                    break;
                }
            }
            if (selectedDevice == null)
            {
                mvm.Messages.Add($"Selected device == null");
            }
            Debug.Print($"selectedDevice= {selectedDevice}");
            uint handle = 0;
            try
            {
                handle = Kos2021.Kos.OpenDevice(selectedDevice, 5000);
                //                string bupFolder = Path.GetDirectoryName(Settings.Default.Bup);
                //                string bupFolder = "e:\\Automaty\\Grati3\\30CJJ01_A20\\TEMP";
                //string bupFolder = "e:\\Automaty\\FeltonKral\\1HC020_A1\\TEMP";
                //string[] bups = Directory.GetFiles(bupFolder, "*.bup");
                //                ushort[] T1 = new ushort[] { 1, 5, 1, 1, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 1, 1, 5 };
                //                ushort[] T2 = new ushort[] { 0, 0, 0, 0, 0, 1, 1, 2, 2, 2, 3, 4, 3, 4, 0, 0, 4 };
                //ushort[] T1 = new ushort[] { 0x8001, 0x8001, 0x8001, 0x8001, 0x8001, 0x8001 };
                //ushort[] T2 = new ushort[] { 0, 0, 0, 0, 0, 0 };
                byte id = isTls ? (byte)2 : (byte)2;
                string ip = isTls ? "172.31.253.1" : "172.31.253.1";
                if (isTls)
                {
                    await UploadHsa(handle, id, ip);
                }
                for (int i = 0; i < repetitiveRate; ++i)
                {
                    foreach (Bup bup in bups)
                    {
                        if (isTls)
                        {
                            string winFileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + "Pertinax" + Path.DirectorySeparatorChar + "sync.hsa";
                            await ExecutionTls(handle, id, ip, bup.Name, (ushort)(bup.T1 | 0x8000), bup.T2, winFileName);
                        }
                        else
                        {
                            await Execution(handle, id, ip, bup.Name, (ushort)(bup.T1 | 0x8000), bup.T2);
                        }
                        if (messages.Count != 0)
                        {
                            foreach (string s in messages)
                            {
                                mvm.Messages.Add(s);
                            }
                        }
                        mvm.NrFiles = i;
                        for (int j = 0; j < 2; ++j)
                        {
                            Thread.Sleep(1000);
                        }
                        //                        Kos2021.SystemInfo si;
                        //                        Kos2021.Kos.SystemInfo(handle, (byte)2, "172.31.253.1", out si);
                        //                        mvm.FreeRam = si.free_ram;
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
