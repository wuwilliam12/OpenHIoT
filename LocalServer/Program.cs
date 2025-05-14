using OpenHIoT.LocalServer;

using OpenHIoT.LocalServer.Data.DataContent;
using Microsoft.EntityFrameworkCore;
using ProtoBuf.Meta;
using System;
using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Services;
using System.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MQTTnet.AspNetCore;


using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenHIoT.LocalServer.Data.Repository;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

// Add services to the container.
builder.Services.AddDbContext<GlobalDbContent>((DbContextOptionsBuilder options) =>
       options.UseSqlite(builder.Configuration.GetConnectionString("GlobalDb")));
builder.Services.AddDbContext<LocalDbContent>((DbContextOptionsBuilder options) =>
       options.UseSqlite(builder.Configuration.GetConnectionString("LocalDb")));
builder.Services.AddDbContext<MqttMsgContent>((DbContextOptionsBuilder options) =>
       options.UseSqlite(builder.Configuration.GetConnectionString("MsgDb")));
builder.Services.AddDbContext<SampleArDb>((DbContextOptionsBuilder options) =>
       options.UseSqlite(builder.Configuration.GetConnectionString("SampleArDb")));
builder.Services.AddDbContext<SampleRtDb>((DbContextOptionsBuilder options) =>
       options.UseSqlite(builder.Configuration.GetConnectionString("SampleRtDb")));
builder.Services.AddDbContext<SysLogDb>((DbContextOptionsBuilder options) =>
       options.UseSqlite(builder.Configuration.GetConnectionString("SyslogDb")));

/*
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
       .AddEntityFrameworkStores<ApplicationDbContext>()
       .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
});
*/

builder.Services.AddSingleton<ISyslogSevice, SyslogSevice>();
builder.Services.AddSingleton<ISampleCache, SampleCache>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUnifiedNameSpaceRepository, UnifiedNameSpaceRepository >();
builder.Services.AddScoped<IMqttMsgRepository, MqttMsgRepository>();
//builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<IClientAuthRepository, ClientAuthRepository>();
builder.Services.AddScoped<IHeadArRepository, HeadArRepository>();
builder.Services.AddScoped<IHeadRtRepository, HeadRtRepository>();
//builder.Services.AddScoped<IMValueArRepository, MValueArRepository>();
builder.Services.AddScoped<ISampleRtRepository, SampleRtRepository>();

builder.Services.AddSingleton<IChannelChannel, ChannelService>();
builder.Services.AddSingleton<IMsgSevice, MsgSevice>();
builder.Configuration.GetValue<bool>("SaveMsg");

builder.WebHost.ConfigureKestrel(opt => {
    opt.ListenAnyIP(MqttService.GetMqttPort(builder.Configuration.GetConnectionString("MqttPort")), l => l.UseMqtt());
    opt.ListenAnyIP(5000); // Default HTTP pipelin
});

builder.Services.AddHostedMqttServer(
    optionsBuilder =>
    {
        optionsBuilder.WithDefaultEndpoint();
    });
builder.Services.AddMqttConnectionHandler();
builder.Services.AddConnections();

builder.Services.AddSingleton<IMqttService, MqttService>();

//services.AddControllers();
services.AddControllers().AddJsonOptions(options =>
            options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        );
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the bearer scheme. Enter Bearer [space] add your token in the text input. Example: Bearer swersdf877sdf",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                },
                Scheme = "oauth2",

                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
    options.AddPolicy("AllowOnlyLocalhost", policy =>
    {
        policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
    });
    options.AddPolicy("AllowOnlyCenter", policy =>
    {
        policy.WithOrigins("http://google.com", "http://gmail.com", "http://drive.google.com").AllowAnyHeader().AllowAnyMethod();
    });

});

var keyJWTSecretforCenter = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("JWTSecretforCenter"));
var keyJWTSecretforLocal = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("JWTSecretforLocal"));
string CenterAudience = builder.Configuration.GetValue<string>("CenterAudience");
string LocalAudience = builder.Configuration.GetValue<string>("LocalAudience");
string CenterIssuer = builder.Configuration.GetValue<string>("CenterIssuer");
string LocalIssuer = builder.Configuration.GetValue<string>("LocalIssuer");
//JWT Authentication Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer("LoginForCenterUsers", options =>
{
    //options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyJWTSecretforCenter),

        ValidateIssuer = true,
        ValidIssuer = CenterIssuer,

        ValidateAudience = true,
        ValidAudience = CenterAudience
    };
}).AddJwtBearer("LoginForLocalUsers", options =>
{
    //options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyJWTSecretforLocal),

        ValidateIssuer = true,
        ValidIssuer = LocalIssuer,

        ValidateAudience = true,
        ValidAudience = LocalAudience
    };
});


var app = builder.Build();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapMqtt("/mqtt");
});

#pragma warning disable CS8601 // Possible null reference assignment.
Setting.Instance.Syslog = app.Services.GetService<ISyslogSevice>();
#pragma warning restore CS8601 // Possible null reference assignment.

IMqttService? iMqttService = app.Services.GetService<IMqttService>();
if (iMqttService != null)
{
    MqttService mqttService = (MqttService)iMqttService;
    app.UseMqttServer(
        server =>
        {
            server.ValidatingConnectionAsync += mqttService.ValidateConnection;
            server.ClientConnectedAsync += mqttService.OnClientConnected;
            server.ClientDisconnectedAsync += mqttService.OnClientDisConnected;
            server.InterceptingInboundPacketAsync += mqttService.InterceptingInboundPacketAsync;
            server.InterceptingSubscriptionAsync += mqttService.InterceptingSubscriptionAsync;
            mqttService.MqttServer = server;
        });
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
