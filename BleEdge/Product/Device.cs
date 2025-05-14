using OpenHIoT.BleEdge.BLE;
using OpenHIoT.BleEdge.BLE.Ble32Feet;
using Newtonsoft.Json;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.HiotMsg;
using OpenHIoT.LocalServer.Operation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using System.Reflection;
using SparkplugNet.VersionB.Data;
using OpenHIoT.LocalServer.Controllers;

using OpenHIoT.LocalServer.Data.SampleDb;


namespace OpenHIoT.BleEdge.Product
{
   
    public class Device : LocalServer.Data.Device,  IDeviceHm, INotifyPropertyChanged
    {
        protected ulong mac_addr;
        protected ulong id;
        Processors.Device? proc;
        [JsonIgnore]
        public ulong Id { get { return id; } }
        [JsonIgnore]
        public string Mac { get; set; }

        public short RSSI;
        public DeviceStatus Status { get; set; }

 //       [JsonIgnore]
 //       public Devices Children { get; set; }

        [JsonIgnore]
        public Channels Channels { get; set; } 
        
        [JsonIgnore]
        public Services? Services { get; set; }


        public Device()
        {

        }
        public static Type? GetType(string name, string? assembly)
        {
            name = name.Replace('-', '_');
           
            if(assembly == null)
                return Type.GetType(name);
            else
            {
                try
                {
                    Assembly a = Assembly.LoadFrom(assembly);
                    return a.GetType(name);
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        protected bool Init()
        {
            if (Services != null)
                return true; 
            Services = new();
            PhyId = (Convert.ToUInt64(Mac, 16) << 8) + (ulong)OpenHIoT.LocalServer.Data.OpenHIoTIdType.Ble;
            id = Asset != null ? ((ulong)Asset << 8) + (ulong)OpenHIoTIdType.Asset : (ulong)PhyId;

            Product? product = Products.Instance.FirstOrDefault(x => x.Name == Name);
            if (product == null)
                return false;
            Channels = new Channels();
            foreach (var c in product.Channels)
            {
                c.Init();
                Channels.Add(c);
            }

            foreach (var s in product.Services)
            {
                Services.Add(s);
                foreach (var a in s.Characteristics)
                {
                    a.Channels = new Channels();
                    if (a.ChIds != null)
                    {
                        foreach (int i in a.ChIds)
                        {
                            Channel? ch = Channels.FirstOrDefault(x => x.IId == i);
                            if (ch != null)
                                a.Channels.Add(ch);
                        }
                    }
                }
            }

            Type? type = GetType($"OpenHIoT.BleEdge.Product.Processors.Device{Name}", null);
            if (type != null)
            {
                var constructor = type.GetConstructor(new Type[0]);
                if (constructor != null)
                {
                    proc = (Processors.Device)constructor.Invoke(null);
                    if (proc != null)
                        proc.SetDevice(this);
                }
            }
            return OnInit();
        }
        protected virtual bool OnInit()
        {
            return true;
        }

        public virtual async Task<bool> Connect(object ble_dev)
        {
            return false;
        }

        public byte[] GetHmBirthPayload()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);

                HmPayloadBlock.WriteBlock(bw, HmPayloadBlockTypes.HM_BT_DEVICE_BIRTH, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((LocalServer.Data.Device)this)));

                foreach (Channel ch in Channels)
                {
                    ChannelDTO channelDTO = new ChannelDTO(ch);
                    if (ch.DefaultVal != null)
                        channelDTO.Val = ValueDataType.ToString(ch.DType, ch.Value);
                    HmPayloadBlock.WriteBlock(bw, HmPayloadBlockTypes.HM_BT_CHANNEL_BIRTH, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(channelDTO)));
                }

                bw.Flush();
                byte[] bs1 = new byte[bw.BaseStream.Position];
                Buffer.BlockCopy(ms.GetBuffer(), 0, bs1, 0, bs1.Length);
                return bs1;
            }
        }

        public void ProcessCmdPayload(byte[] payload)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisedOnPropertyChanged(string _PropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(_PropertyName));
            }
        }

        public virtual void AfterProcessHmBlockData()
        {
            //throw new NotImplementedException();
        }

        public virtual void ProcessHmBlockData(HmPayloadBlock block)
        {
           //throw new NotImplementedException();
        }

        public Characteristic? GetCharacteristic(string uuid)
        {
            foreach(Service s in Services)
            {
                foreach (Characteristic c in s.Characteristics)
                    if (c.UUID == uuid) return c;
            }
            return null;
        }
    }

    public class Devices : List<Device>
    {
        public static Devices GetKnownDevices()
        {
            Devices devices = new Devices();

            string fn = $"{ValueDataType.GetDataParentDirectory()}dat\\ble_edge\\known_devices.json";
            if (File.Exists(fn))
            {
                string txt = File.ReadAllText(fn);

                List<BLE.Device> ds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BLE.Device>>(txt);
                foreach (var d in ds)
                {
                    Device dev = new Device32F()
                    {
                        Status = DeviceStatus.Accepted,
                        Mac = d.Mac,
                        Asset = d.Asset,
                        Name = d.Name,
                    };
                    devices.Add(dev);
                }
            }
            return devices;
        }


    }
}
