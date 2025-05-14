using System.Linq;
//using System.Data.Entity;


using Microsoft.EntityFrameworkCore;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;
using OpenHIoT.LocalServer.Data.DataContent;



//add version to di
namespace OpenHIoT.LocalServer.Data.Repository
{
    public interface IHeadRtRepository
    {
        Task<HeadRt?> GetById(uint id);
        Task<HeadRt?> GetByNs(uint ns_id);
        Task<HeadRt?> GetByDev(uint dev_id, string name);
        Task<HeadRt> Add(HeadRt h);
        Task<HeadRt?> Update(HeadRt h);
        Task Delete(uint id);
        Task SaveChangesAsync();
        Task<uint> GetMaxId();
    }

    public class HeadRtRepository : IHeadRtRepository
    {
        SampleRtDb _context;
        uint nxtHId;            //head
        public uint NxtHId { get { return nxtHId++; } }
        public HeadRtRepository(SampleRtDb context)
        {
            _context = context;
            nxtHId = GetMaxId().Result + 1;
            // var scope = _scopeFactory.CreateScope();
            // _context = scope.ServiceProvider.GetRequiredService<MqttContext>();
        }

        public async Task<uint> GetMaxId()
        {
            if(_context.Heads.Any())
                return await _context.Heads.MaxAsync(x => x.Id);
            return 0; 
        }
        public async Task<HeadRt?> GetById(uint id)
        {
            return await _context.Heads.FirstOrDefaultAsync(x => x.Id == id );
        }
        public async Task<HeadRt?> GetByNs(uint ns_id)
        {
            return await _context.Heads.LastAsync(x => x.NsId == ns_id );
        }
        public async Task<HeadRt?> GetByDev(uint dev_id, string name)
        {
            return await _context.Heads.Where(x => x.DId == dev_id && x.Name == name).LastOrDefaultAsync();
         //   if(v is null) return null;
         //   return v.Last();
        }
        public async Task<HeadRt> Add(HeadRt h)
        {
            //    HeadRt? h1 = _context.Heads.LastOrDefaultAsync( x => x.DId == h.DId && x.IId == h.IId && x.InActive == null ).Result;
            //    if (h1 == null ) //no active head found 
            try
            {

                h.Id = nxtHId++;
                await _context.Heads.AddAsync(h);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
            }
           // else
           //     h.Id = h1.Id;
            return h;
        }

        public async Task<HeadRt?> Update(HeadRt h)
        {
            var itemToUpdate = await _context.Heads.Where(x => x.Id == h.Id).FirstOrDefaultAsync();
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
            var itemToRemove = await _context.Heads.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (itemToRemove == null)
                throw new NullReferenceException();

       //     _context.MHeads.Remove(itemToRemove);
            await _context.SaveChangesAsync();
        }


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
