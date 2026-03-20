using Automations.Instagram.Services.RunModes;

namespace Automations.Instagram.Services;

public class ApplicationService(IEnumerable<IInstagramRunModeService> runModeServices)
{
    private readonly Dictionary<InstagramRunMode, IInstagramRunModeService> _servicesByMode = runModeServices
        .GroupBy(service => service.Mode)
        .ToDictionary(group => group.Key, group => group.Single());

    public Task RunAsync(InstagramRunMode runMode)
    {
        if (_servicesByMode.TryGetValue(runMode, out var service))
        {
            return service.RunAsync();
        }

        throw new ArgumentOutOfRangeException(nameof(runMode), runMode, "Unknown run mode");
    }
}
