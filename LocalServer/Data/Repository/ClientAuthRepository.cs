//using System.Data.Entity;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data.DataContent;
using OpenHIoT.LocalServer.Operation;



//add version to di
namespace OpenHIoT.LocalServer.Data.Repository
{
    public interface IClientAuthRepository
    {
        Task<Auth?> Get(ulong id);
        Task<Auth> Create(Auth a);
        Task<Auth> Update(Auth a);
        Task<bool> ClientValidate(Auth cv);
        Task Delete(ulong id);
    }

    public class ClientAuthRepository : IClientAuthRepository
    {
        LocalDbContent _context;
        public ClientAuthRepository(LocalDbContent context)
        {
            _context = context;
            // var scope = _scopeFactory.CreateScope();
            // _context = scope.ServiceProvider.GetRequiredService<MqttContext>();
        }
        public async Task<Auth?> Get(ulong id)
        {
            return await _context.Auths.FirstOrDefaultAsync(x => x.Id == id );
        }


        public async Task<Auth> Create(Auth a)
        {
            await _context.Auths.AddAsync(a);
            await _context.SaveChangesAsync();
            return a;
        }

        public async Task<Auth?> Update(Auth auth)
        {
            var itemToUpdate = await _context.Auths.Where(x => x.Id == auth.Id).FirstOrDefaultAsync();
            if (itemToUpdate != null)
            {

                await _context.SaveChangesAsync();
                return auth;
            }
            else
            {
                //                throw new NullReferenceException();
                return null;
            }

        } 
    

        public async Task Delete(ulong id)
        {
            var itemToRemove = await _context.Auths.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (itemToRemove == null)
                throw new NullReferenceException();

       //     _context.Devices.Remove(itemToRemove);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ClientValidate(Auth a)
        {
            return true;
            var a1 = await _context.Auths.Where(x => x.Id == a.Id).FirstOrDefaultAsync();
            if (a1 == null) return false;
            return a1.UName == a.UName && a1.Pw == a.Pw;

        }



    }
}
