using Automations.Instagram.DTOs;
using Automations.Instagram.OperationControl;
using Serilog;

namespace Automations.Instagram.Services.RunModes;

public class RemoveFollowersRunModeService(OperationController operationController) : IInstagramRunModeService
{
    private int _removeFollowerCounter;

    public InstagramRunMode Mode => InstagramRunMode.RemoveFollowers;

    public async Task RunAsync()
    {
        Log.Logger.Information("------ Starting to remove followers ------");

        while (true)
        {
            var followersResponse = await GetFirstFollowersUsers();
            if (followersResponse.Users.Count == 0)
            {
                Log.Logger.Information("No users returned. Stopping.");
                break;
            }

            foreach (var (user, index) in followersResponse.Users
                         .Where(u => !u.IsFavorite)
                         .Select((user, index) => (user, index)))
            {
                await RemoveFollower(user, index);
            }

            if (!followersResponse.HasMore)
            {
                Log.Logger.Information("No more followers.");
                break;
            }

            await operationController.SleepIfItsTime();
            await operationController.DelayBetweenInteractions();
        }
    }

    private async Task RemoveFollower(InstagramUser user, int index)
    {
        const int maxAttempts = 3;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await InstagramRequestService.MakeRemoveFollowerRequest(user.Pk);

                _removeFollowerCounter++;
                Log.Logger.Information("Total:{IndexTotal}; Interation Relative:{Index} - Follower removed: {User}",
                    _removeFollowerCounter, index + 1, user);

                await operationController.DelayBetweenOperations();
                return;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error while trying to remove follower {User}. Attempt {Attempt}/{MaxAttempts}",
                    user, attempt, maxAttempts);

                if (attempt == maxAttempts) throw;

                await operationController.HandleRequestFailed();
            }
        }
    }

    private async Task<FollowingResponse> GetFirstFollowersUsers()
    {
        const int maxAttempts = 3;

        var size = operationController.GetInteractionSize();
        var followersInput = new FollowersRequestInput { Count = size };

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                Log.Logger.Information("Fetching first {PageSize} followers", size);
                return await InstagramRequestService.MakeFollowersRequest(followersInput);
            }
            catch (HttpRequestException ex)
            {
                Log.Logger.Warning(ex, "Error when trying to fetch followers. Attempt {Attempt}/{MaxAttempts}",
                    attempt, maxAttempts
                );

                if (attempt == maxAttempts) throw;

                await operationController.HandleRequestFailed();
            }
        }

        throw new InvalidOperationException("Failed to fetch followers after retries");
    }
}
