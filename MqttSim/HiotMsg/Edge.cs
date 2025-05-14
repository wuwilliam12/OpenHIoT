using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.HiotMsg;
using OpenHIoT.LocalServer.Operation;
using SparkplugNet.Core;
using SparkplugNet.Core.Application;
using SparkplugNet.Core.Enumerations;
using SparkplugNet.Core.Extensions;
using SparkplugNet.Core.Node;
using SparkplugNet.Core.Topics;
using SparkplugNet.VersionB;
using SparkplugNet.VersionB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VersionBProtoBuf = SparkplugNet.VersionB.ProtoBuf;

namespace OpenHIoT.MqttSim.HiotMsg
{
    public class Edge : Device, IEdge, IDeviceHm
    {
        protected MqttClient mqttClient;

//        public string User { get; set; }
//        public string Password { get; set; }

        public Edge()
        {

        }

        public static string GetTopic(SparkplugMessageType mt, ulong dev_id)
        {
            return $"{TopicNamespace.hm1_0}/g1/{mt.GetDescription()}/{dev_id}";
        }
        public async void ConnectTo(string broker, int port)
        {
            //   string broker = "broker.emqx.io";
            //   int port = 1883;
            string clientId = GetId().ToString();
            //         string topic = "csharp/mqtt";
            //         string username = "emqx";
            //        string password = "public";

            // Create a MQTT client factory
            var factory = new MqttFactory();

            // Create a MQTT client instance
            mqttClient = (MqttClient)factory.CreateMqttClient();
         
            // Create MQTT client options
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(broker, port) // MQTT broker address and port
                .WithCredentials(   ((StartUp.Edge)startup_dev).User, ((StartUp.Edge)startup_dev).Pw) // Set username and password
                .WithClientId(clientId)
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
                await mqttClient.SubscribeAsync(GetTopic(SparkplugMessageType.NodeCommand, (ulong)Id));
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
            HmPayload.ReadPayloadData(this, arg.ApplicationMessage.PayloadSegment.Array);

            //    Console.WriteLine($"Received message: {Encoding.UTF8.GetString()}");
            return Task.CompletedTask;
           
        }

        public void AfterProcessHmBlockData()
        {
            //throw new NotImplementedException();
        }

        public void ProcessHmBlockData(HmPayloadBlock block)
        {
            Sample? sample = null;
            HeadRt h;
            switch (block.Type)
            {
                case HmPayloadBlockTypes.HM_BT_CHANNEL_DATA_IID:
                case HmPayloadBlockTypes.HM_BT_CHANNEL_DATA_TS_IID:
                    HmBlockChannelValueByIID b1 = (HmBlockChannelValueByIID)block;
                    Channel ch = chs.FirstOrDefault(x => x.DTO.IId == b1.IId);
                    if (ch != null)
                    {
                        h = new HeadRt();
                        h.CopyFrom(ch.DTO);
                        sample = OpenHIoT.LocalServer.Operation.Channel.ReadDataBlockHm(b1, h);
                        Console.WriteLine($"{Name} {h.Name}: { sample.GetVal().ToString()} ");
                    }
                    break;

                case HmPayloadBlockTypes.HM_BT_CHANNEL_DATA_NAME:
                    break;
            }

            //throw new NotImplementedException();
        }

        public async Task<MqttClientPublishResult> PublishDataMessage()
        {
            MqttApplicationMessage dataMessage = new()
            {
                Topic = $"{HmTopic.NameSpace1}/g1/{SparkplugMessageType.NodeData.GetDescription()}/{Id}",
                PayloadSegment = chs.GetValPlayloadBytes()
            };

            // Publish the message.
            return await mqttClient.PublishAsync(dataMessage);
        }


        public async Task<MqttClientPublishResult> PublishBirthMessage()
        {
            MqttApplicationMessage dataMessage = new()
            {
                Topic = $"{HmTopic.NameSpace1}/g1/{SparkplugMessageType.NodeBirth.GetDescription()}/{Id}",
                PayloadSegment = GetHmBirthPayload(),
            };

            // Publish the message.
            return await mqttClient.PublishAsync(dataMessage);
        }


    }
}
