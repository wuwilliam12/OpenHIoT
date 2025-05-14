// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable EmptyConstructor
// ReSharper disable MemberCanBeMadeStatic.Local

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
//using MQTTnet.AspNetCore;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using MQTTnet.Server;


using MQTTnet;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using OpenHIoT.LocalServer.Operation;
using OpenHIoT.LocalServer.Data;
using SparkplugNet.Core.Topics;
using Microsoft.VisualBasic;
using SparkplugNet.Core.Enumerations;
using SparkplugNet.VersionB.Data;
using System.Collections.Concurrent;
using SparkplugNet.Core.Interfaces;

using SparkplugNet.Core;
using SparkplugNet.VersionB;
using System.Xml.Linq;
using OpenHIoT.LocalServer.HiotMsg;
using SparkplugNet.VersionA.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data.Repository;
using OpenHIoT.LocalServer.Data.SampleDb;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;
//using static SparkplugNet.Core.SparkplugBase<T>;


namespace OpenHIoT.LocalServer.Services
{

    public interface IMqttService
    {
        public static IMqttService Instance { get; set; }
  //      Device? NewDevice { get; }
        MqttServer MqttServer { get; set; }
        List<HeadRt> GetHeadRts();
        List<HeadRt>? GetHeadRtsOfDevice(ulong dev_id);
        HeadRt? GetHeadRtByNs(uint ns_id);
        HeadRt? GetByAlias(ulong id);
        HeadRt? GetHeadRtByName(ulong dev_id, string name);
        HeadRt? GetHeadRtByIId(ulong dev_id, byte iid);
        ulong? GetEId(ulong dev_id);

 //       void DisconnectEdge(ulong eid);
        IServiceScopeFactory GetScopeFactory();
        Task<bool> WriteChannel( ulong dev_id, byte[] bs);
        Task<bool> SetSampleVal(SampleDTO sample);
        ConcurrentDictionary<ulong, Operation.Edge> Edges { get; }
        //       void WriteChannelVal(ulong dev_id, byte ch_id, byte[] val);
    }
    public partial class MqttService : IMqttService
    {
        Queue<MqttMsg> procQueue; bool proc_busy;
        public static int GetMqttPort(string? port)
        {
            if (port == null) return 1884;
            return Convert.ToInt32(port);
        }

        private readonly IServiceScopeFactory _scopeFactory;
        IMsgSevice? msgQueue;
        IChannelChannel mRtService;

        public MqttServer MqttServer { get; set; }
        public MqttService(IServiceScopeFactory scopeFactory)
        {
            IMqttService.Instance = this;
            procQueue = new Queue<MqttMsg>(); proc_busy = false;
            _scopeFactory = scopeFactory;
            using (var scope = _scopeFactory.CreateScope())
            {
                msgQueue = scope.ServiceProvider.GetRequiredService<IMsgSevice>();
                mRtService = scope.ServiceProvider.GetRequiredService<IChannelChannel>();

            }
        }

  //      public ConcurrentDictionary<uint, Operation.Device> Devices { get; } = new();
        public ConcurrentDictionary<ulong, Operation.Edge> Edges { get; } = new();

        public IServiceScopeFactory GetScopeFactory()
        {
            return _scopeFactory;
        }
        public ulong? GetEId(ulong dev_id)
        {
            foreach (var edge in Edges.Values)
            {
                Operation.Device? dev = edge.GetDevice(dev_id);
                if (dev != null) return edge.Id;
            }
            return null;
        }
        public List<HeadRt>? GetHeadRtsOfDevice(ulong dev_id)
        {
            Operation.Device? dev = GetActiveDevice(dev_id);
            if (dev == null) return null;
            return dev.KnownChannels.Items.Where(x => x.Head != null ).Select(x => x.Head).ToList();
        }
        public List<HeadRt> GetHeadRts()
        {
            List<HeadRt> hs = new List<HeadRt>();
            foreach (Operation.Edge edge in Edges.Values)
                hs.AddRange(edge.GetHeadRts());
            return hs;
        }


        public HeadRt? GetHeadRtByNs(uint ns_id)
        {
            foreach (Edge edge in Edges.Values)
            {
                var h1 = edge.GetHeadRtByNsId(ns_id);
                if (h1 != null) return h1;
            }
            return null;
        }
        public HeadRt? GetByAlias(ulong alias)
        {
            foreach (Edge edge in Edges.Values)
            {
                var h1 = edge.GetHeadRtByAlias(alias);
                if (h1 != null) return h1;
            }
            return null;
        }


        public HeadRt? GetHeadRtByName(ulong dev_id, string name)
        {
            Operation.Device? dev = GetActiveDevice(dev_id);
            if (dev == null) return null;
            return dev.GetHeadRtByName(name);
        }

        public HeadRt? GetHeadRtByIId(ulong dev_id, byte iid)
        {
            Operation.Device? dev = GetActiveDevice(dev_id);
            if (dev == null) return null;
            return dev.GetHeadRtByIId(iid);
        }

        #region Publishs

        /*
        public void PublishProductID( int pid)
        {
            var message = new MqttApplicationMessageBuilder()
            .WithTopic(TopicType.PID.ToString() + "/0" )
            .WithPayload(BitConverter.GetBytes(pid))
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
            .WithRetainFlag()
            .Build();
            InjectedMqttApplicationMessage msg = new InjectedMqttApplicationMessage(message);
            MqttServer.InjectApplicationMessage(msg);
        }       
        
        public void PublishDeviceID(Device? dev)
        {
            if (dev == null) return;
            var message = new MqttApplicationMessageBuilder()
            .WithTopic(TopicType.Device.ToString() + "/" + dev.PID)
            .WithPayload(BitConverter.GetBytes(dev.ID))
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
            .WithRetainFlag()
            .Build();
            InjectedMqttApplicationMessage msg = new InjectedMqttApplicationMessage(message);
            MqttServer.InjectApplicationMessage(msg);
        }

        public void PublishNewDeviceModule(DeviceModule dm)
        {
          //  DataWriter w = new DataWriter(8 + di.MIDs.Length);
          //  w.WriteData(di.ID); w.WriteData(di.LID); w.WriteData(di.MIDs);
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(TopicType.DeviceModule + "/" + dm.PID)
                .WithPayload(dm.GetBytes())
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                .WithRetainFlag()
                .Build();
            InjectedMqttApplicationMessage msg = new InjectedMqttApplicationMessage(message);
            MqttServer.InjectApplicationMessage(msg);
        }
        public void PublishSetting(SettingItem s)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(TopicType.Setting + "/" + s.DiId + "/" + s.ID  )
                .WithPayload(s.Val)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                .WithRetainFlag()
                .Build();
            InjectedMqttApplicationMessage msg = new InjectedMqttApplicationMessage(message);
            MqttServer.InjectApplicationMessage(msg);
        }
        public void PublishMIDs(int di_id, byte[] mids)
        {
            var message = new MqttApplicationMessageBuilder()
            .WithTopic(TopicType.MIDs + "/" + di_id)
            .WithPayload(mids)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
            .WithRetainFlag()
            .Build();
            InjectedMqttApplicationMessage msg = new InjectedMqttApplicationMessage(message);
            MqttServer.InjectApplicationMessage(msg);
        }

        public void PublishDeleteDevice(int di_id, int pid)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(TopicType.DeleteDevice + "/" + di_id)
                .WithPayload(BitConverter.GetBytes(pid))
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                .WithRetainFlag()
                .Build();
            InjectedMqttApplicationMessage msg = new InjectedMqttApplicationMessage(message);
            MqttServer.InjectApplicationMessage(msg);
        }

        public void PublishDeleteDeviceModule(int pid, int di_id)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(TopicType.DeleteDeviceModule + "/" + pid)
                .WithPayload(BitConverter.GetBytes(di_id))
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                .WithRetainFlag()
                .Build();
            InjectedMqttApplicationMessage msg = new InjectedMqttApplicationMessage(message);
            MqttServer.InjectApplicationMessage(msg);
        }*/
        #endregion


        public Task OnClientConnected(ClientConnectedEventArgs eventArgs)
        {
            ulong cid = Convert.ToUInt64(eventArgs.ClientId);
            return Task.CompletedTask;
        }

        
        public Task OnClientDisConnected(ClientDisconnectedEventArgs eventArgs)
        {
            ulong cid = Convert.ToUInt64(eventArgs.ClientId);
         /*   if (Edges.ContainsKey(cid))
            {
                Edge e = Edges[cid];
                e.Disconnect();
                Edges.TryRemove(cid, out e);
            }
         */
            return Task.CompletedTask;
        }

        public void DisconnectEdge(ulong eid)
        {
            MqttServer.DisconnectClientAsync(eid.ToString(),  MqttDisconnectReasonCode.NotAuthorized);
        }

        public Task ValidateConnection(ValidatingConnectionEventArgs eventArgs)
        {
            ulong cid = Convert.ToUInt64(eventArgs.ClientId);
       //     Auth? a = GetClientAuth(cid);
            return Task.CompletedTask;
            //new device
            /*     if (newDevice != null && newDevice.Id == cid )
                 {
                     // auth already match, not new anymore
                     if (a.UName == eventArgs.UserName && a.Pw == eventArgs.Password)
                         newDevice = null;
                     eventArgs.ReasonCode = MqttConnectReasonCode.Success;
                 }
                 else */
            {
       //         eventArgs.ReasonCode = 
       //             a != null && a.UName == eventArgs.UserName && a.Pw == eventArgs.Password ?
       //             MqttConnectReasonCode.Success : MqttConnectReasonCode.NotAuthorized;                 
            }
            return Task.CompletedTask;
        }
        /*
        DBase.Models.Device? GetDevice(ulong id)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var rep = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
                return rep.Get(id).Result;
            }
        }
        */

        Operation.Device GetActiveDevice(ulong id)
        {
            foreach (Edge e in Edges.Values)
            {
                Operation.Device? dev = e.GetDevice(id);
                if (dev != null) return dev;
            }
            return null;
        }

        Operation.Device GetDeviceOfChannel(ulong ch_alias)
        {
            foreach (Edge e in Edges.Values)
            {
                Operation.Device? dev = e.GetDeviceOfChannel(ch_alias);
                if (dev != null) return dev;
            }
            return null;
        }


        Auth? GetClientAuth(ulong id)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var rep = scope.ServiceProvider.GetRequiredService<IClientAuthRepository>();
                return rep.Get(id).Result;
            }
        }
      
        public Task InterceptingSubscriptionAsync(InterceptingSubscriptionEventArgs arg)
        {
            ulong cid = Convert.ToUInt64(arg.ClientId);
            SparkplugMessageTopic? topic;
            if (SparkplugMessageTopic.TryParse(arg.TopicFilter.Topic, out topic))
            {
                switch (topic.MessageType)
                {
                    case SparkplugMessageType.NodeCommand:
                        break;
                    case SparkplugMessageType.DeviceCommand:
                        //         PublishHID(GetDevice(cid));
                        break;


                    default:
                        break;

                }
            }
            else
            {
         //       if (topic.Contains(SparkplugMessageType.StateMessage.GetDescription()))
                {

                }
            }
            return Task.CompletedTask;
        }

        void ProcMsgs()
        {
            proc_busy = true;
            while (procQueue.Count > 0)
            {
                MqttMsg msg;
                if(procQueue.TryDequeue(out msg))
                    ProcessMessage(msg);
            }
            proc_busy = false;
        }

        public async Task InterceptingInboundPacketAsync(InterceptingPacketEventArgs arg)
        {
            if (arg.Packet is MqttPublishPacket)
            {
                ulong cid = Convert.ToUInt64(arg.ClientId);
                MqttPublishPacket pp = (MqttPublishPacket)arg.Packet;

                MqttMsg mqttMsg = new MqttMsg()
                {
                    Topic = new Topic(cid, pp.Topic),
                    Payload = pp.PayloadSegment.ToArray(),
                };

                procQueue.Enqueue(mqttMsg);
                if (!proc_busy)
                    Task.Run(() => { ProcMsgs(); });

                await msgQueue.AddMsg(mqttMsg);

            };
        }

        public void ProcessMessage(MqttMsg mqttMsg)
        {
            if (mqttMsg == null)
                return;
            if (mqttMsg.Topic.Ns == (int)TopicNamespace.hm1_0 )
               ProcessMessageHm(mqttMsg);
            else
               ProcessMessageSp( mqttMsg);  
        }

        public async Task<bool> WriteChannel(ulong dev_id, byte[] bs)
        {
            Operation.Device? dev = GetActiveDevice(dev_id);
            if (dev == null) return false;

            MqttApplicationMessage message = new MqttApplicationMessage()
            {
                PayloadSegment = bs,
                Topic = dev.GetCommandTopic().ToString(),
            };
            await MqttServer.InjectApplicationMessage(new InjectedMqttApplicationMessage(message));
            return true;

        }

        public async Task<bool> SetSampleVal(SampleDTO sample)
        {
            Operation.Device? dev = GetDeviceOfChannel(sample.Alias);
            if(dev == null) return false;
            HeadRt? h = dev.GetHeadRtByAlias(sample.Alias);
            if(h == null) return  false;
            object? v = ValueDataType.GetValFromSample(h.DType, sample);
            if(v == null) return false;
            MqttApplicationMessage? message = dev.GetCommandMqttApplicationMessage( h, v);

            if (message == null) return false; 
            await MqttServer.InjectApplicationMessage(new InjectedMqttApplicationMessage(message));
            return true;
        }
    }
}