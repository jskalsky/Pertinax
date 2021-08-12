using System;
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
    public class DiagClient : IDisposable, INotifyPropertyChanged
    {
        private TcpClient _tcpClient;
        private const int BufferSize = 4096;
        private string _lastError;
        private string[] _dirs;

        public event PropertyChangedEventHandler PropertyChanged;

        public DiagClient()
        {
            _tcpClient = new TcpClient();
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
        public async Task ConnectAsync(string ip, int port = 20002, int timeout = 5)
        {
            Task cancelTask = Task.Delay(timeout * 1000);
            Task connecttask = _tcpClient.ConnectAsync(ip, port);
            await await Task.WhenAny(connecttask, cancelTask);
            if (cancelTask.IsCompleted)
            {
                throw new TimeoutException();
            }
        }

        public async Task<byte[]> SendAsync(string ip, byte[] sendData, int rxTimeout = 5000)
        {
            try
            {
                if (!_tcpClient.Connected)
                {
                    await ConnectAsync(ip);
                }
                CancellationToken token = default(CancellationToken);
                using (NetworkStream stream = _tcpClient.GetStream())
                {
                    await stream.WriteAsync(sendData, 0, sendData.Length, token);
                    await stream.FlushAsync(token);
                    stream.ReadTimeout = rxTimeout;
                    byte[] buf = new byte[BufferSize];
                    await stream.ReadAsync(buf, 0, 4);
                    IntelMotorola(buf, 0, 4);
                    uint rxLength = BitConverter.ToUInt32(buf, 0);
                    Debug.Print($"rxLength= {rxLength}");
                    byte[] rxdata = new byte[rxLength];
                    await stream.ReadAsync(rxdata, 0, (int)rxLength);
                    return rxdata;
                }
            }
            catch (Exception exc)
            {
                LastError = exc.Message;
                return null;
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
        public async void ReadDir(string ip, string dir)
        {
            try
            {
                byte[] dirBytes = Encoding.ASCII.GetBytes(dir);
                int sendBufLength = 4 + 2 + 2 + dirBytes.Length;
                byte[] sendBuf = new byte[sendBufLength];
                SetHeader(sendBuf, sendBufLength - 4, 4);
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
                            ushort id = IntelMotorola(br.ReadUInt16());
                            uint nr = IntelMotorola(br.ReadUInt32());
                            for (ushort i = 0; i < nr; ++i)
                            {
                                ushort length = IntelMotorola(br.ReadUInt16());
                                byte[] dirName = br.ReadBytes(length);
                                br.ReadByte();
                                string name = $"[{Encoding.ASCII.GetString(dirName)}]";
                                dirs.Add(name);
                            }
                            Dirs = dirs.ToArray();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                LastError = exc.Message;
            }
        }

        private void ReleaseUnmanagedResources()
        {
            _tcpClient.Client.Dispose();
            _tcpClient.Close();
        }

        public void Dispose()
        {
            Debug.Print($"Dispose");
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }
        ~DiagClient()
        {
            Debug.Print($"DiagClient destructor");
            ReleaseUnmanagedResources();
        }
    }
}
