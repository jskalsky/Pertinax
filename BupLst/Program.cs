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
                BupReader.Read("HW_OUTPUT_nove_IDE.bup");
                BupReader.Read("HW_OUTPUT_stare_IDE.bup"); 
            }
            catch(Exception exc)
            {
                Console.WriteLine($"Exception: {exc.Message}");
            }
        }
    }
}
