using Microsoft.Extensions.Configuration;

namespace Automations.Instagram;

public static class Configuration
{
    public static IConfiguration Global { get; set; } = null!;
}