//using System.Data.Entity;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data.DataContent;
using OpenHIoT.LocalServer.Data;
//using OpenHIoT.LocalServer.Operation;



//add version to di
namespace OpenHIoT.LocalServer.Data.Repository
{
    public interface IDeviceRepository
    {
        Task<List<Device>?> GetAll();
        Task<List<Device>?> GetAllLive();
        Task<List<Device>?> GetChildren(ulong? dev_id);
        Task<Device?> Get(ulong id);
        Task<Device?> Create(Device dev);
        Task<Device?> UpdateOrCreate(Device dev);
        Task<Device?> Update(Device dev);
        Task<bool> ClientValidate(Auth cv);

        Task<int> ClearStatus(ulong id, int status);
        Task<int> SetStatus(ulong id, int status);
    }

    public class DeviceRepository : IDeviceRepository
    {
        LocalDbContent _context;
        static uint lid_next = 0;
        public DeviceRepository(LocalDbContent context)
        {
            _context = context;
            //    lid_next = _context.Devices.Any() ? _context.Devices.Max(x => x.LId) + 1 : 1;
            if (lid_next == 0)
            {
                foreach (Device dev in _context.Devices)
                {
                    dev.Status &= Device.status_nv_bits;
                    if (dev.LId > lid_next)
                        lid_next = dev.LId;
                    _context.Entry(dev).Property(x => x.Status).IsModified = true;
                }
                lid_next++;
                _context.SaveChangesAsync();
            }
        }
        // var scope = _scopeFactory.CreateScope();
        // _context = scope.ServiceProvider.GetRequiredService<MqttContext>();
    
        public async Task<List<Device>?> GetAll()
        {
            return await _context.Devices.Where(x=> (x.Status & (int)DeviceStatus.Deleted) == 0).ToListAsync();
        }

        public async Task<List<Device>?> GetAllLive()
        {
            return await _context.Devices.Where(x => (x.Status & (int)DeviceStatus.Deleted) == 0 && (x.Status & (int)DeviceStatus.Connected) != 0).ToListAsync();
        }
        public async Task<List<Device>?> GetChildren(ulong? dev_id)
        {
            return await _context.Devices.Where(x=> (x.Status & (int)DeviceStatus.Deleted) == 0 && x.Parent == dev_id).ToListAsync();
        }

        public async Task<Device?> Get(ulong id)
        {
            OpenHIoTIdType dt = (OpenHIoTIdType)(byte)id;
            switch(dt)
            {
                case OpenHIoTIdType.Asset:
                    uint asset = (uint)(id >> 8);
                    return await _context.Devices.FirstOrDefaultAsync(x => x.Asset == asset );
                case OpenHIoTIdType.Ble:
                case OpenHIoTIdType.Wifi:
                case OpenHIoTIdType.Simulator:
                    return await _context.Devices.FirstOrDefaultAsync(x => x.PhyId == id );
            }
            return null;
        }

        public async Task<bool> ClientValidate(Auth cv)
        {
            Auth? v = await _context.Auths.FirstOrDefaultAsync(x => x.Id == cv.Id );
            if(v is null)return false;
            return cv.UName == v.UName && v.Pw == cv.Pw;
        }
        public async Task<Device?> Create(Device dev)
        {
            try
            {
                dev.LId = lid_next++;
                await _context.Devices.AddAsync(dev);
                dev.Status = 0;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
            return dev;
        }
        public async Task<Device?> UpdateOrCreate(Device dev)
        {
            Device? itemToUpdate = await Update(dev);
            if (itemToUpdate != null) 
                return itemToUpdate;
            return await Create(dev);
        }

        public async Task<Device?> Update(Device dev)
        {
            try
            {
                Device? itemToUpdate = await _context.Devices.FirstOrDefaultAsync(x =>  x.Asset != null && x.Asset == dev.Asset  || x.PhyId != null && x.PhyId == dev.PhyId);
                if (itemToUpdate != null)
                {
                    itemToUpdate.CopyFromWithoutStatus(dev);
                    _context.Entry(itemToUpdate).Property(x => x.Status).IsModified = false;
                    await _context.SaveChangesAsync();
                    return itemToUpdate;
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }     
        public async Task<int> ClearStatus(ulong id, int status)
        {
            var dev = await Get(id);
            if (dev == null)
                throw new NullReferenceException();
            dev.ClearStatus(status);
            _context.Entry(dev).Property(x => x.Status).IsModified = true;
            //     _context.Devices.Remove(itemToRemove);
            await _context.SaveChangesAsync();
            return dev.Status;
        }
        public async Task<int> SetStatus(ulong id, int status)
        {
            var dev = await Get(id);
            if (dev == null)
                throw new NullReferenceException();
            dev.SetStatus(status);
            _context.Entry(dev).Property(x => x.Status).IsModified = true;
            //     _context.Devices.Remove(itemToRemove);
            await _context.SaveChangesAsync();
            return dev.Status;
        }




    }
}
