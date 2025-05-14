using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenHIoT.LocalServer.Data.SampleDb;
using OpenHIoT.MqttSim.Sparkplug;
using SparkplugNet.Core.Enumerations;
using SparkplugNet.VersionB.Data;


namespace OpenHIoT.MqttSim.Devices
{
    public class SensorTHa : OpenHIoT.MqttSim.Sparkplug.Edge
    {
        public SensorTHa()
        {

        }

        protected override async Task AcquireData()
        {
            ulong ts = (ulong)((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
            chs[0].Val = (float)Channel.GenerateNxtData(-10, 1000, 0.1, ValueDataType.GetDoubleVal(chs[0].DType, chs[0].Val));
            chs[1].Val = (float)Channel.GenerateNxtData(0, 100, 0.1, ValueDataType.GetDoubleVal(chs[1].DType, chs[1].Val));
            foreach (MqttSim.Sparkplug.Channel m in chs)
            {
                m.DataMetric.Timestamp = ts;
                m.DataMetric.SetValue(m.DType, m.Val);
            }
            await PublishDataMessage();
        }

    }
}
