namespace NexStructure.Application.Abstractions.Realtime;

public interface IConnectionService
{
    Task AddConnection(string userId, string connectionId);
    HashSet<string> GetUserConnections(string userId);
    Task RemoveConnection(string connectionId);
    
}