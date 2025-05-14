using MQTTnet;
using OpenHIoT.LocalServer.Services;
using SparkplugNet.Core.Enumerations;
using SparkplugNet.Core.Messages;
using SparkplugNet.VersionB.Data;

namespace OpenHIoT.LocalServer.Operation
{
    public partial class Device
    {
        readonly SparkplugMessageGenerator messageGenerator;
        protected int LastSequenceNumber;
        protected long LastSessionNumber;
        public virtual void ProcessSpDataMsg(List<SparkplugNet.VersionB.Data.Metric> ms, IChannelChannel mValQueue)
        {
            foreach (var m in ms)
            {
                if (m.Alias is not null)
                {
                    Channel? m1;
                    if (KnownChannels.ItemsByAlias.TryGetValue((ulong)m.Alias, out m1))
                    {
                        if (m1.Head != null)
                            mValQueue.AddSample(m1.GetSample(m));
                    }
                }
                else
                {
                    if (m.Name is not null)
                    {
                        Channel? m1;
                        if (KnownChannels.ItemsByName.TryGetValue((string)m.Name, out m1))
                        {
                            if (m1.Head != null)
                                mValQueue.AddSample(m1.GetSample(m));
                        }
                    }
                }
            }
            mValQueue.SaveSamples();
        }

        public  MqttApplicationMessage GetCommandMessage(List<Metric> ms)
        {
            SparkplugNamespace ns = Namespace == Data.TopicNamespace.spBv1_0 ? SparkplugNamespace.VersionB : SparkplugNamespace.VersionA;
            if (this is Edge)
                return messageGenerator.GetSparkplugNodeCommandMessage(
                      ns,
                      TopicGroup,
                      Id.ToString(),
                      ms,
                      LastSequenceNumber,
                      LastSessionNumber,
                      DateTimeOffset.UtcNow);
            return messageGenerator.GetSparkplugDeviceCommandMessage(
              ns,
              TopicGroup,
              EId.ToString(),
              Id.ToString(),
              ms,
              LastSequenceNumber,
              LastSessionNumber,
              DateTimeOffset.UtcNow);
        }
        public virtual MqttApplicationMessage GetDataMessage(List<Metric> ms)
        {
            SparkplugNamespace ns = Namespace == Data.TopicNamespace.spBv1_0 ? SparkplugNamespace.VersionB : SparkplugNamespace.VersionA;
            if (this is Edge)
                return messageGenerator.GetSparkplugNodeDataMessage(
                      ns,
                      TopicGroup,
                      Id.ToString(),
                      ms,
                      LastSequenceNumber,
                      LastSessionNumber,
                      DateTimeOffset.UtcNow);
            return messageGenerator.GetSparkplugDeviceDataMessage(
              ns,
              TopicGroup,
              EId.ToString(),
              Id.ToString(),
              ms,
              LastSequenceNumber,
              LastSessionNumber,
              DateTimeOffset.UtcNow);
        }
    }
}
