//using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Operation;
using OpenHIoT.LocalServer.HiotMsg;
using SparkplugNet.VersionB.Data;
using System.Diagnostics.Metrics;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Runtime.Serialization;
using System.Drawing;
using System.Buffers.Binary;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json;

namespace OpenHIoT.LocalServer.Data.SampleDb.Rt
{
    //    public enum HiotRtItemType { Measurement = 0, Property = 1, PropertyReadOnly = 2 };

    [Flags]
    public enum ChannelProperty { Read = 1, Write = 2, Notify = 4, Complex = 0x80 };

    public class HeadRt 
    {
        public static readonly char options_sep = ':';
        public uint Id { get; set; }
        //        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        ulong? alias; 
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte? IId { get; set; }        //  internal id 

        public string Name { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Desc { get; set; }       //Description
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public uint? NsId { get; set; }         // NsID, alias, topic, deployed id
                                                //       public uint? SId { get; set; }          // Server Id 
        public ulong DId { get; set; }           //Device   
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ushort? Property { get; set; }         // Type 
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ushort? DType { get; set; }          //sparkplug Data Type 
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? UOM { get; set; }   //Unit of data
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? TOM { get; set; }   //Type of Unit
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public uint? SA { get; set; }      //sampling rate - samples per second
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Format { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Options { get; set; }        //Enum data type
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

        public bool? InActive { get; set; }

        public HeadRt()
        {

        }
        public HeadRt(Operation.Device dev, Metric m)
        {
            Name = m.Name;
      //      if(Name.StartsWith("M/"))
      //         Name = Name.Substring(2, Name.Length-2);
            DId = dev.Id;
            DType = (ushort)m.DataType;
            if (m.Alias != null)
                alias = m.Alias;
            if (m.Properties is not null)
                Init(m);
        }

        public  void CopyFrom(HeadRt from)
        {
            Id = from.Id;
            Name = from.Name;
            IId = from.IId;
            Desc = from.Desc;
            NsId = from.NsId;
            DId = from.DId;
            Property = from.Property;
            DType = from.DType;
            UOM = from.UOM;
            TOM = from. TOM;
            SA = from.SA;
            Format = from.Format;
            Options = from.Options;
            InActive = from.InActive;
        }
        public PropertySet GetPropertySetStringV()
        {
            PropertySet ps = new PropertySet();
            //     if (IId != null)
            {
                ps.Keys.Add("IId");
                ps.Values.Add(new PropertyValue(DataType.String, IId.ToString()));
            }
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

            //   if (DId != null)
            {
                ps.Keys.Add("DId");
                ps.Values.Add(new PropertyValue(DataType.String, DId.ToString()));
            }

            if (Property != null)
            {
                ps.Keys.Add("Property");
                ps.Values.Add(new PropertyValue(DataType.String, Property.ToString()));
            }

            if (UOM != null)
            {
                ps.Keys.Add("UOM");
                ps.Values.Add(new PropertyValue(DataType.String, UOM));
            }
            if (TOM != null)
            {
                ps.Keys.Add("TOM");
                ps.Values.Add(new PropertyValue(DataType.String, TOM));
            }
            if (Format != null)
            {
                ps.Keys.Add("Format");
                ps.Values.Add(new PropertyValue(DataType.String, Format));
            }
            if (Options != null)
            {
                ps.Keys.Add("Options");
                ps.Values.Add(new PropertyValue(DataType.String, Options));
            }
            if (SA != null)
            {
                ps.Keys.Add("SA");
                ps.Values.Add(new PropertyValue(DataType.String, SA.ToString()));
            }

            return ps;
        }
        public PropertySet GetPropertySet()
        {
            PropertySet ps = new PropertySet();
       //     if (IId != null)
            {
                ps.Keys.Add("IId");
                ps.Values.Add(new PropertyValue(DataType.UInt16, IId));
            }
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

         //   if (DId != null)
            {
                ps.Keys.Add("DId");
                ps.Values.Add(new PropertyValue(DataType.UInt64, DId));
            }

            if (Property != null)
            {
                ps.Keys.Add("Property");
                ps.Values.Add(new PropertyValue(DataType.UInt16, Property));
            }

            if (UOM != null)
            {
                ps.Keys.Add("UOM");
                ps.Values.Add(new PropertyValue(DataType.String, UOM));
            }
            if (TOM != null)
            {
                ps.Keys.Add("TOM");
                ps.Values.Add(new PropertyValue(DataType.String, TOM));
            }
            if (Options != null)
            {
                ps.Keys.Add("Options");
                ps.Values.Add(new PropertyValue(DataType.String, Options));
            }
            if (Format != null)
            {
                ps.Keys.Add("Format");
                ps.Values.Add(new PropertyValue(DataType.String, Format));
            }
            if (SA != null)
            {
                ps.Keys.Add("SA");
                ps.Values.Add(new PropertyValue(DataType.UInt32, SA));
            }

            return ps;
        }
        void SetProperty(string name, object value)
        {
            if (name == "Name")
                Name = value as string;
            else if (name == "Desc")
                Desc = value as string;
            else if (name == "IId")
                IId = (byte)value;
            else if (name == "Property")
                Property = (ushort)value;
            else if (name == "UOM")
                UOM = (string)value;
            else if (name == "TOM")
                TOM = (string)value;
            else if (name == "Options")
                Options = (string)value;
            else if (name == "Format")
                Format = (string)value;
            else if (name == "SA")
                SA = (uint)value;
        }

        void SetPropertyS(string name, string value)
        {
            if (name == "Name")
                Name = value as string;
            else if (name == "Desc")
                Desc = value as string;
            else if (name == "IId")
                IId = Convert.ToByte(value);
            else if (name == "Property")
                Property = Convert.ToUInt16(value);
            else if (name == "UOM")
                UOM = value;
            else if (name == "TOM")
                TOM = value;
            else if (name == "Options")
                Options = value;
            else if (name == "Format")
                Format = value;
            else if (name == "SA")
                SA = Convert.ToUInt32(value); 
        }

        public void Init(Metric m)
        {
            if (m.Properties == null) return;
            PropertySet ps = m.Properties;
            for (int i = 0; i < ps.Keys.Count; i++)
                SetPropertyS(ps.Keys[i], (string)ps.Values[i].Value);
        }

        /*
        public void SetAlias(ulong alias)
        {
            DId = alias >> 8;
            IId = (byte)alias;
        }
        */
        public static ulong? CreateAlias(ulong dev_id, byte? iid)
        {

            if (iid == null) return null;
            return (dev_id << 8) + iid;
        }
        public ulong? GetAlias()
        {
            if (alias != null)
                return alias;
            return CreateAlias(DId, IId);
        }

        public override string ToString()
        {
            return Name;
        }
        public void Close()
        {

        }

    }

    public class HeadRts : List<HeadRt>
    {

    }


}
