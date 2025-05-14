using Microsoft.AspNetCore.Mvc;
using OpenHIoT.LocalServer.Services;
using OpenHIoT.LocalServer.Data.Repository;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;

namespace OpenHIoT.LocalServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HeadRtController : ControllerBase
    {
        IMqttService _mqttService;
        private readonly ILogger<HeadRtRepository> _logger;
        private readonly IHeadRtRepository _rep;
        public HeadRtController(IHeadRtRepository rep, IMqttService mqttService, ILogger<HeadRtRepository> logger)
        {
            _logger = logger;
            _rep = rep;
            _mqttService = mqttService;
        }
        [HttpGet]
        [Route("AllLive")]
        public List<HeadRt>? GetAllLive()
        {
            return _mqttService.GetHeadRts();
        }
        [HttpGet]
        [Route("Dev/{dev_id}")]
        public List<HeadRt> GetMHeadRtsOfDevice(ulong dev_id)
        {
            return _mqttService.GetHeadRtsOfDevice(dev_id);
        }
        [HttpGet]
        [Route("Get")]
        public Task<HeadRt?> Get(uint id)
        {
           return _rep.GetById(id);
        }

        [HttpGet]
        [Route("GetByNs/{ns_id}")]
        public HeadRt? GetByNs(uint ns_id)
        {
           return _mqttService.GetHeadRtByNs(ns_id);
        }
        [HttpGet]
        [Route("GetByAlias/{alias}")]
        public HeadRt? GetByAlias(ulong alias)
        {
           return _mqttService.GetByAlias(alias);
        }
        [HttpGet]
        [Route("GetByName/{dev_id}/{name}")]
        public HeadRt? GetByDev(ulong dev_id, string name)
        {
            return _mqttService.GetHeadRtByName(dev_id, name);
        }

        [HttpGet]
        [Route("GetByIId/{dev_id}/{iid}")]
        public HeadRt? GetByDev(ulong dev_id, byte iid)
        {
            return _mqttService.GetHeadRtByIId(dev_id, iid);
        }

        [HttpPost]
        [Route("Add")]
        public Task Add(HeadRt h)
        {
           return _rep.Add(h);
        }

        [HttpPost]
        [Route("Update")]
        public Task Update(HeadRt h)
        {
            return _rep.Update(h);
        }




    
    }
}