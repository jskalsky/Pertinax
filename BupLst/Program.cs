using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BupLst
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                BupReader.Read("TNAM_1.bup");
                BupReader.Read("TNAM_2.bup"); 
            }
            catch(Exception exc)
            {
                Console.WriteLine($"Exception: {exc.Message}");
            }
        }
    }
}
