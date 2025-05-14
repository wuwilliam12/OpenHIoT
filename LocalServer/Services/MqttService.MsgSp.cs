
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data.Repository;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;
using OpenHIoT.LocalServer.Operation;
using SparkplugNet.Core;
using SparkplugNet.Core.Enumerations;
using SparkplugNet.Core.Interfaces;
using SparkplugNet.Core.Topics;
using SparkplugNet.VersionB;
using SparkplugNet.VersionB.Data;
using SparkplugNet.VersionB.ProtoBuf;
using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace OpenHIoT.LocalServer.Services
{

    public partial class MqttService 
    {
        void ProcessNodeBirthMsgSp(Topic topic, List<Metric> ms)
        {//string gid, ulong dev_id

            using (var scope = _scopeFactory.CreateScope())
            {
                var rep_dev = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
                Operation.Edge e = (Edge)Operation.Device.CreateDevice(topic,  ms);
                e.EId = e.Id = topic.EId;
                Data.Device? dev_db = rep_dev.UpdateOrCreate(e.DeviceBase).Result;
                if (dev_db != null && (dev_db.Accepted || Setting.Instance.AutoAccept))
                {
                    int status = (int)DeviceStatus.Accepted | (int)DeviceStatus.Connected;
                    e.DeviceBase.Status = rep_dev.SetStatus((ulong)dev_db.GetId(), status).Result;
                    Edges.TryAdd(e.EId, e);
                    mRtService.AddHeads(e.KnownChannels);
                }
                else
                    DisconnectEdge(e.Id);
            }
        }
        void ProcessDeviceBirthMsgSp( Edge edge, Topic topic, List<Metric> ms)
        {
          //  ulong dev_id,
            using (var scope = _scopeFactory.CreateScope())
            {
                var rep_dev = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
                Operation.Device dev = Operation.Device.CreateDevice(topic, ms);
                dev.Id = (ulong)topic.DId;
                Data.Device? dev_db = rep_dev.UpdateOrCreate(dev.DeviceBase).Result;
                if (dev_db != null && (dev_db.Accepted || Setting.Instance.AutoAccept))
                {
                    int status = (int)DeviceStatus.Accepted | (int)DeviceStatus.Connected;
                    dev.DeviceBase.Status = rep_dev.SetStatus((ulong)dev_db.GetId(), status).Result;
                    edge.Children.Add(dev);
                    mRtService.AddHeads(dev.KnownChannels);
                }
            }
        }
        void ProcessNodeDataMsgSp(string gid, Edge edge, List<Metric> ms)
        {
            edge.ProcessSpDataMsg(ms, mRtService);
        }

        void ProcessDeviceDataMsgSp(string gid, Edge edge, ulong dev_id, List<Metric> ms)
        {
            Operation.Device? device = edge.Children.FirstOrDefault(x => x.Id == dev_id);
            if(device != null)
                device.ProcessSpDataMsg(ms, mRtService);
            else 
            {

            }      
        }
        void ProcessNodeDeathMsgSp(string gid, Edge edge)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                edge.Close(edge, scope.ServiceProvider.GetRequiredService<IDeviceRepository>());
            }
        }

        void ProcessDeviceDeathMsgSp(string gid, Edge edge, ulong dev_id)
        {

        }
  
        /// <summary>
        /// Handles the received messages for payload version A.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="payload">The payload.</param>
        /// <exception cref="InvalidOperationException">Thrown if the topic is unknown or the device identifier is invalid.</exception>
        private void HandleMessagesForVersionBSp(Topic topic, Payload payload)
        {
            Operation.Edge? edge;
            if (Edges.TryGetValue(topic.EId, out edge))
            {
                //  long dev_id = string.IsNullOrWhiteSpace(topic.DId) ? 0 : Convert.ToUInt32(topic.DeviceIdentifier);
                // Handle messages.
                switch (topic.MType)
                {
                    case (int)SparkplugMessageType.DeviceBirth:
                        ProcessDeviceBirthMsgSp(edge, topic, payload.Metrics);
                        break;
                    case (int)SparkplugMessageType.NodeData:
                        ProcessNodeDataMsgSp(topic.GId, edge, payload.Metrics);
                        break;
                    case (int)SparkplugMessageType.DeviceData:
                        ProcessDeviceDataMsgSp(topic.GId, edge, (ulong)topic.DId, payload.Metrics);
                        break;
                    case (int)SparkplugMessageType.NodeDeath:
                        ProcessNodeDeathMsgSp(topic.GId, edge);
                        break;
                    case (int)SparkplugMessageType.DeviceDeath:
                        ProcessDeviceDeathMsgSp(topic.GId, edge, (ulong)topic.DId);
                        break;

                }
            }
            else if (topic.MType == (int)SparkplugMessageType.NodeBirth)
            {
                ProcessNodeBirthMsgSp(topic, payload.Metrics);
            }
        }

        void ProcessMessageSp(MqttMsg mqttMsg)
        {
            var payloadVersionB = PayloadHelper.Deserialize<SparkplugNet.VersionB.ProtoBuf.ProtoBufPayload>(mqttMsg.Payload);
            if (payloadVersionB is not null)
            {
                var convertedPayload = PayloadConverter.ConvertVersionBPayload(payloadVersionB);

                if (convertedPayload is not Payload )
                {
                    throw new InvalidCastException("The metric cast didn't work properly.");
                }
                 HandleMessagesForVersionBSp(mqttMsg.Topic, convertedPayload);
            }
        }
    }
}
