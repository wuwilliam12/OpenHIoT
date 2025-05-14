using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHIoT.BleEdge.Product
{
    public partial class SparkplugValue
    {
        public static void WriteValInt8(object ob, byte[] dat, ushort wr_pos)
        {
            dat[wr_pos] = Convert.ToByte(ob);
        }
        public static void WriteValInt8s(object ob, byte[] dat, ushort wr_pos)
        {        
            sbyte[] sb = (sbyte[])ob;   
            Buffer.BlockCopy(sb, 0, dat, wr_pos, sb.Length);
        }

        public static void WriteValUInt8(object ob, byte[] dat, ushort wr_pos)
        {
            dat[wr_pos] = (byte)ob;
        }
        public static void WriteValUInt8s(object ob, byte[] dat, ushort wr_pos)
        {
            byte[] bs = (byte[])ob;
            Buffer.BlockCopy(bs, 0, dat, wr_pos, bs.Length);
        }

        public static void WriteValInt16(object ob, byte[] dat, ushort wr_pos)
        {
            byte[] bs = BitConverter.GetBytes((short)ob);
            Buffer.BlockCopy(bs, 0, dat, wr_pos, bs.Length);
        }
        public static void WriteValInt16s(object ob, byte[] dat, ushort wr_pos)
        {
            short[] bs = (short[])ob;
            Buffer.BlockCopy(bs, 0, dat, wr_pos, bs.Length << 1);
        }

        public static void WriteValUInt16(object ob, byte[] dat, ushort wr_pos)
        {
            byte[] bs = BitConverter.GetBytes((ushort)ob);
            Buffer.BlockCopy(bs, 0, dat, wr_pos, bs.Length);
        }
        public static void WriteValUInt16s(object ob, byte[] dat, ushort wr_pos)
        {
            ushort[] bs = (ushort[])ob;
            Buffer.BlockCopy(bs, 0, dat, wr_pos, bs.Length << 1);
        }
        public static void WriteValInt32(object ob, byte[] dat, ushort wr_pos)
        {
            byte[] bs = BitConverter.GetBytes((int)ob);
            Buffer.BlockCopy(bs, 0, dat, wr_pos, bs.Length);
        }
        public static void WriteValInt32s(object ob, byte[] dat, ushort wr_pos)
        {
            int[] bs = (int[])ob;
            Buffer.BlockCopy(bs, 0, dat, wr_pos, bs.Length << 2);
        }
        public static void WriteValUInt32(object ob, byte[] dat, ushort wr_pos)
        {
            byte[] bs = BitConverter.GetBytes((uint)ob);
            Buffer.BlockCopy(bs, 0, dat, wr_pos, bs.Length);
        }
        public static void WriteValUInt32s(object ob, byte[] dat, ushort wr_pos)
        {
            uint[] bs = (uint[])ob;
            Buffer.BlockCopy(bs, 0, dat, wr_pos, bs.Length << 2);
        }
        public static void WriteValInt64(object ob, byte[] dat, ushort wr_pos)
        {
            byte[] bs = BitConverter.GetBytes((long)ob);
            Buffer.BlockCopy(bs, 0, dat, wr_pos, bs.Length);
        }
        public static void WriteValInt64s(object ob, byte[] dat, ushort wr_pos)
        {
            long[] bs = (long[])ob;
            Buffer.BlockCopy(bs, 0, dat, wr_pos, bs.Length << 3);
        }
        public static void WriteValUInt64(object ob, byte[] dat, ushort wr_pos)
        {
            byte[] bs = BitConverter.GetBytes((ulong)ob);
            Buffer.BlockCopy(bs, 0, dat, wr_pos, bs.Length);
        }
        public static void WriteValUInt64s(object ob, byte[] dat, ushort wr_pos)
        {
            ulong[] bs = (ulong[])ob;
            Buffer.BlockCopy(bs, 0, dat, wr_pos, bs.Length << 3);
        }
        public static void WriteValFloat(object ob, byte[] dat, ushort wr_pos)
        {
            byte[] bs = BitConverter.GetBytes((float)ob);
            Buffer.BlockCopy(bs, 0, dat, wr_pos, bs.Length);
        }
        public static void WriteValFloats(object ob, byte[] dat, ushort wr_pos)
        {
            float[] bs = (float[])ob;
            Buffer.BlockCopy(bs, 0, dat, wr_pos, bs.Length << 2);
        }

        public static void WriteValDouble(object ob, byte[] dat, ushort wr_pos)
        {
            byte[] bs = BitConverter.GetBytes((double)ob);
            Buffer.BlockCopy(bs, 0, dat, wr_pos, bs.Length);
        }
        public static void WriteValDoubles(object ob, byte[] dat, ushort wr_pos)
        {
            double[] bs = (double[])ob;
            Buffer.BlockCopy(bs, 0, dat, wr_pos, bs.Length << 3);
        }

    }
}
