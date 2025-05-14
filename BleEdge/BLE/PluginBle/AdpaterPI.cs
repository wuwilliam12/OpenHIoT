using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InTheHand.Bluetooth;
using Microsoft.Extensions.Options;
using OpenHIoT.BleEdge.BLE.Ble32Feet;
using OpenHIoT.BleEdge.MQTT;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Extensions;
using Plugin.BLE.Windows;
using Windows.ApplicationModel.Email.DataProvider;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace OpenHIoT.BleEdge.BLE.PluginBle
{
    public class AdpaterPI : IBleCentral
    {
        CancellationTokenSource tokenSource;
        CancellationToken cancellationToken;

        List<Plugin.BLE.Windows.Device> discovereds;
        private readonly IBluetoothLE bluetoothLE;
         private readonly Plugin.BLE.Abstractions.Contracts.IAdapter adapter;

        public event DeviceFoundEventHandler DeviceFound;
        public event BleNotifiedEventHandler GattNotified;

        Queue<ulong> actions;
        bool busy;
        public bool Busy
        {
            get { return busy; }
            set
            {
                busy = value;
                if (!busy)
                {
                    if (actions.Count > 0)
                    {
                        ulong action = actions.Dequeue();
                        switch ((IBleCentral.BleAction)(byte)action)
                        {
                            case IBleCentral.BleAction.Accept:
                                Task.Run(() => AcceptDevice(action >> 8));
                                break;
                            case IBleCentral.BleAction.Scan:
                                Task.Run(() => ScanAndConnect());
                                break;
                        }
                    }
                }
            }
        }


        public AdpaterPI()
        {
            actions = new Queue<ulong>();
     //       new_devs =  new List<Device>();
            discovereds = new List<Plugin.BLE.Windows.Device>();
            bluetoothLE = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;

      //      var ble = Mvx.Resolve<IBluetoothLE>();
      //      adapter = Mvx.Resolve<IAdapter>();

            adapter.DeviceConnected += Adapter_DeviceConnected;
            adapter.DeviceDisconnected += Adapter_DeviceDisconnected;
            adapter.DeviceConnectionLost += Adapter_DeviceConnectionLost;
            adapter.DeviceConnectionError += Adapter_DeviceConnectionError;
            adapter.DeviceDiscovered += Adapter_DeviceDiscovered;
        //    this.writer = writer;
        }

        private void Adapter_DeviceDiscovered(object? sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            discovereds.Add((Plugin.BLE.Windows.Device)e.Device);
        }

        private void Adapter_DeviceConnectionError(object? sender, Plugin.BLE.Abstractions.EventArgs.DeviceErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Adapter_DeviceConnectionLost(object? sender, Plugin.BLE.Abstractions.EventArgs.DeviceErrorEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Adapter_DeviceDisconnected(object? sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            Product.Device? dev = IMqttClient.Instance.Devices.FirstOrDefault(x => x.Mac == e.Device.Id.ToHexBleAddress());
            if (dev != null)
            {
                IMqttClient.Instance.PublishDeviceDeathMessage(dev.Id);
                dev.ClearStatus((int)LocalServer.Data.DeviceStatus.Connected);
            }            
        }

        private void Adapter_DeviceConnected(object? sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {

        }

        public async Task AcceptDevice(ulong mac_id)
        {
            try
            {
                if (Busy)
                {
                    actions.Enqueue((mac_id << 8) + (ulong)IBleCentral.BleAction.Accept);
                    return;
                }
                Guid guid = ConvertMacToGuid(mac_id);
                Plugin.BLE.Windows.Device dev = (Plugin.BLE.Windows.Device)await adapter.ConnectToKnownDeviceAsync(guid, new Plugin.BLE.Abstractions.ConnectParameters(), cancellationToken);
                if (dev.State == Plugin.BLE.Abstractions.DeviceState.Connected)
                {
                    Console.WriteLine($"Connected to {Device.ToMacString(mac_id)}");

                    var device_pi = IMqttClient.Instance.Devices.FirstOrDefault(x => x.Mac == dev.Id.ToHexBleAddress());
                    if (device_pi == null)
                        device_pi = new DevicePI();
                    Busy = true;
                    if (!await ((DevicePI)device_pi).Connect(dev))
                    {
                        dev.DisconnectInternal();
                        Console.WriteLine("Failed to Connect");
                        Busy = false;
                        return;
                    }
                    Busy = false;
                    await IMqttClient.Instance.AddNewDevice((DevicePI)device_pi);
                    Console.WriteLine("Connected");
                }
            }
            catch (Exception ex)
            {
            }

            //throw new NotImplementedException();
        }

        public Task PublishDeviceBirthMessage()
        {
            throw new NotImplementedException();
        }

        public Task RemoveDevice(ulong mac_id)
        {
            throw new NotImplementedException();
        }

        public async Task ScanAndConnect()
        {
            if (Busy)
            {
                actions.Enqueue((ulong)IBleCentral.BleAction.Scan);
                return;
            }
            Busy = true;
            tokenSource = new CancellationTokenSource();
            cancellationToken = tokenSource.Token;

            Console.WriteLine("Start Scan");
            List<BLE.Device> new_devs = new List<Device>();
            discovereds.Clear();
        //    List<Plugin.BLE.Windows.Device> discoveredDevices = new();
       //     await adapter.StartScanningForDevicesAsync(new Plugin.BLE.Abstractions.ScanFilterOptions(),null,false,cancellationToken);
            await adapter.StartScanningForDevicesAsync();
            if (cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Scan canceled");
                return;
            }
            foreach (var discoveredDevice in discovereds)
            {
                var dev = IMqttClient.Instance.Devices.FirstOrDefault(x => x.Id == discoveredDevice.Id.ToBleAddress());
                if (dev != null)
                    await dev.Connect((Plugin.BLE.Windows.Device)discoveredDevice);
                else
                {
                    Product.Product? p = Product.Products.Instance.FirstOrDefault(x => x.Name == discoveredDevice.Name);
                    if (p != null)
                    {
                        new_devs.Add(new BLE.Device()
                        {
                            Name = discoveredDevice.Name,
                            Mac = discoveredDevice.Id.ToHexBleAddress()
                        });
                    }
                }
            }

            if (new_devs.Count > 0)
            {
                Console.WriteLine($"Found {new_devs.Count} devices. ");
                foreach (var dev in new_devs)
                    Console.WriteLine($"  {dev.Name}: {dev.Mac}");
                await IMqttClient.Instance.PublishNewDevicesMessage(new_devs);
                Console.WriteLine("Published new devices message.");

            }
            Busy = false;
        }

        public async Task<bool> Start()
        {
            await ScanAndConnect();
            return true;
        }

        public void StopScan()
        {

            throw new NotImplementedException();
        }
        public static Guid ConvertMacToGuid(ulong mac)
        {
            byte[] guidBytes = new byte[16]; // GUID is 16 bytes
            byte[] bs = BitConverter.GetBytes(mac);
            // Store MAC address in first 6 bytes
            for (int i = 0; i < 6; i++)
            {
                guidBytes[10+i] = bs[5-i];
                //guidBytes[i] = (byte)((mac >> ((5 - i) * 8)) & 0xFF);
            }

            return new Guid(guidBytes);
        }
    }
}
