using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpenHIoT.BleEdge.Product.Processors
{
    public enum DpType
    {
        RAW = 0,
        BOOLEAN = 1,
        INT = 2,
        STRING = 3,
        ENUM = 4
    }
    public enum DpAction
    {
        ARM_DOWN_PERCENT = 9,
        ARM_UP_PERCENT = 15,
        CLICK_SUSTAIN_TIME = 10,
        TAP_ENABLE = 17,
        MODE = 8,
        INVERT_SWITCH = 11,
        TOGGLE_SWITCH = 2,
        CLICK = 101,
        PROG = 121
    }
    public enum Coder
    {
        FUN_SENDER_DEVICE_INFO = 0,
        FUN_SENDER_PAIR = 1,
        FUN_SENDER_DPS = 2,
        FUN_SENDER_DEVICE_STATUS = 3,
        FUN_RECEIVE_TIME1_REQ = 32785,
        FUN_RECEIVE_DP = 32769
    }


    public class SecretKeyManager
    {
        private byte[] loginKey;
        private Dictionary<int, byte[]> keys;

        public SecretKeyManager(byte[] loginKey)
        {
            this.loginKey = loginKey;
            keys = new Dictionary<int, byte[]>
            {
                { 4, MD5Hash(this.loginKey) }
            };
        }

        private byte[] MD5Hash(byte[] input)
        {
            using (MD5 md5 = MD5.Create())
            {
                return md5.ComputeHash(input);
            }
        }

        public byte[] Get(int securityFlag)
        {
            keys.TryGetValue(securityFlag, out byte[] key);
            return key;
        }

        public void SetSrand(byte[] srand)
        {
            byte[] combined = new byte[loginKey.Length + srand.Length];
            Buffer.BlockCopy(loginKey, 0, combined, 0, loginKey.Length);
            Buffer.BlockCopy(srand, 0, combined, loginKey.Length, srand.Length);

            keys[5] = MD5Hash(combined);
        }
    }

    public class DeviceInfoResp
    {
        public bool Success { get; private set; }
        public string DeviceVersion { get; private set; }
        public string ProtocolVersion { get; private set; }
        public byte Flag { get; private set; }
        public byte IsBind { get; private set; }
        public byte[] Srand { get; private set; }

        public DeviceInfoResp()
        {
            Success = false;
        }

        public void Parse(byte[] raw)
        {
            if (raw.Length < 46) // Ensure raw data is long enough
            {
                Success = false;
                return;
            }

            // Extract data using specific byte offsets
            byte deviceVersionMajor = raw[0];
            byte deviceVersionMinor = raw[1];
            byte protocolVersionMajor = raw[2];
            byte protocolVersionMinor = raw[3];
            Flag = raw[4];
            IsBind = raw[5];
            Srand = new byte[6];
            Array.Copy(raw, 6, Srand, 0, 6); // Copy srand from raw
            byte hardwareVersionMajor = raw[12];
            byte hardwareVersionMinor = raw[13];
            byte[] authKeyBytes = new byte[32];
            Array.Copy(raw, 14, authKeyBytes, 0, 32);

            // Convert authKey to a hexadecimal string
            string authKey = BitConverter.ToString(authKeyBytes).Replace("-", "").ToLower();

            // Set formatted versions
            DeviceVersion = $"{deviceVersionMajor}.{deviceVersionMinor}";
            ProtocolVersion = $"{protocolVersionMajor}.{protocolVersionMinor}";

            // Calculate the protocol number and validate
            int protocolNumber = protocolVersionMajor * 10 + protocolVersionMinor;
            if (protocolNumber < 20)
            {
                Success = false;
                return;
            }

            Success = true;
        }
    }

    public class Ret
    {
        public byte SecurityFlag { get; private set; }
        public byte[] Iv { get; private set; }
        public dynamic Code { get; private set; }
        public DeviceInfoResp Resp { get; private set; }
        private byte[] Raw { get; set; }
        private int Version { get; set; }

        public Ret(byte[] raw, int version)
        {
            Raw = raw;
            Version = version;
        }

        public void Parse(byte[] secretKey)
        {
            if (Raw.Length < 17) // Ensure raw data has at least enough for iv
            {
                throw new ArgumentException("Invalid raw data length.");
            }

            SecurityFlag = Raw[0];
            Iv = new byte[16];
            Array.Copy(Raw, 1, Iv, 0, 16);
            byte[] encryptedData = new byte[Raw.Length - 17];
            Array.Copy(Raw, 17, encryptedData, 0, encryptedData.Length);

            // Assuming decrypt method is implemented elsewhere
            byte[] decryptedData = AesUtils.Decrypt(encryptedData, Iv, secretKey);

            // Unpack the decrypted data
            int sn = BitConverter.ToInt32(decryptedData, 0);
            int snAck = BitConverter.ToInt32(decryptedData, 4);
            ushort code = BitConverter.ToUInt16(decryptedData, 8);
            ushort length = BitConverter.ToUInt16(decryptedData, 10);

            if (decryptedData.Length < 12 + length)
            {
                throw new ArgumentException("Invalid decrypted data length.");
            }

            byte[] rawData = new byte[length];
            Array.Copy(decryptedData, 12, rawData, 0, length);

            // Handle code and response
            Resp = null;
            try
            {
                Code = (Coder)code; // Assuming Coder.Parse is a method to handle the code logic
            }
            catch
            {
                Code = code; // Fallback to raw code
            }

            if (Code.Equals(Coder.FUN_SENDER_DEVICE_INFO))
            {
                var resp = new DeviceInfoResp();
                resp.Parse(rawData); // Parse the device info response
                Resp = resp;
            }
        }
    }

    public class BleReceiver
    {
        private int LastIndex { get; set; }
        private int DataLength { get; set; }
        private int CurrentLength { get; set; }
        private List<byte> Raw { get; set; }
        private int Version { get; set; }
        private SecretKeyManager SecretKeyManager { get; set; }

        public BleReceiver(SecretKeyManager secretKeyManager)
        {
            LastIndex = 0;
            DataLength = 0;
            CurrentLength = 0;
            Raw = new List<byte>();
            Version = 0;
            SecretKeyManager = secretKeyManager;
        }

        public int Unpack(byte[] arr)
        {
            int i = 0;
            int packetNumber = 0;

            // Parse packetNumber
            while (i < 4 && i < arr.Length)
            {
                byte b = arr[i];
                packetNumber |= (b & 255) << (i * 7);
                if (((b >> 7) & 1) == 0)
                {
                    break;
                }
                i++;
            }

            int pos = i + 1;

            // Handle first packet (packetNumber == 0)
            if (packetNumber == 0)
            {
                DataLength = 0;

                while (pos <= i + 4 && pos < arr.Length)
                {
                    byte b2 = arr[pos];
                    DataLength |= (b2 & 255) << (((pos - 1) - i) * 7);
                    if (((b2 >> 7) & 1) == 0)
                    {
                        break;
                    }
                    pos++;
                }

                CurrentLength = 0;
                LastIndex = 0;

                if (pos == i + 5 || arr.Length < pos + 2)
                {
                    return 2; // Indicate incomplete data
                }

                Raw.Clear();
                pos++;
                Version = (arr[pos] >> 4) & 15;
                pos++;
            }

            // Add data if packetNumber is valid
            if (packetNumber == 0 || packetNumber > LastIndex)
            {
                byte[] data = new byte[arr.Length - pos];
                Array.Copy(arr, pos, data, 0, data.Length);
                CurrentLength += data.Length;
                LastIndex = packetNumber;
                Raw.AddRange(data);

                if (CurrentLength < DataLength)
                {
                    return 1; // Indicate more data expected
                }

                return CurrentLength == DataLength ? 0 : 3; // Return status
            }

            return 3; // Indicate invalid state
        }

        public Ret ParseDataReceived(byte[] arr)
        {
            int status = Unpack(arr);
            if (status == 0)
            {
                byte securityFlag = Raw[0];
                byte[] secretKey = SecretKeyManager.Get(securityFlag);

                Ret ret = new Ret(Raw.ToArray(), Version);
                ret.Parse(secretKey);

                return ret;
            }

            return null; // Return null for incomplete or invalid data
        }
    }

    public static class AesUtils
    {
        public static byte[] Decrypt(byte[] data, byte[] iv, byte[] key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;

                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    return PerformCryptography(data, decryptor);
                }
            }
        }

        public static byte[] Encrypt(byte[] data, byte[] iv, byte[] key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;

                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    return PerformCryptography(data, encryptor);
                }
            }
        }

        private static byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
        {
            using (var memoryStream = new System.IO.MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();
                return memoryStream.ToArray();
            }
        }
    }


    public static class CrcUtils
    {
        public static ushort Crc16(byte[] data)
        {
            ushort crc = 0xFFFF; // Initialize CRC to 0xFFFF

            foreach (byte b in data)
            {
                crc ^= (ushort)(b & 0xFF); // XOR byte with current CRC value
                for (int i = 0; i < 8; i++) // Process 8 bits
                {
                    bool tmp = (crc & 1) != 0; // Check if the least significant bit is 1
                    crc >>= 1; // Shift right by 1
                    if (tmp)
                    {
                        crc ^= 0xA001; // XOR with the polynomial value
                    }
                }
            }

            return crc;
        }
    }


    public static class TuyaDataPacket
    {
        public static byte[] PrepareCrc(uint snAck, uint ackSn, ushort code, byte[] inp, ushort inpLength)
        {
            // Combine inputs into a byte array
            byte[] raw = new byte[12 + inp.Length];
            BitConverter.GetBytes(snAck).CopyTo(raw, 0);
            BitConverter.GetBytes(ackSn).CopyTo(raw, 4);
            BitConverter.GetBytes(code).CopyTo(raw, 8);
            BitConverter.GetBytes(inpLength).CopyTo(raw, 10);
            Array.Copy(inp, 0, raw, 12, inp.Length);

            // Calculate CRC
            ushort crc = CrcUtils.Crc16(raw);

            // Append CRC to the raw data
            byte[] crcBytes = BitConverter.GetBytes(crc);
            byte[] result = new byte[raw.Length + 2];
            Array.Copy(raw, result, raw.Length);
            Array.Copy(crcBytes, 0, result, raw.Length, 2);

            return result;
        }

        public static byte[] GetRandomIv()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] iv = new byte[16];
                rng.GetBytes(iv);
                return iv;
            }
        }

        public static byte[] EncryptPacket(byte[] secretKey, byte securityFlag, byte[] iv, byte[] data)
        {
            // Pad the data to make its length a multiple of 16
            int paddingNeeded = 16 - (data.Length % 16);
            if (paddingNeeded > 0)
            {
                Array.Resize(ref data, data.Length + paddingNeeded);
            }

            // Encrypt the data
            byte[] encryptedData = AesUtils.Encrypt(data, iv, secretKey);

            // Prepare the output byte array
            byte[] output = new byte[1 + iv.Length + encryptedData.Length];
            output[0] = securityFlag; // Add security flag
            Array.Copy(iv, 0, output, 1, iv.Length); // Add IV
            Array.Copy(encryptedData, 0, output, 1 + iv.Length, encryptedData.Length); // Add encrypted data

            return output;
        }
    }

    public class XRequest
    {
        private const int GattMtu = 20;

        private uint SnAck { get; set; }
        private uint AckSn { get; set; }
        private Enum Code { get; set; } // Replace 'Enum' with the actual type for 'code'
        private byte SecurityFlag { get; set; }
        private byte[] SecretKey { get; set; }
        private byte[] Iv { get; set; }
        private byte[] Input { get; set; }

        public XRequest(uint snAck, uint ackSn, Enum code, byte securityFlag, byte[] secretKey, byte[] iv, byte[] input)
        {
            SnAck = snAck;
            AckSn = ackSn;
            Code = code;
            SecurityFlag = securityFlag;
            SecretKey = secretKey;
            Iv = iv;
            Input = input;
        }

        private List<byte[]> SplitPacket(int protocolVersion, byte[] data)
        {
            var output = new List<byte[]>();
            int packetNumber = 0;
            int pos = 0;
            int length = data.Length;

            while (pos < length)
            {
                var b = new List<byte>();

                // Add packet number
                b.Add((byte)packetNumber);

                if (packetNumber == 0)
                {
                    // Add length and protocol version
                    b.Add((byte)length);
                    b.Add((byte)(protocolVersion << 4));
                }

                // Add sub-data
                int subDataLength = Math.Min(GattMtu - b.Count, length - pos);
                b.AddRange(data[pos..(pos + subDataLength)]);
                pos += subDataLength;

                output.Add(b.ToArray());
                packetNumber++;
            }

            return output;
        }

        public List<byte[]> Pack()
        {
            // Prepare data with CRC and encryption
            byte[] data = TuyaDataPacket.PrepareCrc(SnAck, AckSn, Convert.ToUInt16(Code), Input, (ushort)Input.Length);
            byte[] encryptedData = TuyaDataPacket.EncryptPacket(SecretKey, SecurityFlag, Iv, data);

            // Split into packets
            return SplitPacket(2, encryptedData);
        }
    }

    public class FingerBot
    {
        private const string NotifUuid = "00002b10-0000-1000-8000-00805f9b34fb";
        private const string CharUuid = "00002b11-0000-1000-8000-00805f9b34fb";

        private string Mac { get; set; }
        private byte[] Uuid { get; set; }
        private byte[] DevId { get; set; }
        private byte[] LoginKey { get; set; }

        private SecretKeyManager SecretKeyManager { get; set; }
        private BleReceiver BleReceiver { get; set; }
        private uint SnAck { get; set; }
        //     private GattAdapter Adapter { get; set; } // Placeholder for BLE adapter
        //     private GattDevice Device { get; set; } // Placeholder for BLE device

        public FingerBot(string mac, string localKey, string uuid, string devId)
        {
            Mac = mac;
            Uuid = System.Text.Encoding.UTF8.GetBytes(uuid);
            DevId = System.Text.Encoding.UTF8.GetBytes(devId);
            LoginKey = System.Text.Encoding.UTF8.GetBytes(localKey.Substring(0, 6));

            SecretKeyManager = new SecretKeyManager(LoginKey);
            BleReceiver = new BleReceiver(SecretKeyManager);

            //      Adapter = new GattAdapter("hci1"); // Replace with actual GATT adapter initialization
            ResetSnAck();
        }

        public void Connect()
        {
            //     Adapter.Start();

            //     Device = Adapter.Connect(Mac, AddressType.Public);
            //     Device.Subscribe(NotifUuid, HandleNotification);

            Console.WriteLine("Connecting...");
            var req = DeviceInfoRequest();
            SendRequest(req);

            while (true)
            {
                Thread.Sleep(10000);
            }
        }

        public uint NextSnAck()
        {
            return ++SnAck;
        }

        public void ResetSnAck()
        {
            SnAck = 0;
        }

        private void HandleNotification(byte[] value)
        {
            var ret = BleReceiver.ParseDataReceived(value);
            if (ret == null) return;

            if (ret.Code.Equals(Coder.FUN_SENDER_DEVICE_INFO))
            {
                SecretKeyManager.SetSrand(ret.Resp.Srand);

                Console.WriteLine("Pairing...");
                var req = PairRequest();
                SendRequest(req);
            }
            else if (ret.Code.Equals(Coder.FUN_SENDER_PAIR))
            {
                while (true)
                {
                    Console.WriteLine("Fingering...");
                    var req = SendDps(new List<object>()); // Replace `object` with actual DPS structure
                    SendRequest(req);
                    Thread.Sleep(4000);
                }
            }
        }

        private void SendRequest(XRequest xrequest)
        {
            var packets = xrequest.Pack();
            foreach (var cmd in packets)
            {
                //           Device.CharWrite(CharUuid, cmd, false); // Replace with actual char_write implementation
            }
        }

        private XRequest DeviceInfoRequest()
        {
            var inp = new byte[0];
            var iv = TuyaDataPacket.GetRandomIv();
            byte securityFlag = 4;
            var secretKey = SecretKeyManager.Get(securityFlag);

            uint snAck = NextSnAck();
            return new XRequest(snAck, 0, Coder.FUN_SENDER_DEVICE_INFO, securityFlag, secretKey, iv, inp);
        }

        private XRequest PairRequest()
        {
            byte securityFlag = 5;
            var secretKey = SecretKeyManager.Get(securityFlag);
            var iv = TuyaDataPacket.GetRandomIv();

            var inp = new List<byte>();
            inp.AddRange(Uuid);
            inp.AddRange(LoginKey);
            inp.AddRange(DevId);

            for (int i = 0; i < 22 - DevId.Length; i++)
            {
                inp.Add(0x00);
            }

            uint snAck = NextSnAck();
            return new XRequest(snAck, 0, Coder.FUN_SENDER_PAIR, securityFlag, secretKey, iv, inp.ToArray());
        }

        private XRequest SendDps(List<object> dps)
        {
            byte securityFlag = 5;
            var secretKey = SecretKeyManager.Get(securityFlag);
            var iv = TuyaDataPacket.GetRandomIv();

            var dpsList = new List<object>
        {
            new object[] { 8, DpType.ENUM, 0 },
            new object[] { DpAction.ARM_DOWN_PERCENT, DpType.INT, 80 },
            new object[] { DpAction.ARM_UP_PERCENT, DpType.INT, 0 },
            new object[] { DpAction.CLICK_SUSTAIN_TIME, DpType.INT, 0 },
            new object[] { DpAction.CLICK, DpType.BOOLEAN, true }
        };

            var raw = new List<byte>();
            foreach (var dp in dpsList)
            {
                // Replace parsing logic for dp depending on your C# enums or types
            }

            uint snAck = NextSnAck();
            return new XRequest(snAck, 0, Coder.FUN_SENDER_DPS, securityFlag, secretKey, iv, raw.ToArray());
        }

        public void Disconnect()
        {
            //        Adapter.Stop();
        }
    }
}
