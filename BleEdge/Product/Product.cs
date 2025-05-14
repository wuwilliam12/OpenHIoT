
using OpenHIoT.LocalServer.Data.SampleDb;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;
using ProtoBuf.Meta;
using System.Reflection.Metadata;
using System.Security.AccessControl;
//using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Windows.Devices.Bluetooth.Advertisement;

namespace OpenHIoT.BleEdge.Product
{
    public delegate void DeviceSetParasEventHandler(int id, byte[] vals);

    public delegate object[] CharacProcEventHandler(byte[] input);

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Pattern { get; set; }
        public List<Service> Services { get; set; }
//        public List<HeadRt> Parameters { get; set; }
        public List<Channel> Channels { get; set; }
        public bool IsMatch(string input)
        {
            if (Pattern == null)
                Pattern = "^" + Regex.Escape(Name)
                    .Replace(@"\*", ".*")
                    .Replace(@"\?", ".")
                   + "$";
            return new Regex(Pattern).IsMatch(input);
        }

        /*
        public class Service
        {
            public string? Name { get; set; }
            public string UUID { get; set; }
            public List<Characteristic> Characteristics { get; set; }

        }

        public class Characteristic
        {
            public string? Name { get; set; }
            public string UUID { get; set; }
            public int Property { get; set; }

            public string? SetParaFunc { get; set; }
            public string? ProcessFunc { get; set; }
            public int[] ChIds { get; set; }

        }
     
        public class Channel : HeadRt
        {
            public string? Val { get; set; }
        }
   */

    }


    public class Products : List<Product>
    {
        public static Products Instance;

        static string dir_name = @"dat\ble_edge\db\product\";
        public static Products GetProducts()
        {
            Products ps = new Products();

            string dir = ValueDataType.GetDataParentDirectory() + dir_name;

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
        /*
        public Product? GetProduct(string name)
        {
            foreach (Product p in this)
                if (p.IsMatch(name))
                    return p;
            return null;
        }
        */

    }
}
