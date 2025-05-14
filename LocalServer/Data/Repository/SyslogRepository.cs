using Microsoft.EntityFrameworkCore;
using OpenHIoT.LocalServer.Data.DataContent;
using OpenHIoT.LocalServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OpenHIoT.LocalServer.Data.Repository
{
    public interface ISyslogRepository
    {
        Task<List<SyslogItem>> GetAll();
        Task<List<SyslogItem>> GetItemsFromId(int id);
        Task<List<SyslogItem>> GetItems(long time_from, long time_to, int mod);
        Task<SyslogItem> AddItem(SyslogItem item);
        Task DeleteItem(int id);
        void Init();

    }
    public class SyslogRepository : ISyslogRepository
    {

        static int nxtId = -1; 
        private readonly SysLogDb dbContext;

        public SyslogRepository(SysLogDb dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Init()
        {
            nxtId = dbContext.Items.Max(a=> a.Id) + 1;
        }

        public async Task<List<SyslogItem>> GetAll()
        {
            return await dbContext.Items.ToListAsync();
        }
        public async  Task<List<SyslogItem>> GetItemsFromId(int id)
        {
            if (id < 0)
            {
                id = dbContext.Items.Max(a => a.Id); id++;
              //  await AddMessage("syslog client started");
            }
            return await dbContext.Items.Where(a => a.Id >= id ).ToListAsync();
        }
        public async Task<List<SyslogItem>> GetItems(long time_from, long time_to, int mod)
        {
            if (time_from == 0)
            {
                if (time_to == 0)
                     return await dbContext.Items.Where(a => a.Module == mod).ToListAsync();
                else
                     return await dbContext.Items.Where(a => a.Module == mod && a.Time <= time_to).ToListAsync();
            }
            else
            {
                if (time_to == 0)
                     return await dbContext.Items.Where(a => a.Module == mod && a.Time >= time_from).ToListAsync();
                else
                     return await dbContext.Items.Where(a => a.Module == mod && a.Time <= time_to && a.Time >= time_from).ToListAsync();
            }
        }

        public async Task<SyslogItem> AddItem(SyslogItem item)
        {
            item.Id = nxtId++;
            var result = await dbContext.Items.AddAsync(item);
            await dbContext.SaveChangesAsync();
            return item;
        }

        public async Task DeleteItem(int id )
        {
            var items = await dbContext.Items.Where(a => a.Id <= id).ToListAsync();
            foreach (SyslogItem a in items)
                dbContext.Items.Remove(a);
            await dbContext.SaveChangesAsync();          
        }



 

    }

}
