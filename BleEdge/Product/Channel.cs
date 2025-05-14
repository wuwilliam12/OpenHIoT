using OpenHIoT.LocalServer.Data.SampleDb;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenHIoT.BleEdge.Product
{

    public class Channel : HeadRt
    {
        //  public HeadRt Head { get; set; }
        public string? DefaultVal { get; set; }

        [JsonIgnore]
        public object? Value { get; set; }


      
        public Channel()
        {
           // Head = new HeadRt();
        }
        /*
        public Channel(HeadRt h)
        {
            Head = h;
        }

        public Channel()
        {
           // Head = new HeadRt();
       //     Head.CopyFrom(ch);
            if(Val != null) 
                Value = ValueDataType.Parse(DType, Val);
        }
*/
        public void Init()
        {
            if (DefaultVal != null)
                Value = ValueDataType.Parse(DType, DefaultVal);
        }

    }

    public class Channels : List<Channel>
    {
        public byte[] GetHmValPlayloadBytesIId()
        {
            ulong ts = (ulong)((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
            using ( MemoryStream ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                foreach ( Channel chn in this ) 
                    OpenHIoT.LocalServer.HiotMsg.HmPayloadBlock.WriteDataBlock(bw, chn, chn.Value, ts);
                ms.SetLength(ms.Length);
                return ms.GetBuffer();
            }
        }

    }
}
