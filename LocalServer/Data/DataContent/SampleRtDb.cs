using Microsoft.EntityFrameworkCore;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;

namespace OpenHIoT.LocalServer.Data.DataContent
{
    public class SampleRtDb : DbContext
    {
        public SampleRtDb(DbContextOptions<SampleRtDb> options) : base(options)
        {

        }
        public DbSet<HeadRt> Heads { get; init; }
        public DbSet<SampleInt> ValIs { get; init; }
        public DbSet<SampleReal> ValRs { get; init; }
        public DbSet<SampleBlob> ValBs { get; init; }

    }
}
