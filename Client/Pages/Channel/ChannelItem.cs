using Newtonsoft.Json;
using OpenHIoT.Client.Pages.Channel.DataList;
using OpenHIoT.Client.Requests;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;
using OpenHIoT.LocalServer.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHIoT.Client.Pages.Channel
{
    public class ChannelItem
    {
     //   public event PropertyChangedEventHandler? PropertyChanged;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public uint? SId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public uint? NsId { get; set; }

        public ulong? Alias { get; set; }

        [JsonIgnore]
        public ulong? DId { get; set; }
        [JsonIgnore]
        public byte? IId { get; set; }
         
        [JsonIgnore] 
        public string? Name
        {
            get
            {
                if(Name2 != null)
                    return "${Name2}.{Name1}" ;
                return Name1;
            }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Name1 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Name2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Property { get; set; }

        HeadRtC? head;
        [JsonIgnore]
        public HeadRtC? Head
        {
            get { return head; }
            set
            {
                head = value;
                if (head != null)
                {
                    NsId = head.NsId; DId = head.DId; IId = head.IId; Name1 = head.Name; SId = head.SId;
                    if (head.Options != null)
                        Options = head.Options.Split(HeadRt.options_sep);
                    Alias = (ulong)head.GetAlias();
                    Property = head.Property;
                }
            }
        }
        [JsonIgnore]
        public string[]? Options { get; set; }

        public ChannelItem()
        {
        }

        public ChannelItem( ChannelItem item)
        {
            CopyFrom(item);
        }

        public virtual void SetSample(SampleDTO sample)
        {

        }
      
        public virtual void SetSamples( SampleDTOs samples)
        {

        }

        public void SetDataSource()
        {
            Head =  IId != null ? SampletRtRequest.GetHeadLiveByIIdAsync(SId, (uint)DId, (byte)IId).Result :
                    NsId != null ? SampletRtRequest.GetHeadLiveByNsAsync(SId, (uint)NsId).Result :
                    Alias != null ? SampletRtRequest.GetHeadLiveByAliasAsync(SId, (ulong)Alias).Result :
                    Name1 != null && DId != null ? SampletRtRequest.GetHeadLiveByNameAsync(SId, (uint)DId, (string)Name1).Result : null;

            // if(DId != null && IId != null)
            //     Alias = (ulong)HeadRt.CreateAlias((ulong)DId, IId);
            if(Head != null)
                Alias = (ulong)Head.GetAlias();
        }

        public void CopyFrom(ChannelItem item)
        {
            NsId = item.NsId;
            DId = item.DId;
            Name1 = item.Name1;
            Name2 = item.Name2;
            Head = item.Head;
            SId = item.SId;
            IId = item.IId;
            Alias = item.Alias;
            Property = item.Property;
     //       if(DId != null)
     //           Alias = (ulong)HeadRt.CreateAlias((ulong)DId, IId);

        }

    }




}
