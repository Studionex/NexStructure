namespace NexStructure.Application.Abstractions.Realtime;

public interface IInMemoryService
{
    Task InsertAsync(string userId, HashSet<string> groupsIds, CancellationToken ct = default);
    Task RemoveAsync(string userId, string groupId, CancellationToken ct = default);

    Task<(bool isExist, HashSet<string> groups)> GetGroupsAsync(string id, CancellationToken ct = default);
}
