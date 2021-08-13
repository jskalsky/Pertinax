﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileManager.Model
{
    public class DiagClient : INotifyPropertyChanged
    {
        private const int BufferSize = 4096;
        private string _lastError;
        private string[] _dirs;
        private string[] _files;

        public event PropertyChangedEventHandler PropertyChanged;

        public DiagClient()
        {
            _lastError = string.Empty;
        }

        public string LastError
        {
            get { return _lastError; }
            set { _lastError = value; OnPropertyChanged("LastError"); }
        }

        public string[] Dirs
        {
            get { return _dirs; }
            set { _dirs = value; OnPropertyChanged("Dirs"); }
        }
        public string[] Files
        {
            get { return _files; }
            set { _files = value; OnPropertyChanged("Files"); }
        }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public static void IntelMotorola(byte[] bytes, int start, int length)
        {
            for (int i = 0; i < length / 2; ++i)
            {
                byte b = bytes[start + i];
                bytes[start + i] = bytes[start + length - i - 1];
                bytes[start + length - i - 1] = b;
            }
        }

        public static ushort IntelMotorola(ushort val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            IntelMotorola(bytes, 0, 2);
            return BitConverter.ToUInt16(bytes, 0);
        }
        public static uint IntelMotorola(uint val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            IntelMotorola(bytes, 0, 4);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static byte[] IntelMotorolaB(uint val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            IntelMotorola(bytes, 0, 4);
            return bytes;
        }
        public static byte[] IntelMotorolaB(ushort val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            IntelMotorola(bytes, 0, 2);
            return bytes;
        }

        public static uint IntelMotorolaUint(byte[] buf)
        {
            IntelMotorola(buf, 0, 4);
            return BitConverter.ToUInt32(buf, 0);
        }
        public static ushort IntelMotorolaUshort(byte[] buf)
        {
            IntelMotorola(buf, 0, 2);
            return BitConverter.ToUInt16(buf, 0);
        }
        public async Task ConnectAsync(string ip, TcpClient tcpClient, int port = 20002, int timeout = 5)
        {
            Task cancelTask = Task.Delay(timeout * 1000);
            Task connecttask = tcpClient.ConnectAsync(ip, port);
            await await Task.WhenAny(connecttask, cancelTask);
            if (cancelTask.IsCompleted)
            {
                throw new TimeoutException();
            }
        }

        public async Task<byte[]> SendAsync(string ip, byte[] sendData, int rxTimeout = 5000)
        {
            TcpClient tcpClient = new TcpClient();
            try
            {
                await ConnectAsync(ip, tcpClient);
                CancellationToken token = default(CancellationToken);
                using (NetworkStream stream = tcpClient.GetStream())
                {
                    await stream.WriteAsync(sendData, 0, sendData.Length, token);
                    await stream.FlushAsync(token);
                    stream.ReadTimeout = rxTimeout;
                    byte[] buf = new byte[BufferSize];
                    await stream.ReadAsync(buf, 0, 4);
                    IntelMotorola(buf, 0, 4);
                    uint rxLength = BitConverter.ToUInt32(buf, 0);
                    Debug.Print($"rxLength= {rxLength}");

                    await stream.ReadAsync(buf, 0, 2);
                    IntelMotorola(buf, 0, 2);
                    ushort id = BitConverter.ToUInt16(buf, 0);

                    switch(id) // V jednotce je chyba, pri GetDirectories a GetFiles je rxLength spatne, je to delka nazvu + 2(id), ale za id je int = pocet nazvu
                    {
                        case 3:
                            rxLength += 2;
                            break;
                        case 4:
                            rxLength += 2;
                            break;
                    }
                    byte[] rxdata = new byte[rxLength];
                    int rx = await stream.ReadAsync(rxdata, 0, (int)rxLength);
                    if(rx == 0)
                    {
                        return null;
                    }
                    return rxdata;
                }
            }
            catch (Exception exc)
            {
                LastError = exc.Message;
                return null;
            }
            finally
            {
                tcpClient.Close();
            }
        }

        public async Task<byte[]> SendIdAsync(string ip, ushort id, int rxTimeout = 5000)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (BinaryWriter bw = new BinaryWriter(ms))
                    {
                        bw.Write((uint)2);
                        bw.Write(id);
                        byte[] buf = ms.GetBuffer();
                        IntelMotorola(buf, 0, 4);
                        IntelMotorola(buf, 4, 2);
                        return await SendAsync(ip, buf);
                    }
                }
            }
            catch (Exception exc)
            {
                LastError = exc.Message;
                return null;
            }
        }

        private void SetHeader(byte[] buf, int length, ushort id)
        {
            byte[] dataLength = BitConverter.GetBytes((uint)(length));
            IntelMotorola(dataLength, 0, 4);
            Array.Copy(dataLength, 0, buf, 0, 4);
            byte[] idBytes = BitConverter.GetBytes(id);
            IntelMotorola(idBytes, 0, 2);
            Array.Copy(idBytes, 0, buf, 4, 2);
        }

        public async void DownloadFile(string ip, string destinationName, byte[] fileInBytes, int rxTimeout = 5000)
        {
            TcpClient tcpClient = new TcpClient();
            try
            {
                await ConnectAsync(ip, tcpClient);
                using (NetworkStream ns = tcpClient.GetStream())
                {
                    uint txLength = 2 + 2 + (uint)destinationName.Length + 1 + 4 + (uint)fileInBytes.Length;
                    await ns.WriteAsync(IntelMotorolaB(txLength), 0, 4);
                    await ns.WriteAsync(IntelMotorolaB((ushort)2), 0, 2);
                    await ns.WriteAsync(IntelMotorolaB((ushort)destinationName.Length), 0, 2);
                    byte[] destination = Encoding.ASCII.GetBytes(destinationName);
                    await ns.WriteAsync(destination, 0, destination.Length);
                    byte[] strEnd = new byte[] { 0 };
                    await ns.WriteAsync(strEnd, 0, 1);
                    await ns.WriteAsync(IntelMotorolaB((uint)fileInBytes.Length), 0, 4);
                    await ns.WriteAsync(fileInBytes, 0, fileInBytes.Length);
                    await ns.FlushAsync();
                    ns.ReadTimeout = rxTimeout;

                    byte[] rxBuffer = new byte[BufferSize];
                    await ns.ReadAsync(rxBuffer, 0, 4);
                    uint rxLength = IntelMotorolaUint(rxBuffer);
                    Debug.Print($"Download= {rxLength}");
                }
            }
            catch (Exception exc)
            {
                LastError = exc.Message;
            }
        }
        public async void ReadDir(string ip, ushort id, string dir)
        {
            try
            {
                byte[] dirBytes = Encoding.ASCII.GetBytes(dir);
                int sendBufLength = 4 + 2 + 2 + dirBytes.Length;
                byte[] sendBuf = new byte[sendBufLength];
                SetHeader(sendBuf, sendBufLength - 4, id);
                byte[] dirLength = BitConverter.GetBytes((ushort)dir.Length);
                IntelMotorola(dirLength, 0, 2);
                Array.Copy(dirLength, 0, sendBuf, 6, 2);
                Array.Copy(dirBytes, 0, sendBuf, 8, dirBytes.Length);
                byte[] rxData = await SendAsync(ip, sendBuf);
                if (rxData != null)
                {
                    Debug.Print($"ReadDir {rxData.Length}");
                    using (MemoryStream ms = new MemoryStream(rxData))
                    {
                        using (BinaryReader br = new BinaryReader(ms))
                        {
                            List<string> dirs = new List<string>();
                            uint nr = IntelMotorola(br.ReadUInt32());
                            for (ushort i = 0; i < nr; ++i)
                            {
                                ushort length = IntelMotorola(br.ReadUInt16());
                                byte[] dirName = br.ReadBytes(length);
                                br.ReadByte();
                                string name = (id == 4)? $"[{Encoding.ASCII.GetString(dirName)}]" : $"{Encoding.ASCII.GetString(dirName)}";
                                dirs.Add(name);
                            }
                            if(id == 4)
                            {
                                Dirs = dirs.ToArray();
                            }
                            else
                            {
                                if(id == 3)
                                {
                                    Files = dirs.ToArray();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                LastError = exc.Message;
            }
        }
    }
}
