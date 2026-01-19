namespace Automations.Instagram.DTOs;

public class FollowingRequestInput
{
    public int Count { get; init; } = 12;
    public string? MaxId { get; init; }
}