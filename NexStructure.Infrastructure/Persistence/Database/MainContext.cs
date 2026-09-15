using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using NexStructure.Domain.Common.Models;
using NexStructure.Infrastructure.Persistence.Database.Interceptors;

namespace NexStructure.Infrastructure.Persistence.Database;

public class MainContext:DbContext
{
    private readonly PublishDomainEventsInterceptor _publishDomainEventsInterceptor;
    public MainContext(DbContextOptions<MainContext> options, PublishDomainEventsInterceptor publishDomainEventsInterceptor) : base(options)
    {
        _publishDomainEventsInterceptor = publishDomainEventsInterceptor;
    } 
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Ignore<List<DomainEvent>>()
            .ApplyConfigurationsFromAssembly(typeof(MainContext).Assembly);


        modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetProperties())
            .Where(p => p.IsPrimaryKey())
            .ToList()
            .ForEach(p => p.ValueGenerated = ValueGenerated.Never);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        optionsBuilder.AddInterceptors(_publishDomainEventsInterceptor);
        base.OnConfiguring(optionsBuilder);
    }
}