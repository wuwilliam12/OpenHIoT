using OpenHIoT.BleEdge.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHIoT.BleEdge.Product.Processors
{
    public class DeviceITH_13_B : Device
    {
        Channel channelT;
        Channel channelH;
        public override void SetDevice(OpenHIoT.BleEdge.Product.Device device)
        {
            base.SetDevice(device);
            Characteristic? c = device.GetCharacteristic("FFF6");

            if (c != null)
            {
                channelT = c.Channels.FirstOrDefault(x => x.Name == "Temperature");
                channelH = c.Channels.FirstOrDefault(x => x.Name == "Humidity");
                c.SetProcessFunc(ProcessData);
            }
        }

        bool ProcessData(byte[] bs)
        {
            if(bs.Length >= 10)
            {
                int d = (bs[6] << 8) + bs[5];
                channelT.Value = (float)d/10;
                d = (bs[8] << 8) + bs[7];
                channelH.Value = (float)d/10;
                return true;
            }
            return false;
        }

    }
}
