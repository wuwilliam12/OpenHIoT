using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet.Server;
using OpenHIoT.LocalServer.Data;

using Serilog;
using SparkplugNet.Core;
using SparkplugNet.Core.Application;
using SparkplugNet.Core.Enumerations;
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

namespace OpenHIoT.MqttSim.Sparkplug
{
    public class Edge : Device , IEdge
    {

        /// <inheritdoc cref="SparkplugNodeBase{T}"/>
        /// <summary>
        /// Initializes a new instance of the <see cref="SparkplugNode"/> class.
        /// </summary>
        /// <param name="knownMetricsStorage">The metric names.</param>
        /// <param name="specificationVersion">The Sparkplug specification version.</param>
        /// <seealso cref="SparkplugNodeBase{T}"/>
        public Edge()
        {
        }

        public async void ConnectTo(string broker, int port)
        {

            CancellationTokenSource CancellationTokenSource = new();
            var nodeOptions = new SparkplugNodeOptions(
      broker,
      port,
      Id.ToString(),
      ((StartUp.Edge)startup_dev).User,
      ((StartUp.Edge)startup_dev).Pw,
      "scada1B",
      TimeSpan.FromSeconds(30),
      SparkplugMqttProtocolVersion.V311,
      null,
      null,
  "g1",
  Id.ToString(),
      CancellationTokenSource.Token);

            //   var node = new VersionB.SparkplugNode(VersionBMetricsNode, SparkplugSpecificationVersion.Version22);

            // Handles the node's connected and disconnected events.
            /*    node.Connected += OnVersionBNodeConnected;
                node.Disconnected += OnVersionBNodeDisconnected;

                // Handles the node's device related events.
                node.DeviceBirthPublishing += OnVersionBNodeDeviceBirthPublishing;
                node.DeviceCommandReceived += OnVersionBNodeDeviceCommandReceived;
                node.DeviceDeathPublishing += OnVersionBNodeDeviceDeathPublishing;

                // Handles the node's node command received event.
                node.NodeCommandReceived += OnVersionBNodeNodeCommandReceived;

                // Handles the node's status message received event.
                node.StatusMessageReceived += OnVersionBNodeStatusMessageReceived;
             */
            // Start a node.
            Log.Information("Starting node...");
         //   this.NodeCommandReceived += Edge_NodeCommandReceived;
            await Start(nodeOptions);
          
            Log.Information("Node started...");
         //  
        }

        private Task Edge_NodeCommandReceived(NodeCommandEventArgs arg)
        {

            return Task.CompletedTask;
        }

        /*
        public override void  Init(StartUp.Device su_dev)
        {
            base.Init(su_dev);
            StartUp.Edge su_edge = (StartUp.Edge)su_dev;

        }
        */

        /// <summary>
        /// Publishes version B metrics for a node.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        /// <exception cref="ArgumentNullException">Thrown if the options are null.</exception>
        /// <returns>A <see cref="MqttClientPublishResult"/>.</returns>
        public override async Task<MqttClientPublishResult> PublishDataMessage()
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
            var dataMessage = messageGenerator.GetSparkplugNodeDataMessage(
                NameSpace,
                Options.GroupIdentifier,
                Options.EdgeNodeIdentifier,
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
            Metric m = metricsWithoutSequenceMetric.First();
            var c = chs.FirstOrDefault(c => c.DataMetric.Alias == m.Alias);
            Console.WriteLine($"{Name} {c.DataMetric.Name}: {m.Value.ToString()} ");
            return;
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





    }
}
/*
 * 
ACT1210E-131-2P-TL00
ISO1452


W/R Modbus mapping table

ACT
addr, regs, sample rate

W/R Port Setting
W/R Registers
W/R ACT
Start/Stop Acquisition
  */