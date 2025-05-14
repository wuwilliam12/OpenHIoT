using InTheHand.Bluetooth;
using OpenHIoT.BleEdge.MQTT;
using OpenHIoT.BleEdge.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenHIoT.BleEdge.BLE.Ble32Feet
{
    public class Characteristic32F
    {
        Device32F dev;
        Characteristic chara;
        GattCharacteristic gatt;

        public Characteristic32F()
        {

        }
        public async Task Start(Device32F _dev,  Characteristic _chara, GattCharacteristic _gatt)
        {
            dev = _dev;
            chara = _chara;
            gatt = _gatt;
            if ((gatt.Properties & GattCharacteristicProperties.Notify) != 0)
            {
                gatt.CharacteristicValueChanged += Characteristic_CharacteristicValueChanged;
                await gatt.StartNotificationsAsync();
            }
        }
        private void Characteristic_CharacteristicValueChanged(object? sender, GattCharacteristicValueChangedEventArgs e)
        {
            if (e.Value != null)
            {
                if(chara.ProcessData(e.Value))
                    IMqttClient.Instance.PublishDeviceDataMessage(dev.Id, dev.Channels);
            }
           // GattCharacteristic gatt = sender as GattCharacteristic;
           //  gatt.Uuid.Value.
           //Debug.WriteLine($"Battery Level has changed to {e.Value[0]} %");
        }
    }
}
