
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using OpenHIoT.BleEdge.Product;
using InTheHand.Bluetooth;
using static OpenHIoT.BleEdge.Product.Product;
using Microsoft.Extensions.Options;
using OpenHIoT.BleEdge.MQTT;
using System.Xml.Linq;
using System.Data.SqlTypes;
using Microsoft.AspNetCore.Mvc;
//using Plugin.BLE.Windows;

namespace OpenHIoT.BleEdge.BLE.Ble32Feet
{
    public class Central32F : IBleCentral
    {
        public event DeviceFoundEventHandler DeviceFound;
        public event BleNotifiedEventHandler GattNotified;

        CancellationTokenSource tokenSource;
        CancellationToken cancellationToken;
        List<BLE.Device> new_devs;
        List<BluetoothDevice> new_bles;
        Queue<ulong> actions;
        bool busy;
        public  bool Busy
        {
            get { return busy; }
            set
            {
                busy = value;
                if(!busy)
                {
                    if(actions.Count > 0)
                    {
                        ulong action = actions.Dequeue();
                        switch((IBleCentral.BleAction)(byte)action)
                        {
                            case IBleCentral.BleAction.Accept:
                                Task.Run(() => AcceptDevice( action >> 8 ));
                                break;
                            case IBleCentral.BleAction.Scan:
                                Task.Run(() => ScanAndConnect());
                                break;
                        }
                    }
                }
            }
        }

        public Central32F()
        {
            tokenSource = new CancellationTokenSource();
            cancellationToken = tokenSource.Token;
            new_devs = new List<BLE.Device>();
            new_bles = new List<BluetoothDevice>();
            actions = new Queue<ulong> { };
            Busy = false;
        }


        public async Task AcceptDevice(ulong mac_id)
        {
            if(Busy)
            {
                actions.Enqueue((mac_id << 8) + (ulong)IBleCentral.BleAction.Accept);
                return;
            }

            string mac  = Device.ToMacStringReverse(mac_id);
            BluetoothDevice? dev = new_bles.FirstOrDefault(x => x.Id == mac);
            if (dev != null)
            {            
                if(dev.Gatt.IsConnected)
                {
                    Console.WriteLine($"{Device.ToMacString(mac_id)} already connected ");
                    return;
                }

                Device32F device32F = new Device32F();
//                dev.Gatt.AutoConnect = true;
                Console.WriteLine($"Connect to {Device.ToMacString(mac_id)}");
                Busy = true;
                if (!await device32F.Connect(dev))
                {
                    dev.Gatt.AutoConnect = false;
                    if (dev.Gatt.IsConnected)
                    {
                        dev.Gatt.Disconnect();
                    }
                    Console.WriteLine("Failed to Connect");
                    Busy = false;
                    return;
                }
                Busy = false;
                Device? d = new_devs.FirstOrDefault(x => x.Mac == mac);
                if (d != null)
                    new_devs.Remove(d);
                device32F.Disconnected += Device32F_Disconnected; 
                IMqttClient.Instance.AddNewDevice(device32F);
                Console.WriteLine("Connected");
            }


        }

        private void Device32F_Disconnected(object? sender, EventArgs e)
        {
            if (sender == null) return;
            Device32F device32F = (Device32F)sender;
            IMqttClient.Instance.RemoveDevice(device32F);
        }

        public async Task RemoveDevice(ulong mac_id)
        {

        }

        public void StopScan()
        {
            tokenSource.Cancel();
        }

        public async Task ScanAndConnect()
        {
            if (Busy)
            {
                actions.Enqueue( (ulong)IBleCentral.BleAction.Scan);
                return;
            }
            Busy = true;
            tokenSource = new CancellationTokenSource();
            cancellationToken = tokenSource.Token;

            Console.WriteLine("Start Scan");
            new_devs.Clear();
            new_bles.Clear();
            var options = new RequestDeviceOptions();

            var discoveredDevices = await Bluetooth.ScanForDevicesAsync(options, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Scan canceled");
                return;
            }
            foreach (var discoveredDevice in discoveredDevices)
            {
                var dev = IMqttClient.Instance.Devices.FirstOrDefault(x => x.Mac == discoveredDevice.Id);
                if(dev != null)
                    await dev.Connect(discoveredDevices);
                else
                {
                    Product.Product? p = Products.Instance.FirstOrDefault(x => x.Name == discoveredDevice.Name);
                    if (p != null)
                    {
                        new_devs.Add(new BLE.Device()
                        {
                            Name = discoveredDevice.Name,
                            Mac = discoveredDevice.Id
                        });
                        new_bles.Add(discoveredDevice);
                    }
                }
            }

            if (new_devs.Count > 0)
            {
                Console.WriteLine($"Found {new_devs.Count} devices. ");
                foreach (var dev in new_devs)
                    Console.WriteLine( $"  {dev.Name}: {dev.Mac}");
                await IMqttClient.Instance.PublishNewDevicesMessage(new_devs);
                Console.WriteLine("Published new devices message.");
                
            }
            discoveredDevices = null;
            GC.Collect(GC.MaxGeneration);
            Busy = false;
        }

        public async Task<bool> Start()
        {        
            bool availability = false;
            Bluetooth.AvailabilityChanged += Bluetooth_AvailabilityChanged;
            Bluetooth.AdvertisementReceived += Bluetooth_AdvertisementReceived;
            while (!availability)
            {
                availability = await Bluetooth.GetAvailabilityAsync();
                await Task.Delay(500);
            }
            await ScanAndConnect();
            return true;
        }

        private async void Bluetooth_AvailabilityChanged(object? sender, EventArgs e)
        {
            var current = await Bluetooth.GetAvailabilityAsync();
            Console.Write($"Availability: {current}");
        }

        private void Bluetooth_AdvertisementReceived(object? sender, BluetoothAdvertisingEvent e)
        {
            Console.WriteLine($"Name:{e.Name} Rssi:{e.Rssi}");
        }



        public static string ByteArrayToString(byte[] data)
        {
            if (data == null)
                return "<NULL>";

            StringBuilder sb = new StringBuilder();
            foreach (byte b in data)
            {
                sb.Append(b.ToString("X"));
            }

            return sb.ToString();
        }

        public Task PublishDeviceBirthMessage()
        {
           // throw new NotImplementedException();
           return Task.CompletedTask;
        }
    }
}
