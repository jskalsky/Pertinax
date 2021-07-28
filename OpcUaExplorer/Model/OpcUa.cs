using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpcUaExplorer.Model
{
    [StructLayout(LayoutKind.Sequential)]
    struct BrowseResponse
    {
        int identifierType;
        uint numeric;
        byte[] str;
        int nodeClass;
        byte[] browseName;
        byte[] displayName;
    }
    internal class OpcUa
    {
        [DllImport("OpcUaLibrary.dll")]
        public static extern int OpenClient(int security);
        [DllImport("OpcUaLibrary.dll")]
        public static extern int Connect(string address);
        [DllImport("OpcUaLibrary.dll")]
        public static extern int Browse(uint numeric, ref BrowseResponse[] responses);
    }
}
