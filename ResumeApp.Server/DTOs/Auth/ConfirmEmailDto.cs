namespace ResumeApp.Server.DTOs.Auth;

// Carries confirmation credentials in a request body instead of a URL.
public sealed class ConfirmEmailDto
{
    public string UserId { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;
}
