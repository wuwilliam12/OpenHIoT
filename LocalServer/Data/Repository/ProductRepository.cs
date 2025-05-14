//using System.Data.Entity;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data.DataContent;
using OpenHIoT.LocalServer.Data;



//add version to di
namespace OpenHIoT.LocalServer.Data.Repository
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAll();

        Task<List<Vendor>> GetAllVendor();
        Task<Product?> Get(uint id);
     /*
        Task<Product> Create(Product ns);
        Task Delete(uint ns_id);
        Task<Product> Update(Product ns);
     */
    }

    public class ProductRepository : IProductRepository
    {
        GlobalDbContent _context;
        public ProductRepository(GlobalDbContent context)
        {
            _context = context;
            // var scope = _scopeFactory.CreateScope();
            // _context = scope.ServiceProvider.GetRequiredService<MqttContext>();
        }

        public async Task<List<Product>> GetAll()
        {            
            return await _context.Products.ToListAsync();
        }

        public async Task<List<Vendor>> GetAllVendor()
        {            
            return await _context.Vendors.ToListAsync();
        }
        public async Task<Product?> Get(uint id)
        {
            return await _context.Products.FirstOrDefaultAsync(x => x.Id == id );
        }
      
    }
}
