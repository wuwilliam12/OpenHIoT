using InTheHand.Bluetooth;
using OpenHIoT.BleEdge.Product;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenHIoT.BleEdge.BLE.Ble32Feet
{
    public class Device32F : BleEdge.Product.Device
    {
        public event EventHandler<EventArgs> Disconnected;

        BluetoothDevice? bleDev;
        List<Characteristic32F> characteristic32Fs;
        CancellationTokenSource tokenSource;
        CancellationToken cancellationToken;

        public Device32F()
        {
            tokenSource = new CancellationTokenSource();
            cancellationToken = tokenSource.Token;
            characteristic32Fs = new List<Characteristic32F>();
        }
        private  void BleDev_GattServerDisconnected(object? sender, EventArgs e)
        {
            var device = sender as BluetoothDevice;
            if (device != null)
            {
                tokenSource.Cancel();
                characteristic32Fs.Clear();
                device.GattServerDisconnected -= BleDev_GattServerDisconnected;
                Console.WriteLine($"{device.Id} disconnected");
                if(Disconnected != null)
                    Disconnected(this, new EventArgs());    
                // may need to send death msg
            }
        }

        public override async Task<bool> Connect(object ble_dev)
        {
            bleDev = (BluetoothDevice)ble_dev;
            Mac = bleDev.Id;
            Name = bleDev.Name;
            Init();

            bleDev.GattServerDisconnected += BleDev_GattServerDisconnected;
            try
            {
                await bleDev.Gatt.ConnectAsync();

              //  await bleDev.Gatt.RequestMtuAsync(16);
             //   RSSI = await bleDev.Gatt.ReadRssi();
             //   Console.WriteLine($"  Mtu: {bleDev.Gatt.Mtu}, RSSI: {RSSI}");
                //     await bleDev.Gatt.RequestMtuAsync(517);
                //    Console.WriteLine($"Mtu: {bleDev.Gatt.Mtu}");             

                characteristic32Fs.Clear();

                var servs = await bleDev.Gatt.GetPrimaryServicesAsync().ConfigureAwait(false); 
                if (servs == null || (!bleDev.Gatt.IsConnected) || cancellationToken.IsCancellationRequested) return false;
                foreach (var serv in servs)
                {
                    Service? service = Services.FirstOrDefault(x => x.UUID == serv.Uuid.ToString());
                    if (service == null)
                        continue;
      
                    Console.WriteLine($"  Service :{serv.Uuid}");

                    var cs = await serv.GetCharacteristicsAsync().ConfigureAwait(false);
                    if (cs == null || (!bleDev.Gatt.IsConnected) || cancellationToken.IsCancellationRequested) return false;
                    foreach (var chars in cs)
                    {
                       // BluetoothUuid uuid =  BluetoothUuid.FromShortId( Convert.ToUInt16( Characteristics[0].UUID, 16) );
                       //var chars = serv.GetCharacteristicAsync(uuid);
                    
                        string char_str = $"    {chars.Uuid} Properties:{chars.Properties}";
                        string uuid_str = chars.Uuid.ToString();
                        Characteristic? char_dev = service.Characteristics.FirstOrDefault(x => x.UUID == uuid_str);


                        if (char_dev != null)
                        {
                            Characteristic32F c = new Characteristic32F();
                            await c.Start(this, char_dev, chars);
                            char_str = char_str + $". Added to receive list";
                            characteristic32Fs.Add(c);
                            Console.WriteLine(char_str);

                            var desc = chars.GetDescriptorsAsync().WaitAsync(cancellationToken).Result; ;
                            if (desc == null || (!bleDev.Gatt.IsConnected) || cancellationToken.IsCancellationRequested) continue;
                            foreach (var descriptors in desc)
                            {
                                Console.WriteLine($"    Descriptor:{descriptors.Uuid}");
                                var val2 = await descriptors.ReadValueAsync();
                                if (val2 == null)
                                {
                                    Console.WriteLine("    Read value failed!");
                                    return false;
                                    //  val2 = await descriptors.ReadValueAsync();
                                }
                                if (descriptors.Uuid == GattDescriptorUuids.ClientCharacteristicConfiguration)
                                {
                                    Console.WriteLine($"    Notifying:{val2[0] > 0}");
                                }
                                else if (descriptors.Uuid == GattDescriptorUuids.CharacteristicUserDescription)
                                {
                                    //  Debug.WriteLine($"UserDescription:{Central32F.ByteArrayToString(val2)}");
                                }
                                else
                                {
                                    //  Debug.WriteLine(Central32F.ByteArrayToString(val2));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine ( $"  {ex.ToString()}");
                return false;
            }
            return true;

        }


    }

  
}
