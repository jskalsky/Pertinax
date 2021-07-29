using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpcUaExplorer.Model
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BrowseResponse
    {
        public ushort namespaceIndex;
        public int identifierType;
        public uint numeric;
        public int strLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = OpcUa.MaxStringLength)]
        public byte[] str;
        public int nodeClass;
        public int browseNameLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = OpcUa.MaxStringLength)]
        public byte[] browseName;
        public int displayNameLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = OpcUa.MaxStringLength)]
        public byte[] displayName;
    }
    internal static class OpcUa
    {
        public const int MaxBrowseItems = 128;
        public const int MaxStringLength = 32;

        [DllImport("OpcUaLibrary.dll")]
        public static extern int OpenClient(int security);
        [DllImport("OpcUaLibrary.dll")]
        public static extern int Connect(string address);
        [DllImport("OpcUaLibrary.dll")]
        public static extern int Browse(ushort namespaceIndex, uint numeric, ref int nr, [Out] BrowseResponse[] responses);

        public static BrowseItem[] Browse(ushort namespaceIndex, uint numeric)
        {
            List<BrowseItem> result = new List<BrowseItem>();
            BrowseResponse[] br = new BrowseResponse[MaxBrowseItems];
            for (int i = 0; i < MaxBrowseItems; ++i)
            {
                br[i] = new BrowseResponse();
                br[i].browseName = new byte[MaxStringLength];
                br[i].displayName = new byte[MaxStringLength];
                br[i].str = new byte[MaxStringLength];
            }
            int nr = MaxBrowseItems;
            int res = Browse(namespaceIndex, numeric, ref nr, br);
            Debug.Print($"Browse= {res}, nr= {nr}");
            for (int i = 0; i < nr; ++i)
            {
                result.Add(new BrowseItem(br[i]));
            }
            return result.ToArray();
        }
    }
}
