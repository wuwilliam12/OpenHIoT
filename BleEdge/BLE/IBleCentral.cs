using OpenHIoT.BleEdge.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHIoT.BleEdge.BLE
{
    public delegate void BleNotifiedEventHandler(byte[] val);
    public delegate void DeviceFoundEventHandler(BleEdge.Product.Device device);
//    public delegate void StartedEventHandler();

    public interface IBleCentral
    {
        public enum BleAction { Scan = 0, StopScan = 1, Accept = 2, Remove = 3  }

        public static IBleCentral BlePort { get; set; }

        event DeviceFoundEventHandler DeviceFound;
        event BleNotifiedEventHandler GattNotified;

        Task<bool> Start();
        Task ScanAndConnect();
        Task AcceptDevice(ulong mac_id);
        Task RemoveDevice(ulong mac_id);

        void StopScan();
        Task PublishDeviceBirthMessage();
//        List<Device> Get
   //     Task StartScanNewAsync();
  //      Task StopScanAsync();


        /* 
         Connect(byte[] mac_addr);
             DiscoverService();
             DiscoverCharacteristic();
             Subcribe();
             WriteGatt();
             ReadGatt();
        */
    }



}
