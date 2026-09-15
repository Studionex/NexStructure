using NexStructure.Application.Abstractions;
using NexStructure.Application.Abstractions.Realtime;

namespace NexStructure.Infrastructure.Services.RealTime;

public class GroupService(IInMemoryService inMemoryService,
    IClaimReader claimReader):IGroupService
{
    public async Task<HashSet<string>> GetUserGroupsAsync(string userId, CancellationToken ct = default)
    {
        var groups = new HashSet<string>();

        

        return groups;
    }

    public async Task AddUsersToGroupAsync(string groupKey, List<string> userIds, CancellationToken ct = default)
    {
        foreach (var userId in userIds) // don't delete or, I will do refactor on invitation service
        {
            await inMemoryService.InsertAsync(userId, [groupKey], ct);
        }
    }

    public async Task RemoveUsersFromGroupAsync(string groupKey, List<string> userIds, CancellationToken ct = default)
    {
        foreach (var userId in userIds)
        {
            await inMemoryService.RemoveAsync(userId, groupKey, ct);
        }
    }
}