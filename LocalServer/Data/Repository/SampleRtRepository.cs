//using System.Data.Entity;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;
using OpenHIoT.LocalServer.Data.DataContent;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Services;



//add version to di
namespace OpenHIoT.LocalServer.Data.Repository
{
    public interface ISampleRtRepository
    {
        Task<SampleInt?> GetIntegerValue(uint id);
        Task<SampleReal?> GetRealValue(uint id);
        Task<SampleBlob?> GetBlobValue(uint id);

        /*
        SampleInts? GetInts(uint id, ulong from_ts);
        SampleDoubles? GetDoubles(uint id, ulong from_ts);
        SampleBlobValues? GetBlobs(uint id, ulong from_ts);
        Double? GetDouble(uint id);
        byte[]? GetBlob(uint id);
   */
        Task Add(Sample v);
        Task SaveChangesAsync();
        Task<uint> GetMaxId();
    }

    public class SampleRtRepository : ISampleRtRepository
    {
        SampleRtDb _context;
        public SampleRtRepository(SampleRtDb context)
        {
            _context = context;
            // var scope = _scopeFactory.CreateScope();
            // _context = scope.ServiceProvider.GetRequiredService<MqttContext>();
        }

        public async Task<uint> GetMaxId()
        {
            uint maxId = 0;
            if (_context.ValIs.Any())
                maxId = await _context.ValIs.MaxAsync(x => x.Id);
            if (_context.ValRs.Any())
                maxId = Math.Max( maxId, await _context.ValRs.MaxAsync(x => x.Id) );  
            if (_context.ValBs.Any())
                return Math.Max( maxId, await _context.ValBs.MaxAsync(x => x.Id) );              
            return maxId;
        }

        public async Task<SampleInt?> GetIntegerValue(uint id)
        {
            return await _context.ValIs.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<SampleReal?> GetRealValue(uint id)
        {
            return await _context.ValRs.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<SampleBlob?> GetBlobValue(uint id)
        {
            return await _context.ValBs.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Add(Sample s)
        {
            if (s is null) 
                return;
            object? v = s.GetVal();
            if (v is null) return;
            if (v is long)
                await _context.ValIs.AddAsync((SampleInt)s);
            else
            {
                if (v is double)
                    await _context.ValRs.AddAsync((SampleReal)s );
                else
                {
                    if (v is byte[])
                        await _context.ValBs.AddAsync((SampleBlob)s);
                }
            }
            await _context.SaveChangesAsync();
        }
       public async Task SaveChangesAsync()
       {
            await _context.SaveChangesAsync();
       }

        /*
       public SampleInts? GetInts(uint id, ulong from_ts)
       {
            return null;
       }

        public SampleDoubles? GetDoubles(uint id, ulong from_ts)
       {
            return null;

       }

        public SampleBlobValues? GetBlobs(uint id, ulong from_ts)
        {
            return null; 
        }
  
       public Double? GetDouble(uint id)
       {
            return null;
       }

        public byte[]? GetBlob(uint id)
        {
            return null;
        }
        */

    }
}
