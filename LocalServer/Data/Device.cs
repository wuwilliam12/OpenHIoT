using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SparkplugNet.VersionB.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;
using DataType = SparkplugNet.VersionB.Data.DataType;

namespace OpenHIoT.LocalServer.Data
{
    /*  public interface ISyncableItem
      {
          public long LaTime { get; set; }
      }
    */

    public enum OpenHIoTIdType
    {
        Asset = 1,
        Ble = 2,
        Wifi = 3,
        Eth = 4,
        Server = 5,     // Sid  
        Simulator = 6,   // SId
        Client = 7,      // GUI

        User = 0x10
    }
    [Flags]
    public enum DeviceStatus
    {
        Connected = 1,
        Accepted = 0x100,
        Enabled = 0x200,
        Deleted = 0x8000
    }

    public class Device
    {
        public static readonly int status_all_bits = 0xffff; 
        public static readonly int status_volatile_bits = 0xff; 
        public static readonly int status_nv_bits = 0xff00; 
        [Key]
        public uint LId { get; set; }       //local id
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Name { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Desc { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public uint? Prod { get; set; }       // product id, model nu
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ulong? PhyId { get; set; }       // physical(mac) Id(address)   
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]                                              //   public uint? Model { get; set; }       //
        public uint? Asset { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
//        public uint? SimId { get; set; }
//        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public uint? ServerId { get; set; }             // Server Id,
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public uint? NsId { get; set; }      // name space id,  

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? LaTime { get; set; }       // last access time 
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ulong? Parent { get; set; }     // Parent Id       
      
  //      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
 //       public bool? Active { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte? HmStCh { get; set; }   // standard HIoT Msg Stream Channel

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte[]? Setting { get; set; }   // parameters
   
        public int Status { get; set; }

        [JsonIgnore]
        [NotMapped]
        public bool Deleted
        {            
           get { return (Status & (int)DeviceStatus.Deleted) != 0; }
            /* 
                        set
                        {
                            if (value) Status |= (int)DeviceStatus.Deleted;
                            else Status &= status_all_bits - (int)DeviceStatus.Deleted;
                        }
            */
        }

        [JsonIgnore]
        [NotMapped]
        public bool Accepted { 
            get { return (Status & (int)DeviceStatus.Accepted) != 0; }
            /*
                       set
                       {
                           if (value) Status |= (int)DeviceStatus.Accepted;
                           else Status &= status_all_bits - (int)DeviceStatus.Accepted;
                       }
            */
        }

        [JsonIgnore]
        [NotMapped]
        public bool Connected
        {
            get { return (Status & (int)DeviceStatus.Connected) != 0; }
            /*
                   set
                   {
                       if (value) Status |= (int)DeviceStatus.Connected;
                       else Status &= status_all_bits - (int)DeviceStatus.Connected;
                   }
            */
        }

        public Device() { }

        public void ClearStatus(int status)
        {
            Status &= status_all_bits - status;
        }
        public void SetStatus(int status)
        {
            Status |= status;
        }

        public void CopyFromWithoutStatus(Device from)
        {
          //  LId = from.LId;
            Name = from.Name;
            Desc = from.Desc;
            PhyId = from.PhyId;
            Prod = from.Prod;
            Asset = from.Asset;
//            SimId = from.SimId;
            ServerId = from.ServerId;
            NsId = from.NsId;
//            Deleted = from.Deleted;
            LaTime = from.LaTime;
            Parent = from.Parent;
//            Active = from.Active;
            HmStCh = from.HmStCh;
 //           Status = from.Status;
        }

        public Metric GetDeviceMetricStringV()
        {
            PropertySet ps = new PropertySet();
            Metric m = new Metric(DataType.UInt64, GetId())
            {
                Properties = ps,
            };

            if (Name != null)
            {
                ps.Keys.Add("Name");
                ps.Values.Add(new PropertyValue(DataType.String, Name));
            }

            if (Desc != null)
            {
                ps.Keys.Add("Desc");
                ps.Values.Add(new PropertyValue(DataType.String, Desc));
            }

            if (PhyId != null)
            {
                ps.Keys.Add("PhyId");
                ps.Values.Add(new PropertyValue(DataType.String, PhyId.ToString()));
            }

            if (Prod != null)
            {
                ps.Keys.Add("Prod");
                ps.Values.Add(new PropertyValue(DataType.String, Prod.ToString()));
            }

            if (Asset != null)
            {
                ps.Keys.Add("Asset");
                ps.Values.Add(new PropertyValue(DataType.String, Asset.ToString()));
            }
     /*       if (SimId != null)
            {
                ps.Keys.Add("SimId");
                ps.Values.Add(new PropertyValue(DataType.String, SimId.ToString()));
            }*/
            return m;
        }
        public Metric GetDeviceMetric()
        {
            PropertySet ps = new PropertySet();
            Metric m = new Metric(DataType.UInt64, GetId())
            {
                Properties = ps,
            };

            if (Name != null)
            {
                ps.Keys.Add("Name");
                ps.Values.Add(new PropertyValue(DataType.String, Name));
            }

            if (Desc != null)
            {
                ps.Keys.Add("Desc");
                ps.Values.Add(new PropertyValue(DataType.String, Desc));
            }

            if (PhyId != null)
            {
                ps.Keys.Add("PhyId");
                ps.Values.Add(new PropertyValue(DataType.UInt64, PhyId));
            }

            if (Prod != null)
            {
                ps.Keys.Add("Prod");
                ps.Values.Add(new PropertyValue(DataType.UInt32, Prod));
            }

            if (Asset != null)
            {
                ps.Keys.Add("Asset");
                ps.Values.Add(new PropertyValue(DataType.UInt32, Asset));
            }
            /*
            if (SimId != null)
            {
                ps.Keys.Add("SimId");
                ps.Values.Add(new PropertyValue(DataType.UInt32, SimId));
            }*/
            return m;
        }

        void SetProperty(string name, object value)
        {
            if (name == "Name")
                Name = value as string;
            else if (name == "Desc")
                Desc = value as string;
            else if (name == "PhyId")
                PhyId = (ulong)value;
            else if (name == "Asset")
                Asset = (uint)value;
            else if (name == "Prod")
                Prod = (uint)value;
  //          else if (name == "SimId")
  //              SimId = (uint)value;
        }

        void SetPropertyS(string name, string value)
        {
            if (name == "Name")
                Name = value as string;
            else if (name == "Desc")
                Desc = value as string;
            else if (name == "PhyId")
                PhyId = Convert.ToUInt64(value);
            else if (name == "Asset")
                Asset = Convert.ToUInt32(value);
            else if (name == "Prod")
                Prod = Convert.ToUInt32(value);
  //          else if (name == "SimId")
  //              SimId = Convert.ToUInt32(value); ;
        }

        public void Init(Metric m)
        {
            if (m.Properties == null) return;
            PropertySet ps = m.Properties;
            for (int i = 0; i < ps.Keys.Count; i++)
                SetPropertyS(ps.Keys[i], (string)ps.Values[i].Value);
        }
        
        public ulong? GetId()
        {
            if (Asset != null)
                return ((ulong)Asset << 8) + (ulong)OpenHIoTIdType.Asset;
            else
            {
                if (PhyId != null)
                    return PhyId;
          /*      else
                {
                    if (SimId != null)
                        return ((ulong)SimId << 8) + (ulong)OpenHIoTIdType.Simulator;
                }
               */
            }
            return null;
        }
        public override string ToString()
        {
            if (Name == null) return $"Id:{GetId()}";
            else return Name;
        }

        public bool SameAs(Device dev)
        {
            return Asset != null && Asset == dev.Asset || PhyId != null && PhyId == dev.PhyId;               
        }
    }

    /*
    public class Device
    {
        public ulong Id { get; set; }
        public uint? NsId { get; set; }      // name space id,  
        public string? Name { get; set; }
        public string? Desc { get; set; }
        public float? SA { get; set; }      //sampling rate, samples per second 
        public uint? Parent { get; set; }     // Parent Id                                      
        public long? LocId { get; set; }      //location id 
        public uint? LocSId { get; set; }      //location server(when created) id 

        public bool? Deleted { get; set; }

        public override string ToString()
        {
            if (Name == null) return $"Id:{Id}";
            else return Name;
        }
    }
    */
    public class Auth
    {
        public ulong Id { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? UName { get; set; } //User Name
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Pw { get; set; }
    }

    public enum MqttServerType { Offline = 1, Local = 2, Center = 3 };

    public class Server
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Desc { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Ip { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? HttpPort { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MqttPort { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public uint? NsId { get; set; }      // name space id,  

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? Deleted { get; set; }
    }
}

