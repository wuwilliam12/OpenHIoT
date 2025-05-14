using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using OpenHIoT.BleEdge.MQTT;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Extensions;

namespace OpenHIoT.BleEdge.BLE.PluginBle
{
    public class DevicePI : BleEdge.Product.Device
    {
        Plugin.BLE.Windows.Device device;
        CharacteristicsPI characteristicPIs;
        public DevicePI()
        {

        }

        public async Task<bool> Connect(Plugin.BLE.Windows.Device _device)
        {
            device = _device;
            Mac = device.Id.ToHexBleAddress();
            Name = device.Name;
            characteristicPIs = new CharacteristicsPI();
            Init();

            var ss = await device.GetServicesAsync();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            foreach (var s in Services)
            {
                var service = await device.GetServiceAsync(GetGuidFromUUId(s.UUID));
                foreach (var c in s.Characteristics)
                {
                    var pi_c = await service.GetCharacteristicAsync(GetGuidFromUUId(c.UUID));
                    if (pi_c != null)
                    {
                        CharacteristicPI characteristicPI = new CharacteristicPI();
                        await characteristicPI.Start(this, c, (Plugin.BLE.Windows.Characteristic)pi_c);
                        characteristicPIs.Add(characteristicPI);
                    }
                }
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            SetStatus((int)LocalServer.Data.DeviceStatus.Connected);
            return true;
        }

        /*
        private void Characteristic_ValueUpdated(object? sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs e)
        {
            if (chara.ProcessData(e.Characteristic.Value))
                IMqttClient.Instance.PublishDeviceDataMessage(Id, Channels);
        }*/
        public static Guid GetGuidFromUUId(string uuid)
        {
            if(uuid.Length == 4)
                return new Guid($"0000{uuid}-0000-1000-8000-00805F9B34FB");
            return new Guid(uuid);
        }
    }
}
