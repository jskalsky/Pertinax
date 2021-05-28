using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Kos2021
{
  public static class DataConversions
  {
    public static ushort GetUshort(byte[] dta, int offset)
    {
      return BitConverter.ToUInt16(dta,offset);
    }
    public static ushort IntelMotorola(ushort val)
    {
      byte[] value = BitConverter.GetBytes(val);
      byte fb = value[0];
      value[0] = value[1];
      value[1] = fb;
      return BitConverter.ToUInt16(value, 0);
    }

    public static uint IntelMotorola(uint val)
    {
      byte[] value = BitConverter.GetBytes(val);
      byte fb = value[0];
      value[0] = value[3];
      value[3] = fb;
      fb = value[1];
      value[1] = value[2];
      value[2] = fb;
      return BitConverter.ToUInt32(value, 0);
    }

    public static short IntelMotorola(short val)
    {
      byte[] value = BitConverter.GetBytes(val);
      byte fb = value[0];
      value[0] = value[1];
      value[1] = fb;
      return BitConverter.ToInt16(value, 0);
    }

    public static int IntelMotorola(int val)
    {
      byte[] value = BitConverter.GetBytes(val);
      byte fb = value[0];
      value[0] = value[3];
      value[3] = fb;
      fb = value[1];
      value[1] = value[2];
      value[2] = fb;
      return BitConverter.ToInt32(value, 0);
    }

    public static float IntelMotorola(float val)
    {
      byte[] value = BitConverter.GetBytes(val);
      byte fb = value[0];
      value[0] = value[3];
      value[3] = fb;
      fb = value[1];
      value[1] = value[2];
      value[2] = fb;
      return BitConverter.ToSingle(value, 0);
    }
  }
}
