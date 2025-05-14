using OpenHIoT.LocalServer.DBase.Models;
using SparkplugNet.Core.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using OpenHIoT.LocalServer;
using OpenHIoT.LocalServer.Services;

namespace OpenHIoT.Client
{
    public partial class ClientEdge
    {
        void ProcessMessageHm(MqttMsg mqttMsg)
        {
            //OpenHIoT.LocalServer.Operation.Edge? edge;
            //if (Edges.TryGetValue(mqttMsg.Topic.EId, out edge))
            ulong eid = Convert.ToUInt64(mqttMsg.Topic.EId);
            {
                switch ((SparkplugMessageType)mqttMsg.Topic.MType)
                {
                    case SparkplugMessageType.DeviceBirth:
                        ProcessDeviceBirthHm(eid, mqttMsg);
                        break;
                    case SparkplugMessageType.NodeData:
                        ProcessNodeDataHm(eid, mqttMsg);
                        break;
                    case SparkplugMessageType.DeviceData:
                        ProcessDeviceDataHm(eid, mqttMsg);
                        break;
                    case SparkplugMessageType.NodeDeath:
                        ProcessNodeDeathHm(eid);
                        break;
                    case SparkplugMessageType.DeviceDeath:
                        ProcessDeviceDeathHm(eid, mqttMsg);
                        break;
                }
            }
         //   else
            {
                if (mqttMsg.Topic.MType == (int)SparkplugMessageType.NodeBirth)
                    ProcessNodeBirthHm(eid, mqttMsg);
            }
        }
        void ProcessNodeBirthHm(ulong edge, MqttMsg mqttMsg)
        {
            OpenHIoT.LocalServer.Operation.Edge e = (OpenHIoT.LocalServer.Operation.Edge)OpenHIoT.LocalServer.Operation.Device.CreateDevice(true, mqttMsg);
            e.DeviceBase.Active = true;
            MainWindow.mainWindow.DeviceMainPage.UpdateItem(e.DeviceBase);

        }
        void ProcessDeviceBirthHm(ulong edge, MqttMsg mqttMsg)
        {
            OpenHIoT.LocalServer.Operation.Device d = (OpenHIoT.LocalServer.Operation.Device)OpenHIoT.LocalServer.Operation.Device.CreateDevice(false, mqttMsg);
            d.DeviceBase.Active = true;
            MainWindow.mainWindow.DeviceMainPage.UpdateItem(d.DeviceBase);
        }

        void ProcessNodeDataHm(ulong eid, MqttMsg mqttMsg)
        {
     /*       Operation.Device? device = edge.Children.FirstOrDefault(x => x.Id == (ulong)mqttMsg.Topic.DId);

            if (device != null)
                device.ProcessPayloadDataHm(mqttMsg.Payload, mRtService);
            else
            {

            }*/
        }
        void ProcessDeviceDataHm(ulong edge, MqttMsg mqttMsg)        
        {
/*            Device? device = MainWindow.mainWindow.DeviceMainPage.GetItem(edge).Device;

            if (device != null)
                device.ProcessPayloadDataHm(mqttMsg.Payload);
            else
            {

            }*/
        }

        void ProcessNodeDeathHm(ulong eid)
        {

        }
        void ProcessDeviceDeathHm(ulong eid, MqttMsg mqttMsg)
        {

        }




    }
}
