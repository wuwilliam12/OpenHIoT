using Newtonsoft.Json;
using OpenHIoT.LocalServer.Data.SampleDb;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;
using OpenHIoT.LocalServer.Operation;
using SparkplugNet.VersionB.Data;
using System.Text;
using System.Text.Json.Serialization;

namespace OpenHIoT.LocalServer.HiotMsg
{
    public enum HmPayloadBlockTypes
    {
        HM_BT_EMPTY = 0,

//        HM_BT_EDGE_BIRTH = 1,
        HM_BT_DEVICE_BIRTH = 1,

        HM_BT_CHANNEL_BIRTH = 2,

        HM_BT_CHANNEL_DATA_IID = 3,
        HM_BT_CHANNEL_DATA_NAME = 4,
        HM_BT_CHANNEL_DATA_TS_IID = 5,
        HM_BT_CHANNEL_DATA_TS_NAME = 6,
 //       HM_BT_MEASUREMENT_DATA_IID_VSR = 7,   //
 //       HM_BT_MEASUREMENT_DATA_IID_CSR = 8,   //
 //       HM_BT_MEASUREMENT_DATA_NAME_VSR = 9,
 //       HM_BT_MEASUREMENT_DATA_NAME_CSR = 10,
        HM_BT_TOPIC = 0x10

    };

    public class HmPayloadBlock
    {
        public ushort Offset { get; set; }
        public ushort Size { get; set; }
        public byte[] Buffer { get; set; }

        public virtual HmPayloadBlockTypes Type { get; }
        public virtual bool Read()
        {
            return true;
        }

        public static void WriteBlock(BinaryWriter bw, HmPayloadBlockTypes btype, byte[] body)
        {
            bw.Write((ushort)body.Length);
            bw.Write((ushort)btype);
            bw.Write(body);
        }

        public static void WriteDataBlock(BinaryWriter bw, HeadRt h, object val)
        {
            byte[]? bs = ValueDataType.GetValBytes(h.DType, val);
            if (bs != null)
            {
                bw.Write((ushort)(bs.Length+1));
                bw.Write((ushort)HmPayloadBlockTypes.HM_BT_CHANNEL_DATA_IID);
                bw.Write((byte)h.IId);
                bw.Write(bs);
            }
        }

        public static void WriteDataBlock(BinaryWriter bw, HeadRt h, object val, ulong ts)
        {
            byte[]? bs = ValueDataType.GetValBytes(h.DType, val);
            if (bs != null)
            {
                bw.Write((ushort)(bs.Length + 9));
                bw.Write((ushort)HmPayloadBlockTypes.HM_BT_CHANNEL_DATA_TS_IID);
                bw.Write((byte)h.IId);
                bw.Write(bs);
                bw.Write(ts);
            }
        }

    }
    public class HmPayloadBlockDummy : HmPayloadBlock
    {
        public HmPayloadBlockTypes Type { get { return HmPayloadBlockTypes.HM_BT_EMPTY; } }
    }
        


    public class HmPayloadBlockBirthDevice : HmPayloadBlock
    {
        public override HmPayloadBlockTypes Type { get { return HmPayloadBlockTypes.HM_BT_DEVICE_BIRTH; } }
        public Data.Device Device { get; set; }  
        public override bool Read()
        {
            string str = Encoding.UTF8.GetString(Buffer, Offset, Size);
            Device = JsonConvert.DeserializeObject<Data.Device>(str);
            return true;
        }

    }

    public class HmBlockChannelBirth : HmPayloadBlock   
    {
        public override HmPayloadBlockTypes Type { get { return HmPayloadBlockTypes.HM_BT_CHANNEL_BIRTH; } }
        public ChannelDTO ChDto { get; set; }
        public override bool Read()
        {
            string str = Encoding.UTF8.GetString(Buffer, Offset, Size);
            ChDto = JsonConvert.DeserializeObject<ChannelDTO>(str);
            return true;
        }

    }

    public class HmBlockChannelValue : HmPayloadBlock   
    {
        protected int rd_pos;
        public Channel Channel { get; set; }
 
        public int RdPos { 
            get { return rd_pos; }
            set { rd_pos = value; }  
        }

    }

    public class HmBlockChannelValueByIID : HmBlockChannelValue   //
    {
        public override HmPayloadBlockTypes Type { get { return HmPayloadBlockTypes.HM_BT_CHANNEL_DATA_IID; } }
        public byte IId { get; set; }         // internal Id, inside device

        public override bool Read()
        {
            rd_pos = Offset;
            IId = Buffer[rd_pos++];
            return true;
        }

 
        public byte[] GetBytes(HeadRt h, ushort cmd, byte[]? paras)
        {
            byte[] bs = BitConverter.GetBytes(cmd);
            if (paras == null)
                return Channel.GetHmValPlayloadBytesIId(h, bs);
            else
            {
                byte[] res = new byte[paras.Length +2];
                res[0] = bs[0]; res[1] = bs[1];
                System.Buffer.BlockCopy(paras, 0, res, 2, paras.Length); 
                return Channel.GetHmValPlayloadBytesIId(h, res);
            }
/*
            int s = 4 + 1 + 2 + 2 +8;
            if (paras != null)
                s += paras.Length;
            byte[] bs = new byte[s];

            using (MemoryStream memoryStream = new MemoryStream(bs))
            {

                BinaryWriter bw = new BinaryWriter(memoryStream);
                bw.Write((ushort)(s-4));    //2     block size
                bw.Write((ushort)Type);     //2           type 
                bw.Write(IId);              //1
                bw.Write((ushort)1);        //2     samples 
                bw.Write(cmd);              //2         body
                bw.Write(ts);               //8         
                if (paras != null)
                    bw.Write(paras);
                bw.Close();
            }
            return bs;
*/
        }

    }

    public class HmBlockChannelValueTsByIID : HmBlockChannelValueByIID
    {
        public override HmPayloadBlockTypes Type { get { return HmPayloadBlockTypes.HM_BT_CHANNEL_DATA_TS_IID; } }
    }

    public class HmBlockChannelValueByName : HmBlockChannelValue   
    {
        public override HmPayloadBlockTypes Type { get { return HmPayloadBlockTypes.HM_BT_CHANNEL_DATA_NAME; } }
        public string Name { get; set; }
        public override bool Read()
        {
            rd_pos = Offset;
            byte s = Buffer[rd_pos++];
            if (s > 0)
            {
                Name = Encoding.UTF8.GetString(Buffer, rd_pos, s);
                rd_pos += s;
          //      ReadHead();
                return true;
            }
            else
                return false;
            
        }
    }
  
}
