using StackExchange.Redis;
using StackExchange.Redis;
using NexStructure.Application.Abstractions.Realtime;
using NexStructure.Domain.Common.Models;

namespace NexStructure.Infrastructure.Services.RealTime;

public class InMemoryService(IConnectionMultiplexer multiplexer) : IInMemoryService
{
    private readonly IDatabase _redis = multiplexer.GetDatabase();
    public async Task InsertAsync(string userId, HashSet<string> groupsIds, CancellationToken ct = default)
    {
        var groupKey = RKeys.SocketGroups.UserPrefix + userId;
        await _redis.SetAddAsync(groupKey, groupsIds.Select(g => (RedisValue)g).ToArray());
        await _redis.KeyExpireAsync(
            groupKey,
            TimeSpan.FromMinutes(120)
        );
    }


    public async Task RemoveAsync(string userId, string groupId, CancellationToken ct = default)
    {
        var groupKey = RKeys.SocketGroups.UserPrefix + userId;
        await _redis.SetRemoveAsync(groupKey, groupId);

    }


    public async Task<(bool isExist, HashSet<string> groups)> GetGroupsAsync(string userId, CancellationToken ct = default)
    {
        var groupKey = RKeys.SocketGroups.UserPrefix + userId;
        var existing = await _redis.SetMembersAsync(groupKey);
        if (existing.Length == 0)
            return (false, []);
        return (true, existing.Select(v => v.ToString()).ToHashSet());
    }
}

