using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Automations.Instagram.DTOs;

public sealed class FollowingResponse
{
    [JsonPropertyName("users")]
    public List<InstagramUser> Users { get; set; } = [];

    [JsonPropertyName("next_max_id")]
    public string? NextMaxId { get; set; }

    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }

    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}


public sealed class InstagramUser
{
    [JsonPropertyName("pk")]
    public string Pk { get; set; } = string.Empty;

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("full_name")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("is_private")]
    public bool IsPrivate { get; set; }

    [JsonPropertyName("is_verified")]
    public bool IsVerified { get; set; }

    [JsonPropertyName("is_favorite")]
    public bool IsFavorite { get; set; }

    public override string ToString()
    {
        return $"PK: {Pk} - {FullName} (@{Username})";
    }
}

