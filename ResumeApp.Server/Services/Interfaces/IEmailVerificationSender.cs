using ResumeApp.Server.ApplicationUserModel;

namespace ResumeApp.Server.Services.Interfaces
{
    // Added to separate verification delivery from authentication logic.
    public interface IEmailVerificationSender
    {
        Task SendVerificationLinkAsync(
            ApplicationUser user,
            string token,
            CancellationToken cancellationToken = default);
    }
}
