using Microsoft.Extensions.DependencyInjection;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data.Repository;
using OpenHIoT.LocalServer.Data;
using SparkplugNet.Core.Enumerations;
using SparkplugNet.Core.Topics;
using SparkplugNet.VersionB;
using System.Security.Cryptography;

namespace OpenHIoT.LocalServer.Services
{
    public interface IMsgSevice
    {
        public Task AddMsg(MqttMsg msg);
    }
    public class MsgSevice : IMsgSevice
    {
        uint nxtId;
        IServiceScopeFactory serviceScopeFactory;

        Queue<MqttMsg> saveQueue;        
        bool save_busy;

        bool save;
        MqttService mqttService;
        public MsgSevice(IServiceScopeFactory _serviceScopeFactory, IConfiguration configuration) {
            save = configuration.GetValue<bool>("SaveMsg");
            save_busy = false;

            nxtId = 1;
            saveQueue = new Queue<MqttMsg>();
            serviceScopeFactory = _serviceScopeFactory;
        }

        void SaveMsgs()
        {
            save_busy = true;
            using (var scope = serviceScopeFactory.CreateAsyncScope())
            { 
                var repo = scope.ServiceProvider.GetRequiredService<IMqttMsgRepository>();
                while(saveQueue.Count > 0)
                    repo.Add(saveQueue.Dequeue());
                repo.Save();
            }
            save_busy = false;
        }



        public Task AddMsg(MqttMsg msg)
        {
            msg.Topic.Id = nxtId++;

            if (save)
            {
                saveQueue.Enqueue(msg);
                if (saveQueue.Count >= 10 && (!save_busy))
                    Task.Run(() => { SaveMsgs(); });
            }

            return Task.CompletedTask;
        }
    }
}
