using Microsoft.AspNetCore.Mvc;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data.Repository;
using OpenHIoT.LocalServer.Data;

namespace OpenHIoT.LocalServer.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class MqttMsgController : ControllerBase
    {
        private readonly ILogger<MqttMsgController> _logger;
        private readonly IMqttMsgRepository _rep;
        public MqttMsgController(IMqttMsgRepository rep, ILogger<MqttMsgController> logger)
        {
            _logger = logger;
            _rep = rep;
        }



        [HttpPost]
        [Route("Add")]
        public Task Add(MqttMsg m)
        {
           return _rep.Add(m);
        }
        [HttpPost]
        [Route("AddRange")]
        public Task Update(List<MqttMsg> ms)
        {
            return _rep.AddRange(ms);
        }

        [HttpGet]
        [Route("Backup")]
        public Task Backup()
        {
            return _rep.Backup();
        }

    }
}