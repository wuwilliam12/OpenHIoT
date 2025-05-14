using OpenHIoT.LocalServer.Data.SampleDb.Rt;
using OpenHIoT.LocalServer.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHIoT.Client.Products
{
    public class Channel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        ulong ts;
        public HeadRt Head { get; set; }
        public ulong Alias {  get; set; }

        SampleDTO? sample;
        public SampleDTO? Sample
        {
            get { return sample; }
            set { sample = value;
                if (sample != null && PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Val"));
            }
        }
        
        public ulong TS
        {
            get
            {
                if (Sample != null)
                    return Sample.TS;
                return ts;
            }
        }
        public Channel() {             
            Head = new HeadRt();
            ts = (ulong)((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
        }
        public Channel(HeadRt h)
        {
            Head = h;
            Alias = (ulong)h.GetAlias();
            ts = (ulong)((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
        }
    }

   

    public class Channels : List<Channel>
    {

    }
}
