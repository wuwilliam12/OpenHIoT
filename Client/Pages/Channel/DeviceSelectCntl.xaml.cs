using OpenHIoT.Client.Requests;
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

namespace OpenHIoT.Client.Pages.Channel
{
    /// <summary>
    /// Interaction logic for DeviceSelectCntl.xaml
    /// </summary>
    public partial class DeviceSelectCntl : System.Windows.Controls.UserControl
    {
        public event SelectionChangedEventHandler DeviceSelected;
        public DeviceSelectCntl()
        {
            InitializeComponent();
        }

        public void Init(LocalServer.Data.Device? dev)
        {
            SelectDevice(dev);
        }

        void SelectDevice(LocalServer.Data.Device? dev)
        {
            List<LocalServer.Data.Device>? devs = dev == null ? DeviceRequest.GetAllLiveDevicesAsync(null).Result 
                : DeviceRequest.GetChildrenOfDevicesAsync(null, dev.GetId()).Result;
           if(devs != null && devs.Count > 0)
           {
                System.Windows.Controls.ComboBox cb = new System.Windows.Controls.ComboBox()
                {
                    ItemsSource = devs, Tag = dev
                };
                cb.SelectionChanged += Cb_SelectionChanged;
                mainSt.Children.Add(cb);
           }
        }

        private void Cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0 )
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                LocalServer.Data.Device dev = (LocalServer.Data.Device)e.AddedItems[0];
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                while (mainSt.Children.Count > 0 && mainSt.Children[mainSt.Children.Count-1] != sender)
                    mainSt.Children.RemoveAt(mainSt.Children.Count-1);
                SelectDevice(dev);
                if (DeviceSelected != null)
                    DeviceSelected(sender, e);
            }
        }
    }
}
