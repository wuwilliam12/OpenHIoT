using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;
using OpenHIoT.LocalServer.HiotMsg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenHIoT.MqttSim.HiotMsg
{

    public class Channel : MqttSim.Channel
    {
   //     public bool? Rec { get; set; }
        public Device? Device { get; set; }
        public object? Val { get; set; }

        public Channel()
        {

        }

        
        public byte[] GetBlockBytesData(ulong ts)
        {
            if (Val != null)
            {
                if (DTO.IId is null)
                {
                   
                }
                else
                    return LocalServer.Operation.Channel.GetHmValTsPlayloadBytesIId(DTO, Val);
            }
            return new byte[0];
        }

    }

    public class Channels : List<Channel>
    {

        public byte[] GetValPlayloadBytes()
        {
            ulong ts = (ulong)((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                foreach (Channel m in this)
                    HmPayloadBlock.WriteDataBlock(bw, m.DTO, m.Val, ts);

                bw.Write((ushort)0);
                byte[] bs1 = new byte[bw.BaseStream.Position];
                Buffer.BlockCopy(ms.GetBuffer(), 0, bs1, 0, bs1.Length);
                return bs1;
            }
        }
    }
}
