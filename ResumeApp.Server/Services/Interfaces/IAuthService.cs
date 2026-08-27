using ResumeApp.Server.DTOs.Auth;

namespace ResumeApp.Server.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Message)> RegisterAsync(RegisterDto dto);
        Task<(AuthResponseDto? Response, string Message)> LoginAsync(LoginDto dto);

        // Added email-confirmation operations.
        Task<(bool Success, string Message)> ConfirmEmailAsync(
            string userId,
            string token);

        Task<(bool Success, string Message)> ResendEmailConfirmationAsync(
            string email);
    }
}
