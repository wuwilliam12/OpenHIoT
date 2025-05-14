using OpenHIoT.LocalServer.Operation;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Security.AccessControl;
//using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace OpenHIoT.MqttSim
{
    public delegate void DeviceSetParasEventHandler(int id, byte[] vals);

    public delegate object[] CharacProcEventHandler(byte[] input);


    public class Product
    {
        //	public static Products Products;

        public uint Id { get; set; }
        public string Name { get; set; }
        public string? Pattern { get; set; }
        public List<ChannelDTO> Channels { get; set; }

        public Product()
        {

        }

        public List<ChannelDTO> GetChannelsCopy()
        {
            List<ChannelDTO> chs = new List<ChannelDTO>();
            foreach (ChannelDTO channel in Channels)
            {
                ChannelDTO channelDTO = new ChannelDTO();
                channelDTO.CopyFrom(channel);
                chs.Add(channelDTO);
            }
            return chs;
        }

    }


    public class Products : List<Product>
    {
        public static Products Instance;
        static string dir_name = @"dat\mqtt_sim\db\products\";

        public static Products GetProducts()
        {
            Products ps = new Products();
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            int i = dir.IndexOf("bin");
            dir = dir.Substring(0, i) + dir_name;

            DirectoryInfo d = new DirectoryInfo(dir);

            FileInfo[] Files = d.GetFiles("*.json");
            //    string str = "";

            foreach (FileInfo file in Files)
            {
                string txt = File.ReadAllText(dir + file.Name);
                Product? p = Newtonsoft.Json.JsonConvert.DeserializeObject<Product>(txt);
                if (p != null)
                {
                    if (p.Name == null)
                        p.Name = file.Name.Substring(0, file.Name.Length - 5);
                    ps.Add(p);
                }
            }
            return ps;
        }

        public Product? GetProduct(string name)
        {
            return this.FirstOrDefault(x => x.Name == name);
        }


    }
}
