using Automations.Instagram;
using Automations.Instagram.Services;
using Microsoft.Extensions.Configuration;
using Serilog;

Configuration.Global = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs.txt", rollingInterval: RollingInterval.Day, shared: true, flushToDiskInterval: TimeSpan.FromSeconds(1))
    .CreateLogger();

await ApplicationService.UnfollowNonFavoriteUsersAsync();