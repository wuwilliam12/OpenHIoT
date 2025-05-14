using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHIoT.BleEdge.Product
{
    public partial class SparkplugValue
    {
        //sbyte
        public static object? GetValueInt8(string str)
        {
            return Convert.ToSByte(str);
        }

        public static object? GetValueInt8s(string str)
        {
            string[] ss = str.Split(',');
            sbyte[] us = new sbyte[ss.Length];
            for (int i = 0; i < ss.Length; i++)
                us[i] = Convert.ToSByte(ss[i]);
            return us;
        }

        public static string? GetValueStringInt8(object ob, string? format)
        {
            if (ob == null) return null;
            if (format == null)
                return ob.ToString();
            else return ((sbyte)ob).ToString(format);
        }

        public static string? GetValueStringInt8s(object ob, string? format)
        {
            if (ob == null) return null;
            sbyte[] bs = (sbyte[])ob;
            if (format == null)
                return string.Join(",", bs);
            else
                return bs.Skip(1).Aggregate(bs[0].ToString(format), (s, i) => s + "," + i.ToString(format));
        }
        //byte
        public static object? GetValueUInt8(string str)
        {
            return Convert.ToByte(str);
        }

        public static object? GetValueUInt8s(string str)
        {
            string[] ss = str.Split(',');
            byte[] us = new byte[ss.Length];
            for (int i = 0; i < ss.Length; i++)
                us[i] = Convert.ToByte(ss[i]);
            return us;
        }

        public static string? GetValueStringUInt8(object ob, string? format)
        {
            if (ob == null) return null;
            if (format == null)
                return ob.ToString();
            else return ((ushort)ob).ToString(format);
        }

        public static string? GetValueStringUInt8s(object ob, string? format)
        {
            if (ob == null) return null;
            byte[] bs = (byte[])ob;
            if (format == null)
                return string.Join( ",", bs );
            else
                return bs.Skip(1).Aggregate(bs[0].ToString(format), (s, i) => s + "," + i.ToString(format)); 
        }
        //short
        public static object? GetValueInt16(string str)
        {
            return Convert.ToInt16(str);
        }

        public static object? GetValueInt16s(string str)
        {
            string[] ss = str.Split(',');
            short[] us = new short[ss.Length];
            for (int i = 0; i < ss.Length; i++)
                us[i] = Convert.ToInt16(ss[i]);
            return us;
        }

        public static string? GetValueStringInt16(object ob, string? format)
        {
            if (ob == null) return null;
            if (format == null)
                return ob.ToString();
            else return ((short)ob).ToString(format);
        }
        public static string? GetValueStringInt16s(object ob, string? format)
        {
            if (ob == null) return null;
            short[] bs = (short[])ob;
            if (format == null)
                return string.Join(",", bs);
            else
                return bs.Skip(1).Aggregate(bs[0].ToString(format), (s, i) => s + "," + i.ToString(format));
        }
        //ushort
        public static object? GetValueUInt16(string str)
        {
            return Convert.ToUInt16(str);
        }

        public static object? GetValueUInt16s(string str)
        {
            string[] ss = str.Split(',');
            ushort[] us = new ushort[ss.Length];
            for (int i = 0; i < ss.Length; i++)
                us[i] = Convert.ToUInt16(ss[i]);
            return us;
        }

        public static string? GetValueStringUInt16(object ob, string? format)
        {
            if (ob == null) return null;
            if (format == null)
                return ob.ToString();
            else return ((ushort)ob).ToString(format);
        }
        public static string? GetValueStringUInt16s(object ob, string? format)
        {
            if (ob == null) return null;
            ushort[] bs = (ushort[])ob;
            if (format == null)
                return string.Join(",", bs);
            else
                return bs.Skip(1).Aggregate(bs[0].ToString(format), (s, i) => s + "," + i.ToString(format));
        }
        //int
        public static object? GetValueInt32(string str)
        {
            return Convert.ToInt32(str);
        }

        public static object? GetValueInt32s(string str)
        {
            string[] ss = str.Split(',');
            int[] us = new int[ss.Length];
            for (int i = 0; i < ss.Length; i++)
                us[i] = Convert.ToInt32(ss[i]);
            return us;
        }

        public static string? GetValueStringInt32(object ob, string? format)
        {
            if (ob == null) return null;
            if (format == null)
                return ob.ToString();
            else return ((int)ob).ToString(format);
        }
        public static string? GetValueStringInt32s(object ob, string? format)
        {
            if (ob == null) return null;
            int[] bs = (int[])ob;
            if (format == null)
                return string.Join(",", bs);
            else
                return bs.Skip(1).Aggregate(bs[0].ToString(format), (s, i) => s + "," + i.ToString(format));
        }

        //uint
        public static object? GetValueUInt32(string str)
        {
            return Convert.ToUInt32(str);
        }

        public static object? GetValueUInt32s(string str)
        {
            string[] ss = str.Split(',');
            uint[] us = new uint[ss.Length];
            for (int i = 0; i < ss.Length; i++)
                us[i] = Convert.ToUInt32(ss[i]);
            return us;
        }

        public static string? GetValueStringUInt32(object ob, string? format)
        {
            if (ob == null) return null;
            if (format == null)
                return ob.ToString();
            else return ((uint)ob).ToString(format);
        }
        public static string? GetValueStringUInt32s(object ob, string? format)
        {
            if (ob == null) return null;
            uint[] bs = (uint[])ob;
            if (format == null)
                return string.Join(",", bs);
            else
                return bs.Skip(1).Aggregate(bs[0].ToString(format), (s, i) => s + "," + i.ToString(format));
        }
        //long
        public static object? GetValueInt64(string str)
        {
            return Convert.ToInt64(str);
        }

        public static object? GetValueInt64s(string str)
        {
            string[] ss = str.Split(',');
            long[] us = new long[ss.Length];
            for (int i = 0; i < ss.Length; i++)
                us[i] = Convert.ToInt64(ss[i]);
            return us;
        }

        public static string? GetValueStringInt64(object ob, string? format)
        {
            if (ob == null) return null;
            if (format == null)
                return ob.ToString();
            else return ((long)ob).ToString(format);
        }

        public static string? GetValueStringInt64s(object ob, string? format)
        {
            if (ob == null) return null;
            long[] bs = (long[])ob;
            if (format == null)
                return string.Join(",", bs);
            else
                return bs.Skip(1).Aggregate(bs[0].ToString(format), (s, i) => s + "," + i.ToString(format));
        }

        //ulong
        public static object? GetValueUInt64(string str)
        {
            return Convert.ToUInt64(str);
        }

        public static object? GetValueUInt64s(string str)
        {
            string[] ss = str.Split(',');
            ulong[] us = new ulong[ss.Length];
            for (int i = 0; i < ss.Length; i++)
                us[i] = Convert.ToUInt64(ss[i]);
            return us;
        }

        public static string? GetValueStringUInt64(object ob, string? format)
        {
            if (ob == null) return null;
            if (format == null)
                return ob.ToString();
            else return ((uint)ob).ToString(format);
        }

        public static string? GetValueStringUInt64s(object ob, string? format)
        {
            if (ob == null) return null;
            ulong[] bs = (ulong[])ob;
            if (format == null)
                return string.Join(",", bs);
            else
                return bs.Skip(1).Aggregate(bs[0].ToString(format), (s, i) => s + "," + i.ToString(format));
        }
        //float
        public static object? GetValueFloat(string str)
        {
            return Convert.ToSingle(str);
        }
        public static object? GetValueFloats(string str)
        {
            string[] ss = str.Split(',');
            float[] fs = new float[ss.Length];
            for (int i = 0; i < ss.Length; i++)
                fs[i] = Convert.ToSingle(ss[i]);
            return fs;
        }

        public static string? GetValueStringFloat(object ob, string? format)
        {
            if (ob == null) return null;
            if (format == null)
                return ob.ToString();
            else return ((float)ob).ToString(format);
        }

        public static string? GetValueStringFloats(object ob, string? format)
        {
            if (ob == null) return null;
            float[] bs = (float[])ob;
            if (format == null)
                return string.Join(",", bs);
            else
                return bs.Skip(1).Aggregate(bs[0].ToString(format), (s, i) => s + "," + i.ToString(format));
        }
        //double
        public static object? GetValueDouble(string str)
        {
            return Convert.ToDouble(str);
        }
        public static object? GetValueDoubles(string str)
        {
            string[] ss = str.Split(',');
            double[] fs = new double[ss.Length];
            for (int i = 0; i < ss.Length; i++)
                fs[i] = Convert.ToDouble(ss[i]);
            return fs;
        }

        public static string? GetValueStringDouble(object ob, string? format)
        {
            if (ob == null) return null;
            if (format == null)
                return ob.ToString();
            else return ((double)ob).ToString(format);
        }
        public static string? GetValueStringDoubles(object ob, string? format)
        {
            if (ob == null) return null;
            double[] bs = (double[])ob;
            if (format == null)
                return string.Join(",", bs);
            else
                return bs.Skip(1).Aggregate(bs[0].ToString(format), (s, i) => s + "," + i.ToString(format));
        }
    }
}
