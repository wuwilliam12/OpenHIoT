using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenHIoT.LocalServer.Data.SampleDb;
using OpenHIoT.LocalServer.HiotMsg;
using OpenHIoT.LocalServer.Operation;
using OpenHIoT.MqttSim.HiotMsg;



//using OpenHIoT.LocalServer.Operation;
using SparkplugNet.Core.Enumerations;
using SparkplugNet.VersionB.Data;

namespace OpenHIoT.MqttSim.Devices
{
    public class SensorTPa : OpenHIoT.MqttSim.HiotMsg.Edge
    {
        public SensorTPa()
        {
            /*
           // Id = 2;
           // ProdId = 2;
            chs = new Channels()
            {
                new Channel()
                {
                    Head = new LocalServer.DBase.Models.Data.Rt.HeadRt(){
                        IId = 1,
                        Name = "Temperature",               
                        DType = (int)DataType.Float,
                        UOM = "F"
                    },
                    Val = (float)70,
                },
                new Channel()
                {
                     Head = new LocalServer.DBase.Models.Data.Rt.HeadRt(){
                        IId = 2,
                        Name = "Pressure",
                        DType = (int) DataType.Float,
                        UOM = "psi"
                    },
                    Val = (float)13.5,

                }
            };
    */
        }



        protected override async Task AcquireData()
        {

            chs[0].Val = (float)Channel.GenerateNxtData(-10, 1000, 0.1,
                ValueDataType.GetDoubleVal(chs[0].DType, chs[0].Val));
            chs[1].Val = (float)Channel.GenerateNxtData(0, 100, 0.1,
                ValueDataType.GetDoubleVal(chs[1].DType, chs[1].Val));

            await PublishDataMessage();
        }

    }
}
