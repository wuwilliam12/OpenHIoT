using OpenHIoT.Client.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Newtonsoft.Json;

namespace OpenHIoT.Client.Products.BleEdge
{
    public class BleEdge : Device
    {
        BleEdgeCntl cntl;
        Channel? streamChannel;
        public Channel? StreamChannel { get { return streamChannel; } }
        public BleEdge(BleEdgeCntl _cntl)
        {
            cntl = _cntl;
            /*   Channels = new Channels() { new Channel() };
               LocalServer.Operation.Channel.InitBidirectionalStreamHead(Channels[0].Head);
               Channels[0].Head.IId = 1;
            */
        }

        public override void OnInit()
        {
            base.OnInit();
            streamChannel = Channels.FirstOrDefault(x => x.Head.IId == 1);
            foreach (var channel in Channels)
            {
                channel.PropertyChanged += Channel_PropertyChanged;
            }

        }

        void ProcessNewDevices(byte[] bs)
        {
            string str = Encoding.UTF8.GetString(bs, 1, bs.Length - 1);
            List<DeviceBle>? devs = JsonConvert.DeserializeObject<List<DeviceBle>>(str);
            if (devs != null)
            {
                
                cntl.Dispatcher.Invoke (new Action(() => {
                    cntl.newDevSp.Visibility = System.Windows.Visibility.Visible;
                    cntl.newDevList.ItemsSource = devs;
                }));
            }
        }

        private void Channel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender != null)
            {
                Channel channel = (Channel)sender;
                if(channel.Head.IId == DeviceBase.HmStCh && channel.Sample != null && channel.Sample.ValBlob != null)
                {
                    byte[] bs = channel.Sample.ValBlob;
                    LocalServer.Operation.Channel.BdStreamUlCode code = (LocalServer.Operation.Channel.BdStreamUlCode)bs[0];
                    switch(code)
                    {
                        case LocalServer.Operation.Channel.BdStreamUlCode.NewDevices:
                            ProcessNewDevices(bs);
                            break;
                    }

                }
            }
        
        }
    }

    public class DeviceBle : LocalServer.Data.Device
    {

        public override string ToString()
        {
            return $"{Name}: { OpenHIoT.BleEdge.BLE.Device.ToMacStringReverse( (ulong)PhyId >> 8) }";
        }
    }
}
