namespace NexStructure.Application.Abstractions.InMemoryBusService;

public interface IStashService
{
    void Set(IAppEvent ev);
    bool TryTake(out IAppEvent? ev);
}