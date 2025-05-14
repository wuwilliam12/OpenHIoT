using Microsoft.EntityFrameworkCore;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data.SampleDb.Archive;
using OpenHIoT.LocalServer.Data;


namespace OpenHIoT.LocalServer.Data.DataContent
{
    public class SampleArDb : DbContext
    {
        public SampleArDb(DbContextOptions<SampleArDb> options) : base(options)
        {

        }
        public DbSet<HeadAr> Heads { get; init; }
        public DbSet<ValueBlock> VBs { get; init; }
        public DbSet<BinObj> Vs { get; init; }

    }
}
