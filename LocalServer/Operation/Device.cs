using OpenHIoT.LocalServer.Services;
using OpenHIoT.LocalServer.HiotMsg;
using System.Collections.Generic;
using System.Drawing;
using SparkplugNet.VersionB.Data;
using System.Net.Http.Headers;
using System.Xml.Linq;
using OpenHIoT.LocalServer.Data;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using Newtonsoft.Json;
using System.Text;
using SparkplugNet.Core.Enumerations;
using MQTTnet;
using SparkplugNet.Core.Messages;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data.Repository;
using OpenHIoT.LocalServer.Data.SampleDb;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;

namespace OpenHIoT.LocalServer.Operation
{

    public partial class Device 
    {
        public TopicNamespace Namespace { get; set; }
        public string TopicGroup { get; set; }
        public Data.Device DeviceBase { get; set; }

        protected IChannelChannel mValQueue;
        public ulong Id { get; set; }   
        public ulong EId { get; set; } 
        public KnownChannels KnownChannels { get; set; }
        public Devices Children { get; set; }
        
        public static Device CreateDevice(Topic topic, List<Metric> ms)
        {
            Device new_dev = topic.DId == null ? new Edge(topic, ms ) : new Device(topic, ms);
            new_dev.DeviceBase = new Data.Device();
        //    Metric m = ms.FirstOrDefault(x => x.Alias == Id;
            new_dev.DeviceBase.Init(ms[ms.Count-2]);           
            ms.RemoveAt(ms.Count - 2);
            new_dev.KnownChannels = ms == null ? new KnownChannels(new_dev, new List<Metric>()) :
                                       new KnownChannels(new_dev, ms);
            return new_dev;
        }
        
        public static Device CreateDevice( MqttMsg mqttMsg)
        {
            Device new_dev = mqttMsg.Topic.DId == null ? new Edge(mqttMsg.Topic) : new Device(mqttMsg.Topic);
            new_dev.ReadHmPayloadBirth(mqttMsg.Payload);
            return new_dev;
        }
        protected  void Init(Topic topic)
        {
            Namespace = (TopicNamespace)topic.Ns;
            TopicGroup = topic.GId;
            EId = topic.EId;
        }
        public Device(Topic topic) {
            Init(topic);
            if(topic.DId != null)
                Id = (ulong)topic.DId;
            Children = new Devices();
            KnownChannels = new KnownChannels();
        }
        public Device(Topic topic, List<SparkplugNet.VersionB.Data.Metric> ms) {
            Init(topic);
            if (topic.DId != null)
                Id = (ulong)topic.DId;
            Children = new Devices();
            KnownChannels = new KnownChannels(this, ms);
            messageGenerator = new SparkplugMessageGenerator(SparkplugSpecificationVersion.Version30);
        }

        public Device() { 
            Children = new Devices();
            KnownChannels = new KnownChannels();
        }

        public HeadRt? GetHeadRtByNsId(uint ns_id)
        {
            var v = KnownChannels.Items.FirstOrDefault(x => x.Head != null && x.Head.NsId == ns_id);
            if (v != null) return v.Head;
            foreach(Device device in Children)
            {
                HeadRt? h  = device.GetHeadRtByNsId(ns_id);
                if (h != null) return h;
            }
            return null;
        }

        public HeadRt? GetHeadRtByAlias(ulong alias)
        {
            var v = KnownChannels.Items.FirstOrDefault(x => x.Head != null && x.Head.GetAlias() == alias);
            if (v != null) return v.Head;
            foreach(Device device in Children)
            {
                HeadRt? h  = device.GetHeadRtByAlias(alias);
                if (h != null) return h;
            }
            return null;
        }
        public HeadRt? GetHeadRtByName(string name)
        {
            var v = KnownChannels.Items.FirstOrDefault(x => x.Head != null && x.Head.Name == name);
            if (v != null) return v.Head;
            return null;
        }
        public HeadRt? GetHeadRtByIId(byte iid)
        {
            var v = KnownChannels.Items.FirstOrDefault(x => x.Head != null && x.Head.IId == iid);
            if (v != null) return v.Head;
            return null;
        }

        public virtual void Close(Edge edge, IDeviceRepository deviceRepository)
        {
            foreach (var dev in Children)
                dev.Close(edge, deviceRepository);
            Children.Clear();
            deviceRepository.ClearStatus(Id, (int)DeviceStatus.Connected);

            if (DeviceBase.Parent != null)
            {
                Device? d = edge.GetDevice((ulong)DeviceBase.Parent);
                if (d != null) 
                    d.Children.Remove(this);
            }
            else
            {
                Edge? e;
                IMqttService.Instance.Edges.TryRemove(edge.Id, out e);
            }
        }

        public Device? GetDeviceOfChannel(ulong ch_alias)
        {
            if (KnownChannels.Items.FirstOrDefault(x => x.Head.GetAlias() == ch_alias) != null)
                return this;
            foreach (Device dev in Children)
            {
                Device? dev1 = dev.GetDeviceOfChannel(ch_alias);
                if (dev1 != null) return dev1;
            }
            return null;
        }

        public Device? GetDevice(ulong id)
        {
            if (Id == id) return this;
            foreach(Device dev in Children)
            {
                Device? dev1 = dev.GetDevice(id);
                if (dev1 != null) return dev1;
            }
            return null;
        }

        public List<HeadRt> GetHeadRts()
        {
            List<HeadRt> hs = new List<HeadRt>();
            List<HeadRt> hs1 = KnownChannels.Items.Where(predicate: x => x.Head != null).Select(x => x.Head).ToList();
            hs.AddRange(hs1);

            foreach (Device dev in Children)
                hs.AddRange(dev.GetHeadRts());
            return hs;
        }

        public Topic GetCommandTopic()
        {
            return new Topic()
            {
                Ns = (int)Namespace,
                MType = this is Edge ? (int)SparkplugMessageType.NodeCommand : (int)SparkplugMessageType.DeviceCommand,
                EId = EId,
                DId = Id,
                GId = TopicGroup
            };
        }

        public MqttApplicationMessage? GetCommandMqttApplicationMessage(HeadRt h, object v)
        {
            switch (Namespace)
            {
                case TopicNamespace.hm1_0:
                    return new MqttApplicationMessage()
                    {
                        Topic = GetCommandTopic().ToString(),
                        PayloadSegment = Channel.GetHmValTsPlayloadBytesIId(h, v)
                    };
                case TopicNamespace.spBv1_0:                 
                    return GetCommandMessage( new List<Metric> {
                        new Metric(ValueDataType.GetDataType(h.DType), v)
                        {
                            Alias = h.GetAlias(),
                        }});
                    
                default:
                    return null;
            }

        }
    }

    public class Devices : List<Device>
    {

    }
}
