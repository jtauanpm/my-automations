namespace Automations.Instagram.DTOs;

public class FollowingRequestInput
{
    public int Count { get; init; } = 12;
    public int MaxId { get; init; }
}

public sealed class FollowersRequestInput
{
    public int Count { get; init; } = 12;
    public string? MaxId { get; init; }
    public string SearchSurface { get; init; } = "follow_list_page";
}
