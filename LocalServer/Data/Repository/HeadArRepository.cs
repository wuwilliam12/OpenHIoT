//using System.Data.Entity;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using OpenHIoT.LocalServer.Data.SampleDb.Archive;
using OpenHIoT.LocalServer.Data.DataContent;


//add version to di
namespace OpenHIoT.LocalServer.Data.Repository
{
    public interface IHeadArRepository
    {
        Task<HeadAr?> GetByNs(uint ns_id, uint s_id);
        Task<HeadAr?> GetByDevice(uint d_id);
        Task<HeadAr> Create(HeadAr h);
        Task<HeadAr> Update(HeadAr h);
        Task Delete(uint id);
    }

    public class HeadArRepository : IHeadArRepository
    {
        SampleArDb _context;
        public HeadArRepository(SampleArDb context)
        {
            _context = context;
            // var scope = _scopeFactory.CreateScope();
            // _context = scope.ServiceProvider.GetRequiredService<MqttContext>();
        }
        public async Task<HeadAr?> GetByDevice(uint d_id)
        {
            return await _context.Heads.FirstOrDefaultAsync(x => x.DId == d_id );
        }
        public async Task<HeadAr?> GetByNs(uint ns_id, uint s_id)
        {
            return await _context.Heads.FirstOrDefaultAsync(x => x.NsId == ns_id && x.SId == s_id );
        }

        public async Task<HeadAr> Create(HeadAr h)
        {
        //    h.NsId = _context.Heads.Max(x => x.NsId) + 1;
            await _context.Heads.AddAsync(h);
            await _context.SaveChangesAsync();
            return h;
        }

        public async Task<HeadAr?> Update(HeadAr h)
        {
            var itemToUpdate = await _context.Heads.Where(x => x.NsId == h.NsId).FirstOrDefaultAsync();
            if (itemToUpdate != null)
            {

                await _context.SaveChangesAsync();
                return h;
            }
            else
            {
                //                throw new NullReferenceException();
                return null;
            }

        } 
    

        public async Task Delete(uint id)
        {
            var itemToRemove = await _context.Heads.Where(x => x.NsId == id).FirstOrDefaultAsync();
            if (itemToRemove == null)
                throw new NullReferenceException();

       //     _context.MHeads.Remove(itemToRemove);
            await _context.SaveChangesAsync();
        }




    }
}
