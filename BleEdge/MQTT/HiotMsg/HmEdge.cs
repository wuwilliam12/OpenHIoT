using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OpenHIoT.BleEdge;
using OpenHIoT.BleEdge.Product;
using MQTTnet;
using MQTTnet.Client;
using InTheHand.Net.Bluetooth;
using OpenHIoT.LocalServer.Data;
using SparkplugNet.Core.Enumerations;
using OpenHIoT.LocalServer.HiotMsg;
using SparkplugNet.Core.Extensions;
using Newtonsoft.Json;
using OpenHIoT.LocalServer.Operation;
using Microsoft.AspNetCore.Components.Forms;

using BdStreamDlCode = OpenHIoT.LocalServer.Operation.Channel.BdStreamDlCode;
using OpenHIoT.BleEdge.BLE;

using OpenHIoT.LocalServer.Data;
using SparkplugNet.VersionB.Data;
using MQTTnet.Packets;
using System.Security.Cryptography;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;
//using static OpenHIoT.LocalServer.Operation.Channel;
//using Channel = OpenHIoT.LocalServer.Operation.Channel;

//using Java.Util;
//using Xamarin.Google.Crypto.Tink.Subtle;


namespace OpenHIoT.BleEdge.MQTT.HiotMsg
{
    public class HmEdge : Product.Device,  IMqttClient
    {
     //   ulong mac_addr;
     //   ulong id;
     //   static string NameSpace1 = "hiotV1.0";

        protected MqttClient mqttClient;

        public event MqttMsgReceivedEventHandler MqttMsgRecieved;
        
        [JsonIgnore]
        public Product.Devices Devices { get; set; }


        /* [JsonIgnore]
public Channels Channels { get; set; }
*/
        [Flags]
        public enum EdgeStatus { BleOn = 1, BleScan = 2, };

        public HmEdge()
        {
            Name = "BleEdge_A";
            Prod = 3;
            HmStCh = 1;
            BluetoothRadio myRadio =  BluetoothRadio.Default;
            mac_addr =  myRadio.LocalAddress;
            PhyId = (mac_addr << 8) + (ulong)OpenHIoTIdType.Ble;
            id = (ulong)PhyId;
            Devices = new Product.Devices();

            Channels = new Channels();
            Product.Channel ch = new Product.Channel();
            ch.IId = 1;
            ch.DType = (ushort)DataType.Bytes;
            OpenHIoT.LocalServer.Operation.Channel.InitBidirectionalStreamHead(ch);
            Channels.Add(ch);

            /*
            ch = new Device.Channel();
            ch.Head.IId = 2;
            ch.Head.Name = "New Devices";
            ch.Head.DType = (ushort)SparkplugNet.VersionB.Data.DataType.String;
            ch.Head.Property = (ushort)(ChannelProperty.Notify | ChannelProperty.Read);
            Channels.Add(ch); */

        }

        public bool Start()
        {
            ConnectToBroker();
            return true;
        }

        public bool Subscribe(string topic)
        {
            return true;
        }

        public string GetTopic(SparkplugMessageType mt, ulong dev_id)
        {
            return $"{TopicNamespace.hm1_0}/g1/{mt.GetDescription()}/{dev_id}";
        }


        public async void ConnectToBroker()
        {
            var factory = new MqttFactory();

            // Create a MQTT client instance
             mqttClient = (MqttClient)factory.CreateMqttClient();

            // Create MQTT client options
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(EdgeSetting.Setting.Broker.Ip, EdgeSetting.Setting.Broker.Port) // MQTT broker address and port
              //  .WithCredentials(((StartUp.Edge)startup_dev).User, ((StartUp.Edge)startup_dev).Pw) // Set username and password
                .WithClientId(id.ToString())
                .WithCleanSession()
                .Build();

            // Connect to MQTT broker
            var connectResult = await mqttClient.ConnectAsync(options);

            if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
            {
                Console.WriteLine("Connected to MQTT broker successfully.");
                await PublishBirthMessage();
                // Subscribe to a topic
                await mqttClient.SubscribeAsync(GetTopic(  SparkplugMessageType.NodeCommand, id ));

                // Callback function when a message is received
                mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;    
            }
            else
            {
                Console.WriteLine($"Failed to connect to MQTT broker: {connectResult.ResultCode}");
            }

        }

        private Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
           // arg.ApplicationMessage.Topic 
            

            HmPayload.ReadPayloadData(this, arg.ApplicationMessage.PayloadSegment.Array);
  //          OpenHIoT.LocalServer.Operation.Channel.
            return Task.CompletedTask;
        }

        async Task<MqttClientPublishResult> PublishDataMessage(Product.Channel ch)
        {
            MqttApplicationMessage dataMessage = new()
            {
                Topic = $"{HmTopic.NameSpace1}/g1/{SparkplugMessageType.NodeData.GetDescription()}/{id}",
              //  Payload = ch.GetValPlayloadBytes()
            };

            // Publish the message.
            return await mqttClient.PublishAsync(dataMessage);
        }
        public async Task RemoveDevice(Product.Device device)
        {
            if (!Devices.Contains(device)) return;
            Devices.Remove(device);
            MqttApplicationMessage dataMessage = new()
            {
                Topic = $"{HmTopic.NameSpace1}/g1/{SparkplugMessageType.DeviceDeath.GetDescription()}/{id}/{device.Id}",               
            };
            await mqttClient.PublishAsync(dataMessage);
        }

        public async Task AddNewDevice(Product.Device dev)
        {
            if (!Devices.Contains(dev))
            {
                dev.Parent = Id;
                Devices.Add(dev);
            }
            MqttApplicationMessage dataMessage = new()
            {
                Topic = $"{HmTopic.NameSpace1}/g1/{SparkplugMessageType.DeviceBirth.GetDescription()}/{Id}/{dev.Id}",

                PayloadSegment = dev.GetHmBirthPayload(),
            };
            await mqttClient.PublishAsync(dataMessage);
        }

        async Task<MqttClientPublishResult> PublishBirthMessage()
        {
            MqttApplicationMessage dataMessage = new()
            {
                Topic = $"{HmTopic.NameSpace1}/g1/{SparkplugMessageType.NodeBirth.GetDescription()}/{id}",

                PayloadSegment = GetHmBirthPayload(),
            };

            // Publish the message.
            return await mqttClient.PublishAsync(dataMessage);
        }

        public async Task<MqttClientPublishResult> PublishDeviceDataMessage(ulong dev_id, Product.Channels chs)
        {
            MqttApplicationMessage dataMessage = new()
            {
                Topic = $"{HmTopic.NameSpace1}/g1/{SparkplugMessageType.DeviceData.GetDescription()}/{id}/{dev_id}",
                PayloadSegment = chs.GetHmValPlayloadBytesIId()
            };

            // Publish the message.
            return await mqttClient.PublishAsync(dataMessage);
        }

        public async Task<MqttClientPublishResult> PublishNewDevicesMessage(List<BLE.Device> new_devs)
        {
            try
            {
                List<LocalServer.Data.Device> devs = new List<LocalServer.Data.Device>();
                foreach (BLE.Device d in new_devs)
                    devs.Add(d.GetModelsDevice());
                byte[] bs1 = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(devs));
                byte[] bs2 = new byte[bs1.Length + 1];
                bs2[0] = (byte)OpenHIoT.LocalServer.Operation.Channel.BdStreamUlCode.NewDevices;
                Buffer.BlockCopy(bs1, 0, bs2, 1, bs1.Length);
                Channels[0].Value = bs2;
                MqttApplicationMessage dataMessage = new()
                {
                    Topic = $"{HmTopic.NameSpace1}/g1/{SparkplugMessageType.NodeData.GetDescription()}/{id}",
                    PayloadSegment = OpenHIoT.LocalServer.Operation.Channel.GetHmValTsPlayloadBytesIId(Channels[0], Channels[0].Value)
                };
                return await mqttClient.PublishAsync(dataMessage);
            }
            catch (Exception ex)
            {
                return new MqttClientPublishResult(null, MqttClientPublishReasonCode.UnspecifiedError, "", null);
                //new List<MqttUserProperty>( ;

            }
        }

        public override void AfterProcessHmBlockData()
        {

        }

        public override void ProcessHmBlockData(HmPayloadBlock block)
        {
            Product.Channel? ch = null;
            Sample? sample = null;
            switch (block.Type)
            {
                case HmPayloadBlockTypes.HM_BT_CHANNEL_DATA_IID:
                case HmPayloadBlockTypes.HM_BT_CHANNEL_DATA_TS_IID:
                    HmBlockChannelValueByIID b1 = (HmBlockChannelValueByIID)block;
                    ch = Channels.FirstOrDefault(x => x.IId == b1.IId);
                    if ( ch != null)
                    {
                        sample = OpenHIoT.LocalServer.Operation.Channel.ReadDataBlockHm(b1, ch);
                    }
                    break;

                case HmPayloadBlockTypes.HM_BT_CHANNEL_DATA_NAME:
                    break;
            }
            if (sample != null)
            {
                if (ch.IId == HmStCh)
                    ProcessStreamDataBlock((byte[])sample.GetVal());

            }
        }

        void ProcessStreamDataBlock(byte[] buf )
        {
            int offset = 0;
            BdStreamDlCode code = (BdStreamDlCode)BitConverter.ToUInt16(buf, offset );
            offset += 2;
            switch (code)
            {
                case BdStreamDlCode.SearchNew:
                    IBleCentral.BlePort.ScanAndConnect();
                    break;
                case BdStreamDlCode.AcceptDevice:
                    IBleCentral.BlePort.AcceptDevice(BitConverter.ToUInt64(buf, offset) >> 8);
                    break;
                case BdStreamDlCode.RemoveDevice:
                    IBleCentral.BlePort.RemoveDevice(BitConverter.ToUInt64(buf, offset) >> 8);
                    break;
                case BdStreamDlCode.ReconnectDevice:
                    break;
                case BdStreamDlCode.DisconnectDevice:
                    break;
                default:
                    break;
            }
        }

        public async Task<MqttClientPublishResult> PublishDeviceDeathMessage(ulong dev_id)
        {
            MqttApplicationMessage dataMessage = new()
            {
                Topic = $"{HmTopic.NameSpace1}/g1/{SparkplugMessageType.DeviceDeath.GetDescription()}/{id}/{dev_id}",
                PayloadSegment = null,
            };
            return await mqttClient.PublishAsync(dataMessage);

        }
    }
}



