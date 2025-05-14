using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data;

namespace OpenHIoT.Client.Requests
{
    public class ProductC : Product
    {
        public Vendor? Vendors { get; set; }
    }
    public class ProductRequest
    {
        public static string str_controller = "api/Product";
        public static async Task<List<ProductC>?> GetAllProductAsync()
        {
            var hc = ServerRequest.GetHttpClient(null);
            if (hc == null) return null;
            HttpResponseMessage response =  hc.GetAsync($"{str_controller}/All", MainWindow.GetCancellationTokenSource().Token).Result;
            if (response.IsSuccessStatusCode)
            {
                List<ProductC>? ps = await response.Content.ReadFromJsonAsync<List<ProductC>>();
                return ps;
            }
            return null;
        }
        public static async Task<List<Vendor>?> GetAllVendorsAsync()
        {
            var hc = ServerRequest.GetHttpClient(null);
            if (hc == null) return null;
            HttpResponseMessage response =  hc.GetAsync($"{str_controller}/Vendors", MainWindow.GetCancellationTokenSource().Token).Result;
            if (response.IsSuccessStatusCode)
            {
                List<Vendor>? vs = await response.Content.ReadFromJsonAsync<List<Vendor>>();
                return vs;
            }
            return null;
        }
        public static async Task<Device?> GetProductAsync(uint  edge)
        {
            var hc = ServerRequest.GetHttpClient(null);
            if (hc == null) return null;
            var jsonString = JsonSerializer.Serialize(edge);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await hc.PostAsync($"{str_controller}/Add/", httpContent, MainWindow.GetCancellationTokenSource().Token);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Device>();
            }
            return null;
        }



    }
}
