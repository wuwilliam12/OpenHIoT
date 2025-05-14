using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using OpenHIoT.LocalServer;
using static System.Runtime.InteropServices.JavaScript.JSType;
using OpenHIoT.LocalServer.Data;

namespace OpenHIoT.Client.Requests
{
    public class ServerC : Server
    {

        [JsonIgnore]
        public HttpClient HttpClient { get; set; }
        public bool Default {  get; set; }

        public void SetHttpClient()
        {
            string ip = Ip == null ? "localhost" : Ip;
            int port = HttpPort == null ? 5000 : (int)HttpPort;
            string httpClientUri = $"http://{ip}:{port}";
            HttpClient = new HttpClient()
            {
                BaseAddress = new Uri(httpClientUri)
            };
        }

    }

    public class ServerCs : List<ServerC>
    {
        ServerC default_server;
        [JsonIgnore]
        public ServerC Default  { 
            get {
                if (default_server == null)
                {
                    default_server = new ServerC();
                    default_server.SetHttpClient();                                       
                }            
                return default_server; 
            }
            set { default_server = value; }
        }
    }

    public class ServerRequest
    {
        static HttpClient HttpClient { get; set; }
        public static ServerCs? Servers { get; set; }
        public static string str_controller = "api/Server";


        public static async void CreateHttpClient()
        {
            if(Servers == null)
                Servers = new ServerCs();
            foreach (ServerC s in Servers)
            {
                s.SetHttpClient();
                if (s.Default)
                    Servers.Default = s;
            }
            HttpClient = Servers.Default.HttpClient;

        //    WsUri = httpClientUri.EndsWith("/") ? $"ws:{httpClientUri.Remove(0, httpClientUri.IndexOf('/'))}bin" :
        //        $"ws:{httpClientUri.Remove(0, httpClientUri.IndexOf('/'))}/bin";

        }
        public static HttpClient? GetHttpClient(uint? sid)
        {
            if (sid == null || Servers == null)
                return HttpClient;
            var v = ServerRequest.Servers.FirstOrDefault(x => x.Id == sid);
            if (v == null) return null;
            return v.HttpClient;
        }
        public static async Task<ServerCs?> GetAllServersAsync()
        {
            HttpResponseMessage response =  ServerRequest.HttpClient.GetAsync($"{str_controller}/All").Result;
            if (response.IsSuccessStatusCode)
            {
                ServerCs? es = await response.Content.ReadFromJsonAsync<ServerCs>();
            //    if (es != null && ClientGlobals.ActiveServer != null)
            //        es.SelectServer(ClientGlobals.ActiveServer.Id);
                return es;
            }
            return null;
        }

        public static async Task<ServerC> CreateServerAsync(ServerC edge)
        {
            var jsonString = JsonSerializer.Serialize(edge);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await ServerRequest.HttpClient.PostAsync($"{str_controller}/Add/", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ServerC>();
            }
            return null;
        }

        public static async Task<ServerC> UpdateServersAsync(ServerC edge)
        {
            var jsonString = JsonSerializer.Serialize(edge);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await ServerRequest.HttpClient.PostAsync($"{str_controller}/Update", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ServerC>();
            }
            return null;
        }

        public static async Task<ServerC> DeleteServersAsync(int edge_id)
        {
            HttpResponseMessage response = await ServerRequest.HttpClient.GetAsync($"{str_controller}/Delete/{edge_id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ServerC>();
            }
            return null;
        }


    }
}
