using OpenHIoT.BleEdge.BLE;
using OpenHIoT.BleEdge.Product;


using OpenHIoT.BleEdge.MQTT;
using SparkplugNet.Core.Enumerations;
using OpenHIoT.LocalServer.Data.SampleDb;
//using BleEdge.Edge;

namespace OpenHIoT.BleEdge
{
    public class EdgeSetting
    {  
        public static EdgeSetting Setting {  get; set; }
        public Broker? Broker { get; set; }
        public uint? Asset { get; set; }
        public string? User { get; set; }
        public string? Pw { get; set; }

        public static void ReadSetting()
        {
            string fn = $"{ValueDataType.GetDataParentDirectory()}dat\\ble_edge\\edge_setting.json";
            if (File.Exists(fn))
            {
                string txt = File.ReadAllText(fn);
                Setting = Newtonsoft.Json.JsonConvert.DeserializeObject<EdgeSetting>(txt);
            }

            if(Setting == null) 
                Setting = new EdgeSetting();
            if(Setting.Broker == null)
                Setting.Broker = new Broker();
            if(Setting.Broker.Ip == null)
                Setting.Broker.Ip = "localhost"; 
            if(Setting.Broker.Port == null)
                Setting.Broker.Port = 1884; 

        }
    }
    public class Broker
    {
        public string? Ip { get; set; }
        public int? Port { get; set; }
    }
}
