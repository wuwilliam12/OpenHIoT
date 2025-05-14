using Newtonsoft.Json;

namespace OpenHIoT.CenterServer.Operation
{
    public class Server : LocalServer.Data.Server
    {
        [JsonIgnore]
        public HttpClient? HttpClient { get; set; }

    }

}
