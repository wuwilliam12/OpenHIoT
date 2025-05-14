using Microsoft.AspNetCore.Mvc;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data.Repository;
using OpenHIoT.LocalServer.Data;

namespace OpenHIoT.LocalServer.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UnifiedNameSpaceController : ControllerBase
    {
        private readonly ILogger<UnifiedNameSpaceRepository> _logger;
        private readonly IUnifiedNameSpaceRepository _rep;
        public UnifiedNameSpaceController(IUnifiedNameSpaceRepository rep, ILogger<UnifiedNameSpaceRepository> logger)
        {
            _logger = logger;
            _rep = rep;
        }

        [HttpGet]
        [Route("GetById")]
        public Task<UnifiedNameSpace?> GetById(uint id)
        {
           return _rep.GetById(id);
        }

        [HttpGet]
        [Route("GetByParent/{name}/{parent}")]
        public Task<UnifiedNameSpace?> GetByParent(string name, uint parent)
        {
           return _rep.GetByParent(name, parent);
        }

        [HttpGet]
        [Route("GetByParentN/{name}")]
        public Task<UnifiedNameSpace?> GetByParent(string name)
        {
           return _rep.GetByParent(name, null);
        }

        [HttpGet]
        [Route("Children/{id}")]
        public Task<List<UnifiedNameSpace>> GetChildren(uint id)
        {
           return _rep.GetChildren(id);
        }

        [HttpGet]
        [Route("ChildrenN")]
        public Task<List<UnifiedNameSpace>> GetChildren()
        {
            return _rep.GetChildren(null);
        }

        [HttpGet]
        [Route("FullName/{id}")]
        public Task<string[]?> GetFullName(uint id)
        {
           return _rep.GetFullName(id);
        }


        [HttpPost]
        [Route("Add")]
        public Task Add(UnifiedNameSpace ns)
        {
           return _rep.Create(ns);
        }

        [HttpPost]
        [Route("Update")]
        public Task Update(UnifiedNameSpace ns)
        {
            return _rep.Update(ns);
        }




    
    }
}