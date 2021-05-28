using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Kos2021;

namespace FileManager.Kos2021
{
  [StructLayout(LayoutKind.Sequential, Pack = 0)]
  public struct SystemTime
  {
    public ushort wYear;
    public ushort wMonth;
    public ushort wDayofWeek;
    public ushort wDay;
    public ushort wHour;
    public ushort wMinute;
    public ushort wSecond;
    public ushort wMilli;

    public SystemTime(ushort year, ushort month, ushort dayOfWeek, ushort day, ushort hour, ushort minute,
      ushort second, ushort mili)
    {
      wYear = year;
      wMonth = month;
      wDayofWeek = dayOfWeek;
      wDay = day;
      wHour = hour;
      wMinute = minute;
      wSecond = second;
      wMilli = mili;
    }
  }

  public class Kos
  {
    private static readonly BlackBirdClass KosLib = new BlackBirdClass();
    public const ushort ModeP3Route = 3;

    public static int LastHResult { get; private set; }
    public static string[] GetDevices()
    {
      List<string> devs = new List<string>();
      try
      {
        KosLib.kosGetIoDriversList(out var drivers);
        string[] driversList = drivers.Split('\n');
        foreach (string s in driversList)
        {
          if (s.Length == 0) continue;
          try
          {
            KosLib.kosGetDriverInfo(s, out var info, out var deviceList);
            string[] devices = deviceList.Split('\n');
            foreach (string d in devices)
            {
              if (d.Length == 0) continue;
              devs.Add($"{s},{d}");
            }
          }
          catch (Exception exc)
          {
            Console.WriteLine($"Exc= {exc.Message}");
          }
        }
      }
      catch (COMException e)
      {
        Console.WriteLine($"Exception= {e.Message}");
        TestKosException(e);
        throw;
      }
      return devs.ToArray();
    }

    public static uint OpenDevice(string name, ushort timeout)
    {
      try
      {
        string[] parts = name.Split(',');
        Debug.Print($"parts= {parts.Length}");
        if (parts.Length != 2) throw new ApplicationException("No connected device");
        KosLib.kosOpenDevice(parts[0], parts[1], timeout, ModeP3Route, out var handle);
        return handle;
      }
      catch (COMException e)
      {
        TestKosException(e);
        throw;
      }
    }

    public static void CloseDevice(uint handle)
    {
      try
      {
        KosLib.kosCloseDevice(handle);
      }
      catch (COMException e)
      {
        TestKosException(e);
        throw;
      }
    }

    public static void ReadDirectory(uint handle, byte id, string ip, string dir, out string[] folders, out string[] files)
    {
      List<string> resultFiles = new List<string>();
      List<string> resultFolders = new List<string>();
      try
      {
        KosLib.kosOpenDir(handle, KosIpAddress.GetAddress(ip, id), dir);
        ushort valid;
        do
        {
          string name;
          KosLib.kosReadDir(handle, KosIpAddress.GetAddress(ip, id), out valid, out name);
          Debug.Print($"name= {name}, valid= {valid:X}");
          if (valid != 0)
          {
            if ((valid & 0x8000) != 0)
            {
              resultFolders.Add(name);
            }
            else
            {
              resultFiles.Add(name);
            }
          }
        }
        while (valid != 0);
      }
      catch (COMException e)
      {
        TestKosException(e);
        throw;
      }
      folders = resultFolders.ToArray();
      files = resultFiles.ToArray();
      Debug.Print($"folders= {folders.Length}, files= {files.Length}");
    }

    public static void DownloadFile(uint handle, byte id, string ip, string fileName, string fileNameTarget, IntPtr windowHandle)
    {
      try
      {
        KosLib.kosDownloadFile(handle, KosIpAddress.GetAddress(ip, id), fileName, fileNameTarget, 0, (uint)windowHandle);
      }
      catch (COMException exc)
      {
        TestKosException(exc);
        throw;
      }
    }

    public static void UploadFile(uint handle, byte id, string ip, string targetFileName, string fileName, IntPtr windowHandle)
    {
      try
      {
        KosLib.kosUploadFile(handle, KosIpAddress.GetAddress(ip, id), targetFileName, fileName, 0, (uint)windowHandle);
      }
      catch (COMException exc)
      {
        TestKosException(exc);
        throw;
      }
    }

    public static void GetTime(uint handle, byte id, string ip, out SystemTime st)
    {
      try
      {
        KosLib.kosGetTime(handle, KosIpAddress.GetAddress(ip, id), out object t);
        byte[] dta = t as byte[];
        st = new SystemTime(DataConversions.GetUshort(dta, 0), DataConversions.GetUshort(dta, 2), DataConversions.GetUshort(dta, 4),
          DataConversions.GetUshort(dta, 6), DataConversions.GetUshort(dta, 8), DataConversions.GetUshort(dta, 10),
          DataConversions.GetUshort(dta, 12), DataConversions.GetUshort(dta, 14));
      }
      catch (COMException exc)
      {
        TestKosException(exc);
        throw;
      }
    }
    public static string GetCurrentUser(uint handle, byte id, string ip)
    {
      try
      {
        KosLib.kosGetCurrentUser(handle, KosIpAddress.GetAddress(ip, id), out string s);
        return s;
      }
      catch (COMException exc)
      {
        TestKosException(exc);
        throw;
      }
    }

    public static void Authorization(uint handle, byte id, string ip, string userName, string password)
    {
      try
      {
        byte[] un = Encoding.ASCII.GetBytes(userName);
        byte[] pass = Encoding.ASCII.GetBytes(password);
        KosLib.kosAuthorization(handle, KosIpAddress.GetAddress(ip, id), un, pass);
      }
      catch (COMException exc)
      {
        TestKosException(exc);
        throw;
      }
    }
    private static void TestKosException(COMException exc)
    {
      Console.WriteLine($"Exception {exc.ErrorCode:X}");
      LastHResult = exc.ErrorCode & 0xffff;
      if (((exc.ErrorCode >> 16) & 0x7ff) == 100)
      {
        KosLib.kosGetLastError(out var error);
        if (error.Length != 0)
        {
          throw new ApplicationException(error);
        }
      }
    }
  }
}
