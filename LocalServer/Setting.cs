using Newtonsoft.Json.Serialization;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Services;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OpenHIoT.LocalServer
{
    public class Setting
    {
        static Setting? instance;
        public static Setting Instance {
            get
            {
                if (instance == null)
                    instance = new Setting();
                return instance;
            }
        }
        public int Port { get; set; }
        public int Id { get; set; }
        public bool AutoAccept { get; set; }

        public ulong PhyId
        {
            get
            {
                return ((ulong)Id << 8) + (ulong)OpenHIoTIdType.Server;
            }
        } 

        [JsonIgnore]
        public ISyslogSevice Syslog {  get; set; }


        public Setting()
        {
            Port = 5130;
            AutoAccept = true;
        }


        public SettingDTO GetDTO()
        {
            return new SettingDTO()
            {
                AutoAccept = this.AutoAccept,
            };
        }

        public void SetDTO(SettingDTO dto)
        {
            AutoAccept = dto.AutoAccept;
        }
    }

    public class SettingDTO
    {
        public bool AutoAccept { get; set; }


    }
}
