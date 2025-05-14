using OpenHIoT.Client.Requests;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using TreeGridView.Common;

namespace OpenHIoT.Client.Pages.Device
{
    /// <summary>
    /// Interaction logic for DevicePage.xaml
    /// </summary>
    public partial class DeviceMainPage : Page
    {
        public enum DeviceType{
            BleEdge = 3
        };

        TreeGridModel model;
        System.Timers.Timer refreshTimer;
        IDeviceCntl? deviceCntl;
        public DeviceMainPage()
        {
            InitializeComponent();
            model = new TreeGridModel();
            UpdateItems();
            refreshTimer = new System.Timers.Timer();
            refreshTimer.Interval = 1000; 
            refreshTimer.Elapsed += RefreshTimer_Elapsed; 
            refreshTimer.Start();
        }

        private async void RefreshTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if(deviceCntl != null)
            {
               await deviceCntl.GetDevice().UpdateChanelValues();
            }
        }

        void UpdateItems()
        {
            List<LocalServer.Data.Device>? ds = DeviceRequest.GetAllDevicesAsync(null).Result;
            if (ds != null)
            {
                List<DeviceItem> items_ne = new List<DeviceItem> ();  // no edge devices 
                // Create the model
                model = new TreeGridModel();
                foreach (var dev in ds)
                {
                    DeviceItem item = new DeviceItem()
                    {
                        Device = dev,
                    };
                    if (dev.PhyId != null && ((byte)dev.PhyId == (byte)OpenHIoTIdType.Client))
                        continue;
                    DeviceItem new_item = new DeviceItem()
                    {
                        Device = dev,
                    };
                    if (dev.Parent == null)
                    {
                        model.Add(new_item);
                        new_item.EId = new_item.Id;
                    }
                    else
                        items_ne.Add(new_item);
                }
                foreach(var item in items_ne)
                {
#pragma warning disable CS8629 // Nullable value type may be null.
                    DeviceItem? p = GetItemById((ulong)item.Device.Parent);
#pragma warning restore CS8629 // Nullable value type may be null.
                    if (p != null)
                    {
                        p.Children.Add(item);
                        item.EId = item.Id;
                    }
                    else
                    {
                        item.Device.ClearStatus((int)DeviceStatus.Connected);
                        model.Add(item);
                    }

                }
                treeviewList.ItemsSource = model.FlatModel;
            }
        }

        public DeviceItem? GetItemByLId( uint lid)
        {
            foreach (DeviceItem item in model)
            {
                DeviceItem? res = item.GetItem(lid);
                if (res != null) return res;
            }
            return null;
        }
        public DeviceItem? GetItemById(ulong id)
        {
            foreach (DeviceItem item in model)
            {
                DeviceItem? res = item.GetItem(id);
                if (res != null) return res;
            }
            return null;
        }
        public DeviceItem? GetItem( LocalServer.Data.Device dev)
        {
            foreach (DeviceItem item in model)
            {
                DeviceItem? res = item.GetItem(dev);
                if (res != null) return res;
            }
            return null;
        }

        public void UpdateItem(LocalServer.Data.Device dev)
        {
            DeviceItem? item = GetItem( dev);
            if(item != null)
            {
                this.Dispatcher.Invoke(() => {
                   item.Device = dev; 
                });
            }
        }

        private void refreshBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateItems();
        }

        private void delButton_Click(object sender, RoutedEventArgs e)
        {
            if (treeviewList.SelectedItem == null) return;
            DeviceItem item = (DeviceItem)treeviewList.SelectedItem;
            if(item.Device.Connected )
            {
                if(System.Windows.MessageBox.Show("Disconnect/Disable the device?", "?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                 //   item.Device.Connected = false;

                }                 
            }
            else
            {
                if (System.Windows.MessageBox.Show("Drop the device from Db?", "?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                   // item.Device.Deleted = true;
                }
                    
            }
        }

        private void treeviewList_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (detailCntl.Children.Count > 0)
            {
                ((IDeviceCntl)detailCntl.Children[0]).CloseCntl();
                detailCntl.Children.Clear();
            }
            DeviceItem? item = (DeviceItem)treeviewList.SelectedItem;
            if (item != null)
            {
                if (item.Device.Connected && item.Device.Prod != null)
                {
                    deviceCntl = null;
                    switch ((DeviceType)item.Device.Prod)
                    {
                        case DeviceType.BleEdge:
                            deviceCntl = new Products.BleEdge.BleEdgeCntl();
                            break;

                    };
                    if (deviceCntl != null)
                    {
                        deviceCntl.SetDevice(item.Device);
                        deviceCntl.GetDevice().Init();
                        detailCntl.Children.Add((System.Windows.Controls.UserControl)deviceCntl);
                    }
                }
            }
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {

        }
    }
}

