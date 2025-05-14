using Newtonsoft.Json;
using OpenHIoT.LocalServer.Data.SampleDb;
using OpenHIoT.LocalServer.HiotMsg;
using OpenHIoT.LocalServer.Operation;
using OpenHIoT.MqttSim.Devices;
using SparkplugNet.VersionB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHIoT.MqttSim
{
    public interface IDevice
    {
        void Init(StartUp.Device su_dev);
        void StartAcquisition();
        void StopAcquisition();
    }
    public interface IEdge
    {
         void ConnectTo(string broker, int port);
    }
    public class StartUp
    {
        public string BrokerIp { get; set; }
        public int BrokerPort { get; set; }

        public List<Edge> Edges { get; set; }

        public static void Start(Products products)
        {
            string txt = File.ReadAllText($"{ValueDataType.GetDataParentDirectory()}dat\\mqtt_sim\\startup.json");
            StartUp? su = Newtonsoft.Json.JsonConvert.DeserializeObject<StartUp>(txt);
            Console.WriteLine($"Broker: {su.BrokerIp}, post: {su.BrokerPort}.");
            foreach(Edge e in su.Edges)
            {

                if (e.User == null) e.User = "user";
                if (e.Pw == null) e.Pw = "password";
                IEdge? edge = null;
                if(e.Product == "SensorTHa")
                {
                    edge = new SensorTHa();
                }
                else if(e.Product == "SensorTPa")
                {
                    edge = new SensorTPa();
                }
                if(edge != null)
                {
                    Product? product = products.GetProduct(e.Product);
                    if (product != null)
                    {
                        if (edge is HiotMsg.Edge)
                        {
                            ((HiotMsg.Edge)edge).Name = e.Product;
                        }
                        else
                        {
                            ((Sparkplug.Edge)edge).Name = e.Product;
                        }

                        e.Channels = product.GetChannelsCopy();
                        ((IDevice)edge).Init(e);
                        Console.WriteLine($"Connects {e.Product}, Sim Id: {e.SimId}. "); 
                        edge.ConnectTo(su.BrokerIp, su.BrokerPort);
                        ((IDevice)edge).StartAcquisition();
                    }
                }
            }

        }


        public class Device
        {
            public string Product { get; set; }
            public uint SimId { get; set; }
            public List<ChannelDTO> Channels { get; set; }

            Product? product;

            public List<Metric> GetChannelMetrics(ulong dev_id)
            {
                List<Metric> ms = new List<Metric>();
                foreach (ChannelDTO ch in Channels)
                    ms.Add(ch.GetChannelMetric(dev_id));
                return ms;
            }
 
        }
        public class Edge : Device
        {
            public string User { get; set; }    
            public string Pw { get; set; }
            public List<Edge> Devices { get; set; }
        }


    }
}
