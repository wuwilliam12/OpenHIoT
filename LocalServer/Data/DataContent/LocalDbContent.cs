using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenHIoT.LocalServer.Data;

//using OpenHIoT.LocalServer.Models;
//using System.Data.Entity;

namespace OpenHIoT.LocalServer.Data.DataContent
{
    public class LocalDbContent : DbContext
    {
        public LocalDbContent(DbContextOptions<LocalDbContent> options) : base(options)
        {
                          
        }
        public DbSet<UnifiedNameSpace> UNSs { get; init; }
     //   public DbSet<Edge> Edges { get; init; }
        public DbSet<Device> Devices { get; init; }     
        public DbSet<Auth> Auths { get; init; }  //client
//        public DbSet<Measurement.Models.MHead> Measurements { get; init; }
    }
}