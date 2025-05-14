using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using System.Text.Json.Serialization;
using OpenHIoT.LocalServer;
using System.IO;
using OpenHIoT.Client.Requests;
using System.Windows.Controls;
using OpenHIoT.LocalServer.Data.SampleDb;

namespace OpenHIoT.Client
{

    public class ClientSetting
    {
        static ClientSetting setting;
        public static ClientSetting Setting
        {
            get
            {
                if (setting == null)
                    setting = ReadSetting();
                return setting;
            }
        }

        public static ClientSetting  ReadSetting()
        {
            string fn = $"{ValueDataType.GetDataParentDirectory()}dat\\client\\client_setting.json";
            if (File.Exists(fn))
            {
                string txt = File.ReadAllText(fn);
                setting = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientSetting>(txt);
            }

            if (setting == null)
                setting = new ClientSetting();

            return setting;
        }

        public ServerCs? Severs { get; set; }
        
        public int Id { get; set; }

        public  void Save()
        {


        }

        public class Server
        {
            public string? Ip { get; set; }
            public int? HttpPort { get; set; }
            public int? MqttPort { get; set; }
        }
    }

}
