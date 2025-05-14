using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data;

//using OpenHIoT.LocalServer.Models;
//using System.Data.Entity;

namespace OpenHIoT.LocalServer.Data.DataContent
{
    public class MqttMsgContent : DbContext
    {
        public MqttMsgContent(DbContextOptions<MqttMsgContent> options) : base(options)
        {

        }
        public DbSet<Topic> Topics { get; init; }
        public DbSet<BinObj> Payloads { get; init; }

    }
     /*
    public enum MqttMsgDataSource
    {
        Primary,
        Secondary
    }
   
    public class MqttMsgDataSourceProvider : IMqttMsgDataSourceProvider
    {
        private readonly IConfiguration _configuration;
        public MqttMsgDataSource CurrentDataSource { get; set; }
        public MqttMsgDataSourceProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString()
        {
            return CurrentDataSource switch
            {
                MqttMsgDataSource.Primary => _configuration.GetConnectionString("Primary")!,
                MqttMsgDataSource.Secondary => _configuration.GetConnectionString("Secondary")!,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
    public interface IMqttMsgDataSourceProvider
    {
        MqttMsgDataSource CurrentDataSource { get; set; }
        string GetConnectionString();
    }*/
}