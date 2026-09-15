
namespace NexStructure.Application.Abstractions.InMemoryBusService;

public interface IInMemoryEventBus
{
    void Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IAppEvent;

    Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct = default)
        where TEvent : IAppEvent;

    Task PublishAsync(IAppEvent @event, CancellationToken ct = default);
}