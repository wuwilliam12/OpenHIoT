using Newtonsoft.Json;
using OpenHIoT.LocalServer.Data.Repository;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;
using OpenHIoT.LocalServer.HiotMsg;
using OpenHIoT.LocalServer.Services;
using System.Text;
using static OpenHIoT.LocalServer.Operation.Channel;

namespace OpenHIoT.LocalServer.Operation
{
    public interface IDeviceHm
    {
        void AfterProcessHmBlockData();
        void ProcessHmBlockData(HmPayloadBlock block);
    }
    public partial class Device : IDeviceHm
    {
        void ProcessHmBlockBirth(HmPayloadBlock block)
        {
            switch (block.Type)
            {
                case HmPayloadBlockTypes.HM_BT_DEVICE_BIRTH:
                    HmPayloadBlockBirthDevice b1 = (HmPayloadBlockBirthDevice)block;
                    DeviceBase = b1.Device;
                    DeviceBase.LaTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                    break;
                case HmPayloadBlockTypes.HM_BT_CHANNEL_BIRTH:
                    HmBlockChannelBirth b2 = (HmBlockChannelBirth)block;
                    b2.ChDto.DId = Id;
                    if (b2.ChDto.IId == null)
                        KnownChannels.ItemsByName.TryAdd(b2.ChDto.Name, new Channel(b2.ChDto));
                    else
                        KnownChannels.ItemsByIId.TryAdd((byte)b2.ChDto.IId, new Channel(b2.ChDto));

                    break;
            }
        }
        public void AfterProcessHmBlockData()
        {
            mValQueue.SaveSamples();
        }

        async void ProcessHmNewDevices(Channel ch, byte[] bs)
        {
            string str = Encoding.UTF8.GetString(bs, 1, bs.Length - 1);
            List<Data.Device>? devs = JsonConvert.DeserializeObject<List<Data.Device>>(str);

            using (var scope = IMqttService.Instance.GetScopeFactory().CreateScope())
            {
                var rep_dev = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
                foreach (Data.Device dev in devs)
                {
                    Data.Device? dev_db = await rep_dev.Get((ulong)dev.GetId());
                    if(dev_db != null)
                    {
                        HmBlockChannelValueByIID block = new HmBlockChannelValueByIID()
                        {
                            IId = (byte)DeviceBase.HmStCh,
                        };
                        byte[] bs1 = BitConverter.GetBytes((ulong)dev_db.PhyId);
                        IMqttService.Instance.WriteChannel(Id, block.GetBytes( ch.Head, (ushort)BdStreamDlCode.AcceptDevice, bs1));
                    }
                }                
            }
        }
        void ProcessHmStandardStreamChannelData(Channel ch, Sample sample)
        {
            byte[] bs = (byte[])sample.GetVal();
            Channel.BdStreamUlCode code = (LocalServer.Operation.Channel.BdStreamUlCode)bs[0];
            switch (code)
            {
                case Channel.BdStreamUlCode.NewDevices:
                    ProcessHmNewDevices(ch, bs);
                    break;
            }
        }

        public virtual void ProcessHmBlockData(HmPayloadBlock block)
        {
            switch (block.Type)
            {
                case HmPayloadBlockTypes.HM_BT_CHANNEL_DATA_IID:
                case HmPayloadBlockTypes.HM_BT_CHANNEL_DATA_TS_IID:
                    Channel? ch;
                    HmBlockChannelValueByIID b1 = (HmBlockChannelValueByIID)block;
                    if (KnownChannels.ItemsByIId.TryGetValue(b1.IId, out ch))
                    {
                        Sample? sample = Channel.ReadDataBlockHm(b1, ch.Head);
                        if (sample != null)
                        {
                            if (DeviceBase.HmStCh != null && DeviceBase.HmStCh == ch.Head.IId)
                                ProcessHmStandardStreamChannelData( ch, sample);
                            mValQueue.AddSample(sample);
                        }
                    }
                    break;

                case HmPayloadBlockTypes.HM_BT_CHANNEL_DATA_NAME:
                    break;
            }
        }

        public virtual void ProcessPayloadDataHm(byte[] payload, IChannelChannel _mValQueue)
        {
            mValQueue = _mValQueue;
            HmPayload.ReadPayloadData(this, payload);
        }


        public void ReadHmPayloadBirth(byte[] payload)
        {
            ushort rd_pos = 0;
            HmPayloadBlock? new_block = HmPayload.ReadNextBlock(payload, rd_pos);
            while (new_block != null)
            {
                rd_pos += 4;
                if (new_block.Read())
                    ProcessHmBlockBirth(new_block);
                rd_pos += new_block.Size;
                new_block = HmPayload.ReadNextBlock(payload, rd_pos);
            }
        }

        
    }


}
