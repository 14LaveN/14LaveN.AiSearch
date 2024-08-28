using Application.Core.Abstractions;
using Application.Core.Abstractions.Helpers.JWT;
using Identity.API.Common.Abstractions.Presence;
using Identity.API.Domain.Entities;
using Identity.API.Domain.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Hubs;

public sealed class PresenceHub(
    PresenceTracker presenceTracker,
    IUserRepository userRepository,
    IUserNameProvider userNameProvider,
    IDbContext dbContext)
    : Hub
{
    private readonly PresenceTracker _presenceTracker = presenceTracker;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserNameProvider _userNameProvider = userNameProvider;

    public override async Task OnConnectedAsync()
    {
        string? userName = _userNameProvider.UserName;

        bool isOnline = userName is not null && await _presenceTracker.UserConnected(userName, Context.ConnectionId);

        if (isOnline)
        {
            User? user = await _userRepository.GetByNameAsync(userName!);
            await Clients.Others.SendAsync("UserIsOnline", user);
        }

        string[] currentUser = await _presenceTracker.GetOnlineUsers();

        foreach (var user in currentUser)
        {
            var usersOnline = await dbContext
                .Set<User>()
                .Where(u => u.UserName == user)
                .SingleOrDefaultAsync();
        }
    }
}