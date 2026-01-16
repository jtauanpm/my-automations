using System.Net;
using System.Text;
using Automations.Instagram.DTOs;

namespace Automations.Instagram;

public class InstagramRequestFactory
{
    public static HttpClient CreateClient(RequestOptions options)
    {
        var cookies = new CookieContainer();
        cookies.Add(new Uri("https://www.instagram.com"), new Cookie("sessionid", options.SessionId));
        cookies.Add(new Uri("https://www.instagram.com"), new Cookie("csrftoken", options.CsrfToken));

        var handler = new HttpClientHandler
        {
            UseCookies = true,
            CookieContainer = cookies
        };

        var client = new HttpClient(handler);

        client.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", options.UserAgent);
        client.DefaultRequestHeaders.TryAddWithoutValidation("x-csrftoken", options.CsrfToken);
        client.DefaultRequestHeaders.TryAddWithoutValidation("x-ig-app-id", options.IgAppId);
        client.DefaultRequestHeaders.TryAddWithoutValidation("x-requested-with", "XMLHttpRequest");

        return client;
    }
    
    public static HttpRequestMessage BuildFollowingRequest(FollowingRequestInput input)
    {
        var query = new StringBuilder();
        query.Append($"count={input.Count}");

        if (!string.IsNullOrWhiteSpace(input.MaxId))
            query.Append($"&max_id={input.MaxId}");

        var url =
            $"https://www.instagram.com/api/v1/friendships/{input.UserId}/following/?{query}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Referrer =
            new Uri($"https://www.instagram.com/{input.UserId}/following/");

        return request;
    }
    
    public static HttpRequestMessage BuildUnfollowRequest(string targetUserId, string myUsername, string jazoest)
    {
        var url =
            $"https://www.instagram.com/api/v1/friendships/destroy/{targetUserId}/";

        var body =
            $"container_module=profile&" +
            $"user_id={targetUserId}" +
            $"&jazoest={myUsername}";

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(
                body,
                Encoding.UTF8,
                "application/x-www-form-urlencoded"
            )
        };

        request.Headers.Referrer = new Uri($"https://www.instagram.com/{myUsername}/following/");

        return request;
    }
}
