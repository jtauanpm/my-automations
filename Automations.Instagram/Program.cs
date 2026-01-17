using System.Net;
using System.Text.Json;
using Automations.Instagram;
using Automations.Instagram.DTOs;
using Microsoft.Extensions.Configuration;
using Serilog;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs.txt")
    .CreateLogger();

var options = new RequestOptions
{
    SessionId = WebUtility.UrlDecode(config["Automations:Instagram:SessionId"])!,
    CsrfToken = WebUtility.UrlDecode(config["Automations.Instagram.CsrfToken"])!,
};

using var client = InstagramRequestFactory.CreateClient(options);
using var request = InstagramRequestFactory.BuildFollowingRequest(new FollowingRequestInput
{
    UserId = config["Automations:Instagram:UserId"]!,
    Count = 10,
    MaxId = null,
});

var response = await client.SendAsync(request);
var json = await response.Content.ReadAsStringAsync();

var responseObj = JsonSerializer.Deserialize<FollowingResponse>(json,
    new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    }
)!;


     Log.Logger.Information("Trying to unfollow {User}.", responseObj.Users[0]);
     var result = await InstagramRequestFactory.MakeUnfollowCurlRequest(responseObj.Users[0].Pk, config);
     Console.WriteLine($"{responseObj.Users[0].FullName} @{responseObj.Users[0].Username}");

// using (var unfollowRequest = InstagramRequestFactory.BuildUnfollowRequest(responseObj.Users[0].Pk, config["Automations:Instagram:Username"]!, config["Automations.Instagram.jazoest"]))
// {
//     Log.Logger.Information("Trying to unfollow {User}.", responseObj.Users[0]);
//     
//     Console.WriteLine($"{responseObj.Users[0].FullName} @{responseObj.Users[0].Username}");
//     var jsonResponse = await client.SendAsync(unfollowRequest);
// }

// Console.WriteLine(response.StatusCode);
// Console.WriteLine(json);