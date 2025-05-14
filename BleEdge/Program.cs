// See https://aka.ms/new-console-template for more information
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using OpenHIoT.BleEdge.BLE;
using OpenHIoT.BleEdge.Product;
using OpenHIoT.BleEdge.MQTT;
using InTheHand.Bluetooth;
using MQTTnet.Protocol;


namespace OpenHIoT.BleEdge
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            EdgeSetting.ReadSetting();

           Products.Instance = Products.GetProducts();

            IMqttClient.Instance = new OpenHIoT.BleEdge.MQTT.HiotMsg.HmEdge();
            IMqttClient.Instance.Devices = OpenHIoT.BleEdge.Product.Devices.GetKnownDevices();
            IMqttClient.Instance.Start();

        //         IBleCentral.BlePort = new OpenHIoT.BleEdge.BLE.Ble32Feet.Central32F();
            IBleCentral.BlePort = new OpenHIoT.BleEdge.BLE.PluginBle.AdpaterPI();

            IBleCentral.BlePort.Start();

            while (true)
            {

            }
      //      var discoveryTask = TestDeviceDiscovery();
      //      discoveryTask.Wait();
        }
        /*
        private static async Task<bool> TestDeviceDiscovery()
        {
            var discoveredDevices = await Bluetooth.ScanForDevicesAsync();
            Console.WriteLine($"found {discoveredDevices?.Count} devices");
            return discoveredDevices?.Count > 0;
        }
//        BluetoothLEScan scan;

   */

    }
}