using System.Collections.Concurrent;
using NexStructure.Application.Abstractions.InMemoryBusService;

namespace NexStructure.Infrastructure.Services.InMemoryBusService;

public class StashService : IStashService
{
    private ConcurrentQueue<IAppEvent> _event =new();
    public void Set(IAppEvent ev) => _event.Enqueue(ev);

    public bool TryTake(out IAppEvent? ev)
    {
        if(_event.TryDequeue(out ev))
         return true;
        ev = default;
        return false;
    }
}
