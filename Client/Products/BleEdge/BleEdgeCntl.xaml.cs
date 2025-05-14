using OpenHIoT.Client.Pages.Device;
using OpenHIoT.Client.Requests;
using OpenHIoT.LocalServer.HiotMsg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static OpenHIoT.LocalServer.Operation.Channel;

namespace OpenHIoT.Client.Products.BleEdge
{
    /// <summary>
    /// Interaction logic for BleEdgeCntl.xaml
    /// </summary>
    public partial class BleEdgeCntl : System.Windows.Controls.UserControl, IDeviceCntl
    {
        BleEdge edge;
        ulong id;
        public BleEdgeCntl()
        {
            InitializeComponent();
            edge = new BleEdge(this);
            newDevSp.Visibility = System.Windows.Visibility.Collapsed;


        }
        public Products.Device GetDevice()
        {
            return edge;
        }
        public void CloseCntl()
        {
            
        }

        public void SetDevice(LocalServer.Data.Device  _device)
        {
            edge.DeviceBase = _device ;
            id = (ulong)edge.DeviceBase.GetId() ;
        }

        private async void newDevicesBtn_Click(object sender, RoutedEventArgs e)
        {
            if (edge.StreamChannel == null) return;
            newDevSp.Visibility = System.Windows.Visibility.Collapsed;
            HmBlockChannelValueByIID block = new HmBlockChannelValueByIID()
            {
                IId = (byte)edge.StreamChannel.Head.IId,
            };
            await DeviceRequest.WriteChannel(edge.DeviceBase.ServerId, id, block.GetBytes(edge.StreamChannel.Head, (ushort)BdStreamDlCode.SearchNew, null));
        }

        public void ProcessPayloadDataHm(byte[] bs)
        {

        }

        private async void newDevList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (edge.StreamChannel == null && newDevList.SelectedItem == null) return;
            DeviceBle dev = (DeviceBle)newDevList.SelectedItem;
            if (System.Windows.MessageBox.Show($"Sure to connect to {OpenHIoT.BleEdge.BLE.Device.ToMacStringReverse( (ulong)dev.PhyId>>8) }?", "?", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;
    //        newDevSp.Visibility = System.Windows.Visibility.Collapsed;
            HmBlockChannelValueByIID block = new HmBlockChannelValueByIID()
            {
                IId = (byte)edge.StreamChannel.Head.IId,
            };

            byte[] bs = BitConverter.GetBytes( (ulong)dev.PhyId );
            await DeviceRequest.WriteChannel(edge.DeviceBase.ServerId, id, block.GetBytes(edge.StreamChannel.Head, (ushort)BdStreamDlCode.AcceptDevice, bs));
        }
    }
}
