using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BupLst
{
    class Program
    {
        private static SortedDictionary<ushort, List<string>> Net1 = new SortedDictionary<ushort, List<string>>();
        private static SortedDictionary<ushort, List<string>> Net2 = new SortedDictionary<ushort, List<string>>();
        private static readonly SortedDictionary<ushort, byte[]> Pars1 = new SortedDictionary<ushort, byte[]>();
        private static SortedDictionary<string, List<ushort>> FbPars1 = new SortedDictionary<string, List<ushort>>();
        private static SortedDictionary<ushort, byte[]> Pars2 = new SortedDictionary<ushort, byte[]>();
        private static SortedDictionary<string, List<ushort>> FbPars2 = new SortedDictionary<string, List<ushort>>();
        private static SortedDictionary<string, List<ushort>> Fbs1 = new SortedDictionary<string, List<ushort>>();
        private static SortedDictionary<string, List<ushort>> Fbs2 = new SortedDictionary<string, List<ushort>>();
        private static List<string> FbOrder1 = new List<string>();
        private static List<string> FbOrder2 = new List<string>();
        static void Main(string[] args)
        {
            try
            {
                string bup1 = "TNAM_1.bup";
                string bup2 = "TNAM_2.bup";
                BupReader.Net = Net1;
                BupReader.Pars = Pars1;
                BupReader.FbPars = FbPars1;
                BupReader.Fbs = Fbs1;
                BupReader.FbOrder = FbOrder1;
                BupReader.Read(bup1);
                BupReader.Net = Net2;
                BupReader.Pars = Pars2;
                BupReader.FbPars = FbPars2;
                BupReader.Fbs = Fbs2;
                BupReader.FbOrder = FbOrder2;
                BupReader.Read(bup2);
                int errors = 0;
                if (Net1.Count != Net2.Count)
                {
                    Console.WriteLine($"Different net count {Net1.Count}, {Net2.Count}");
                    ++errors;
                }
                if (FbPars1.Count != FbPars2.Count)
                {
                    Console.WriteLine($"Different fb count {FbPars1.Count}, {FbPars2.Count}");
                    ++errors;
                }
                if (Pars1.Count != Pars2.Count)
                {
                    Console.WriteLine($"Different parameter count {Pars1.Count}, {Pars2.Count}");
                    ++errors;
                }
                for (int i = 0; i < FbOrder1.Count; ++i)
                {
                    if (FbOrder1[i] != FbOrder2[i])
                    {
                        Console.WriteLine($"Different block order {FbOrder1[i]}, {FbOrder2[i]}");
                        ++errors;
                    }
                }
                foreach (KeyValuePair<ushort, List<string>> pair in Net1)
                {
                    if (pair.Value.Count == 1)
                    {
                        continue;
                    }
                    string io = pair.Value[0];
                    int fc = 0;
                    List<string> net = null;
                    foreach (KeyValuePair<ushort, List<string>> pair2 in Net2)
                    {
                        foreach (string s in pair2.Value)
                        {
                            if (s == io)
                            {
                                ++fc;
                                net = pair2.Value;
                            }
                        }
                    }
                    if (fc == 0)
                    {
                        Console.WriteLine($"Item {io} doesn't exist in {bup2}");
                        ++errors;
                    }
                    else
                    {
                        if (fc > 1)
                        {
                            Console.WriteLine($"Item {io} exists {fc}x in {bup2}");
                            ++errors;
                        }
                        else
                        {
                            if (pair.Value.Count != net.Count)
                            {
                                Console.WriteLine($"Different count in net {pair.Value.Count}, {net.Count} - {io}");
                                ++errors;
                            }
                            else
                            {
                                foreach (string io1 in pair.Value)
                                {
                                    bool found = false;
                                    foreach (string io2 in net)
                                    {
                                        if (io1 == io2)
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (!found)
                                    {
                                        Console.WriteLine($"Different net");
                                        StringBuilder sb = new StringBuilder();
                                        foreach (string item1 in pair.Value)
                                        {
                                            sb.Append($"{item1}-");
                                        }
                                        Console.WriteLine($"1->{sb.ToString()}");
                                        foreach (string item2 in net)
                                        {
                                            sb.Append($"{item2}-");
                                        }
                                        Console.WriteLine($"2->{sb.ToString()}");
                                        ++errors;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (KeyValuePair<string, List<ushort>> pair in FbPars1)
                {
                    if (!FbPars2.TryGetValue(pair.Key, out List<ushort> idxList2))
                    {
                        Console.WriteLine($"Fb {pair.Key} doesn't exist in {bup2}");
                        ++errors;
                        continue;
                    }
                    if (pair.Value.Count != idxList2.Count)
                    {
                        Console.WriteLine($"Different parameters count in fb {pair.Key}, {pair.Value.Count}, {idxList2.Count}");
                        ++errors;
                        continue;
                    }
                    for (int i = 0; i < pair.Value.Count; ++i)
                    {
                        if (!Pars1.TryGetValue(pair.Value[i], out byte[] par1))
                        {
                            Console.WriteLine($"Parameter index {pair.Value[i]} doesn't exist in {bup1}");
                            ++errors;
                            continue;
                        }
                        if (!Pars2.TryGetValue(idxList2[i], out byte[] par2))
                        {
                            Console.WriteLine($"Parameter index {idxList2[i]} doesn't exist in {bup2}");
                            ++errors;
                            continue;
                        }
                        if (par1.Length != par2.Length)
                        {
                            Console.WriteLine($"Different parameter format {pair.Key}, {par1.Length}, {par2.Length}");
                            ++errors;
                            continue;
                        }
                        for (int j = 0; j < par1.Length; ++j)
                        {
                            if (par1[j] != par2[j])
                            {
                                Console.WriteLine($"Different parameter value {pair.Key}, {i}");
                                ++errors;
                                continue;
                            }
                        }
                    }
                }
                Console.WriteLine($"Errors= {errors}");
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Exception: {exc.Message}");
            }
        }
    }
}
