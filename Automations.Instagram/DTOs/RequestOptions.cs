namespace Automations.Instagram.DTOs;

public sealed class RequestOptions
{
    // Sessão
    public required string SessionId { get; init; }
    public required string CsrfToken { get; init; }

    // Headers sensíveis
    public string IgAppId { get; init; } = "936619743392459";
    public string UserAgent { get; init; } = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/143.0.0.0 Safari/537.36";
}