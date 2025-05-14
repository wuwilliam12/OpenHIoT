using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHIoT.BleEdge.Product
{
    public partial class SparkplugValue
    {
        public static object ReadValInt8(byte[] dat, ushort rd_pos, ushort size)
        {
            return Convert.ToSByte(dat[rd_pos]);
        }
        public static object ReadValInt8s(byte[] dat, ushort rd_pos, ushort size)
        {
            int s = size;
            sbyte[] sb = new sbyte[s];
            for (int i = 0; i < s; i++)
            {
                sb[i] = Convert.ToSByte(dat[ rd_pos++]); 
            }
            return sb;
        }

        public static object ReadValUInt8(byte[] dat, ushort rd_pos, ushort size)
        {
            return dat[rd_pos];
        }
        public static object ReadValUInt8s(byte[] dat, ushort rd_pos, ushort size)
        {
            int s = size;
            byte[] us = new byte[s];
            for (int i = 0; i < s; i++)
            {
                us[i] = dat[rd_pos++];
            }
            return us;
        }

        public static object ReadValInt16(byte[] dat, ushort rd_pos, ushort size)
        {
            return BitConverter.ToInt16(dat, rd_pos);
        }
        public static object ReadValInt16s(byte[] dat, ushort rd_pos, ushort size)
        {
            int s = size >> 1;
            short[] ss = new short[s];
            for (int i = 0; i < s; i++)
            {
                ss[i] = BitConverter.ToInt16(dat, rd_pos); rd_pos += 2;
            }
            return ss;
        }

        public static object ReadValUInt16(byte[] dat, ushort rd_pos, ushort size)
        {
            return BitConverter.ToUInt16(dat, rd_pos);
        }
        public static object ReadValUInt16s(byte[] dat, ushort rd_pos, ushort size)
        {
            int s = size >> 1;
            ushort[] us = new ushort[s];
            for (int i = 0; i < s; i++)
            {
                us[i] = BitConverter.ToUInt16(dat, rd_pos); rd_pos += 2;
            }
            return us;
        }
        public static object ReadValInt32(byte[] dat, ushort rd_pos, ushort size)
        {
            return BitConverter.ToInt32(dat, rd_pos);
        }
        public static object ReadValInt32s(byte[] dat, ushort rd_pos, ushort size)
        {
            int s = size >> 2;
            int[] ss = new int[s];
            for (int i = 0; i < s; i++)
            {
                ss[i] = BitConverter.ToInt32(dat, rd_pos); rd_pos += 4;
            }
            return ss;
        }
        public static object ReadValUInt32(byte[] dat, ushort rd_pos, ushort size)
        {
            return BitConverter.ToUInt32(dat, rd_pos);
        }
        public static object ReadValUInt32s(byte[] dat, ushort rd_pos, ushort size)
        {
            int s = size >> 2;
            uint[] us = new uint[s];
            for (int i = 0; i < s; i++)
            {
                us[i] = BitConverter.ToUInt32(dat, rd_pos); rd_pos += 4;
            }
            return us;
        }    
        public static object ReadValInt64(byte[] dat, ushort rd_pos, ushort size)
        {
            return BitConverter.ToInt64(dat, rd_pos);
        }
        public static object ReadValInt64s(byte[] dat, ushort rd_pos, ushort size)
        {
            int s = size >> 3;
            long[] us = new long[s];
            for (int i = 0; i < s; i++)
            {
                us[i] = BitConverter.ToInt64(dat, rd_pos); rd_pos += 8;
            }
            return us;
        }
        public static object ReadValUInt64(byte[] dat, ushort rd_pos, ushort size)
        {
            return BitConverter.ToUInt64(dat, rd_pos);
        }
        public static object ReadValUInt64s(byte[] dat, ushort rd_pos, ushort size)
        {
            int s = size >> 3;
            ulong[] us = new ulong[s];
            for (int i = 0; i < s; i++)
            {
                us[i] = BitConverter.ToUInt64(dat, rd_pos); rd_pos += 8;
            }
            return us;
        }
        public static object ReadValFloat(byte[] dat, ushort rd_pos, ushort size)
        {
            return BitConverter.ToSingle(dat, rd_pos);
        }
        public static object ReadValFloats(byte[] dat, ushort rd_pos, ushort size)
        {
            int s = size >> 2;
            float[] fs = new float[s];
            for (int i = 0; i < s; i++)
            {
                fs[i] = BitConverter.ToSingle(dat, rd_pos); rd_pos += 4;
            }
            return fs;
        }

        public static object ReadValDouble(byte[] dat, ushort rd_pos, ushort size)
        {
            return BitConverter.ToDouble(dat, rd_pos);
        }
        public static object ReadValDoubles(byte[] dat, ushort rd_pos, ushort size)
        {
            int s = size >> 3;
            double[] fs = new double[s];
            for (int i = 0; i < s; i++)
            {
                fs[i] = BitConverter.ToDouble(dat, rd_pos); rd_pos += 8;
            }
            return fs;
        }

    }
}
