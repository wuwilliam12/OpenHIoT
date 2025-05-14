using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.VoiceCommands;

namespace OpenHIoT.BleEdge.Product.Processors
{
    public class Device
    {
        protected BleEdge.Product.Device? product;
        public virtual void LoadSetting(string setting)
        {

        }

        public virtual void SetDevice(OpenHIoT.BleEdge.Product.Device prod)
        { 
            product = prod;
        //    foreach (Characteristic c in prod.Characteristics)
        //        c.SetProcessFunc(ProcessData);
        }

  

    }
}
