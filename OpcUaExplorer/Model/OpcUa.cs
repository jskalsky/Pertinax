﻿using System;
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

    [StructLayout(LayoutKind.Sequential)]
    public struct OpcValue
    {
        public int valueType;
        public int status;
        public float floatValue;
        public byte byteValue;
        public ushort ushortValue;
        public uint uintValue;
        public char charValue;
        public short shortValue;
        public int intValue;
        public int booleanValue;
    }
    internal static class OpcUa
    {
        public const int MaxBrowseItems = 128;
        public const int MaxStringLength = 64;
        public const int MaxBuffer = 8192;

        [DllImport("OpcUaLibrary.dll")]
        public static extern int OpenClient(int security, uint localMaxMessage, uint remoteMaxMessage);
        [DllImport("OpcUaLibrary.dll")]
        public static extern int Connect(string address);
        [DllImport("OpcUaLibrary.dll")]
        public static extern int Browse(ushort namespaceIndex, uint numeric, ref int nr, [Out] BrowseResponse[] responses);
        [DllImport("OpcUaLibrary.dll")]
        public static extern int Read(ushort namespaceIndex, uint numeric, ref int length, ref int type, ref int arrayLength, [Out] byte[] buffer);
        [DllImport("OpcUaLibrary.dll")]
        public static extern int ServiceRead(ushort namespaceIndex, int length, uint[] ids, [Out] OpcValue[] values);

        public static OpcValue[] ServiceReadItems(ushort namespaceIndex, uint[] ids)
        {
            OpcValue[] values = new OpcValue[ids.Length];
            int result = ServiceRead(namespaceIndex, ids.Length, ids, values);
            if(result != 0)
            {
                return null;
            }
            return values;
        }
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
        public static byte[] Read(ushort namespaceIndex, uint numeric, ref int type, ref int arrayLength)
        {
            byte[] buffer = new byte[MaxBuffer];
            int length = MaxBuffer;
            int t = 0;
            int al = 0;
            int res = Read(namespaceIndex, numeric, ref length, ref t, ref al, buffer);
            if(res != 0)
            {
                return null;
            }
            byte[] result = new byte[length];
            Array.Copy(buffer, result, length);
            type = t;
            arrayLength = al;
            return result;
        }
    }
}
