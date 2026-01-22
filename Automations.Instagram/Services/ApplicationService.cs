using Automations.Instagram.DTOs;
using Serilog;

namespace Automations.Instagram.Services;

public static class ApplicationService
{
    // a very hopeful attempt to get Instagram to not block me
    private const int PageSize = 25;

    // TODO: configure values by IConfiguration
    // TODO: sleep tonigth
    public static async Task UnfollowNonFavoriteUsersAsync()
    {
        var count = 0;
        
        while (true)
        {
            var followingResponse = await GetFirstFollowingUsers();

            if (followingResponse.Users.Count == 0)
            {
                Log.Logger.Information("No users returned. Stopping.");
                break;
            }
            
            foreach (var (user, index) in followingResponse.Users
                         .Where(u => !u.IsFavorite)
                         .Select((user, index) => (user, index)))
            {
                try
                {
                    await InstagramRequestService.MakeUnfollowRequest(user.Pk);
                    
                    count++;
                    Log.Logger.Information("Total: {IndexTotal}; Relative {Index} - User unfollowed: {User}", count, index+1, user);

                    var timeToAwait = TimeSpan.FromSeconds(Random.Shared.Next(5, 10));
                    await Task.Delay(timeToAwait);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "Error while trying to unfollow {User}", user);
                    throw;
                }
            }

            if (!followingResponse.HasMore)
            {
                Log.Logger.Information("No more following users.");
                break;
            }

            var betweenCyclesDelay = Random.Shared.Next(1, 3);
            Log.Logger.Information("---- Awaiting {Minutes} minutes for fetch next users ----", betweenCyclesDelay);
            await Task.Delay(TimeSpan.FromMinutes(betweenCyclesDelay));
        }
    }

    private static async Task<FollowingResponse> GetFirstFollowingUsers()
    {
        try
        {
            var followingInput = new FollowingRequestInput
            {
                Count = PageSize,
                MaxId = PageSize // always fetch the first page on purpose
            };

            Log.Logger.Debug("Fetching first {PageSize} following users", PageSize);

            return await InstagramRequestService.MakeFollowingRequest(followingInput);
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "Error when trying to fetch following users");
            throw;
        }
    }
}