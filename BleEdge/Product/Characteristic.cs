using OpenHIoT.BleEdge.BLE.Ble32Feet;
using Newtonsoft.Json;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.HiotMsg;
using OpenHIoT.LocalServer.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenHIoT.BleEdge.Product;
using OpenHIoT.LocalServer.Data.SampleDb;

namespace OpenHIoT.BleEdge.Product
{

    public class Characteristic
    {
        public delegate bool ProcessDelegate(byte[] bs);

        public string? Name { get; set; }
        public string UUID { get; set; }
            
        public int Property { get; set; }
        public Channels Channels { get; set; }

        public string? SetParaFunc { get; set; }
        public string? ProcessFunc { get; set; }
        public int[] ChIds { get; set; }

        protected ProcessDelegate? procHandle;

        /*
        public void  Init (Channels chs)
        {
            Channels = chs;
        }
        */
        public void SetProcessFunc(ProcessDelegate proc)
        {
            procHandle = proc;
        }

        public bool ProcessData(byte[] bs)
        {
            if (procHandle != null)
                return procHandle(bs);
            else
            {
                if (Channels != null && Channels.Count == 1)
                {
                    OpenHIoT.BleEdge.Product.Channel ch = Channels[0];
                    int offset = 0;
                    try
                    {
                        ch.Value = ValueDataType.ConvertBytesToObject(ch.DType, bs, ref offset, bs.Length);
                        return true;

                    }
                    catch (Exception e1)
                    {
                    }
                }
                return false;
            }
        }

    }


    public class Characteristics : List<Characteristic>
    {
    }

    public class Service
    {
        public string? Name { get; set; }
        public string UUID { get; set; }
        public List<Characteristic> Characteristics { get; set; }

    }

    public class Services : List<Service>
    {

    }
}
