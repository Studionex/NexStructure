using System.Collections.Concurrent;
using NexStructure.Application.Abstractions.Realtime;

namespace NexStructure.Infrastructure.Services.RealTime;

public class ConnectionService : IConnectionService
{
    private readonly ConcurrentDictionary<string, HashSet<string>> _connections = [];
    private readonly ConcurrentDictionary<string, string> _userIds = [];
    
    public  Task AddConnection(string userId, string connectionId)
    {
        if (!_connections.ContainsKey(userId))
        {
            _connections[userId] = [];

        }
        _connections[userId].Add(connectionId);
        _userIds[connectionId] = userId;
        return Task.CompletedTask;
    }
    public HashSet<string> GetUserConnections(string userId)
    {
        return _connections.TryGetValue(userId, out var connection) ? connection : [];
    }
    public Task RemoveConnection(string connectionId)
    {
        if (!_userIds.TryGetValue(connectionId, out var userId))
            return Task.CompletedTask;

        if (string.IsNullOrEmpty(userId))
            return Task.CompletedTask;

        _userIds.Remove(connectionId, out _);

        if (!_connections.TryGetValue(userId, out var userConnections))
            return Task.CompletedTask;

        userConnections.Remove(connectionId);

        if (userConnections.Count == 0)
        {
            _connections.Remove(userId, out _);

        }
        return Task.CompletedTask;
    }


}