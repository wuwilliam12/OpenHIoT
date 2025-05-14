using OpenHIoT.LocalServer.Data.SampleDb.Archive;
using OpenHIoT.LocalServer.Services;
using SparkplugNet.VersionB.Data;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace OpenHIoT.LocalServer.Data.SampleDb
{
    public class ValueDataType
    {
        public static string GetDataParentDirectory()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            int i = dir.IndexOf("bin");
            return dir.Substring(0, i);
        }
        public static DataType GetDataType(ushort? dt)
        {
            if (dt == null) return DataType.Float; return (DataType)dt;
        }

        public static long? ConvertBytesToInteger(ushort? dt, byte[] bs, ref int offset)
        {
            return ConvertBytesToInteger(GetDataType(dt), bs, ref offset);
        }

        public static long? ConvertBytesToInteger(DataType dt, byte[] bs, ref int offset)
        {
            long? res;
            switch (dt)
            {
                case DataType.UInt8:
                    return bs[offset++];
                case DataType.Int8:
                    return unchecked((sbyte)bs[offset++]);
                case DataType.Boolean:
                    return bs[offset++];


                case DataType.Int16:
                    res = BitConverter.ToInt16(bs, offset);

                    return res;
                case DataType.UInt16:
                    res = BitConverter.ToUInt16(bs, offset);
                    offset += 2;
                    return res;

                case DataType.Int32:
                    res = BitConverter.ToInt32(bs, offset);
                    offset += 4;
                    return res;
                case DataType.UInt32:
                    res = BitConverter.ToUInt32(bs, offset);
                    offset += 4;
                    return res;
                default:
                    return null;
            }
        }

        public static double? ConvertBytesToDouble(ushort? dt, byte[] bs, ref int offset)
        {
            return ConvertBytesToDouble(GetDataType(dt), bs, ref offset);
        }
        public static double? ConvertBytesToDouble(DataType dt, byte[] bs, ref int offset)
        {
            double? res;
            switch (dt)
            {
                case DataType.Float:
                    res = BitConverter.ToSingle(bs, offset);
                    offset += 4;
                    return res;
                case DataType.Double:
                    res = BitConverter.ToDouble(bs, offset);
                    offset += 8;
                    return res;
                default:
                    return null;
            }
        }
        public static object? ConvertBytesToObject(ushort? dt, byte[] bs, ref int offset, int size)
        {
            return ConvertBytesToObject(GetDataType(dt), bs, ref offset, size);
        }
        public static object? ConvertBytesToObject(DataType dt, byte[] bs, ref int offset, int size)
        {
            switch (dt)
            {
                case DataType.UInt8:
                    return bs[0];
                case DataType.Int8:
                    return unchecked((sbyte)bs[0]);
                case DataType.Boolean:
                    return bs[0];


                case DataType.Int16:
                    return BitConverter.ToInt16(bs, offset);
                case DataType.UInt16:
                    return BitConverter.ToUInt16(bs, offset);

                case DataType.Int32:
                    return BitConverter.ToInt32(bs, offset);
                case DataType.UInt32:
                    return BitConverter.ToUInt32(bs, offset);
                case DataType.Float:
                    return BitConverter.ToSingle(bs, offset);
                case DataType.Double:
                    return BitConverter.ToDouble(bs, offset);

                case DataType.String:
                case DataType.Text:
                    byte[] bs1 = new byte[size];
                    Buffer.BlockCopy(bs, offset, bs1, 0, size);
                    return Encoding.UTF8.GetString(bs1);

                case DataType.BooleanArray:
                    bool[] bs2 = new bool[size];
                    for (int i = 0; i < bs2.Length; i++)
                        bs2[i] = BitConverter.ToBoolean(bs, i);
                    return bs2;
                case DataType.Int8Array:
                    sbyte[] bs3 = new sbyte[size];
                    Buffer.BlockCopy(bs, offset, bs3, 0, size);
                    return bs3;
                case DataType.UInt8Array:
                    return bs;
                case DataType.Int16Array:
                    short[] ss = new short[size >> 1];
                    Buffer.BlockCopy(bs, offset, ss, 0, size);
                    return ss;
                case DataType.UInt16Array:
                    ushort[] us = new ushort[size >> 1];
                    Buffer.BlockCopy(bs, offset, us, 0, size);
                    return us;
                case DataType.Int32Array:
                    int[] ints = new int[size >> 2];
                    Buffer.BlockCopy(bs, offset, ints, 0, size);
                    return ints;
                case DataType.UInt32Array:
                    int[] uints = new int[size >> 2];
                    Buffer.BlockCopy(bs, offset, uints, 0, size);
                    return uints;
                case DataType.FloatArray:
                    float[] fs = new float[size >> 2];
                    Buffer.BlockCopy(bs, offset, fs, 0, size);
                    return fs;
                case DataType.Int64Array:
                    long[] ls = new long[size >> 3];
                    Buffer.BlockCopy(bs, offset, ls, 0, size);
                    return ls;
                case DataType.UInt64Array:
                    ulong[] uls = new ulong[size >> 3];
                    Buffer.BlockCopy(bs, offset, uls, 0, size);
                    return uls;
                case DataType.DoubleArray:
                    double[] ds = new double[size >> 3];
                    Buffer.BlockCopy(bs, offset, ds, 0, size);
                    return ds;

                default:
                    return null;
            }
        }
        public static object? GetValFromSample(ushort? dt, SampleDTO sample)
        {
            return GetValFromSample(GetDataType(dt), sample);
        }

        public static object? GetValFromSample(DataType dt, SampleDTO sample)
        {
            if (sample.ValInt != null)
                return GetValFromInt(dt, (long)sample.ValInt);
            else if(sample.ValDouble != null)
                return GetValFromDouble(dt, (double)sample.ValDouble);
            else if(sample.ValBlob != null)
                return GetValFromBytes(dt, sample.ValBlob);
            return null;
        }
            
        public static object? GetValFromInt(DataType dt, long d)
        {
            //  if (val is null) return null;
            switch (dt)
            {
                case DataType.UInt8:
                    return (byte)d;
                case DataType.Int8:
                    return (sbyte)d;
                case DataType.Boolean:
                    return d != 0;
                case DataType.Int16:
                    return (short)d;
                case DataType.UInt16:
                    return (ushort)d;

                case DataType.Int32:
                    return (int)d;
                case DataType.UInt32:
                    return (uint)d;
                case DataType.Int64:
                    return d;
                case DataType.UInt64:
                case DataType.DateTime:
                    return (ulong)d;
            
            }
            return null;
        }
        public static long? GetIntegerVal(DataType dt, object val)
        {
            switch (dt)
            {
                case DataType.UInt8:
                    return (byte)val;
                case DataType.Int8:
                    return (sbyte)val;
                case DataType.Boolean:
                    if ((bool)val) return 1;
                    else return 0;

                case DataType.Int16:
                    return (short)val;
                case DataType.UInt16:
                    return (ushort)val;

                case DataType.Int32:
                    return (int)val;
                case DataType.UInt32:
                    return (uint)val;
                case DataType.Int64:
                    return (long)val;
                case DataType.UInt64:
                case DataType.DateTime:
                    return (long)(ulong)val;

                default:
                    return null;
            }
        }

        public static double? GetDoubleVal(ushort? dt, object val)
        {
            return GetDoubleVal(GetDataType(dt), val);
        }

        public static double? GetDoubleVal(DataType dt, object val)
        {
            switch (dt)
            {
                case DataType.Float:
                    return (double)(float)val;
                case DataType.Double:
                    return (double)val;
                default:
                    return null;
            }
        }
        public static object? GetValFromDouble(DataType dt, double val)
        {
            switch (dt)
            {
                case DataType.Float:
                    return (float)val;
                case DataType.Double:
                    return val;
                default:
                    return null;
            }
        }
        public static object? GetVal(ushort? dt, object? m_val)
        {
            return GetVal(GetDataType(dt), m_val);
        }
        public static object? GetVal(DataType dt, object? m_val)
        {
            if (m_val is null) return null;
            switch (dt)
            {
                case DataType.UInt8:
                    return (byte)m_val;
                case DataType.Int8:
                    return (sbyte)m_val;
                case DataType.Boolean:
                    if ((bool)m_val) return 1;
                    else return 0;

                case DataType.Int16:
                    return (short)m_val;
                case DataType.UInt16:
                    return (ushort)m_val;

                case DataType.Int32:
                    return (int)m_val;
                case DataType.UInt32:
                    return (uint)m_val;
                case DataType.Int64:
                    return (long)m_val;
                case DataType.UInt64:
                case DataType.DateTime:
                    return (ulong)m_val;

                case DataType.Float:
                    return (double)(float)m_val;
                case DataType.Double:
                    return (double)m_val;

                case DataType.String:
                case DataType.Text:
                    return Encoding.UTF8.GetBytes((string)m_val);

                case DataType.BooleanArray:
                case DataType.Int8Array:
                case DataType.UInt8Array:

                case DataType.Int16Array:
                case DataType.UInt16Array:

                case DataType.Int32Array:
                case DataType.UInt32Array:
                case DataType.FloatArray:

                case DataType.Int64Array:
                case DataType.UInt64Array:
                case DataType.DoubleArray:
                    Array array = (Array)m_val;
                    int es = ValueBlock.ElementSizeOf(dt);
                    byte[] bs = new byte[array.Length * es];
                    Buffer.BlockCopy(array, 0, bs, 0, bs.Length);
                    return bs;

                default:
                    return null;
            }
        }
        public static byte[]? GetValBytes(ushort? dt, object? val)
        {
            return GetValBytes(GetDataType(dt), val);
        }

        public static byte[]? GetValBytes(DataType dt, object? val)
        {
            if (val == null) return null;
            switch (dt)
            {
                case DataType.Float:
                    return BitConverter.GetBytes((float)val);
                case DataType.Double:
                    return BitConverter.GetBytes((double)val);
                case DataType.UInt8:
                    return new byte[] { (byte)val };
                case DataType.Int8:
                    return new byte[] { (byte)(sbyte)val };
                case DataType.Boolean:
                    if ((bool)val) return new byte[] { 1 };
                    return new byte[] { 0 };

                case DataType.Int16:
                    return BitConverter.GetBytes((short)val);
                case DataType.UInt16:
                    return BitConverter.GetBytes((ushort)val);

                case DataType.Int32:
                    return BitConverter.GetBytes((int)val);
                case DataType.UInt32:
                    return BitConverter.GetBytes((int)val);

                case DataType.Int64:
                    return BitConverter.GetBytes((long)val);
                case DataType.UInt64:
                case DataType.DateTime:
                    return BitConverter.GetBytes((ulong)val);

                case DataType.String:
                case DataType.Text:
                    return Encoding.UTF8.GetBytes((string)val);

                case DataType.UInt8Array:
                case DataType.Bytes:
                    return (byte[])val;

                case DataType.BooleanArray:
                case DataType.Int8Array:


                case DataType.Int16Array:
                case DataType.UInt16Array:

                case DataType.Int32Array:
                case DataType.UInt32Array:
                case DataType.FloatArray:

                case DataType.Int64Array:
                case DataType.UInt64Array:
                case DataType.DoubleArray:
                    Array array = (Array)val;
                    int es = ValueBlock.ElementSizeOf(dt);
                    byte[] bs = new byte[array.Length * es];
                    Buffer.BlockCopy(array, 0, bs, 0, bs.Length);
                    return bs;

                default:
                    return null;
            }
        }

        public static object? GetValFromBytes(DataType dt, byte[] val)
        {
            switch (dt)
            {

                case DataType.String:
                case DataType.Text:
                    return Encoding.UTF8.GetString(val);

                case DataType.UInt8Array:
                case DataType.Bytes:
                    return val;

                case DataType.BooleanArray:
                    bool[] bs = new bool[val.Length];
                    for (int i = 0; i < val.Length; i++)
                        bs[i] = val[i] != 0;
                    return bs;

                case DataType.Int8Array:
                    sbyte[] bs1 = new sbyte[val.Length];
                    Buffer.BlockCopy(val, 0, bs1, 0, val.Length);
                    return bs1;

                case DataType.Int16Array:
                    short[] bs2 = new short[val.Length >> 1];
                    Buffer.BlockCopy(val, 0, bs2, 0, val.Length);
                    return bs2;
                case DataType.UInt16Array:
                    ushort[] bs3 = new ushort[val.Length >> 1];
                    Buffer.BlockCopy(val, 0, bs3, 0, val.Length);
                    return bs3;
                case DataType.Int32Array:
                    int[] bs4 = new int[val.Length >> 2];
                    Buffer.BlockCopy(val, 0, bs4, 0, val.Length);
                    return bs4;

                case DataType.UInt32Array:
                    uint[] bs5 = new uint[val.Length >> 2];
                    Buffer.BlockCopy(val, 0, bs5, 0, val.Length);
                    return bs5;
                case DataType.FloatArray:
                    float[] bs6 = new float[val.Length >> 2];
                    Buffer.BlockCopy(val, 0, bs6, 0, val.Length);
                    return bs6;
                case DataType.Int64Array:
                    long[] bs7 = new long[val.Length >> 3];
                    Buffer.BlockCopy(val, 0, bs7, 0, val.Length);
                    return bs7;
                case DataType.UInt64Array:
                    ulong[] bs8 = new ulong[val.Length >> 3];
                    Buffer.BlockCopy(val, 0, bs8, 0, val.Length);
                    return bs8;
                case DataType.DoubleArray:
                    double[] bs9 = new double[val.Length >> 3];
                    Buffer.BlockCopy(val, 0, bs9, 0, val.Length);
                    return bs9;

                default:
                    return null;
            }
        }


        public static object? Parse(ushort? dt, string str)
        {
            DataType dataType = dt == null ? DataType.Float : (DataType)dt;
            return Parse(dataType, str);
        }

        public static object? Parse( DataType dt, string str)
        {
         //   object? v = null;
            switch (dt)
            {
                case DataType.UInt8:
                    byte b = 0;
                    if(byte.TryParse(str, out b))
                        return b;
                    return null;
                case DataType.Int8:
                    sbyte sb = 0;
                    if(sbyte.TryParse(str, out sb))
                        return sb;
                    return null;
                case DataType.Boolean:
                    string str_lower = str.ToLower();
                    if (str_lower == "true")
                        return true;
                    if (str_lower == "false")
                        return false;
                    byte b1 = 0;
                    if (byte.TryParse(str, out b1))
                        return b1!=0;
                    return null;

                case DataType.Int16:
                    short s = 0;
                    if (short.TryParse(str, out s))
                        return s;
                    return null;
                case DataType.UInt16:
                    ushort us = 0;
                    if (ushort.TryParse(str, out us))
                        return us;
                    return null;

                case DataType.Int32:
                    int i32 = 0;
                    if (int.TryParse(str, out i32))
                        return i32;
                    return null;

                case DataType.UInt32:
                    uint ui = 0;
                    if (uint.TryParse(str, out ui))
                        return ui;
                    return null;
                case DataType.Int64:
                    long i64 = 0;
                    if (long.TryParse(str, out i64))
                        return i64;
                    return null;

                case DataType.UInt64:
                    ulong ui64 = 0;
                    if (ulong.TryParse(str, out ui64))
                        return ui64;
                    return null;
                case DataType.Float:
                    float f = 0;
                    if (float.TryParse(str, out f))
                        return f;
                    return null;

                case DataType.Double:
                    double d = 0;
                    if (double.TryParse(str, out d))
                        return d;
                    return null;

                case DataType.String:
                case DataType.Text:                
                    return str;

                case DataType.BooleanArray:
                    string[] ss1 = str.Split(',');
                    bool[] bs1 = new bool[ss1.Length];
                    byte b2 = 0;
                    for (int i = 0; i < ss1.Length; i++)
                        if (byte.TryParse(ss1[i], out b2))
                            bs1[i] = b2 != 0;
                        else return null;
                    return bs1;
                case DataType.Int8Array:
                    string[] ss2 = str.Split(',');
                    sbyte[] bs2 = new sbyte[ss2.Length];
                    sbyte b3 = 0;
                    for (int i = 0; i < ss2.Length; i++)
                        if (sbyte.TryParse(ss2[i], out b3))
                            bs2[i] = b3;
                        else return null;
                    return bs2;
                case DataType.UInt8Array:
                    string[] ss3 = str.Split(',');
                    byte[] bs3 = new byte[ss3.Length];
                    byte b4 = 0;
                    for (int i = 0; i < ss3.Length; i++)
                        if (byte.TryParse(ss3[i], out b4))
                            bs3[i] = b4;
                        else return null;
                    return bs3;
                case DataType.Int16Array:
                    string[] ss4 = str.Split(',');
                    short[] bs4 = new short[ss4.Length];
                    short b5 = 0;
                    for (int i = 0; i < ss4.Length; i++)
                        if (short.TryParse(ss4[i], out b5))
                            bs4[i] = b5;
                        else return null;
                    return bs4;
                case DataType.UInt16Array:
                    string[] ss5 = str.Split(',');
                    ushort[] bs5 = new ushort[ss5.Length];
                    ushort b6 = 0;
                    for (int i = 0; i < ss5.Length; i++)
                        if (ushort.TryParse(ss5[i], out b6))
                            bs5[i] = b6;
                        else return null;
                    return bs5;
                case DataType.Int32Array:
                    string[] ss6 = str.Split(',');
                    int[] bs6 = new int[ss6.Length];
                    int b7 = 0;
                    for (int i = 0; i < ss6.Length; i++)
                        if (int.TryParse(ss6[i], out b7))
                            bs6[i] = b7;
                        else return null;
                    return bs6;
                case DataType.UInt32Array:
                    string[] ss7 = str.Split(',');
                    uint[] bs7 = new uint[ss7.Length];
                    uint b8 = 0;
                    for (int i = 0; i < ss7.Length; i++)
                        if (uint.TryParse(ss7[i], out b8))
                            bs7[i] = b8;
                        else return null;
                    return bs7;
                case DataType.FloatArray:
                    string[] ss8 = str.Split(',');
                    float[] bs8 = new float[ss8.Length];
                    float b9 = 0;
                    for (int i = 0; i < ss8.Length; i++)
                        if (float.TryParse(ss8[i], out b9))
                            bs8[i] = b9;
                        else return null;
                    return bs8;
                case DataType.Int64Array:
                    string[] ss9 = str.Split(',');
                    long[] bs9 = new long[ss9.Length];
                    long b10 = 0;
                    for (int i = 0; i < ss9.Length; i++)
                        if (long.TryParse(ss9[i], out b10))
                            bs9[i] = b10;
                        else return null;
                    return bs9;
                case DataType.UInt64Array:
                    string[] ss10 = str.Split(',');
                    ulong[] bs10 = new ulong[ss10.Length];
                    ulong b11 = 0;
                    for (int i = 0; i < ss10.Length; i++)
                        if (ulong.TryParse(ss10[i], out b11))
                            bs10[i] = b11;
                        else return null;
                    return bs10;
                case DataType.DoubleArray:
                    string[] ss11 = str.Split(',');
                    double[] bs11 = new double[ss11.Length];
                    double b12 = 0;
                    for (int i = 0; i < ss11.Length; i++)
                        if (double.TryParse(ss11[i], out b12))
                            bs11[i] = b12;
                        else return null;
                    return bs11;

                default:
                    return null;
            }
        }
        public static string? ToString(ushort? dt, object val)
        {
            DataType dataType = dt == null ? DataType.Float : (DataType)dt;
            return ToString(dataType, val);
        }
        public static string? ToString(DataType dt, object val)
        {
            //   object? v = null;
            switch (dt)
            {
                case DataType.UInt8:
                    return ((byte)val).ToString();
                case DataType.Int8:
                    return ((sbyte)val).ToString();
                case DataType.Boolean:
                    return ((bool)val).ToString();

                case DataType.Int16:
                    return ((short)val).ToString();
                case DataType.UInt16:
                    return ((ushort)val).ToString();

                case DataType.Int32:
                    return ((int)val).ToString();

                case DataType.UInt32:
                    return ((uint)val).ToString();
                case DataType.Int64:
                    return ((long)val).ToString();

                case DataType.UInt64:
                    return ((ulong)val).ToString();
                case DataType.Float:
                    return ((float)val).ToString();

                case DataType.Double:
                    return ((double)val).ToString();

                case DataType.String:
                case DataType.Text:
                    return (string)val;

                case DataType.BooleanArray:
                    bool[] bs1 = (bool[])val;
                    return string.Join(",", bs1);
                case DataType.Int8Array:
                    sbyte[] bs2 = (sbyte[])val;
                    return string.Join(",", bs2);
                case DataType.UInt8Array:
                    byte[] bs3 = (byte[])val;
                    return string.Join(",", bs3);
                case DataType.Int16Array:
                    short[] bs4 = (short[])val;
                    return string.Join(",", bs4);
                case DataType.UInt16Array:
                    ushort[] bs5 = (ushort[])val;
                    return string.Join(",", bs5);
                case DataType.Int32Array:
                    int[] bs6 = (int[])val;
                    return string.Join(",", bs6);
                case DataType.UInt32Array:
                    uint[] bs7 = (uint[])val;
                    return string.Join(",", bs7);
                case DataType.FloatArray:
                    float[] bs8 = (float[])val;
                    return string.Join(",", bs8);
                case DataType.Int64Array:
                    long[] bs9 = (long[])val;
                    return string.Join(",", bs9);
                case DataType.UInt64Array:
                    ulong[] bs10 = (ulong[])val;
                    return string.Join(",", bs10);
                case DataType.DoubleArray:
                    double[] bs11 = (double[])val;
                    return string.Join(",", bs11);

                default:
                    return null;
            }
        }

    }
}
