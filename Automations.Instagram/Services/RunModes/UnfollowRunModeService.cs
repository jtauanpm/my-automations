using Automations.Instagram.DTOs;
using Automations.Instagram.OperationControl;
using Serilog;

namespace Automations.Instagram.Services.RunModes;

public class UnfollowRunModeService(OperationController operationController) : IInstagramRunModeService
{
    private int _unfollowCounter;

    public InstagramRunMode Mode => InstagramRunMode.Unfollow;

    public async Task RunAsync()
    {
        Log.Logger.Information("------ Starting to unfollow ------");

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
                await UnfollowUser(user, index);
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

    private async Task UnfollowUser(InstagramUser user, int index)
    {
        const int maxAttempts = 3;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await InstagramRequestService.MakeUnfollowRequest(user.Pk);

                _unfollowCounter++;
                Log.Logger.Information("Total:{IndexTotal}; Interation Relative:{Index} - User unfollowed: {User}",
                    _unfollowCounter, index + 1, user);

                await operationController.DelayBetweenOperations();
                return;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error while trying to unfollow {User}. Attempt {Attempt}/{MaxAttempts}",
                    user, attempt, maxAttempts);

                if (attempt == maxAttempts) throw;

                await operationController.HandleRequestFailed();
            }
        }
    }

    private async Task<FollowingResponse> GetFirstFollowingUsers()
    {
        const int maxAttempts = 3;

        var size = operationController.GetInteractionSize();
        var followingInput = new FollowingRequestInput { Count = size, MaxId = size };

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                Log.Logger.Information("Fetching first {PageSize} following users", size);
                return await InstagramRequestService.MakeFollowingRequest(followingInput);
            }
            catch (HttpRequestException ex)
            {
                Log.Logger.Warning(ex, "Error when trying to fetch following users. Attempt {Attempt}/{MaxAttempts}",
                    attempt, maxAttempts
                );

                if (attempt == maxAttempts) throw;

                await operationController.HandleRequestFailed();
            }
        }

        throw new InvalidOperationException("Failed to fetch following users after retries");
    }
}
