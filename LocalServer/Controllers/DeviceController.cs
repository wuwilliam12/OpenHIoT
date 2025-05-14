using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data.Repository;
using OpenHIoT.LocalServer.Services;

//using OpenHIoT.LocalServer.Operation;
using static System.Net.Mime.MediaTypeNames;

namespace OpenHIoT.LocalServer.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
 //   [Authorize]
    public class DeviceController : ControllerBase
    {
        private readonly ILogger<DeviceRepository> _logger;
        private readonly IDeviceRepository _rep;
        private readonly IMqttService _mqttService;

        public DeviceController(IDeviceRepository rep, 
            IMqttService mqttService,
            ILogger<DeviceRepository> logger)
        {
            _logger = logger;
            _rep = rep;
            _mqttService = mqttService;
        }

        [HttpGet]
        [Route("All")]
        public Task<List<Device>?> GetAll()
        {
           return _rep.GetAll();
        }

        [HttpGet]
        [Route("AllLive")]
        public Task<List<Device>?> GetAllLive()
        {
            return _rep.GetAll();
        }

        [HttpGet]
        [Route("Children/{dev_id}")]
        public Task<List<Device>?> GetChildren(uint dev_id)
        {
           return _rep.GetChildren(dev_id);
        }
        [HttpGet]
        [Route("ChildrenN")]
        public Task<List<Device>?> GetChildren()
        {
           return _rep.GetChildren(null);
        }

        [HttpGet]
        [Route("Get")]
        public Task<Device?> Get(uint id)
        {
           return _rep.Get(id);
        }

        [HttpPost]
        [Route("Add")]
        public Task<Device> Add(Device ns)
        {
           return _rep.Create(ns);
        }

        [HttpPost]
        [Route("Update")]
        public Task<Device> Update(Device ns)
        {
            return _rep.Update(ns);
        }

        [HttpPost]
        [Route("ClearStatus/{dev_id}")]
        public Task<int> ClearStatus(ulong dev_id, int status)
        {
            return _rep.ClearStatus(dev_id, status);
        }

        [HttpPost]
        [Route("SetStatus/{dev_id}")]
        public Task<int> SetStatus(ulong dev_id, int status)
        {
            return _rep.SetStatus(dev_id, status);
        }

        [HttpPost]
        [Route("WriteChannel/{dev_id}")]
        public async Task<bool> WriteChannelByIId( ulong dev_id)
        {
            using (var ms = new MemoryStream())
            {
                HttpContext.Request.Body.CopyToAsync(ms);
                ms.Capacity = (int)ms.Length;
                await _mqttService.WriteChannel(dev_id,  ms.ToArray());
            }
            return true;
        }


        [HttpPost]
        [Route("SampleVal")]
        public Task<bool> SetSampleVal(SampleDTO sample)
        {
            return _mqttService.SetSampleVal(sample);
        }


        [HttpPost]
        [Route("ClientValidate")]
        public Task<bool> ClientValidate(Auth cv)
        {
            return _rep.ClientValidate(cv);
        }



        [HttpGet]
        [Route("SettingGet")]
        public SettingDTO GetSetting()
        {
            return Setting.Instance.GetDTO();
        }

        [HttpPost]
        [Route("SettingPost")]
        public void PostSetting(SettingDTO dto)
        {
            Setting.Instance.SetDTO(dto);
        }




    }
}