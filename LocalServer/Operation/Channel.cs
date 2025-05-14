using SparkplugNet.VersionB.Data;
using System.Collections.Concurrent;
using System.Text;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data.SampleDb.Archive;
using System.Collections;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using OpenHIoT.LocalServer.HiotMsg;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Security.Cryptography;
using OpenHIoT.LocalServer.Data.SampleDb;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;
//using static SparkplugNet.VersionB.ProtoBuf.ProtoBufPayload;

namespace OpenHIoT.LocalServer.Operation
{
    public class Channel     
    {
        public enum BdStreamDlCode { SearchNew = 1, AcceptDevice = 2, RemoveDevice = 3, ReconnectDevice = 4, DisconnectDevice = 5 };
        public enum BdStreamUlCode { NewDevices = 1, DeviceStatus = 2, };

        public static void InitBidirectionalStreamHead(HeadRt h) 
        {
            h.Name = "StreamBd";
            h.DType = (ushort)SparkplugNet.VersionB.Data.DataType.Bytes;
            h.Property = (ushort)(ChannelProperty.Notify | ChannelProperty.Write);
        }
        public ulong? Alias { get; set; }
        public HeadRt Head { get; set; }
        public Sample? Sample { get; set; } 

        public Channel() {
     //       Head = new HeadRt();
        }    
        public Channel(Device? dev, Metric metric)
        {          
            Head = new HeadRt(dev, metric);
            Alias = Head.GetAlias();
        }

        public Channel(ChannelDTO dto)
        {
            Head = new HeadRt();
            Head.CopyFrom(dto);
            if(dto.Val != null)
            {
                Sample = Sample.CreateSample(ValueDataType.GetDataType(Head.DType));
                Sample.HId = Head.Id;
                Sample.SetVal( dto.Val );
            }
            Alias = Head.GetAlias();
        }

        public static Sample? ReadDataBlockHm(HmBlockChannelValueByIID block, HeadRt? h )
        {
            if(h is null) return null;
            Sample sample = Sample.CreateSample(ValueDataType.GetDataType(h.DType));
            sample.HId = h.Id;
            sample.Alias = (ulong)h.GetAlias();
            int rd_pos = block.RdPos;
            int val_size = block.Size - 1;
            if (block.Type == HmPayloadBlockTypes.HM_BT_CHANNEL_DATA_TS_IID)
                val_size -= 8;
            sample.ReadVal(block.Buffer, ref rd_pos, (ushort)val_size);

            block.RdPos = rd_pos;
            if (block is HmBlockChannelValueTsByIID)
                sample.TS = BitConverter.ToUInt64(block.Buffer, block.RdPos);
                
            return sample;
        }


        public Sample? GetSample(Metric metric_d)
        {
            Sample sample = Sample.CreateSample(ValueDataType.GetDataType(Head.DType));
            sample.Alias = (ulong)metric_d.Alias;
            sample.TS = metric_d.Timestamp == null ? (ulong)((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds() : (ulong)metric_d.Timestamp;
            sample.HId = Head.Id;

            if (metric_d.Value is not null)
            {
                sample.SetVal( ValueDataType.GetVal(metric_d.DataType, metric_d.Value) );
                if (sample.GetVal() is null)
                    sample.SetVal( ValueDataType.GetValBytes(metric_d.DataType, metric_d.Value) );
                if (sample.GetVal() is not null)
                    return sample;
            }
            return null;
        }
        public static byte[] GetHmValTsPlayloadBytesIId(HeadRt h, object v)
        {
            ulong ts = (ulong)((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                HmPayloadBlock.WriteDataBlock(bw,  h,  v, ts );
                ms.Capacity = (int)ms.Length;
                return ms.GetBuffer();
            }
        }

        public static byte[] GetHmValPlayloadBytesIId(HeadRt h, object v)
        {
            ulong ts = (ulong)((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                HmPayloadBlock.WriteDataBlock(bw,  h,  v );
                ms.Capacity = (int)ms.Length;
                return ms.GetBuffer();
            }
        }

        /*
        public static byte[]? GetDataBytesByIId( ushort? dtype, byte iid, val)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(iid);
                byte[]? bs = ValueDataType.GetValBytes(dtype, v);
                bw.Write(bs);
       
                bw.Write(ts);
                ms.Capacity = (int)ms.Length;
                return ms.GetBuffer();
            }
        }
  

        public static byte[]? GetDataBytesByIId(ulong ts, ushort? dtype, byte iid, object[] vals)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(iid);
                bw.Write((ushort)(vals.Length + 0x8000));
                foreach (object v in vals)
                {
                    byte[]? bs = ValueDataType.GetValBytes(dtype, v);
                    if (bs == null) return null;
                    bw.Write(bs);
                }
                bw.Write(ts);
                ms.Capacity = (int)ms.Length;
                return ms.GetBuffer();
            }
        }
      */
        public static byte[]? GetDataBytesByName(ulong ts, ushort? dtype, string name, object[] vals)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                HmPayload.WriteByteSizeString(bw, name);
                bw.Write((byte)vals.Length);
                foreach (object v in vals)
                {
                    byte[]? bs = ValueDataType.GetValBytes(dtype, v);
                    if (bs == null) return null;
                    bw.Write(bs);
                }
                bw.Write(ts);
                ms.Capacity = (int)ms.Length;
                return ms.GetBuffer();
            }
        }

        /*
        public static byte[]? GetDataBytesByIId(ulong[] ts, ushort? dtype, byte iid, object[] vals)
        {
            if (ts.Length != vals.Length)
                return null;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(iid);
                bw.Write((ushort)vals.Length);              //constant space
                for (int i = 0; i < ts.Length; i++)
                {
                    byte[]? bs = ValueDataType.GetValBytes(dtype, vals[i]);
                    if (bs == null) return null;
                    bw.Write(bs);
                    bw.Write(ts[i]);
                }              
                ms.Capacity = (int)ms.Position;
                return ms.GetBuffer();
            }

        }*/

        public static byte[]? GetDataBytesByName(ulong[] ts, ushort? dtype, string name, object[] vals)
        {
            if (ts.Length != vals.Length)
                return null;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                HmPayload.WriteByteSizeString(bw, name);
                bw.Write((ushort)vals.Length);               //constant space
                for (int i = 0; i < ts.Length; i++)
                {
                    byte[]? bs = ValueDataType.GetValBytes(dtype, vals[i]);
                    if (bs == null) return null;
                    bw.Write(bs);
                    bw.Write(ts[i]);
                }
                ms.Capacity = (int)ms.Position;
                return ms.GetBuffer();
            }
        }


    }
    public class ChannelDTO : HeadRt   // Data exchange object
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Val {  get; set; }

        public ChannelDTO() { }
        public ChannelDTO(HeadRt h) {
            base.CopyFrom(h);
        }
        public void CopyFrom(ChannelDTO from)
        {
            base.CopyFrom(from);
            Val = from.Val;
        }

        /*
        public ulong? GetMetricAlias()
        {
            if (IId == null) return null;
            return (DId << 8) + IId;
        }
        */
        public byte[] GetHmDbBodyBytes()
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
        }

        public  Metric GetChannelMetric(ulong dev_id)
        {
            DId = dev_id;
            DataType dt = DType == null ? DataType.Float : (DataType)DType;
            object? v = null;
            if (Val != null)
                v = ValueDataType.Parse(dt, Val);
            Metric m = new Metric(dt, v)
            {
                Name = Name,
                Alias = GetAlias(),
                Properties = GetPropertySetStringV()
            };

            return m;
        }
    }
 
    public class ChannelStreamBd : Channel // bi-direction
    {

    }
    
    public class KnownChannels
    {
        /// <summary>
        /// The known metrics by name.
        /// </summary>
        private readonly ConcurrentDictionary<string, Channel> knownItemsByName = new();

        /// <summary>
        /// The known metrics by alias.
        /// </summary>
        private readonly ConcurrentDictionary<ulong, Channel> knownItemsByAlias = new();

        /// The known metrics by alias.
        /// </summary>
        private readonly ConcurrentDictionary<byte, Channel> knownItemsByIId = new();

        /// <summary>
        /// Gets the metrics as <see cref="List{T}"/>.
        /// </summary>
        public List<Channel> Items => [..this.knownItemsByIId.Values, ..this.knownItemsByAlias.Values,  ..this.knownItemsByName.Values];

        public ConcurrentDictionary<string, Channel> ItemsByName { get { return knownItemsByName; } }

        public ConcurrentDictionary<ulong, Channel> ItemsByAlias { get { return knownItemsByAlias; } }
        public ConcurrentDictionary<byte, Channel> ItemsByIId { get { return knownItemsByIId; } }

        public KnownChannels()
        {

        }
        public KnownChannels(Device? dev, List<Metric> ms)
        {
            foreach (var metric in ms)
            {
                if ("bdSeq" == metric.Name)
                    continue;
                if (metric.Alias is not null)
                {
                    knownItemsByAlias[(ulong)metric.Alias] = new Channel(dev, metric);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(metric.Name))
                    {
                        knownItemsByName[metric.Name] = new Channel(dev, metric);
                    }
                    else
                    {
                        //not valid birth metric
                    }
                }

            }

        }


    }
}
