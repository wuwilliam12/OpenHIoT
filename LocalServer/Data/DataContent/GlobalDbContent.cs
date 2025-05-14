using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data;

//using OpenHIoT.LocalServer.Models;
//using System.Data.Entity;

namespace OpenHIoT.LocalServer.Data.DataContent
{
    public class GlobalDbContent : DbContext
    {
        public GlobalDbContent(DbContextOptions<GlobalDbContent> options) : base(options)
        {

        }
     //   public DbSet<UnifiedNameSpaceId> UnsIds { get; init; }
     //   public DbSet<UnifiedNameSpace> Unss { get; init; }
        public DbSet<Vendor> Vendors { get; init; }       
        public DbSet<Product> Products { get; init; }
        public DbSet<Device> Devices { get; init; }
     //   public DbSet<MHead> Measurements { get; init; }
    }
}