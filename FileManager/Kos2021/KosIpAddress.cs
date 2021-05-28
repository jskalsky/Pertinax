using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Kos2021
{
  public static class KosIpAddress
  {
    public static byte[] GetAddress(string ip, byte id)
    {
      byte[] result = new byte[16];
      BinaryWriter bw = new BinaryWriter(new MemoryStream(result));
      bw.Write(DataConversions.IntelMotorola((ushort)6));
      bw.Write(id);
      bw.Write((byte)4);
      byte[] ipAddress = IPAddress.Parse(ip).GetAddressBytes();
      bw.Write(ipAddress[0]);
      bw.Write(ipAddress[1]);
      bw.Write(ipAddress[2]);
      bw.Write(ipAddress[3]);
      bw.Write(DataConversions.IntelMotorola((ushort)6));
      bw.Write(id);
      bw.Write((byte)4);
      bw.Write(ipAddress[0]);
      bw.Write(ipAddress[1]);
      bw.Write(ipAddress[2]);
      bw.Write(ipAddress[3]);
      return result;
    }
  }
}
