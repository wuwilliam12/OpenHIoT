using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHIoT.LocalServer.Data.DataContent;

public class SysLogDb : DbContext
{
    public SysLogDb(DbContextOptions<SysLogDb> options) : base(options)
    {

    }
    public DbSet<SyslogItem> Items { get; init; }

}

