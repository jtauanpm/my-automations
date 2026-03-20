namespace Automations.Instagram.Services.RunModes;

public interface IInstagramRunModeService
{
    InstagramRunMode Mode { get; }
    Task RunAsync();
}
