using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinOpcUa
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                int res = DrvOpcUa.Open("c:\\WorkProjects\\Pertinax\\WinOpcUa\\Xml\\Pub.xml");
            }
            catch(Exception exc)
            {
                Console.WriteLine($"Exception: {exc.Message}");
            }
        }
    }
}
