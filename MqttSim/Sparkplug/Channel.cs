
using SparkplugNet.VersionB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHIoT.MqttSim.Sparkplug
{
    public class Channel : OpenHIoT.MqttSim.Channel
    {        
        public object Val { get; set; }
        public Device Device { get; set; }
        protected Metric? dataMetric;

        public Metric DataMetric
        {
            get
            {
                if (dataMetric == null)
                {
                    dataMetric = new Metric($"{DTO.Name}", DType, Val, null);
                    dataMetric.Alias = (Device.Id << 8) + DTO.IId;
                }
                return dataMetric;
            }
        }

        /*
        public Metric GetBirthMetric()
        {
            List<string> ns = new List<string>();
            List<PropertyValue> vs = new List<PropertyValue>();
            if (UOM != null)
            { ns.Add("UOM"); vs.Add(new(DataType.String, UOM)); }
            if (TOM != null)
            { ns.Add("TOM"); vs.Add(new(DataType.String, TOM)); }
            if (Rec != null)
            {
                ns.Add("Rec");
                if ((bool)Rec) vs.Add(new(DataType.UInt8, (byte)1));
                else vs.Add(new(DataType.UInt8, (byte)0));
            }
            if (SA != null)
            { ns.Add("SA"); vs.Add(new(DataType.Float, SA)); }


            if (vs.Count > 0)
            {
                DataMetric.Properties = new PropertySet
                {
                    Keys = ns,
                    Values = vs
                };
            }
            return DataMetric;
        }*/



    }
 
    public class Channels : List<Channel>
    {
        List<Metric>? dataMetrics;
        public List<Metric> DataMetrics
        {
            get
            {
                if (dataMetrics == null)
                {
                    dataMetrics = new List<Metric>();
                    foreach (Channel m in this)
                        dataMetrics.Add(m.DataMetric);
                }
                return dataMetrics;
            }
        }
    }


}
