using NexStructure.Application.Abstractions.InMemoryBusService;

namespace NexStructure.Infrastructure.Services.InMemoryBusService;

public class InMemoryEventBus : IInMemoryEventBus
{
    private readonly Dictionary<Type, List<Func<IAppEvent, Task>>> _handlers = new();

    public void Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IAppEvent
    {
        var eventType = typeof(TEvent);

        if (!_handlers.TryGetValue(eventType, out var list))
        {
            list = [];
            _handlers[eventType] = list;
        }

        list.Add(iEvent=> handler((TEvent)iEvent));
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct = default)
        where TEvent : IAppEvent
    {
        var eventType = typeof(TEvent);
        if (_handlers.TryGetValue(eventType, out var list))
        {
            foreach (var handler in list)
            {
                await handler(@event);
            }
        }
    }
    public async Task PublishAsync(IAppEvent @event, CancellationToken ct = default)
    {
        var eventType = @event.GetType();
        if (_handlers.TryGetValue(eventType, out var list))
        {
            foreach (var handler in list)
                await handler(@event);
        }
    }
}



