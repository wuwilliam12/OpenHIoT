
using MQTTnet.Client;
using SparkplugNet.Core.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHIoT.BleEdge.MQTT
{
    public delegate void MqttMsgReceivedEventHandler(string topic, byte[] payload);
    public interface IMqttClient
    {
        public static IMqttClient Instance { get; set; }
        event MqttMsgReceivedEventHandler MqttMsgRecieved;
        bool Start();

        Product.Devices Devices { get; set; }
        Product.Channels Channels { get; set; }

        bool Subscribe(string topic);
        //    string GetTopic(SparkplugMessageType mt, ulong dev_id);
        Task<MqttClientPublishResult> PublishDeviceDataMessage(ulong dev_id, Product.Channels chs);
        Task<MqttClientPublishResult> PublishDeviceDeathMessage(ulong dev_id);

        Task<MqttClientPublishResult> PublishNewDevicesMessage(List<BLE.Device> new_devs);

        Task AddNewDevice(Product.Device device);
        Task RemoveDevice(Product.Device device);

    }
}
