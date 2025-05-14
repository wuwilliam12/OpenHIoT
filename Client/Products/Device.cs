using OpenHIoT.Client.Pages.Channel.DataList;
using OpenHIoT.Client.Requests;
using OpenHIoT.LocalServer.Operation;
using OpenHIoT.LocalServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHIoT.Client.Products
{
    public class Device
    {
      
        public LocalServer.Data.Device DeviceBase { get; set; }
        public Channels Channels { get; set; }
        public Devices Children { get; set; }


        public Device() {
            Channels = new Channels();
            Children = new Devices();           
        }

        public void Init()
        {
            CreateChannels();
            OnInit();
        }

        async void CreateChannels()
        {
            List<HeadRtC>? hs = await SampletRtRequest.GetAllHeadsOfDevAsync(null, (ulong)DeviceBase.GetId());
            if(hs != null)
            {
                Channels = new Channels();
                foreach(HeadRtC h in hs)
                {
                    Channels.Add(new Channel(h));
                }
            }
            OnInit();
        }
        public virtual void OnInit()
        {

        }
        public async Task UpdateChanelValues()
        {

            ulong[] m_ids = Channels.Select(x => x.Alias).ToArray();
            ulong[] tss = Channels.Select(x => x.TS).ToArray();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (m_ids.Length > 0)
            {
                SampleDTOs? ss = await SampletRtRequest.GetSamplesFromS(DeviceBase.ServerId, m_ids, tss );
        //        ts_pre += 1000;

                if (ss != null)
                {
                    foreach (SampleDTO dto in ss)
                    {
                        Channel? ch = Channels.FirstOrDefault(x => x.Alias == dto.Alias);
                        if (ch != null)
                            ch.Sample = dto;
                    }
                }
            }

        }
    }
}
