using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Services;


namespace OpenHIoT.Client.Requests
{

    public class DeviceRequest
    {
        public static string str_controller = "api/Device";

        public static async Task<List<Device>?> GetAllDevicesAsync(uint? sid)
        {
            var hc = ServerRequest.GetHttpClient(sid);
            if (hc == null) return null;
            HttpResponseMessage response = hc.GetAsync($"{str_controller}/All", MainWindow.GetCancellationTokenSource().Token).Result;
            if (response.IsSuccessStatusCode)
            {
                List<Device>? es = await response.Content.ReadFromJsonAsync<List<Device>>();
                return es;
            }
            return null;
        }

        public static async Task<List<Device>?> GetAllLiveDevicesAsync(uint? sid)
        {
            var hc = ServerRequest.GetHttpClient(sid);
            if (hc == null) return null;
            HttpResponseMessage response = hc.GetAsync($"{str_controller}/AllLive", MainWindow.GetCancellationTokenSource().Token).Result;
            if (response.IsSuccessStatusCode)
            {
                List<Device>? es = await response.Content.ReadFromJsonAsync<List<Device>>();
                return es;
            }
            return null;
        }

        public static async Task<Device?> GetDeviceAsync(uint? sid, ulong id)
        {
            var hc = ServerRequest.GetHttpClient(sid);
            if (hc == null) return null;
            HttpResponseMessage response = hc.GetAsync($"{str_controller}/Get/{id}", MainWindow.GetCancellationTokenSource().Token).Result;
            if (response.IsSuccessStatusCode)
            {
                Device? dev = await response.Content.ReadFromJsonAsync<Device>();
            //    if (es != null && ClientGlobals.ActiveDevice != null)
            //        es.SelectDevice(ClientGlobals.ActiveDevice.Id);
                return dev;
            }
            return null;
        }
        public static async Task<List<Device>?> GetChildrenOfDevicesAsync(uint? sid, ulong? dev_id)
        {
            var hc = ServerRequest.GetHttpClient(sid);
            if (hc == null) return null;
            HttpResponseMessage response = dev_id == null? hc.GetAsync($"{str_controller}/ChildrenN", MainWindow.GetCancellationTokenSource().Token).Result 
                : hc.GetAsync($"{str_controller}/Children/{dev_id}").Result;
            if (response.IsSuccessStatusCode)
            {
                List<Device>? ds = await response.Content.ReadFromJsonAsync<List<Device>>();
                return ds;
            }
            return null;
        }

        public static async Task<Device?> CreateDeviceAsync(uint? sid, Device dev)
        {
            var hc = ServerRequest.GetHttpClient(sid);
            if (hc == null) return null;
            var jsonString = JsonSerializer.Serialize(dev);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await hc.PostAsync($"{str_controller}/Add/", httpContent, MainWindow.GetCancellationTokenSource().Token);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Device>();
            }
            return null;
        }

        public static async Task<Device?> UpdateDevicesAsync(uint? sid, Device dev)
        {
            var hc = ServerRequest.GetHttpClient(sid);
            if (hc == null) return null;
            var jsonString = JsonSerializer.Serialize(dev);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await hc.PostAsync($"{str_controller}/Update", httpContent, MainWindow.GetCancellationTokenSource().Token);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Device>();
            }
            return null;
        }

        public static async Task<Device?> DeleteDevicesAsync(uint? sid, ulong edge_id)
        {
            var hc = ServerRequest.GetHttpClient(sid);
            if (hc == null) return null;
            HttpResponseMessage response = await hc.GetAsync($"{str_controller}/Delete/{edge_id}", MainWindow.GetCancellationTokenSource().Token);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Device>();
            }
            return null;
        }
        public static async Task<bool> WriteChannel(uint? sid,  ulong dev_id, byte[] bs)
        {
            var hc = ServerRequest.GetHttpClient(sid);
            if (hc == null) return true;
            ByteArrayContent byteContent = new ByteArrayContent(bs);
            HttpResponseMessage response = await hc.PostAsync($"{str_controller}/WriteChannel/{dev_id}", byteContent, MainWindow.GetCancellationTokenSource().Token);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return Boolean.Parse(data);
            }
            return false;
        }

        public static async Task<bool> SetSampleValue(uint? sid, SampleDTO s)
        {
            var hc = ServerRequest.GetHttpClient(sid);
            if (hc == null) return false;

            var jsonString = JsonSerializer.Serialize(s);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await hc.PostAsync($"{str_controller}/SampleVal", httpContent);
            return response.IsSuccessStatusCode;
        }
    }
}
