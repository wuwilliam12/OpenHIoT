using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet.Server;
using OpenHIoT.LocalServer.Data.SampleDb;
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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VersionBProtoBuf = SparkplugNet.VersionB.ProtoBuf;

namespace OpenHIoT.MqttSim.Sparkplug
{
    public class Device : SparkplugNodeBase<Metric>, IDevice
    {
        ulong? id;
        public string Name {  get; set; }

        public ulong? Id
        {
            get
            {
         //       if (id == null) id = GetId();
                return id;
            }
        }

        public double? SA
        {
            get { return sa; }
            set
            {
                sa = value;
                if (sa == null)
                    acq_timer.Interval = 1000;
                else
                    acq_timer.Interval = (double)sa;
            }
        }
        double? sa;
        protected System.Timers.Timer acq_timer;
        protected Channels chs;

        protected StartUp.Device startup_dev;
   //     protected PropertySet properties;

        /// <inheritdoc cref="SparkplugNodeBase{T}"/>
        /// <summary>
        /// Initializes a new instance of the <see cref="SparkplugNode"/> class.
        /// </summary>
        /// <param name="knownMetricsStorage">The metric names.</param>
        /// <param name="specificationVersion">The Sparkplug specification version.</param>
        /// <seealso cref="SparkplugNodeBase{T}"/>
        public Device()
            : base(null, SparkplugNet.Core.Enumerations.SparkplugSpecificationVersion.Version30)
        {
            acq_timer = new System.Timers.Timer();
            acq_timer.Elapsed += Acq_timer_Elapsed;
       //     properties.Keys.Add()
        }
        public void  Init(StartUp.Device su_dev)
        {
            startup_dev = su_dev;
            
            id = ( (ulong)su_dev.SimId << 8 ) + (ulong)OpenHIoTIdType.Simulator;
            chs = new Channels();
            foreach(ChannelDTO dto in startup_dev.Channels)
            {
                Channel new_ch = new Channel()
                {
                    DTO = dto,
                    DType = dto.DType == null? DataType.Float : (DataType)dto.DType,
                    Device = this
                };
                if (dto.Val != null)
                    new_ch.Val = ValueDataType.Parse(new_ch.DType, dto.Val);
                chs.Add(new_ch);
            }
            List<Metric> ms = startup_dev.GetChannelMetrics((ulong)id);
            ms.Add( new Metric(DataType.UInt64, id)
            {
                Alias = id,
                Properties = new PropertySet()
                {
                    Keys = { "Name", "PhyId" },
                    Values = { new PropertyValue(DataType.String, $"{Name}"), new PropertyValue(DataType.String, $"{id}") },
                }
            });
            this.knownMetrics = new KnownMetricStorage(ms );
        }

        private void Acq_timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            AcquireData();
        }

        /// <summary>
        /// Publishes version B metrics for a node.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
        /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
        ///     
        protected override Task<MqttClientPublishResult> PublishMessage(IEnumerable<Metric> metrics)
        {
            return PublishDataMessage();
        }
        public virtual async Task<MqttClientPublishResult> PublishDataMessage()
        {
            if (Options is null)
            {
                throw new ArgumentNullException(nameof(Options), "The options aren't set properly.");
            }

            if (KnownMetrics is null)
            {
                throw new ArgumentNullException(nameof(KnownMetrics), "The KnownMetrics aren't set properly.");
            }

            // Get the data message.
            var dataMessage = messageGenerator.GetSparkplugDeviceDataMessage(
                NameSpace,
                Options.GroupIdentifier,
                Options.EdgeNodeIdentifier,
                Id.ToString(),
                chs.DataMetrics,
                LastSequenceNumber,
                LastSessionNumber,
                DateTimeOffset.UtcNow);

            // Increment the sequence number.
            IncrementLastSequenceNumber();

            // Publish the message.
            return await client.PublishAsync(dataMessage);
        }

        /// <summary>
        /// Called when a node message was received.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="payload">The payload.</param>
        /// <exception cref="InvalidCastException">Thrown if the metric cast didn't work properly.</exception>
        protected override async Task OnMessageReceived(SparkplugMessageTopic topic, byte[] payload)
        {
            var payloadVersionB = PayloadHelper.Deserialize<VersionBProtoBuf.ProtoBufPayload>(payload);

            if (payloadVersionB is not null)
            {
                var convertedPayload = PayloadConverter.ConvertVersionBPayload(payloadVersionB);

                if (convertedPayload is not Payload _)
                {
                    throw new InvalidCastException("The metric cast didn't work properly.");
                }

                await HandleMessagesForVersionB(topic, convertedPayload);
            }
        }

        /// <summary>
        /// Handles the received messages for payload version B.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="payload">The payload.</param>
        /// <exception cref="InvalidOperationException">Thrown if the topic is invalid.</exception>
        private async Task HandleMessagesForVersionB(SparkplugMessageTopic topic, Payload payload)
        {
            // Filter out session number metric.
            var sessionNumberMetric = payload.Metrics.FirstOrDefault(m => m.Name == SparkplugNet.Core.Constants.SessionNumberMetricName);
            var metricsWithoutSequenceMetric = payload.Metrics.Where(m => m.Name != SparkplugNet.Core.Constants.SessionNumberMetricName);
            var filteredMetrics = KnownMetricsStorage.FilterMetrics(metricsWithoutSequenceMetric, topic.MessageType).ToList();

            if (sessionNumberMetric is not null)
            {
                filteredMetrics.Add(sessionNumberMetric);
            }

            // Handle messages.
            switch (topic.MessageType)
            {
                case SparkplugMessageType.DeviceCommand:
                    if (string.IsNullOrWhiteSpace(topic.DeviceIdentifier))
                    {
                        throw new InvalidOperationException($"Topic {topic} is invalid!");
                    }

                    await FireDeviceCommandReceived(topic.DeviceIdentifier, filteredMetrics);
                    break;

                case SparkplugMessageType.NodeCommand:
                    await FireNodeCommandReceived(filteredMetrics);
                    break;
            }
        }

        protected virtual void AddDeviceBirthMetrics(List<Metric> ms)
        {

        }

       

        protected virtual async Task AcquireData()
        {

        }

        public void StartAcquisition()
        {
            SA = sa;
            acq_timer.Start();
        }

        public void StopAcquisition()
        {
            acq_timer.Stop();
        }



    }
}
