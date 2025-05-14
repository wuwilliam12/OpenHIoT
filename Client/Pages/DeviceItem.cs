using Newtonsoft.Json;
using OpenHIoT.Client.Requests;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TreeGridView.Common;

namespace OpenHIoT.Client.Pages.Device
{
    public interface IDeviceCntl
    {
        Products.Device GetDevice(); 
        void SetDevice(LocalServer.Data.Device device);
        void CloseCntl();
        void ProcessPayloadDataHm(byte[] bs);

    }
    public class DeviceItem : TreeGridElement
    {
        public ulong Id { get; set; }
        public string PhyId
        {
            get
            {
                if (Device.PhyId == null)
                    return "";
                else
                {
                    OpenHIoTIdType openHIoTIdType = ((OpenHIoTIdType)(byte)Device.PhyId);
                    ulong id = (ulong)Device.PhyId >> 8;
                    return $"{openHIoTIdType}: {id.ToString("x")}";
                }
            }
        }
        public SolidColorBrush NameColor
        {
            get {
                if (Device.Connected)
                    return new SolidColorBrush(Colors.Black);
                else return new SolidColorBrush(Colors.Gray);
            }
        }
        LocalServer.Data.Device device;
        public LocalServer.Data.Device Device {
            get { return device; }
            set { device = value ;
                Id = (ulong)device.GetId();
            }
        }
        public uint? SId { get; set; }

        public ulong? EId { get; set; }
        public DeviceItem? GetItem(ulong id)
        {
            if (Id == id)
                return this;
            foreach (DeviceItem item in Children)
            {
                DeviceItem? res = item.GetItem(id);
                if (res != null) return res;
            }
            return null;
        }

        public DeviceItem? GetItem(uint lid)
        {
            if (Device.LId == lid)
                return this;
            foreach (DeviceItem item in Children)
            {
                DeviceItem? res = item.GetItem(lid);
                if (res != null) return res;
            }
            return null;
        }
        public DeviceItem? GetItem( LocalServer.Data.Device dev)
        {
            if (Device.SameAs(dev))
                return this;
            foreach (DeviceItem item in Children)
            {
                DeviceItem? res = item.GetItem(dev);
                if (res != null) return res;
            }
            return null;
        }
    }


}
