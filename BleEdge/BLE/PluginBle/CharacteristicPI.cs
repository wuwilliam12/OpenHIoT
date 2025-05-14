using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenHIoT.BleEdge.BLE.Ble32Feet;
using OpenHIoT.BleEdge.MQTT;
using OpenHIoT.BleEdge.Product;
using Plugin.BLE;
using Plugin.BLE.Windows;
using Characteristic = OpenHIoT.BleEdge.Product.Characteristic;

namespace OpenHIoT.BleEdge.BLE.PluginBle
{
    public class CharacteristicPI
    {
        DevicePI dev;
        Characteristic chara;

        public async Task Start(DevicePI _dev, Characteristic _chara, Plugin.BLE.Windows.Characteristic chara_pi)
        {
            dev = _dev;
            chara = _chara;
            chara_pi.ValueUpdated += Characteristic_ValueUpdated;
            await chara_pi.StartUpdatesAsync();
        }

        private void Characteristic_ValueUpdated(object? sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs e)
        {
            if (chara.ProcessData(e.Characteristic.Value))
                IMqttClient.Instance.PublishDeviceDataMessage(dev.Id, dev.Channels);
        }
    }

    public class CharacteristicsPI : List<CharacteristicPI>
    {
       
    }
}
