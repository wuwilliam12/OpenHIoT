using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using OpenHIoT.LocalServer.DBase;
using OpenHIoT.LocalServer.DBase.Models;
using OpenHIoT.LocalServer.HiotMsg;
using OpenHIoT.LocalServer.Operation;
using SparkplugNet.Core.Enumerations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SparkplugNet.Core.Extensions;
using MQTTnet.Packets;

namespace OpenHIoT.Client
{
    public partial class ClientEdge : OpenHIoT.LocalServer.DBase.Models.Device
    {
        static ClientEdge edge;
        public static ClientEdge Edge { get { return edge; } }
        public static void Start()
        {
            edge = new ClientEdge();
            edge.ConnectToBroker();
        }

        protected MqttClient mqttClient;
        Queue<MqttMsg> procQueue; bool proc_busy;

        [JsonIgnore]
        public MqttClient MqttClient
        {
            get
            {
                return mqttClient;
            }
        }


        public async void ConnectToBroker()
        {
            PhyId = ((ulong)ClientSetting.Setting.Id << 8) + (ulong)OpenHIoTIdType.Client;
            var factory = new MqttFactory();

            // Create a MQTT client instance
            mqttClient = (MqttClient)factory.CreateMqttClient();

            // Create MQTT client options
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(ClientSetting.Setting.Sever.Ip, ClientSetting.Setting.Sever.MqttPort) // MQTT broker address and port
                                                                                                     //  .WithCredentials(((StartUp.Edge)startup_dev).User, ((StartUp.Edge)startup_dev).Pw) // Set username and password
                .WithClientId(PhyId.ToString())
                .WithCleanSession()
                .Build();

            // Connect to MQTT broker
            var connectResult = await mqttClient.ConnectAsync(options);

            if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
            {
                Console.WriteLine("Connected to MQTT broker successfully.");

                // Subscribe to a topic
                //               await mqttClient.SubscribeAsync(topic);

                // Callback function when a message is received
                mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;
                await PublishBirthMessage();
                // Unsubscribe and disconnect
                //                await mqttClient.UnsubscribeAsync(topic);
                //              await mqttClient.DisconnectAsync();
            }
            else
            {
                Console.WriteLine($"Failed to connect to MQTT broker: {connectResult.ResultCode}");
            }

        }
 
        private Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {

                ulong cid = Convert.ToUInt32(arg.ClientId);
                MqttMsg mqttMsg = new MqttMsg()
                {
                    Topic = new Topic(cid, arg.ApplicationMessage.Topic),
                    Payload = arg.ApplicationMessage.PayloadSegment.ToArray(),
                };

                procQueue.Enqueue(mqttMsg);
                if (!proc_busy)
                    Task.Run(() => { ProcMsgs(); });
            return Task.CompletedTask;
        }
        void ProcMsgs()
        {
            proc_busy = true;
            while (procQueue.Count > 0)
            {
                MqttMsg msg;
                if (procQueue.TryDequeue(out msg))
                    ProcessMessage(msg);
            }
            proc_busy = false;
        }

        public void ProcessMessage(MqttMsg mqttMsg)
        {
            if (mqttMsg == null)
                return;
            if (mqttMsg.Topic.Ns == (int)TopicNamespace.hm1_0)
                ProcessMessageHm(mqttMsg);
            else
                ProcessMessageSp(mqttMsg);
        }





        async Task<MqttClientPublishResult> PublishBirthMessage()
        {
            MqttApplicationMessage dataMessage = new()
            {
                Topic = $"{HmTopic.NameSpace1}/g1/{SparkplugMessageType.NodeBirth.GetDescription()}/{PhyId}",

                Payload = GetHmBirthPayload(),
            };

            // Publish the message.
            return await mqttClient.PublishAsync(dataMessage);
        }

        byte[] GetHmBirthPayload()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);

                HmPayloadBlock.WriteBlock(bw, HmPayloadBlockTypes.HM_BT_DEVICE_BIRTH, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((OpenHIoT.LocalServer.DBase.Models.Device)this)));

        /*        foreach (Device.Channel ch in Channels)
                {
                    ChannelDTO channelDTO = new ChannelDTO(ch.Head);
                    if (ch.Val != null)
                        channelDTO.Val = ValueDataType.ToString(ch.Head.DType, ch.Val);
                    HmPayloadBlock.WriteBlock(bw, HmPayloadBlockTypes.HM_BT_CHANNEL_BIRTH, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(channelDTO)));
                }
        */
                bw.Flush();
                byte[] bs1 = new byte[bw.BaseStream.Position];
                Buffer.BlockCopy(ms.GetBuffer(), 0, bs1, 0, bs1.Length);
                return bs1;
            }
        }


    }
}
