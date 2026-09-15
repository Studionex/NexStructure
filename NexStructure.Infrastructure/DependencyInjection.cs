using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using NexStructure.Application.Abstractions;
using NexStructure.Application.Abstractions.InMemoryBusService;
using NexStructure.Application.Abstractions.Realtime;
using NexStructure.Domain.Common.Repositories;
using NexStructure.Infrastructure.Persistence.Database;
using NexStructure.Infrastructure.Persistence.Database.Interceptors;
using NexStructure.Infrastructure.Services;
using NexStructure.Infrastructure.Services.InMemoryBusService;
using NexStructure.Infrastructure.Services.RealTime;

namespace NexStructure.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        Env.Load();

        services.AddSingleton<IInMemoryEventBus,InMemoryEventBus>();
        services.AddScoped<IStashService,StashService>();
        services.AddSingleton<IConnectionService, ConnectionService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IInMemoryService, InMemoryService>();
        services.AddScoped<IClaimReader,ClaimReader>();
        var isDev = Env.GetBool("DEV") == true;
        services.AddHttpContextAccessor();

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(isDev ? Env.GetString("LOCAL_REDIS_ENDPOINT") : Env.GetString("REDIS")));

        services.AddStackExchangeRedisCache(options =>
        {
            options.InstanceName = "NexStructure:";
            options.ConfigurationOptions = new ConfigurationOptions
            {
                Password = isDev ? Env.GetString("LOCAL_REDIS_PASSWORD") : Env.GetString("REDIS_PASSWORD"),
                Ssl = !isDev,
                SslProtocols = isDev ? null : System.Security.Authentication.SslProtocols.Tls12,
                EndPoints = { isDev ? Env.GetString("LOCAL_REDIS_ENDPOINT") : Env.GetString("REDIS_ENDPOINT") }
            };
        });


    }
    private static void AddResistance(IServiceCollection services)
    {
        var isDev = Env.GetBool("DEV");
        services.AddDbContext<MainContext>(o =>
        {
            o.UseNpgsql(
                isDev
                    ? Env.GetString("LOCAL_POSTGRES")
                    : Env.GetString("POSTGRES"),
                npgsqlOptions =>
                {
                    npgsqlOptions.UseNetTopologySuite();
                });
        }); 

        services.AddTransient<PublishDomainEventsInterceptor>();
        services.AddScoped<IUnitOfWorkRepository,IUnitOfWorkRepository>();
        
    }
}