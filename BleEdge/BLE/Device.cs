using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenHIoT.BleEdge.BLE
{

    public class Device 
    {
        public string Name { get; set; }
        public string Mac { get; set; }
        public uint? Asset { get; set; }

        public void CopyFrom(Device dev)
        {
            Name = dev.Name;
            Mac = dev.Mac;
            Asset = dev.Asset;
        }

        public LocalServer.Data.Device GetModelsDevice()
        {
            return new OpenHIoT.LocalServer.Data.Device()
            {
                Name = Name,
                PhyId = (Convert.ToUInt64(Mac, 16) << 8) + (ulong)OpenHIoT.LocalServer.Data.OpenHIoTIdType.Ble,
                Asset = Asset
            };
        }

        public static string ToMacString(ulong mac_id)
        {
            //     byte[] bs = BitConverter.GetBytes(mac_id).Reverse().ToArray();
            //      string str = BitConverter.ToString(bs).Replace("-", "").Substring(4,12);
            byte[] bs = BitConverter.GetBytes(mac_id);
            string str = BitConverter.ToString(bs).Replace("-", "").Substring(0,12);
            return str;
        }

        public static string ToMacStringReverse(ulong mac_id)
        {
            byte[] bs = BitConverter.GetBytes(mac_id).Reverse().ToArray();
            string str = BitConverter.ToString(bs).Replace("-", "").Substring(4,12);
            return str;
        }
    }


}
