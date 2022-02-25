using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinOpcUa
{
    internal static class DrvOpcUa
    {
        [DllImport("OpcUaLibrary.dll")]
        public static extern int Open(string path);
    }
}
