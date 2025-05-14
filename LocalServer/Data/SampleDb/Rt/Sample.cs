using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Converters;
using OpenHIoT.LocalServer.Data.SampleDb;
using OpenHIoT.LocalServer.HiotMsg;
using SparkplugNet.Core.Data;
using SparkplugNet.VersionB.Data;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;

namespace OpenHIoT.LocalServer.Data.SampleDb.Rt
{
    //   [Keyless]

    public class Sample 
    {
        public static Sample CreateSample(DataType dt)
        {
            Sample sample;
            switch (dt)
            {
                case DataType.Boolean:
                case DataType.Int8:
                case DataType.UInt8:
                case DataType.Int16:
                case DataType.UInt16:
                case DataType.Int32:
                case DataType.UInt32:
                case DataType.Int64:
                case DataType.UInt64:
                case DataType.DateTime:
                    sample =  new SampleInt(); break;
                case DataType.Float:
                case DataType.Double:
                    sample =  new SampleReal(); break;

                default:
                    sample = new SampleBlob(); break;
            }
            sample.dt = dt;
            return sample;
        }

        protected DataType dt;
        public uint Id { get; set; }         //  id
        [NotMapped]
        public ulong Alias { get; set; }
        public uint HId { get; set; }         // head id
        public ulong TS { get; set; }

        public void CopyFrom(Sample f)
        {
            Id = f.Id;
            TS = f.TS;
        }

        public virtual object? GetVal()
        {
            return null;
        }

        public virtual void SetVal( object? v )
        {
            
        }

        public virtual void SetVal(string str_v)
        {

        }

        public virtual void ReadVal(byte[] dat, ref int offset,  ushort size)
        {

        }
        
    }
        

    public class SampleInt : Sample
    {
        public long? val;
        public  long? Val {
            get { return val; }
            set{ val = value; }
        }

        public override object? GetVal()
        {
            return val;
        }

        public override void SetVal(object? v)
        {
            if(v != null)
                val = Convert.ToInt64(v);
        }
        public override void SetVal(string str_v)
        {
            Convert.ToInt64(str_v);
        }
        public SampleInt() { }
        public SampleInt(SampleInt v) {
            CopyFrom(v);
            if (v.Val is not null)
                Val = (long)v.Val;
        }

        public override void ReadVal(byte[] dat, ref int offset, ushort size)
        {
            val = ValueDataType.ConvertBytesToInteger(dt, dat, ref offset );
        }


    }

    public class SampleReal : Sample
    {
        public double? val; 
        public double? Val
        {
            get { return val; }
            set { val = value; }
        }
        public SampleReal() { }
        public SampleReal(SampleReal v)
        {
            CopyFrom(v);
            if (v.Val is not null)
                Val = (double)v.Val;
        }

        public override void ReadVal(byte[] dat, ref int offset, ushort size)
        {
            val = ValueDataType.ConvertBytesToDouble(dt, dat, ref offset);
        }
        public override object? GetVal()
        {
            return val;
        }

        public override void SetVal(object? v)
        {
            if(v != null)
                val = Convert.ToDouble(v);
        }

        public override void SetVal(string str_v)
        {
            Convert.ToDouble(str_v);
        }
    }


    //blob
    public class SampleBlob :Sample // 
    {
        public byte[]? val;

        public byte[]? Val
        {
            get { return val; }
            set { val = value; }
        }

        public SampleBlob() { }

        public SampleBlob(SampleBlob v)
        {
            CopyFrom(v);
            if (v.Val is not null)
                Val = v.Val;
        }

        public override object? GetVal()
        {
            return val;
        }

        public override void SetVal(object? v)
        {
            val = (byte[]?)v;
        }

        public override void SetVal(string str_v)
        {
            val = Encoding.UTF8.GetBytes(str_v);
        }
        public override void ReadVal(byte[] dat, ref int offset, ushort size)
        {
        //    ushort size = BitConverter.ToUInt16(dat, offset);
        //    offset += 2;
            val = new byte[size];
            Buffer.BlockCopy(dat, offset, val, 0, size);
            offset += size;
        }

        public override string ToString()
        {
            if (val == null)
                return "";
            return Encoding.UTF8.GetString(val);
        }
    }

}
