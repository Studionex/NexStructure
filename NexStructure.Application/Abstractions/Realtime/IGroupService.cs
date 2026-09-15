namespace NexStructure.Application.Abstractions.Realtime;

public interface IGroupService
{
    Task<HashSet<string>> GetUserGroupsAsync(string userId, CancellationToken ct = default);
    Task AddUsersToGroupAsync(string groupKey, List<string> userIds, CancellationToken ct = default);
    Task RemoveUsersFromGroupAsync(string groupKey, List<string> userIds, CancellationToken ct = default);
}