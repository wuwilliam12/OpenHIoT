using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data;


//using System.Data.Entity;

namespace OpenHIoT.CenterServer.DBase.DataContent
{
    public class GlobalContext : DbContext
    {
        public GlobalContext(DbContextOptions<GlobalContext> options) : base(options)
        {

        }
  //      public DbSet<Node> Nodes { get; init; }
        public DbSet<Product> Products { get; init; }

    }
}