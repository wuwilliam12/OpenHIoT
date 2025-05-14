using Newtonsoft.Json.Converters;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Operation;
using SparkplugNet.VersionB.Data;
using System.Drawing;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Xml.Linq;

namespace OpenHIoT.LocalServer.HiotMsg
{
    public class HmPayload
    {
        public static string? ReadByteSizeString(byte[] buf, ref ushort offset)
        {
            int s = buf[offset++];
            s += buf[offset++] << 8;
            if (s == 0) return null;
            string str = Encoding.UTF8.GetString(buf, offset, s);
            offset += (ushort)s;
            return str.TrimEnd();
        }

        public static string? Read2ByteSizeString(byte[] buf, ref ushort offset)
        {
            int s = buf[offset++];
            s += buf[offset++] << 8;
            if (s == 0) return null;
            string str = Encoding.UTF8.GetString(buf, offset, s);
            offset += (ushort)s;
            return str.TrimEnd();
        }
        public static void WriteByteSizeString(BinaryWriter bw, string? str)
        {
            if (str == null)
            {
                bw.Write((byte)0); 
            }
            else
            {
                byte[] bs = Encoding.UTF8.GetBytes(str);
                if (bs.Length < 255)
                {
                    bw.Write((byte)bs.Length);
                    bw.Write(bs);
                }
                else bw.Write((byte)0);
            }
        }
        public static void Write2ByteSizeString(BinaryWriter bw, string? str)
        {
            if (str == null)
            {
                bw.Write((byte)0); bw.Write((byte)0);
            }
            else
            {
                byte[] bs = Encoding.UTF8.GetBytes(str);
                bw.Write((byte)bs.Length);
                bw.Write(bs.Length >> 8);
                bw.Write(bs);
            }
        }

        public static HmPayloadBlock? ReadNextBlock(byte[] payload,  ushort offset)
        {
            if (offset >= payload.Length)
                return null;
            ushort s = BitConverter.ToUInt16(payload, offset);
            offset += 2;
           
            if (s > 0 && offset < payload.Length)
            {
                HmPayloadBlock new_block;
                HmPayloadBlockTypes b_type = (HmPayloadBlockTypes)BitConverter.ToUInt16(payload, offset);
                offset += 2;
                switch (b_type)
                {
                    case HmPayloadBlockTypes.HM_BT_CHANNEL_DATA_IID:
                        new_block = new HmBlockChannelValueByIID();
                        break;
                    case HmPayloadBlockTypes.HM_BT_CHANNEL_DATA_TS_IID:
                        new_block = new HmBlockChannelValueTsByIID();
                        break;
                    case HmPayloadBlockTypes.HM_BT_CHANNEL_DATA_NAME:
                        new_block = new  HmBlockChannelValueByName();
                        break;
                    case HmPayloadBlockTypes.HM_BT_CHANNEL_BIRTH:
                        new_block = new HmBlockChannelBirth();
                        break;
                    case HmPayloadBlockTypes.HM_BT_DEVICE_BIRTH:
                        new_block = new HmPayloadBlockBirthDevice();
                        break;
                    default:
                        new_block = new HmPayloadBlockDummy();                        
                        break;
                }
                new_block.Offset = offset;
                new_block.Size = s;
                new_block.Buffer = payload;      
                return new_block;
            }            
            return null;
        }

        public static bool ReadPayloadData(IDeviceHm dev, byte[] payload)
        {
            ushort rd_pos = 0;
            HmPayloadBlock? new_block = ReadNextBlock(payload,  rd_pos);
            while (new_block != null)
            {
                rd_pos += 4;
                if (new_block.Read())
                    dev.ProcessHmBlockData(new_block);
                rd_pos += new_block.Size;
                new_block = ReadNextBlock(payload, rd_pos );
            }
            dev.AfterProcessHmBlockData();
            return true;
        }
    }
}
