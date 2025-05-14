using OpenHIoT.LocalServer.Operation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHIoT.MqttSim
{
    public  class Channel
    {
        public ChannelDTO DTO { get; set; }
        public SparkplugNet.VersionB.Data.DataType DType { get; set; }
        public static double GenerateNxtData(double min, double max, double step, double? val)
        {
            Random r = new Random();
            double d = val == null ? min : (double)val;
            d += (r.NextDouble() - 0.5) * step;
            if (d < min) return min;
            if (d > max) return max;
            return d;
        }
    }
}
