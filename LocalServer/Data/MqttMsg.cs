using OpenHIoT.LocalServer.HiotMsg;
using SparkplugNet.Core;
using SparkplugNet.Core.Enumerations;
using SparkplugNet.Core.Extensions;
using SparkplugNet.Core.Topics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
//using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpenHIoT.LocalServer.Data
{
    public enum TopicNamespace { spAv1_0 = 1, spBv1_0 = 2, hm1_0 = 3 };
    public class Topic
    {
        public uint Id { get; set; }
        public int Ns {  get; set; } //Namespace
        public long TimeStamp { get; set; }
        //        
        public int MType { get; set; }
        public string GId { get; set; }        //Group Id 
        public ulong EId { get; set; }      //Edge Id
        public ulong? DId { get; set; }       //Dev Id

        public Topic()
        {

        }

        public Topic(ulong cid, string topic_str)
        {
            if (topic_str[0] == 's')
            {
                SparkplugMessageTopic topic = SparkplugMessageTopic.Parse(topic_str);
                GId = topic.GroupIdentifier;
                MType = (int)topic.MessageType;
                Ns = topic.Namespace == SparkplugNamespace.VersionB ? (int)TopicNamespace.spBv1_0 : (int)TopicNamespace.spAv1_0;
                if (topic.DeviceIdentifier != null)
                    DId = Convert.ToUInt64(topic.DeviceIdentifier); 
           }
           else
                HmTopic.Parse(topic_str, this);
           EId = cid;
           TimeStamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public override string ToString()
        {

            if (EId == DId || DId == null)
                return $"{(TopicNamespace)Ns}/{GId}/{((SparkplugMessageType)MType).GetDescription()}/{EId}";
            else
                return $"{(TopicNamespace)Ns}/{GId}/{(SparkplugMessageType)MType}/{EId}/{DId}";
        }

    }

    public class MqttMsg()
    {
        public Topic Topic { get; set; }
        public byte[] Payload { get; set; }

    }
}