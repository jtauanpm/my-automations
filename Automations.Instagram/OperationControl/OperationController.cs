using System.Runtime.InteropServices;
using Serilog;

namespace Automations.Instagram.OperationControl;

// a very hopeful attempt to get Instagram to not block me
public class OperationController(OperationControllerOptions options)
{
    public async Task DelayBetweenOperations()
    {
        await Task.Delay(
            TimeSpan.FromSeconds(
                RandomFromRangeOptions(options.SecondsBetweenOperation)));
    }
    
    public async Task DelayBetweenInteractions()
    {
        var random = RandomFromRangeOptions(options.MinutesBetweenInteractions);
        Log.Logger.Information("---- Awaiting {Minutes} minutes for fetch next users ----", random);
        await Task.Delay(TimeSpan.FromSeconds(random));
    }
    public int GetInteractionSize() => options.InteractionPageSize;

    public async Task SleepIfItsTime()
    {
        if (ShouldIBeSleeping())
        {
            await Sleep();
        }
    }
    
    private static async Task Sleep()
    {
        var sleepTime = Random.Shared.Next(5, 7);
        Log.Logger.Information("Sleeping {Hours} hours", sleepTime);
        await Task.Delay(TimeSpan.FromHours(sleepTime));
    }
    
    private static bool ShouldIBeSleeping()
    {
        var startBlock = new TimeSpan(22, 45, 0);
        var endBlock   = new TimeSpan(6, 0, 0);

        var current = BrazilTimeNow().TimeOfDay;

        return !(current >= startBlock || current < endBlock);
    }

    private const string BrazilTimeZoneWindows = "E. South America Standard Time";
    private const string BrazilTimeZoneUnix = "America/Sao_Paulo";

    private static DateTime BrazilTimeNow()
    {
        var utcNow = DateTime.UtcNow;

        var timeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? BrazilTimeZoneWindows
            : BrazilTimeZoneUnix;

        var brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

        return TimeZoneInfo.ConvertTimeFromUtc(utcNow, brazilTimeZone);
    }
    
    private static int RandomFromRangeOptions(RangeOptions rangeOptions)
    {
        var seconds = System.Random.Shared.Next(
            rangeOptions.Min,
            rangeOptions.Max);
        return seconds;
    }

}