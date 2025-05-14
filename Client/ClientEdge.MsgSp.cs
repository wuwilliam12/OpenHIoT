using OpenHIoT.LocalServer.DBase.Models;
using SparkplugNet.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHIoT.Client
{
    public partial class ClientEdge
    {
        void ProcessMessageSp(MqttMsg mqttMsg)
        {

            var payloadVersionB = PayloadHelper.Deserialize<SparkplugNet.VersionB.ProtoBuf.ProtoBufPayload>(mqttMsg.Payload);
     /*       if (payloadVersionB is not null)
            {
                var convertedPayload = PayloadConverter.ConvertVersionBPayload(payloadVersionB);

                if (convertedPayload is not Payload)
                {
                    throw new InvalidCastException("The metric cast didn't work properly.");
                }
                HandleMessagesForVersionBSp(mqttMsg.Topic, convertedPayload);
            }*/
        }
    }
}
