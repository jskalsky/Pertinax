using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileManager.Model
{
    public class DiagClient
    {
        private TcpClient _tcpClient;
        private const int BufferSize = 4096;
        public DiagClient()
        {
            _tcpClient = new TcpClient();
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

        public async Task ConnectAsync(string ip, int port = 20002, int timeout = 5)
        {
            Task cancelTask = Task.Delay(timeout * 1000);
            Task connecttask = _tcpClient.ConnectAsync(ip, port);
            await await Task.WhenAny(connecttask, cancelTask);
            if(cancelTask.IsCompleted)
            {
                throw new TimeoutException();
            }
        }

        public async Task<byte[]> SendAsync(string ip, byte[] sendData, int rxTimeout = 5000)
        {
            if(!_tcpClient.Connected)
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
                byte[] rxdata = new byte[rxLength];
                await stream.ReadAsync(rxdata, 0, (int)rxLength);
                return rxdata;
            }
        }

        public async Task<byte[]> SendIdAsync(string ip, ushort id, int rxTimeout = 5000)
        {
            using(MemoryStream ms = new MemoryStream())
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
        public async void ReadDir(string ip)
        {
            byte[] rxData = await SendIdAsync(ip, 4);
        }
    }
}
