using Microsoft.AspNetCore.Mvc;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data.Repository;
using OpenHIoT.LocalServer.Data;

namespace OpenHIoT.LocalServer.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductRepository> _logger;
        private readonly IProductRepository _rep;
        public ProductController(IProductRepository rep, ILogger<ProductRepository> logger)
        {
            _logger = logger;
            _rep = rep;
        }

        [HttpGet]
        [Route("All")]
        public Task<List<Product>> GetAll()
        {
           return _rep.GetAll();
        }
        [HttpGet]
        [Route("Vendors")]
        public Task<List<Vendor>> GetAllVendor()
        {
            var res = _rep.GetAllVendor();
           return res;
        }

        [HttpGet]
        [Route("Get")]
        public Task<Product?> Get(uint id)
        {
           return _rep.Get(id);
        }
     
        /*
        [HttpPost]
        [Route("Add")]
        public Task Add(Product ns)
        {
           return _rep.Create(ns);
        }

        [HttpPost]
        [Route("Update")]
        public Task Update(Product ns)
        {
            return _rep.Update(ns);
        }
        */



    
    }
}