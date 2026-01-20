using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Automations.Instagram.DTOs;

namespace Automations.Instagram.Services;

// yes, it's horrible, I should avoid mutiple IConfiguration accesses, there are several loose strings here, inconsistent calls...
// but I only want it working :(
public class InstagramRequestService
{
    private static HttpClient? _client = null;
    
    private static HttpClient GetClient()
    {
        _client ??= CreateClient();
        return _client;
    }

    private static HttpClient CreateClient()
    {
        var csrfToken = WebUtility.UrlDecode(Configuration.Global["Automations:Instagram:CsrfToken"]);
        var sessionCookie = new Cookie("sessionid", WebUtility.UrlDecode(Configuration.Global["Automations:Instagram:SessionId"]));
        var csrfTokenCookie = new Cookie("csrftoken", csrfToken);
        var cookies = new CookieContainer();
        
        cookies.Add(new Uri("https://www.instagram.com"), sessionCookie);
        cookies.Add(new Uri("https://www.instagram.com"), csrfTokenCookie);

        var handler = new HttpClientHandler
        {
            UseCookies = true,
            CookieContainer = cookies
        };

        var client = new HttpClient(handler);

        client.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/143.0.0.0 Safari/537.36");
        client.DefaultRequestHeaders.TryAddWithoutValidation("x-csrftoken", csrfToken);
        client.DefaultRequestHeaders.TryAddWithoutValidation("x-ig-app-id", Configuration.Global["Automations:Instagram:Headers:x-ig-app-id"]);
        client.DefaultRequestHeaders.TryAddWithoutValidation("x-requested-with", "XMLHttpRequest");

        return client;
    }
    
    public static async Task<FollowingResponse> MakeFollowingRequest(FollowingRequestInput input)
    {
        var query = new StringBuilder();
        query.Append($"count={input.Count}");
        query.Append($"&max_id={input.MaxId}");

        var userId = Configuration.Global["Automations:Instagram:UserId"];
        
        var url =
            $"https://www.instagram.com/api/v1/friendships/{userId}/following/?{query}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Referrer =
            new Uri($"https://www.instagram.com/{userId}/following/");

        var client = GetClient();
        var response = await client.SendAsync(request);
        
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<FollowingResponse>())!;
    }

    public static async Task<string> MakeUnfollowRequest(string userIdToUnfollow)
    {
        var client = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Post,
            $"https://www.instagram.com/api/v1/friendships/destroy/{userIdToUnfollow}/"
        );
        
        request.Headers.Add("accept", "*/*");
        request.Headers.Add("accept-language", "en-US,en;q=0.9,pt;q=0.8");
        request.Headers.Add("origin", "https://www.instagram.com");
        request.Headers.Add("priority", "u=1, i");
        request.Headers.Add("referer", $"https://www.instagram.com/{Configuration.Global["Automations:Instagram:UserId"]}/following/");
        request.Headers.Add("sec-ch-prefers-color-scheme", "dark");
        request.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"143\", \"Chromium\";v=\"143\", \"Not A(Brand\";v=\"24\"");
        request.Headers.Add("sec-ch-ua-full-version-list", "\"Google Chrome\";v=\"143.0.7499.169\", \"Chromium\";v=\"143.0.7499.169\", \"Not A(Brand\";v=\"24.0.0.0\"");
        request.Headers.Add("sec-ch-ua-mobile", "?0");
        request.Headers.Add("sec-ch-ua-model", "\"\"");
        request.Headers.Add("sec-ch-ua-platform", "\"Linux\"");
        request.Headers.Add("sec-ch-ua-platform-version", "\"6.17.9\"");
        request.Headers.Add("sec-fetch-dest", "empty");
        request.Headers.Add("sec-fetch-mode", "cors");
        request.Headers.Add("sec-fetch-site", "same-origin");
        request.Headers.Add("user-agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/143.0.0.0 Safari/537.36");
        request.Headers.Add("x-asbd-id", "359341");
        request.Headers.Add("x-ig-app-id", "936619743392459");
        request.Headers.Add("x-requested-with", "XMLHttpRequest");

        // Headers sensíveis (vindos do secret)
        AddHeaderFromConfig(request, "cookie");
        AddHeaderFromConfig(request, "x-csrftoken");
        AddHeaderFromConfig(request, "x-ig-www-claim");
        AddHeaderFromConfig(request, "x-instagram-ajax");
        AddHeaderFromConfig(request, "x-web-session-id");
        
        request.Content = new StringContent(
            $"container_module=profile&" +
            $"nav_chain=PolarisProfilePostsTabRoot:profilePage:2:unexpected&" +
            $"user_id={userIdToUnfollow}&" +
            $"jazoest={Configuration.Global["Instagram:jazoest"]}"
        );

        request.Content.Headers.ContentType =
            new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
    
    private static void AddHeaderFromConfig(HttpRequestMessage request, string headerName)
    {
        var value = Configuration.Global[$"Automations:instagram:headers:{headerName}"];

        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException(
                $"Header obrigatório não encontrado no configuration: {headerName}");

        request.Headers.Add(headerName, value);
    }
}
