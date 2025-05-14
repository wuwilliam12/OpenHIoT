//using EnumsNET;
using OpenHIoT.BleEdge.Product;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using Newtonsoft.Json;
using OpenHIoT.LocalServer.HiotMsg;
using SparkplugNet.Core.Enumerations;
using SparkplugNet.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenHIoT.LocalServer.Operation.Channel;
using MqttClient = MQTTnet.Client.MqttClient;

namespace OpenHIoT.BleEdge.MQTT.Sparkplug
{
    public  class SparkplugPort : IMqttClient
    {
        ulong id;
        protected MqttClient mqttClient;
        public event MqttMsgReceivedEventHandler MqttMsgRecieved;
        public Devices Devices { get; set; }
        public Channels Channels { get; set; }
        public bool Start()
        {
            return true;
        }

        public async Task<MqttClientPublishResult> PublishDeviceDataMessage(ulong dev_id, Product.Channels chs)
        {
            MqttApplicationMessage dataMessage = new()
            {
                Topic = $"{HmTopic.NameSpace1}/g1/{SparkplugMessageType.DeviceData.GetDescription()}/{dev_id}",
               // Payload = ch.GetValPlayloadBytes()
            };

            // Publish the message.
            return await mqttClient.PublishAsync(dataMessage);
        }
        public async Task<MqttClientPublishResult> PublishNewDevicesMessage(List<BLE.Device> new_devs)
        {
            
            MqttApplicationMessage dataMessage = new()
            {
                Topic = $"{HmTopic.NameSpace1}/g1/{SparkplugMessageType.NodeData.GetDescription()}/{id}",
             //   Payload = Channels[0].GetHmValPlayloadBytesIId()
            };
            return await mqttClient.PublishAsync(dataMessage);
        }
        public bool Subscribe(string topic)
        {
            return true;
        }

        public Task AddNewDevice(Product.Device device)
        {
            throw new NotImplementedException();
        }

        public Task RemoveDevice(Device device)
        {
            throw new NotImplementedException();
        }

        public Task<MqttClientPublishResult> PublishDeviceDeathMessage(ulong dev_id)
        {
            throw new NotImplementedException();
        }
    }
}
