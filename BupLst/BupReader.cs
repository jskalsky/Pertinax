﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BupLst
{
    public class BupReader
    {
        const int BufferSize = 1024 * 1024;
        private static Dictionary<ushort, Io> Ios = new Dictionary<ushort, Io>();

        public static SortedDictionary<ushort, List<string>> Net { get; set; }
        public static SortedDictionary<ushort,byte[]> Pars { get; set; }
        public static SortedDictionary<string,List<ushort>> FbPars { get; set; }
        public static SortedDictionary<string,List<ushort>> Fbs { get; set; }
        public static List<string> FbOrder { get; set; }
        public static void Read(string fileName)
        {
            Ios.Clear();
            Net.Clear();
            string lstName = Path.ChangeExtension(fileName, "lst");
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize))
            {
                long fileSize = fs.Length;
                using (BinaryReader br = new BinaryReader(fs))
                {
                    using (StreamWriter sw = new StreamWriter(lstName))
                    {
                        while (true)
                        {
                            try
                            {
                                ushort blockId = ReadWord(br);
                                uint length = ReadUint(br) * 2;
                                sw.WriteLine($"BlockId= {blockId}, Length= {length}, Pos= {fs.Position:X}");
                                switch (blockId)
                                {
                                    case 11:
                                        Read11(length, br, sw);
                                        break;
                                    case 16:
                                        Read16(length, br, sw);
                                        break;
                                    case 20:
                                        Read20(length, br, sw);
                                        break;
                                    case 24:
                                        Read24(length, br, sw);
                                        break;
                                    case 25:
                                        Read25(length, br, sw);
                                        break;
                                    default:
                                        br.ReadBytes((int)length);
                                        break;

                                }
                                uint crc = ReadUint(br);
                                if (fileSize - fs.Position == 4)
                                {
                                    break;
                                }
                            }
                            catch
                            {
                                break;
                            }
                        }
                        foreach (KeyValuePair<ushort, List<string>> pair in Net)
                        {
                            sw.Write($"{pair.Key}->");
                            bool isFirst = true;
                            foreach (string s in pair.Value)
                            {
                                if (isFirst)
                                {
                                    isFirst = false;
                                    sw.Write($"{s}");
                                }
                                else
                                {
                                    sw.Write($"-{s}");
                                }
                            }
                            sw.WriteLine();
                        }
                        foreach(KeyValuePair<string,List<ushort>> pair in Fbs)
                        {
                            for(int i=0;i<pair.Value.Count;++i)
                            {
                                if(!Net.TryGetValue(pair.Value[i],out List<string> net))
                                {
                                    Console.WriteLine($"Nenasel {pair.Key}, {i}");
                                }
                                sw.Write($"{pair.Key}:{i}-");
                                foreach(string s in net)
                                {
                                    sw.Write($"{s}-");
                                }
                                sw.WriteLine();
                            }
                        }                      
                    }
                }
            }
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

        private static void Read11(uint length, BinaryReader br, StreamWriter sw)
        {
            byte[] block = br.ReadBytes((int)length);
        }

        private static void Read16(uint length, BinaryReader br, StreamWriter sw)
        {
            ushort nrIos = ReadWord(br);
            for (ushort i = 0; i < nrIos; ++i)
            {
                ushort t = ReadWord(br);
                if (i == 0)
                {
                    sw.WriteLine($"Cpu= {t}");
                }
                else
                {
                    IoClass ioClass = IoClass.Unknown;
                    switch (t & 0xc000)
                    {
                        case 0x0000:
                            ioClass = IoClass.BlockOut;
                            break;
                        case 0x4000:
                            if ((t & 0x2000) != 0)
                            {
                                ioClass = IoClass.ExtOut;
                            }
                            else
                            {
                                ioClass = IoClass.ExtIn;
                            }
                            break;
                        case 0x8000:
                            ioClass = IoClass.UnconnectedIn;
                            break;
                        case 0xc000:
                            ioClass = IoClass.UnconnectedOut;
                            break;
                    }
                    sw.WriteLine($"Io= {i}, {ioClass}, {t & 0x1fff}");
                    Ios[i] = new Io(ioClass, (ushort)(t & 0x1fff));
                }
            }
            ushort nrPars = ReadWord(br);
            for (int i = 0; i < nrPars; ++i)
            {
                ushort p = ReadWord(br);
                bool sram = false;
                if ((p & 0x8000) != 0)
                {
                    sram = true;
                    p &= 0x7fff;
                }
                ushort parLength = ReadWord(br);
                byte[] par = null;
                if (parLength != 0)
                {
                    if ((parLength % 2) != 0)
                    {
                        ++parLength;
                    }
                    par = br.ReadBytes(parLength);
                    Pars[(ushort)i] = par;
                }
                sw.Write($"Par {i}, {sram}, {p}");
                if (par == null)
                {
                    sw.WriteLine(" null");
                }
                else
                {
                    foreach (byte b in par)
                    {
                        sw.Write($" {b:X}");
                    }
                    sw.WriteLine();
                }
            }
        }
        private static void Read20(uint length, BinaryReader br, StreamWriter sw)
        {
            ushort nrBlocks = ReadWord(br);
            for (int i = 0; i < nrBlocks; ++i)
            {
                string blockName = ReadString(br);
                FbOrder.Add(blockName);
                List<ushort> pins = new List<ushort>();
                Fbs[blockName] = pins;
                List<ushort> indexList = new List<ushort>();
                FbPars[blockName] = indexList;
                ushort nrPars = ReadWord(br);
                sw.Write($"{blockName} {nrPars}");
                for (ushort j = 0; j < nrPars; ++j)
                {
                    ushort parIndex = ReadWord(br);
                    indexList.Add(parIndex);
                    sw.Write($" {parIndex}");
                }
                sw.WriteLine();
                ushort nrPins = ReadWord(br);
                sw.WriteLine($"Pins= {nrPins}");
                for (ushort j = 0; j < nrPins; ++j)
                {
                    ushort pinIndex = ReadWord(br);
                    pins.Add(pinIndex);
                    if (!Net.TryGetValue(pinIndex, out List<string> ios))
                    {
                        ios = new List<string>();
                        Net[pinIndex] = ios;
                    }
                    string s = $"{blockName}:{j + 1}";
                    ios.Add(s);
                    sw.WriteLine($"  {j + 1}, {pinIndex}");
                }
            }
        }

        private static void Read24(uint length, BinaryReader br, StreamWriter sw)
        {
            StringBuilder sb = new StringBuilder();
            ushort nrDrvs = ReadWord(br);
            sw.WriteLine($"In {nrDrvs}");
            for (ushort i = 0; i < nrDrvs; ++i)
            {
                string drvName = ReadString(br);
                ushort nrFlags = ReadWord(br);
                for (ushort j = 0; j < nrFlags; ++j)
                {
                    ushort flagLength = ReadWord(br);
                    if((flagLength % 2) != 0)
                    {
                        ++flagLength;
                    }
                    byte[] flag = br.ReadBytes(flagLength);
                    ushort index = ReadWord(br);
                    sw.Write($"{drvName},{index}-");
                    sb.Clear();
                    foreach(byte b in flag)
                    {
                        sw.Write($"{b:X} ");
                        sb.Append($"{b:X} ");
                    }
                    sw.WriteLine();
                    if (!Net.TryGetValue(index, out List<string> ios))
                    {
                        ios = new List<string>();
                        Net[index] = ios;
                    }
                    string s = $"{drvName}:";
                    s += sb.ToString();
                    ios.Add(s);
                }
            }
        }
        private static void Read25(uint length, BinaryReader br, StreamWriter sw)
        {
            ushort nrDrvs = ReadWord(br);
            sw.WriteLine($"Out {nrDrvs}");
            for (ushort i = 0; i < nrDrvs; ++i)
            {
                string drvName = ReadString(br);
                ushort nrFlags = ReadWord(br);
                for (ushort j = 0; j < nrFlags; ++j)
                {
                    ushort flagLength = ReadWord(br);
                    if ((flagLength % 2) != 0)
                    {
                        ++flagLength;
                    }
                    byte[] flag = br.ReadBytes(flagLength);
                    ushort index = ReadWord(br);
                    ushort indexOut = ReadWord(br);
                    sw.Write($"{drvName},{index},{indexOut}-");
                    foreach (byte b in flag)
                    {
                        sw.Write($"{b:X} ");
                    }
                    sw.WriteLine();
                    if (!Net.TryGetValue(index, out List<string> ios))
                    {
                        ios = new List<string>();
                        Net[index] = ios;
                    }
                    string s = $"{drvName}:{j}";
                    ios.Add(s);
                }
            }
        }
    }
}
