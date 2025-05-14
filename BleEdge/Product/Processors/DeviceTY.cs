using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using OpenHIoT.BleEdge.Product;

using System.Text;

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using InTheHand.Bluetooth;
using System.Data.Common;

namespace OpenHIoT.BleEdge.Product.Processors
{
    public class DeviceTY : Device
    {
        public override void LoadSetting(string setting)
        {

        }

        public override void SetDevice(OpenHIoT.BleEdge.Product.Device device)
        {
            base.SetDevice(device);
        }


    }


    public class FingerBit1
    {
        bool sendCommand(
            object advDeviceToUse,//NimBLEAdvertisedDevice* advDeviceToUse,
            string type,  //const char* type,
            int attempts, bool disconnectAfter)
        {
            if (advDeviceToUse == null)
            {
                return false;
            }
            //printAString("Sending command...");
            byte NULL = 0;
            byte[] bArrayPress = { 0x57, 0x01 };
            byte[] bArrayOn = { 0x57, 0x01, 0x01 };
            byte[] bArrayOff = { 0x57, 0x01, 0x02 };
            byte[] bArrayPlugOn = { 0x57, 0x0F, 0x50, 0x01, 0x01, 0x80 };
            byte[] bArrayPlugOff = { 0x57, 0x0F, 0x50, 0x01, 0x01, 0x00 };
            byte[] bArrayOpen = { 0x57, 0x0F, 0x45, 0x01, 0x05, 0xFF, 0x00 };
            byte[] bArrayClose = { 0x57, 0x0F, 0x45, 0x01, 0x05, 0xFF, 0x64 };
            byte[] bArrayPause = { 0x57, 0x0F, 0x45, 0x01, 0x00, 0xFF };
            byte[] bArrayPos = { 0x57, 0x0F, 0x45, 0x01, 0x05, 0xFF, 0 };
            byte[] bArrayGetSettings = { 0x57, 0x02 };
            byte[] bArrayHoldSecs = { 0x57, 0x0F, 0x08, 0 };
            byte[] bArrayBotMode = { 0x57, 0x03, 0x64, 0, 0 };

            byte[] bArrayPressPass = { 0x57, 0x11, NULL, NULL, NULL, NULL };
            byte[] bArrayOnPass = { 0x57, 0x11, NULL, NULL, NULL, NULL, 0x01 };
            byte[] bArrayOffPass = { 0x57, 0x11, NULL, NULL, NULL, NULL, 0x02 };
            byte[] bArrayGetSettingsPass = { 0x57, 0x12, NULL, NULL, NULL, NULL };
            byte[] bArrayHoldSecsPass = { 0x57, 0x1F, NULL, NULL, NULL, NULL, 0x08, NULL };
            byte[] bArrayBotModePass = { 0x57, 0x13, NULL, NULL, NULL, NULL, 0x64, NULL };       // The proper array to use for setting mode with password (firmware 4.9)
        
        return true;
        }
    }




}
