using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data.SampleDb;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.HiotMsg;
using OpenHIoT.LocalServer.Operation;
using OpenHIoT.MqttSim.Sparkplug;
using SparkplugNet.VersionB.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
//using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenHIoT.MqttSim.HiotMsg
{
    public class Device : LocalServer.Data.Device, IDevice
    {
        ulong? id;
        [JsonIgnore]
        public ulong? Id
        {
            get
            {
                if (id == null) id = GetId();
                return id;
            }
        }
        [JsonIgnore]
        public double? SA
        {
            get { return sa; }
            set
            {
                sa = value;
                if (sa == null)
                    acq_timer.Interval = 1000;
                else
                    acq_timer.Interval = (double)sa;
            }
        }
        double? sa;
        protected System.Timers.Timer acq_timer;
        protected Channels chs;
        protected StartUp.Device startup_dev;
        public Device()
        {
            chs = new Channels();
            acq_timer = new System.Timers.Timer();
            acq_timer.Elapsed += Acq_timer_Elapsed;
        }

        public void Init(StartUp.Device su_dev)
        {
            startup_dev = su_dev;
            Name = su_dev.Product;
            PhyId = ((ulong)su_dev.SimId << 8) + (ulong)OpenHIoTIdType.Simulator;
            chs = new Channels();
            foreach (ChannelDTO dto in startup_dev.Channels)
            {
                Channel new_ch = new Channel()
                {
                    DTO = dto,
                    DType = dto.DType == null ? DataType.Float : (DataType)dto.DType,
                    Device = this,

                };
                if (dto.Val != null)
                    new_ch.Val = ValueDataType.Parse(new_ch.DType, dto.Val);
                chs.Add(new_ch);

            }
        }

        public byte[] GetHmBirthPayload()
        {
           /* LocalServer.DBase.Models.Device dev = new LocalServer.DBase.Models.Device()
            {

                PhyId = Phy,
            };
            if (product != null) dev.Prod = product.Id;*/
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                HmPayloadBlock.WriteBlock(bw, HmPayloadBlockTypes.HM_BT_DEVICE_BIRTH, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this)));
                foreach (Channel ch in chs)
                    HmPayloadBlock.WriteBlock(bw, HmPayloadBlockTypes.HM_BT_CHANNEL_BIRTH, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ch.DTO)));
                bw.Flush();
                byte[] bs1 = new byte[bw.BaseStream.Position];
                Buffer.BlockCopy(ms.GetBuffer(), 0, bs1, 0, bs1.Length);
                return bs1;
            }
        }

        private void Acq_timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            AcquireData();
        }

        protected virtual async Task AcquireData()
        {

        }

        public void StartAcquisition()
        {
            SA = sa;
            acq_timer.Start();
        }

        public void StopAcquisition()
        {
            acq_timer.Stop();
        }
   


    }
}

/*



*/
