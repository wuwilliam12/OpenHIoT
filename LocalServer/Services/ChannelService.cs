using Microsoft.Extensions.DependencyInjection;
using OpenHIoT.LocalServer.Data.Repository;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Operation;
using SparkplugNet.VersionB.Data;

namespace OpenHIoT.LocalServer.Services
{
    public interface IChannelChannel
    {
     //   public uint NxtHId { get; }
        public Task AddHeads(KnownChannels ms);
        public void AddSample(Sample? s);
        public void AddSamples(List<Sample>? ss);
        public void SaveSamples();

    }

    public class ChannelService : IChannelChannel
    {
        ISampleCache valueCache;
        Queue<Sample> v_queue;

        uint nxtVId;
        bool save_busy;
        private readonly IServiceScopeFactory scopeFactory;

        public ChannelService(IServiceScopeFactory _scopeFactory)
        {
            v_queue = new Queue<Sample>();
            nxtVId = 1;
            save_busy = false;
            scopeFactory = _scopeFactory;
            using (var scope = scopeFactory.CreateAsyncScope())
            {
                var repo_h = scope.ServiceProvider.GetRequiredService<IHeadRtRepository>();
       //         nxtHId = repo_h.GetMaxId().Result + 1;
                var repo_v = scope.ServiceProvider.GetRequiredService<ISampleRtRepository>();
                nxtVId = repo_v.GetMaxId().Result + 1;
                valueCache = scope.ServiceProvider.GetRequiredService<ISampleCache>(); 
            }
        }

        public async Task AddHeads(KnownChannels ms)
        {
            using (var scope = scopeFactory.CreateAsyncScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IHeadRtRepository>();
                foreach (Channel m in ms.Items)
                {
                    if (m.Head is not null)
                        await repo.Add(m.Head);
                }
                await repo.SaveChangesAsync();
            }
        }

        public void AddSample(Sample? s)
        {
            if (s == null) return;
            s.Id = nxtVId++;
            v_queue.Enqueue(s);
            valueCache.Add(s);
        }

        public void AddSamples(List<Sample>? ss)
        {
            if (ss == null) return;
            foreach (Sample s in ss)
            {
                s.Id = nxtVId++;
                v_queue.Enqueue(s);
                valueCache.Add(s);
            }
        }

        public void SaveSamples()
        {
            if (!save_busy)
                Task.Run(() => SaveSamplesTask());
        }

        async Task SaveSamplesTask()
        {
            save_busy = true;
            using (var scope = scopeFactory.CreateAsyncScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ISampleRtRepository>();
                while (v_queue.Count > 0)
                    await repo.Add(v_queue.Dequeue());
               await repo.SaveChangesAsync();
            }

            save_busy = false;
        }



    }
}

