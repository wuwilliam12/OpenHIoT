using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Security.Cryptography;
using OpenHIoT.LocalServer.Data;

namespace OpenHIoT.Client.Requests
{
    public class UnifiedNameSpaceC : UnifiedNameSpace
    {

        /*        
        [JsonIgnore]
        public string SEType
        {
            get
            {
                if (EType == null) return UnifiedNameSpaceType.OpenWLS_Std.ToString();
                else return EType.ToString();
            }
        }
        [JsonIgnore]
        public int? IEType
        {
            get
            {
                if (EType == null) return 0;
                else return (int)EType;
            }
            set
            {
                EType = value == null? null : (UnifiedNameSpaceType)value;
            }
        }        
        */

    }

    public class UnifiedNameSpaceCs : List<UnifiedNameSpaceC>
    {

    }
    public class UnifiedNameSpaceRequest
    {
        public static string str_controller = "api/UnifiedNameSpace";

        public static async Task<UnifiedNameSpaceC?> GetByIdAsync(uint id)
        {
            var hc = ServerRequest.GetHttpClient(null);
            if (hc == null) return null;
            HttpResponseMessage response = await hc.GetAsync($"{str_controller}/GetById/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UnifiedNameSpaceC?>();
            }
            return null;
        }

        public static async Task<UnifiedNameSpaceC?>  GetByParent(string name, uint? parent)
        {
            var hc = ServerRequest.GetHttpClient(null);
            if (hc == null) return null;
            HttpResponseMessage response = parent == null? await hc.GetAsync($"{str_controller}/GetByParentN/{name}"):
                                                           await hc.GetAsync($"{str_controller}/GetByParent/{name}/{parent}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<UnifiedNameSpaceC?>();

            return null;
        }

        public static async Task<UnifiedNameSpaceCs?> GetChildrenAsync(uint? id)
        {
            var hc = ServerRequest.GetHttpClient(null);
            if (hc == null) return null;
            HttpResponseMessage response = id == null? hc.GetAsync($"{str_controller}/ChildrenN").Result:
                                                       hc.GetAsync($"{str_controller}/Children/{id}").Result;
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<UnifiedNameSpaceCs>();
            return null;
        }

        public static async Task<UnifiedNameSpaceC?> CreateUnifiedNameSpaceAsync(UnifiedNameSpaceC uns)
        {
            var hc = ServerRequest.GetHttpClient(null);
            if (hc == null) return null;
            var jsonString = JsonSerializer.Serialize(uns);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await hc.PostAsync($"{str_controller}/Add/", httpContent);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<UnifiedNameSpaceC>();

            return null;
        }

        public static async Task<UnifiedNameSpaceC> UpdateUnifiedNameSpacesAsync(UnifiedNameSpaceC uns)
        {
            var hc = ServerRequest.GetHttpClient(null);
            if (hc == null) return null;
            var jsonString = JsonSerializer.Serialize(uns);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await hc.PostAsync($"{str_controller}/Update", httpContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UnifiedNameSpaceC>();
            }
            return null;
        }

        public static async Task<UnifiedNameSpaceC> DeleteUnifiedNameSpacesAsync(int id)
        {
            var hc = ServerRequest.GetHttpClient(null);
            if (hc == null) return null;
            HttpResponseMessage response = await hc.GetAsync($"{str_controller}/Delete/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UnifiedNameSpaceC>();
            }
            return null;
        }

        public static async Task<string[]> GetFullNameAsync(uint id)
        {
            var hc = ServerRequest.GetHttpClient(null);
            if (hc == null) return null;
            HttpResponseMessage response = await hc.GetAsync($"{str_controller}/FullName/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<string[]>();
            }
            return null;
        }
    }
}
