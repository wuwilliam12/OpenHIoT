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
using OpenHIoT.LocalServer.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Windows.Markup;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;

namespace OpenHIoT.Client.Requests
{
    public class HeadRtC : HeadRt
    {
        [JsonIgnore]
        public uint? SId { get; set; }
    }
    public class SampletRtRequest
    {
        public static string str_controller_h = "api/HeadRt";
        public static string str_controller_v = "api/SampleRt";

        public static async Task<HeadRtC?> GetHeadLiveByNsAsync(uint? sid, uint ns_id)
        {
            var hc = ServerRequest.GetHttpClient(sid);
            if (hc == null) return null;
            HttpResponseMessage response =  hc.GetAsync($"{str_controller_h}/GetByNs/{ns_id}", MainWindow.GetCancellationTokenSource().Token).Result;
            if (response.IsSuccessStatusCode)
            {
                HeadRtC? h = await response.Content.ReadFromJsonAsync<HeadRtC>();
                if (h != null) h.SId = sid;
                return h;
            }
            return null;
        }

        public static async Task<HeadRtC?> GetHeadLiveByAliasAsync(uint? sid, ulong alias)
        {
            var hc = ServerRequest.GetHttpClient(sid);
            if (hc == null) return null;
            HttpResponseMessage response =  hc.GetAsync($"{str_controller_h}/GetByAlias/{alias}", MainWindow.GetCancellationTokenSource().Token).Result;
            if (response.IsSuccessStatusCode)
            {
                HeadRtC? h = await response.Content.ReadFromJsonAsync<HeadRtC>();
                if (h != null) h.SId = sid;
                return h;
            }
            return null;
        }

        public static async Task<HeadRtC?> GetHeadLiveByNameAsync(uint? sid, ulong dev_id, string name)
        {
            var hc = ServerRequest.GetHttpClient(sid);
            if (hc == null) return null;
            HttpResponseMessage response =  hc.GetAsync($"{str_controller_h}/GetByName/{dev_id}/{name}", MainWindow.GetCancellationTokenSource().Token).Result;
            if (response.IsSuccessStatusCode)
            {
                HeadRtC? h = await response.Content.ReadFromJsonAsync<HeadRtC>();
                if(h != null)h.SId = sid;
                return h;
            }
            return null;
        }
        public static async Task<HeadRtC?> GetHeadLiveByIIdAsync(uint? sid, ulong dev_id, byte iid)
        {
            var hc = ServerRequest.GetHttpClient(sid);
            if (hc == null) return null;
            HttpResponseMessage response =  hc.GetAsync($"{str_controller_h}/GetByIId/{dev_id}/{iid}", MainWindow.GetCancellationTokenSource().Token).Result;
            if (response.IsSuccessStatusCode)
            {
                HeadRtC? h = await response.Content.ReadFromJsonAsync<HeadRtC>();
                if(h != null)h.SId = sid;
                return h;
            }
            return null;
        }

        public static async Task<List<HeadRtC>?> GetAllHeadsOfDevAsync(uint? sid, ulong dev_id)
        {
            var hc = ServerRequest.GetHttpClient(sid);
            if (hc == null) return null;
            HttpResponseMessage response = hc.GetAsync($"{str_controller_h}/Dev/{dev_id}", MainWindow.GetCancellationTokenSource().Token).Result;
            if (response.IsSuccessStatusCode && response.StatusCode !=  System.Net.HttpStatusCode.NoContent)
            {
                List<HeadRtC>? hs = await response.Content.ReadFromJsonAsync<List<HeadRtC>>();
                if(hs != null)
                    foreach( var h in hs) h.SId = sid;
                return hs;
            }
            return null;
        }
        public static async Task<List<HeadRtC>?> GetAllHeadsLiveAsync(uint? sid)
        {
            var hc = ServerRequest.GetHttpClient(sid);
            if (hc == null) return null;
            HttpResponseMessage response = hc.GetAsync($"{str_controller_h}/AllLive", MainWindow.GetCancellationTokenSource().Token).Result;
            if (response.IsSuccessStatusCode)
            {
                List<HeadRtC>? hs = await response.Content.ReadFromJsonAsync<List<HeadRtC>>();
                if(hs != null)
                    foreach( var h in hs) h.SId = sid;
                return hs;
            }
            return null;
        }
        public static async Task<SampleDTOs?> GetSamples(uint? sid, ulong[] ids)
        {
            var hc = ServerRequest.GetHttpClient(sid);
            if (hc == null) return null;
            HttpResponseMessage response = hc.GetAsync($"{str_controller_v}/Samples/{string.Join(",", ids)}", MainWindow.GetCancellationTokenSource().Token).Result;
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<SampleDTOs?>();
            return null;
        }


        public static async Task<SampleDTOs?> GetSamplesFrom(uint? sid, ulong id, long from_ts)
        {
            var hc = ServerRequest.GetHttpClient(sid);
            if (hc == null) return null;
            HttpResponseMessage response = hc.GetAsync($"{str_controller_v}/SamplesFrom/{id}/{from_ts}", MainWindow.GetCancellationTokenSource().Token).Result;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<SampleDTOs>();
            }
            return null;
        }

        public static async Task<SampleDTOs?> GetSamplesFromS(uint? sid, ulong[] ids, ulong[] from_tss)
        {
            var hc = ServerRequest.GetHttpClient(sid);
            if (hc == null) return null;
        //    var options = new JsonSerializerOptions { IncludeFields = true };
            HttpResponseMessage response = hc.GetAsync($"{str_controller_v}/SamplesFromS/{string.Join(",", ids)}/{string.Join(",", from_tss )}", MainWindow.GetCancellationTokenSource().Token).Result;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<SampleDTOs>();
            }
            return null;
        }

        public static async Task<SampleDTOs?> GetSamplesFromByNs(uint? sid, uint[] ns_ids)
        {            
            var hc = ServerRequest.GetHttpClient(sid);
            if (hc == null) return null;
            HttpResponseMessage response = hc.GetAsync($"{str_controller_v}/SamplesByNs/{string.Join(",", ns_ids)}", MainWindow.GetCancellationTokenSource().Token).Result;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<SampleDTOs>();
            }
            return null;
        }
        public static async Task<SampleDTOs?> GetSamplesFromByNsS(uint? sid, uint[] ns_ids, long from_ts)
        {
            var hc = ServerRequest.GetHttpClient(sid);
            if (hc == null) return null;
            HttpResponseMessage response = hc.GetAsync($"{str_controller_v}/SamplesFromByNsS/{string.Join(",", ns_ids)}/{from_ts}", MainWindow.GetCancellationTokenSource().Token).Result;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<SampleDTOs>();
            }
            return null;
        } 

    }
}
