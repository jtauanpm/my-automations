using Automations.Instagram.DTOs;
using Serilog;

namespace Automations.Instagram.Services;

public class ApplicationService(OperationControl.OperationController operationController)
{
    public async Task UnfollowNonFavoriteUsersAsync()
    {
        Log.Logger.Information("------ Starting to unfollow ------");
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

                    await operationController.DelayBetweenOperations();
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

            await operationController.SleepIfItsTime();

            await operationController.DelayBetweenInteractions();
        }
    }

    private async Task<FollowingResponse> GetFirstFollowingUsers()
    {
        try
        {
            var size = operationController.GetInteractionSize();
            var followingInput = new FollowingRequestInput { Count = size, MaxId = size };

            Log.Logger.Debug("Fetching first {PageSize} following users", size);

            return await InstagramRequestService.MakeFollowingRequest(followingInput);
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "Error when trying to fetch following users");
            throw;
        }
    }
}