using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using OpenHIoT.LocalServer.Data;




//using OpenHIoT.LocalServer.Models;
using OpenHIoT.LocalServer.Data.DataContent;
using OpenHIoT.LocalServer.Data;



//add version to di
namespace OpenHIoT.LocalServer.Data.Repository
{
    public interface IMqttMsgRepository
    {
        Task Add(MqttMsg mm);
        Task AddRange(List<MqttMsg> mms);
        Task Save();
        Task Backup();
    }

    public class MqttMsgRepository : IMqttMsgRepository
    {
      //  public static IMqttMsgDataSourceProvider? mdsProvider;
        MqttMsgContent _context;

        public MqttMsgRepository(MqttMsgContent context )
        {
            _context = context;
        }

        public async Task Add(MqttMsg msg)
        {
            if (msg == null)
                return;
            await _context.Topics.AddAsync(msg.Topic);
            await _context.Payloads.AddAsync(new BinObj() { Id = msg.Topic.Id, Val = msg.Payload });
           // await _context.SaveChangesAsync();
        }
        public async Task AddRange(List<MqttMsg> ms)
        {
            foreach (MqttMsg msg in ms)
            {
                await _context.Topics.AddAsync(msg.Topic);
                await _context.Payloads.AddAsync(new BinObj() { Id = msg.Topic.Id, Val = msg.Payload });
            }

         //   await _context.SaveChangesAsync();
        }
        public async Task Backup()
        {
            await _context.Database.CloseConnectionAsync();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            //rename db
            //create new one
           // await _context.SaveChangesAsync();
        }


        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
        
        /*
        public Task SwitchActiveDb()
        {
            if(mdsProvider!= null)
                mdsProvider.CurrentDataSource = mdsProvider.CurrentDataSource == MqttMsgDataSource.Primary? MqttMsgDataSource.Secondary : MqttMsgDataSource.Primary;
            return Task.CompletedTask;
        }
        */
    }
}
