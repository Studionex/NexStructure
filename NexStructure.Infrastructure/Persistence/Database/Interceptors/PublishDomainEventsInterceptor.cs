using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NexStructure.Application.Abstractions.InMemoryBusService;
using NexStructure.Domain.Common.Models;

namespace NexStructure.Infrastructure.Persistence.Database.Interceptors;

public abstract class PublishDomainEventsInterceptor(IPublisher mediator , IInMemoryEventBus memoryService,IStashService stash) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        PublishDomainEvents(eventData.Context).GetAwaiter().GetResult();
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = new())
    {
        await PublishDomainEvents(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = new ())
    {
        while (stash.TryTake(out var ev))
        {
            try
            {
                await memoryService.PublishAsync(ev!, cancellationToken);
            }
            catch
            {
                // Log the exception or handle it as needed
            }
        }
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private async Task PublishDomainEvents(DbContext? dbContext)
    {
        if (dbContext is null) return;
        var entities = dbContext.ChangeTracker.Entries<IEntity>()
            .Where(entry => entry.Entity.DomainEvents.Any())
            .Select(entry => entry.Entity)
            .ToList();
        var domainEvents = entities
            .SelectMany(entity => entity.DomainEvents)
            .ToList();
        entities.ForEach(entity => entity.ClearDomainEvents());
        foreach (var domainEvent in domainEvents) await mediator.Publish(domainEvent);
    }
}
