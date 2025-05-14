using Microsoft.Extensions.DependencyInjection;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data.Repository;
using SparkplugNet.Core.Enumerations;
using SparkplugNet.Core.Topics;
using SparkplugNet.VersionB;
using System.Collections.Concurrent;
using System.Drawing;
using System.Security.Cryptography;

namespace OpenHIoT.LocalServer.Services
{

    public interface ISyslogSevice
    {
        public Task AddMessage(SyslogItem item);
        public Task AddMessage(SyslogModule mod, string msg, Color? c);
        public Task AddMessage(SyslogModule mod, string msg, uint c);
        public Task AddMessage( string msg);
    }
    public class SyslogSevice : ISyslogSevice
    {
        uint nxtId;
        IServiceScopeFactory serviceScopeFactory;
   //     Queue<MqttMsg> mqttMsgs; 
        bool save_busy;

        public SyslogSevice(IServiceScopeFactory _serviceScopeFactory) {
            save_busy = false;
            nxtId = 1;
   //         mqttMsgs = new Queue<MqttMsg>();
            serviceScopeFactory = _serviceScopeFactory;
        }



        public Task AddMessage(SyslogItem item)
        {
            using (var scope = serviceScopeFactory.CreateAsyncScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ISyslogRepository>();
                return repo.AddItem(item);
            }
        }

        public Task AddMessage( string msg )
        {
            SyslogItem item = new SyslogItem()
            {
                Module = 0,
                Msg = msg,
                Time = DateTime.Now.Ticks,
            };
            return AddMessage(item);
        }
        public Task AddMessage(SyslogModule mod, string msg, Color? c)
        {
            SyslogItem item = new SyslogItem()
            {
                Module = (int)mod,
                Msg = msg,
                Time = DateTime.Now.Ticks,
            };
            if (c != null)
                item.Color = ConvertToUint32((Color)c);
            return AddMessage(item);
        }
        public Task AddMessage(SyslogModule mod, string msg, uint c)
        {
            SyslogItem si = new SyslogItem()
            {
                Module = (int)mod,
                Msg = msg,
                Time = DateTime.Now.Ticks,
                Color = c
            };
            return AddMessage(si);
        }
        public void AddMessage(SyslogModule mod, string msg, bool normal)
        {
            SyslogItem item = new SyslogItem()
            {
                Module = (int)mod,
                Msg = msg,
                Time = DateTime.Now.Ticks,
                Color = normal ? null : ConvertToUint32(Color.FromKnownColor(KnownColor.Red))
            };
            AddMessage(item);
        }

        static public uint ConvertToUint32(Color c)
        {
            return (uint)((c.A << 24) | (c.R << 16) |
                    (c.G << 8) | (c.B << 0));
        }


    }
}
