using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgmError
{
    class Program
    {
        private static int startBez = 0, startS = 0;
        static void Main(string[] args)
        {
            string folder = "c:\\WorkAutomaty\\TestDiagVynechani\\STANICE_6_01\\TEMP";
            string filenameBez = folder + Path.DirectorySeparatorChar + "nicBez.txt";
            string filenameS = folder + Path.DirectorySeparatorChar + "nicS.txt";
            string filenameOut = folder + Path.DirectorySeparatorChar + "nicout.txt";
            int addrBez = 22;
            int addrS = 79;
            using (StreamReader srBez = new StreamReader(filenameBez))
            using (StreamReader srS = new StreamReader(filenameS))
            using (StreamWriter sw = new StreamWriter(filenameOut))
            {
                srBez.ReadLine();
                srS.ReadLine();
                bool cont = false;
                int countBez = 0;
                int countS = 0;
                List<int> durBez = new List<int>();
                List<int> durS = new List<int>();
                do
                {
                    bool findBez = false;
                    bool findS = false;
                    cont = false;
                    durBez.Clear();
                    if (Find(addrBez, srBez, durBez, ref startBez, out int seqBez, out int ticksBez, out double tBez))
                    {
                        findBez = true;
                        ++countBez;
                    }
                    durS.Clear();
                    if (Find(addrS, srS, durS, ref startS, out int seqS, out int ticksS, out double tS))
                    {
                        ++countS;
                        findS = true;
                    }
                    if (findBez && findS)
                    {
                        if(durBez.Count != 0 && durS.Count !=0)
                        {
                            if(durBez.Count == durS.Count)
                            {
                                for(int i=0;i<durBez.Count;++i)
                                {
                                    sw.WriteLine($"    {durBez[i]} - {durS[i]}");
                                }
                            }
                        }
                        sw.WriteLine($"{addrBez}, {seqBez}, {ticksBez}, {tBez} - {addrS}, {seqS}, {ticksS}, {tS}");
                    }
                    if (findBez || findS)
                    {
                        cont = true;
                    }
                } while (cont);
                sw.WriteLine($"CountBez= {countBez}, CountS= {countS}");
            }
            Console.WriteLine("Ok");
        }

        static bool Find(int addr, StreamReader sr, List<int> dur, ref int startTime, out int sequence, out int ticks, out double time)
        {
            string line;
            sequence = 0;
            ticks = 0;
            time = 0;
            try
            {
                line = sr.ReadLine();
                int lastTime = startTime;
                while (line != null)
                {
                    string[] items = line.Split(',');
                    if (items.Length == 4)
                    {
                        int seq = int.Parse(items[0]);
                        int a = int.Parse(items[1]);
                        int instr = int.Parse(items[2]);
                        int t = int.Parse(items[3]);
                        if(startTime != 0)
                        {
                            dur.Add(t - lastTime);
                            lastTime = t;
                        }
                        if (a == addr)
                        {
                            sequence = seq;
                            ticks = t;
                            time = (startTime == 0) ? 0 : (t - startTime) * 40.0e-9 * 1000.0;
                            startTime = t;
                            return true;
                        }
                    }
                    line = sr.ReadLine();
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Exception: {exc.Message}");
            }
            return false;
        }
    }
}
