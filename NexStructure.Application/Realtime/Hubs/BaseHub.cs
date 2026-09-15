using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalR.Documentation.Reflection;
using NexStructure.Application.Abstractions;
using NexStructure.Application.Abstractions.Realtime;

namespace NexStructure.Application.Realtime.Hubs;

[Authorize]
[HubDoc(Summary = "this base hub" , Description = "base connections hub")]
public class BaseHub(
    IConnectionService connectionService,
    IClaimReader claimReader,
    IGroupService groupService,
    IInMemoryService inMemoryService) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = claimReader.GetUserId();
        if (userId is null)
        {
            Context.Abort();
            return;
        }
        await connectionService.AddConnection(userId, Context.ConnectionId);
        await AddToGroupsAsync(userId);
        await base.OnConnectedAsync();


    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await connectionService.RemoveConnection(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
    
    [HubMethodDoc(Summary = "Test Connection")]
    public async Task<string> Ping(string connectionId)
    {
        return "pong";
    }
    [HubMethodDoc(Summary = "Test another ping")]
    public async Task<string> AnotherPing()
    {
        return "pong";
    }
    [HubMethodDoc(Summary = "Test another ping")]
    public async Task TestNull(int? i)
    {
        return;
    }


    private async Task AddToGroupsAsync(string userId)
    {
        var (isExist, cachedGroups) = await inMemoryService.GetGroupsAsync(userId);

        if (!isExist)
        {
            var groups = await groupService.GetUserGroupsAsync(userId);

            if (groups.Count > 0)
            {
                await inMemoryService.InsertAsync(userId, groups);

                foreach (var groupKey in groups)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, groupKey);
                }
            }
        }
        else
        {
            foreach (var groupKey in cachedGroups)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupKey);
            }
        }
    }
}
