
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data.Repository;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;
//using OpenHIoT.LocalServer.Operation;
using OpenHIoT.LocalServer.HiotMsg;
using OpenHIoT.LocalServer.Operation;
using SparkplugNet.Core;
using SparkplugNet.Core.Enumerations;
using SparkplugNet.Core.Interfaces;
using SparkplugNet.Core.Topics;
using SparkplugNet.VersionB.Data;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Device = OpenHIoT.LocalServer.Operation.Device;

namespace OpenHIoT.LocalServer.Services
{
    public partial class MqttService 
    {
        void ProcessNodeBirthHm(MqttMsg mqttMsg)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                //get device in local server
                var rep_dev = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
                Operation.Edge e = (Edge)Operation.Device.CreateDevice( mqttMsg);
                Data.Device? dev_db = rep_dev.UpdateOrCreate(e.DeviceBase).Result;
                if (dev_db != null && (dev_db.Accepted || Setting.Instance.AutoAccept))
                {
                    int status = (int)DeviceStatus.Accepted | (int)DeviceStatus.Connected;
                    e.DeviceBase.Status = rep_dev.SetStatus((ulong)dev_db.GetId(), status).Result;
                    Edges.TryAdd(mqttMsg.Topic.EId, e);
                    mRtService.AddHeads(e.KnownChannels);
                }
                else
                    DisconnectEdge(e.Id);

            }
        }
        void ProcessDeviceBirthHm(Edge edge, MqttMsg mqttMsg)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var rep_dev = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
                Operation.Device dev = Operation.Device.CreateDevice(mqttMsg);
                Data.Device? dev_db = rep_dev.UpdateOrCreate(dev.DeviceBase).Result;
                if (dev_db != null && (dev_db.Accepted || Setting.Instance.AutoAccept))
                {
                    int status = (int)DeviceStatus.Accepted | (int)DeviceStatus.Connected;
                    dev.DeviceBase.Status = rep_dev.SetStatus((ulong)dev_db.GetId(), status).Result;
                    if (dev.DeviceBase.Parent != null)
                    {
                        Device? parent = edge.GetDevice((ulong)dev.DeviceBase.Parent);
                        if (parent != null)
                            parent.Children.Add(dev);
                    }
  //                  edge.Children.Add(dev);
                    mRtService.AddHeads(dev.KnownChannels);
                }
            }
        }
        void ProcessNodeDataHm(Edge edge, MqttMsg mqttMsg)
        {
            edge.ProcessPayloadDataHm( mqttMsg.Payload, mRtService);           
        }

        void ProcessDeviceDataHm(Edge edge, MqttMsg mqttMsg)
        {
            Operation.Device? device = edge.Children.FirstOrDefault(x=> x.Id == (ulong)mqttMsg.Topic.DId);

            if (device != null)
                device.ProcessPayloadDataHm( mqttMsg.Payload, mRtService);
            else 
            {

            }      
        }
        void ProcessNodeDeathHm( Topic topic)
        {
             Operation.Edge? edge;
            if (Edges.TryGetValue(topic.EId, out edge))
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    edge.Close(edge, scope.ServiceProvider.GetRequiredService<IDeviceRepository>());
                }
            }
        }

        void ProcessDeviceDeathHm(Topic topic)
        {
            Operation.Edge? edge;
            if (Edges.TryGetValue(topic.EId, out edge) && topic.DId != null)
            {
                Operation.Device? dev = edge.GetDevice((ulong)topic.DId);
                if (dev != null)
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        dev.Close(edge, scope.ServiceProvider.GetRequiredService<IDeviceRepository>());
                    }
                }
            }
        }

        void ProcessMessageHm(MqttMsg mqttMsg)
        {
            Operation.Edge? edge;
            if (Edges.TryGetValue(mqttMsg.Topic.EId, out edge))
            {
                switch ((SparkplugMessageType)mqttMsg.Topic.MType)
                {
                    case SparkplugMessageType.DeviceBirth:
                        ProcessDeviceBirthHm(edge, mqttMsg);
                        break;
                    case SparkplugMessageType.NodeData:
                        ProcessNodeDataHm(edge, mqttMsg);
                        break;
                    case SparkplugMessageType.DeviceData:
                        ProcessDeviceDataHm(edge, mqttMsg);
                        break;
                    case SparkplugMessageType.NodeDeath:
                        ProcessNodeDeathHm(mqttMsg.Topic);
                        break;
                    case SparkplugMessageType.DeviceDeath:
                        ProcessDeviceDeathHm(mqttMsg.Topic);
                        break;
                }
            }
            else
            {
                if (mqttMsg.Topic.MType == (int)SparkplugMessageType.NodeBirth)
                    ProcessNodeBirthHm( mqttMsg);
            }
        }

    }
}
