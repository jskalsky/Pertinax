using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpcUaExplorer.Model
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BrowseResponse
    {
        public int identifierType;
        public uint numeric;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] str;
        public int nodeClass;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] browseName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] displayName;

        public BrowseResponse(int iType, uint num, byte[] s, int nClass, byte[] bN, byte[] dN)
        {
            identifierType = iType;
            numeric = num;
            str = s;
            nodeClass = nClass;
            browseName = bN;
            displayName = dN;
        }
    }
    internal class OpcUa
    {
        [DllImport("OpcUaLibrary.dll")]
        public static extern int OpenClient(int security);
        [DllImport("OpcUaLibrary.dll")]
        public static extern int Connect(string address);
        [DllImport("OpcUaLibrary.dll")]
        public static extern int Browse(uint numeric, ref int nr, [Out]BrowseResponse[] responses);
    }
}
