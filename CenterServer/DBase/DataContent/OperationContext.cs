using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data;

//using OpenHIoT.LocalServer.Models;
//using System.Data.Entity;

namespace OpenHIoT.CenterServer.DBase.DataContent
{
    public class OperationContext : DbContext
    {
        public OperationContext(DbContextOptions<OperationContext> options) : base(options)
        {
                          
        }
        public DbSet<UnifiedNameSpace> UNSs { get; set; }
   //     public DbSet<Server> Servers { get; init; }
  //      public DbSet<Edge> Edges { get; init; }
        public DbSet<Device> Devices { get; init; }

    }
}