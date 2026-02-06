using Automations.Instagram;
using Automations.Instagram.OperationControl;
using Automations.Instagram.Services;
using Microsoft.Extensions.Configuration;
using Serilog;

Configuration.Global = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/logs-.txt", rollingInterval: RollingInterval.Day, shared: true, flushToDiskInterval: TimeSpan.FromSeconds(1))
    .CreateLogger();

var operationController = new OperationController(Configuration.GetOperationControlOptions());
var service = new ApplicationService(operationController);

await service.UnfollowNonFavoriteUsersAsync();