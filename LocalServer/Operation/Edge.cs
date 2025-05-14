using Microsoft.Extensions.Options;
using MQTTnet;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data;
using SparkplugNet.Core.Enumerations;
using SparkplugNet.Core.Messages;
using SparkplugNet.VersionB.Data;

namespace OpenHIoT.LocalServer.Operation
{
    public class Edge : Device
    {
  //      int bdSeq;

 
        public Edge(Topic topic, List<SparkplugNet.VersionB.Data.Metric> ms) : base(topic, ms) 
        {
          //  Init(topic);
            Id = topic.EId;
          
        }

        public Edge(Topic topic) : base(topic) 
        {
         //   Init(topic);
            Id = topic.EId;
        }

        public Edge() 
        {

        }


        public void Disconnect()
        {

        }

    }


}
