using System.Text.Json;
using Automations.Instagram.OperationControl;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Automations.Instagram;

public static class Configuration
{
    public static IConfiguration Global { get; set; } = null!;
    private const string RunModeKey = "Automations:Instagram:RunMode";

    public static OperationControllerOptions GetOperationControlOptions()
    {
        var result = new OperationControllerOptions();
        Global.GetSection(OperationControllerOptions.SectionName).Bind(result);
        
        Log.Logger.Information("Using OperationControlOptions: {Options}", 
            JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
        
        return result;
    }

    public static InstagramRunMode GetRunMode()
    {
        var raw = Global[RunModeKey];

        if (string.IsNullOrWhiteSpace(raw))
        {
            Log.Logger.Information("RunMode not set. Defaulting to {RunMode}", InstagramRunMode.Unfollow);
            return InstagramRunMode.Unfollow;
        }

        if (Enum.TryParse<InstagramRunMode>(raw, true, out var mode))
        {
            Log.Logger.Information("RunMode set to {RunMode}", mode);
            return mode;
        }

        throw new InvalidOperationException(
            $"Invalid RunMode '{raw}'. Valid values: {string.Join(", ", Enum.GetNames<InstagramRunMode>())}");
    }
}
