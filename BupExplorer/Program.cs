using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BupExplorer
{
    class Program
    {
        const int BufferSize = 1024 * 1024;

        static void Main(string[] args)
        {
            Read("c:\\WorkWork\\Bup\\Vaclav\\S104_BIN.bup");
        }

        public static ushort IntelMotorola(ushort val)
        {
            byte[] value = BitConverter.GetBytes(val);
            byte fb = value[0];
            value[0] = value[1];
            value[1] = fb;
            return BitConverter.ToUInt16(value, 0);
        }
        public static short IntelMotorola(short val)
        {
            byte[] value = BitConverter.GetBytes(val);
            byte fb = value[0];
            value[0] = value[1];
            value[1] = fb;
            return BitConverter.ToInt16(value, 0);
        }
        public static uint IntelMotorola(uint val)
        {
            byte[] value = BitConverter.GetBytes(val);
            byte fb = value[0];
            value[0] = value[3];
            value[3] = fb;
            fb = value[1];
            value[1] = value[2];
            value[2] = fb;
            return BitConverter.ToUInt32(value, 0);
        }
        public static int IntelMotorola(int val)
        {
            byte[] value = BitConverter.GetBytes(val);
            byte fb = value[0];
            value[0] = value[3];
            value[3] = fb;
            fb = value[1];
            value[1] = value[2];
            value[2] = fb;
            return BitConverter.ToInt32(value, 0);
        }
        public static float IntelMotorola(float val)
        {
            byte[] value = BitConverter.GetBytes(val);
            byte fb = value[0];
            value[0] = value[3];
            value[3] = fb;
            fb = value[1];
            value[1] = value[2];
            value[2] = fb;
            return BitConverter.ToSingle(value, 0);
        }
        public static ushort ReadWord(BinaryReader br)
        {
            ushort w = br.ReadUInt16();
            return IntelMotorola(w);
        }
        public static uint ReadUint(BinaryReader br)
        {
            uint w = br.ReadUInt32();
            return IntelMotorola(w);
        }
        public static string ReadString(int length, BinaryReader br)
        {
            byte[] bytes = br.ReadBytes(length);
            List<byte> corrected = new List<byte>();
            foreach (byte b in bytes)
            {
                if (b != 0)
                {
                    corrected.Add(b);
                }
            }
            return Encoding.ASCII.GetString(corrected.ToArray());
        }
        public static string ReadString(BinaryReader br)
        {
            ushort length = ReadWord(br);
            if ((length % 2) != 0) ++length;
            return ReadString((int)length, br);
        }

        static void Read(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize))
            {
                long fileSize = fs.Length;
                using (BinaryReader br = new BinaryReader(fs))
                {
                    while(true)
                    {
                        try
                        {
                            ushort blockId = ReadWord(br);
                            uint length = ReadUint(br) * 2;
                            Console.WriteLine($"blockId= {blockId}, length= {length}");
                            br.ReadBytes((int)length);
                            uint crc = ReadUint(br);
                            if(fileSize - fs.Position == 4)
                            {
                                break;
                            }
                        }
                        catch
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
