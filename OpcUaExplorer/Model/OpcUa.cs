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
        public const int MaxStringLength = 64;
        public const int MaxArray = 512;

        [DllImport("OpcUaLibrary.dll")]
        public static extern int OpenClient(int security);
        [DllImport("OpcUaLibrary.dll")]
        public static extern int Connect(string address);
        [DllImport("OpcUaLibrary.dll")]
        public static extern int Browse(ushort namespaceIndex, uint numeric, ref int nr, [Out] BrowseResponse[] responses);
        [DllImport("OpcUaLibrary.dll")]
        public static extern int ReadFloatArray(ushort namespaceIndex, uint numeric, ref int floatArraySize, [Out] float[] floatArray);
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
            Debug.Print($"Browse= {res}, nr= {nr}, {namespaceIndex}, {numeric}");
            for (int i = 0; i < nr; ++i)
            {
                Debug.Print($"br= {br[i]}");
                BrowseItem bi = new BrowseItem(br[i]);
                result.Add(bi);
                Debug.Print($"  {i} : {bi.NodeClass}, {bi.NodeIdType}, {bi.Numeric}, {bi.BrowseName}, {bi.DisplayName}");
            }
            return result.ToArray();
        }
        public static float[] ReadFloatArray(ushort namespaceIndex, uint numeric, ref int length)
        {
            float[] floatArray = new float[MaxArray];
            int floatArraySize = MaxArray;
            int res = ReadFloatArray(namespaceIndex, numeric, ref floatArraySize, floatArray);
            length = floatArraySize;
            if(res != 0)
            {
                return null;
            }
            return floatArray;
        }
    }
}
