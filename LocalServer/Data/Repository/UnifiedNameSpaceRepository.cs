//using System.Data.Entity;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data.DataContent;
using OpenHIoT.LocalServer.Data;



//add version to di
namespace OpenHIoT.LocalServer.Data.Repository
{
    public interface IUnifiedNameSpaceRepository
    {
        Task<List<UnifiedNameSpace>> GetChildren(uint? id);
      //  Task<List<UnifiedNameSpace>?> GetChildren(string full_name);
        Task<UnifiedNameSpace?> GetById(uint id);
        Task<UnifiedNameSpace?> GetByParent(string name, uint? parent);
        Task<string[]?> GetFullName(uint id);
        Task<UnifiedNameSpace> Create(string full_name);
        Task<UnifiedNameSpace> Create(UnifiedNameSpace ns);
        Task Delete(uint ns_id);
        Task<UnifiedNameSpace> Update(UnifiedNameSpace ns);
    }

    public class UnifiedNameSpaceRepository : IUnifiedNameSpaceRepository
    {
        LocalDbContent _context;
        public UnifiedNameSpaceRepository(LocalDbContent context)
        {
            _context = context;
            // var scope = _scopeFactory.CreateScope();
            // _context = scope.ServiceProvider.GetRequiredService<MqttContext>();
        }
        public async Task<UnifiedNameSpace?> GetById(uint id)
        {
            return await _context.UNSs.FirstOrDefaultAsync(x => x.Id == id );
        }
        public async Task<UnifiedNameSpace?> GetByParent(string name, uint? parent)
        {
            return await _context.UNSs.FirstOrDefaultAsync(x => x.Name == name && x.PId == parent);
        }
        public async Task<List<UnifiedNameSpace>> GetChildren(uint? id)
        {
            return await _context.UNSs.Where(x => x.PId == id).ToListAsync();
        }
        public async Task<string[]?> GetFullName(uint id)
        {
            return null;
         //return await _context.UNSs.Where(x => x.PId == id).ToListAsync();
        }
        public async Task<UnifiedNameSpace> Create(UnifiedNameSpace ns)
        {
            await _context.UNSs.AddAsync(ns);
            await _context.SaveChangesAsync();
            return ns;
        }

        private async Task<UnifiedNameSpace> Create(string name, uint? p_id)
        {
            UnifiedNameSpace ns = new UnifiedNameSpace()
            {
                Name = name,
                PId = p_id,
                Id = _context.UNSs.Max(x => x.Id),
            };
            await _context.UNSs.AddAsync(ns);
            //    await _context.SaveChangesAsync();
            return ns;
        }
    

        public async Task<UnifiedNameSpace> Create(string full_name)
        {
           string[] ss = full_name.Split(UnifiedNameSpace.sep_char);
           uint? parent_id = null;
            UnifiedNameSpace? ns;
            int i;
            for ( i = 0; i < ss.Length - 1; i++)
            {
                ns = await _context.UNSs.FirstOrDefaultAsync(x => x.Name == ss[i] && x.PId == parent_id);
                if (ns == null) 
                    ns = Create(ss[i],  parent_id).Result;
                parent_id = ns.Id;
            }
            ns = Create(ss[i],  parent_id).Result;
            await _context.UNSs.AddAsync(ns);
            await _context.SaveChangesAsync();
            return ns;
        }

        public async Task Delete(uint ns_id)
        {
            var itemToRemove = await _context.UNSs.Where(x => x.Id == ns_id).FirstOrDefaultAsync();
            if (itemToRemove == null)
                throw new NullReferenceException();

            _context.UNSs.Remove(itemToRemove);
            await _context.SaveChangesAsync();
        }



        public async Task<UnifiedNameSpace> Update(UnifiedNameSpace ns)
        {
            var itemToUpdate = await _context.UNSs.Where(x => x.Id == ns.Id).FirstOrDefaultAsync();
            if (itemToUpdate == null)
                throw new NullReferenceException();


            await _context.SaveChangesAsync();
            return ns;
        }
    }
}
