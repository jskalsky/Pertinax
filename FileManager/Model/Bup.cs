using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Model
{
    public class Bup
    {
        public Bup(string name)
        {
            Name = name;
            T1 = 0;
            T2 = 0;
            Major = 0;
            Minor = 0;
            Build = 0;
            GetVersion(name);
        }
        public ushort T1 { get; set; }
        public ushort T2 { get; set; }
        public string Name { get; }
        public int Major { get; private set; }
        public int Minor { get; private set; }
        public int Build { get; private set; }

        private void GetVersion(string name)
        {
            if(!File.Exists(name))
            {
                return;
            }
            using(FileStream fs = new FileStream(name, FileMode.Open))
            {
                using(BinaryReader br = new BinaryReader(fs))
                {
                    ushort id = br.ReadUInt16();
                    uint length = br.ReadUInt32();
                    List<byte> buf = new List<byte>();
                    byte b = 0;
                    do
                    {
                        b = br.ReadByte();
                        if(b!=0)
                        {
                            buf.Add(b);
                        }
                    } while (b != 0);
                    string s = Encoding.ASCII.GetString(buf.ToArray());
                    StringBuilder sbMajor = new StringBuilder();
                    StringBuilder sbMinor = new StringBuilder();
                    StringBuilder sbBuild = new StringBuilder();
                    int st = 0;
                    foreach (char ch in s)
                    {
                        switch(st)
                        {
                            case 0:
                                st = 1;
                                break;
                            case 1:
                                if(ch == ' ')
                                {
                                    st = 2;
                                }
                                break;
                            case 2:
                                if(ch == ' ')
                                {
                                    st = 3;
                                }
                                break;
                            case 3:
                                if(ch == '.')
                                {
                                    st = 4;
                                }
                                else
                                {
                                    sbMajor.Append(ch);
                                }
                                break;
                            case 4:
                                if(ch == '.')
                                {
                                    st = 5;
                                }
                                else
                                {
                                    sbMinor.Append(ch);
                                }
                                break;
                            case 5:
                                if(ch == ' ')
                                {
                                    st = 6;
                                }
                                else
                                {
                                    sbBuild.Append(ch);
                                }
                                break;
                            case 6:
                                break;
                        }
                    }
                    Major = int.Parse(sbMajor.ToString());
                    Minor = int.Parse(sbMinor.ToString());
                    Build = int.Parse(sbBuild.ToString());
                }
            }
        }
    }
}
